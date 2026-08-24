using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Contracts.Requests.LoginRequests;
using Contracts.Responses.Login;
using UniNet.Client.State;

namespace UniNet.Client.Services.Http;

// يُرفق Bearer في كل نداء مُصادَق، وعند 401 يحاول تدوير الرمز مرة واحدة ثم يعيد المحاولة.
public class AuthTokenHandler : DelegatingHandler
{
    private readonly TokenStore _store;
    private readonly IServiceProvider _services;

    public AuthTokenHandler(TokenStore store, IServiceProvider services)
    {
        _store = store;
        _services = services;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var access = await _store.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(access))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access);

        var response = await base.SendAsync(request, ct);
        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        // محاولة تدوير واحدة.
        if (await TryRefreshAsync(ct))
        {
            var newAccess = await _store.GetAccessTokenAsync();
            var retry = await CloneAsync(request);
            if (!string.IsNullOrWhiteSpace(newAccess))
                retry.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccess);
            response.Dispose();
            return await base.SendAsync(retry, ct);
        }

        return response;
    }

    private async Task<bool> TryRefreshAsync(CancellationToken ct)
    {
        var refresh = await _store.GetRefreshTokenAsync();
        if (string.IsNullOrWhiteSpace(refresh))
            return false;

        // عميل مستقل (بلا هذا المعالِج) لتفادي التكرار اللانهائي.
        var baseAddress = _services.GetRequiredService<ApiBaseAddress>().Value;
        using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
        var resp = await client.PostAsJsonAsync("api/Login/refresh", new RefreshToken_LogOut_Request { RefreshToken = refresh }, ct);
        if (!resp.IsSuccessStatusCode)
        {
            await _store.ClearAsync();
            _services.GetRequiredService<JwtAuthStateProvider>().NotifyChanged();
            return false;
        }

        var tokens = await resp.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct);
        if (tokens is null)
            return false;

        await _store.SaveAsync(tokens.AccesseToken, tokens.RefreshToken);
        _services.GetRequiredService<JwtAuthStateProvider>().NotifyChanged();
        return true;
    }

    private static async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);
        if (request.Content is not null)
        {
            var bytes = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(bytes);
            foreach (var header in request.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        foreach (var header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        return clone;
    }
}

// عنوان الـ API يُحقَن كقيمة مفردة (يُقرأ من appsettings.json).
public sealed record ApiBaseAddress(string Value);
