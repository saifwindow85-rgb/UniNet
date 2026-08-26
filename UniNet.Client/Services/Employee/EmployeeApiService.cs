using System.Net.Http.Json;
using Contracts.Requests.EmployeeRequests;
using Contracts.Requests.EmployeeRequests.UniversityAdminRequests;
using Contracts.Requests.EmployeeRequests.CollegeAdminRequests;
using Contracts.Requests.EmployeeRequests.DepartmentAdminRequests;
using Contracts.Responses.EmployeeResponse;
using Contracts.Results;
using UniNet.Client.Services.Http;

namespace UniNet.Client.Services.Employee;

// خدمة الموظفين: قائمة مُدركة للنطاق + إنشاء/تعديل مسؤولي الجامعة/الكلية/القسم.
// الخادم هو الفارض الحقيقي للنطاق والصلاحيات؛ الواجهة تُبسّط الاختيار فقط.
public class EmployeeApiService
{
    private readonly IHttpClientFactory _factory;
    public EmployeeApiService(IHttpClientFactory f) => _factory = f;
    private HttpClient Api => _factory.CreateClient("Api");

    private async Task<ApiResult<T>> Send<T>(Func<HttpClient, Task<HttpResponseMessage>> call, Func<HttpResponseMessage, Task<ApiResult<T>>> read)
    {
        try { return await read(await call(Api)); }
        catch (HttpRequestException) { return ApiResult<T>.Fail("تعذّر الاتصال بالخادم.", 0); }
    }

    // قائمة مُدركة للنطاق: SuperAdmin/UA/CA/DA — يُطبَّق نطاق المستخدم في الخادم.
    // يمكن التضييق بكلية/قسم (اختياري) وبحث نصّي وحالة التفعيل.
    public Task<ApiResult<PagedResult<EmployeeDTO>>> GetEmployeesAsync(
        int page, int size, string? search = null, bool? isActive = null, int? collegeId = null, int? departmentId = null)
    {
        var q = $"api/Employee?PageNumber={page}&PageSize={size}";
        if (!string.IsNullOrWhiteSpace(search)) q += $"&Search={Uri.EscapeDataString(search)}";
        if (isActive is not null) q += $"&IsActive={isActive.Value.ToString().ToLowerInvariant()}";
        if (collegeId is > 0) q += $"&CollegeId={collegeId}";
        if (departmentId is > 0) q += $"&DepartmentId={departmentId}";
        return Send(api => api.GetAsync(q), ApiResponse.ReadAsync<PagedResult<EmployeeDTO>>);
    }

    public Task<ApiResult<EmployeeDTO>> GetByIdAsync(int employeeId) =>
        Send(api => api.GetAsync($"api/Employee/by_id?EmployeeId={employeeId}"), ApiResponse.ReadAsync<EmployeeDTO>);

    // ---- إنشاء ----
    public Task<ApiResult<EmployeeDTO>> AddUniversityAdminAsync(AddUniversityAdminDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/Employee/university_admin", dto), ApiResponse.ReadAsync<EmployeeDTO>);

    public Task<ApiResult<EmployeeDTO>> AddCollegeAdminAsync(AddCollegeAdminDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/Employee/college_admin", dto), ApiResponse.ReadAsync<EmployeeDTO>);

    public Task<ApiResult<EmployeeDTO>> AddDepartmentAdminAsync(AddDepartmentAdminDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/Employee/department_admin", dto), ApiResponse.ReadAsync<EmployeeDTO>);

    // ---- تعديل ----
    public Task<ApiResult<EmployeeDTO>> UpdateUniversityAdminAsync(int employeeId, UpdateUniversityAdminDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/Employee/university_admin?EmployeeId={employeeId}", dto), ApiResponse.ReadAsync<EmployeeDTO>);

    public Task<ApiResult<EmployeeDTO>> UpdateCollegeAdminAsync(int employeeId, UpdateCollegeAdminDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/Employee/college_admin?EmployeeId={employeeId}", dto), ApiResponse.ReadAsync<EmployeeDTO>);

    public Task<ApiResult<EmployeeDTO>> UpdateDepartmentAdminAsync(int employeeId, UpdateDepartmentAdminDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/Employee/department_admin?EmployeeId={employeeId}", dto), ApiResponse.ReadAsync<EmployeeDTO>);
}
