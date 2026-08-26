using Contracts.Results;

namespace UniNet.Client.Services.Http;

// أساس مشترك لخدمات الـ API: العميل المُصادَق "Api" + التقاط أخطاء الاتصال + مساعدات استعلام.
public abstract class ApiServiceBase
{
    private readonly IHttpClientFactory _factory;
    protected ApiServiceBase(IHttpClientFactory factory) => _factory = factory;
    protected HttpClient Api => _factory.CreateClient("Api");

    protected async Task<ApiResult<T>> Send<T>(
        Func<HttpClient, Task<HttpResponseMessage>> call,
        Func<HttpResponseMessage, Task<ApiResult<T>>> read)
    {
        try { return await read(await call(Api)); }
        catch (HttpRequestException) { return ApiResult<T>.Fail("تعذّر الاتصال بالخادم.", 0); }
    }

    // يضيف معاملًا للاستعلام إن كانت القيمة غير فارغة.
    protected static string P(string name, string? value) =>
        string.IsNullOrWhiteSpace(value) ? "" : $"&{name}={Uri.EscapeDataString(value)}";

    protected static string P(string name, int? value) =>
        value is > 0 ? $"&{name}={value}" : "";

    protected static string P(string name, bool? value) =>
        value is null ? "" : $"&{name}={value.Value.ToString().ToLowerInvariant()}";
}
