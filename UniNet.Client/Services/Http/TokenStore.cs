using Microsoft.JSInterop;

namespace UniNet.Client.Services.Http;

// تخزين الرمزين في localStorage. (أداة داخلية: عمر وصول قصير + تدوير Refresh يحدّان أثر XSS.)
public class TokenStore
{
    private const string AccessKey = "uninet.accessToken";
    private const string RefreshKey = "uninet.refreshToken";
    private readonly IJSRuntime _js;

    public TokenStore(IJSRuntime js) => _js = js;

    public async Task SaveAsync(string accessToken, string refreshToken)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", AccessKey, accessToken);
        await _js.InvokeVoidAsync("localStorage.setItem", RefreshKey, refreshToken);
    }

    public ValueTask<string?> GetAccessTokenAsync() =>
        _js.InvokeAsync<string?>("localStorage.getItem", AccessKey);

    public ValueTask<string?> GetRefreshTokenAsync() =>
        _js.InvokeAsync<string?>("localStorage.getItem", RefreshKey);

    public async Task ClearAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", AccessKey);
        await _js.InvokeVoidAsync("localStorage.removeItem", RefreshKey);
    }
}
