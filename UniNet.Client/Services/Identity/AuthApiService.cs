using System.Net.Http.Json;
using System.Text.Json;
using Contracts.Requests.LoginRequests;
using Contracts.Responses.Login;
using UniNet.Client.Services.Http;
using UniNet.Client.State;

namespace UniNet.Client.Services.Identity;

// نتيجة موجزة لعملية المصادقة تُغذّي الواجهة.
public record AuthResult(bool Success, string? Error = null);

// خدمة المصادقة: login / logout. تستخدم عميلًا عامًّا (بلا معالِج الرمز).
public class AuthApiService
{
    private readonly HttpClient _http;
    private readonly TokenStore _store;
    private readonly JwtAuthStateProvider _authState;

    public AuthApiService(HttpClient http, TokenStore store, JwtAuthStateProvider authState)
    {
        _http = http;
        _store = store;
        _authState = authState;
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        HttpResponseMessage response;
        try
        {
            response = await _http.PostAsJsonAsync("api/Login/login", request, ct);
        }
        catch (HttpRequestException)
        {
            return new AuthResult(false, "تعذّر الاتصال بالخادم. تأكّد من تشغيل الخدمة ثم حاول مجددًا.");
        }

        if (!response.IsSuccessStatusCode)
            return new AuthResult(false, await ReadErrorAsync(response));

        var tokens = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct);
        if (tokens is null || string.IsNullOrWhiteSpace(tokens.AccesseToken))
            return new AuthResult(false, "استجابة غير متوقّعة من الخادم.");

        await _store.SaveAsync(tokens.AccesseToken, tokens.RefreshToken);
        _authState.NotifyChanged();
        return new AuthResult(true);
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        var refresh = await _store.GetRefreshTokenAsync();
        if (!string.IsNullOrWhiteSpace(refresh))
        {
            try
            {
                await _http.PostAsJsonAsync("api/Login/logOut", new RefreshToken_LogOut_Request { RefreshToken = refresh }, ct);
            }
            catch (HttpRequestException)
            {
                // تسجيل الخروج محليًا يكفي حتى لو تعذّر إبلاغ الخادم.
            }
        }
        await _store.ClearAsync();
        _authState.NotifyChanged();
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var body = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body))
                return DefaultMessage(response);

            // الخادم يعيد أحيانًا نصًّا مباشرًا، وأحيانًا JSON فيه message/detail.
            if (body.TrimStart().StartsWith('{'))
            {
                using var doc = JsonDocument.Parse(body);
                foreach (var key in new[] { "message", "Message", "detail", "Detail" })
                    if (doc.RootElement.TryGetProperty(key, out var el) && el.ValueKind == JsonValueKind.String)
                        return el.GetString()!;
                return DefaultMessage(response);
            }
            return body.Trim('"');
        }
        catch
        {
            return DefaultMessage(response);
        }
    }

    private static string DefaultMessage(HttpResponseMessage response) => (int)response.StatusCode switch
    {
        401 => "اسم المستخدم أو كلمة المرور غير صحيحة.",
        403 => "هذا الحساب موقوف.",
        _ => "تعذّر تسجيل الدخول. حاول مرة أخرى."
    };
}
