using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace UniNet.Client.State;

// يقرأ نطاق المستخدم الحالي من مطالبات الـ JWT.
// SuperAdmin لا يحمل أي claim نطاق ⇒ حرية اختيار كاملة.
// المسؤول المُقيَّد يحمل أعلى مستوى في نطاقه (Univ/College/Dept/Batch) ⇒ يُثبَّت له كقراءة فقط.
public class ScopeContext
{
    private const string RoleUri = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    private readonly AuthenticationStateProvider _auth;

    public ScopeContext(AuthenticationStateProvider auth) => _auth = auth;

    public int? UniversityId { get; private set; }
    public int? CollegeId { get; private set; }
    public int? DepartmentId { get; private set; }
    public int? BatchId { get; private set; }
    public int? StudentId { get; private set; }
    public int? EmployeeId { get; private set; }
    public string Role { get; private set; } = "";
    public string UserType { get; private set; } = "";   // SystemAdmin | Employee | Student

    public bool IsSuperAdmin => Role == "Super Admin";
    public bool IsStudent => UserType == "Student";

    // تُقرأ في كل مرة (بلا تخزين مؤقّت) حتى لا تبقى قيم مستخدم سابق بعد تبديل الحساب —
    // ScopeContext في WASM يعيش كـ Singleton طوال الجلسة، فالتخزين المؤقّت كان يُبقي نطاق أول مستخدم.
    public async Task EnsureAsync()
    {
        var state = await _auth.GetAuthenticationStateAsync();
        var u = state.User;
        UniversityId = ParseInt(u, "UniversityId");
        CollegeId = ParseInt(u, "CollegeId");
        DepartmentId = ParseInt(u, "DepartmentId");
        BatchId = ParseInt(u, "BatchId");
        StudentId = ParseInt(u, "StudentId");
        EmployeeId = ParseInt(u, "EmployeeId");
        Role = u.FindFirst(RoleUri)?.Value ?? u.FindFirst(ClaimTypes.Role)?.Value ?? "";
        UserType = u.FindFirst("Type")?.Value ?? "";
    }

    private static int? ParseInt(ClaimsPrincipal u, string type) =>
        int.TryParse(u.FindFirst(type)?.Value, out var v) ? v : null;
}
