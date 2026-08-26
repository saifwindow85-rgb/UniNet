using System.Net.Http.Json;
using System.Text.Json;

namespace UniNet.Client.Services.Http;

// نتيجة موحّدة لنداءات الـ API تعكس مغلّف الخادم (نجاح/أخطاء).
public record ApiResult<T>(bool Success, T? Data, string? Error, int Status)
{
    public static ApiResult<T> Ok(T data) => new(true, data, null, 200);
    public static ApiResult<T> Fail(string error, int status) => new(false, default, error, status);
}

public static class ApiResponse
{
    private static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web);

    // يقرأ ردًّا يحمل جسم DTO عند النجاح، أو رسالة خطأ مستخلَصة من مغلّف الخادم.
    public static async Task<ApiResult<T>> ReadAsync<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return ApiResult<T>.Ok(default!);

            var data = await response.Content.ReadFromJsonAsync<T>(Web);
            return ApiResult<T>.Ok(data!);
        }

        return ApiResult<T>.Fail(await ExtractErrorAsync(response), (int)response.StatusCode);
    }

    // لعمليات الحذف (2xx = نجاح، غير ذلك = فشل).
    public static async Task<ApiResult<bool>> ReadOkAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return ApiResult<bool>.Ok(true);

        return ApiResult<bool>.Fail(await ExtractErrorAsync(response), (int)response.StatusCode);
    }

    private static async Task<string> ExtractErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body) && body.TrimStart().StartsWith('{'))
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                // مصفوفة errors (أي حالة أحرف).
                foreach (var key in new[] { "errors", "Errors" })
                {
                    if (root.TryGetProperty(key, out var arr) && arr.ValueKind == JsonValueKind.Array)
                    {
                        var messages = arr.EnumerateArray()
                            .Where(e => e.ValueKind == JsonValueKind.String)
                            .Select(e => e.GetString())
                            .Where(s => !string.IsNullOrWhiteSpace(s));
                        var joined = string.Join(" · ", messages!);
                        if (!string.IsNullOrWhiteSpace(joined))
                            return joined;
                    }
                }

                // رسالة/عنوان مفرد.
                foreach (var key in new[] { "message", "Message", "detail", "Detail", "title", "Title" })
                    if (root.TryGetProperty(key, out var el) && el.ValueKind == JsonValueKind.String)
                        return el.GetString()!;
            }
        }
        catch
        {
            // نتجاهل ونعود للرسالة الافتراضية.
        }

        return (int)response.StatusCode switch
        {
            401 => "انتهت الجلسة. يرجى تسجيل الدخول مجددًا.",
            403 => "لا تملك صلاحية تنفيذ هذه العملية.",
            404 => "العنصر المطلوب غير موجود.",
            409 => "تعارض: العنصر موجود مسبقًا.",
            _ => "تعذّر تنفيذ العملية. حاول مرة أخرى."
        };
    }
}
