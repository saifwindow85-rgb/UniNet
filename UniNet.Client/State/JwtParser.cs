using System.Security.Claims;
using System.Text.Json;

namespace UniNet.Client.State;

// يفكّ حمولة الـ JWT (بدون تحقق توقيع — التحقق مسؤولية الخادم) ويحوّلها إلى مطالبات.
public static class JwtParser
{
    public static IReadOnlyList<Claim> Parse(string jwt)
    {
        var claims = new List<Claim>();
        var parts = jwt.Split('.');
        if (parts.Length < 2)
            return claims;

        var payload = Decode(parts[1]);
        var map = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payload);
        if (map is null)
            return claims;

        foreach (var kvp in map)
        {
            if (kvp.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in kvp.Value.EnumerateArray())
                    claims.Add(new Claim(kvp.Key, item.ToString()));
            }
            else
            {
                claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
            }
        }
        return claims;
    }

    public static DateTimeOffset? GetExpiry(string jwt)
    {
        var exp = Parse(jwt).FirstOrDefault(c => c.Type == "exp");
        if (exp is not null && long.TryParse(exp.Value, out var seconds))
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
        return null;
    }

    public static bool IsExpired(string jwt)
    {
        var expiry = GetExpiry(jwt);
        // هامش 10 ثوانٍ لتفادي حالة الحدّ.
        return expiry is null || expiry <= DateTimeOffset.UtcNow.AddSeconds(10);
    }

    private static string Decode(string base64Url)
    {
        var s = base64Url.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }
        var bytes = Convert.FromBase64String(s);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}
