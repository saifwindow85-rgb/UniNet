using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using UniNet.Client.Services.Http;

namespace UniNet.Client.State;

// مزوّد حالة المصادقة: يقرأ الـ JWT من التخزين، يفكّ مطالباته (الدور + النطاق)،
// ويعرّض ClaimsPrincipal لبقية الواجهة. التحقق الفعلي يبقى على الخادم.
public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly TokenStore _store;

    public JwtAuthStateProvider(TokenStore store) => _store = store;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _store.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token) || JwtParser.IsExpired(token))
            return Anonymous;

        var identity = new ClaimsIdentity(JwtParser.Parse(token), authenticationType: "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
