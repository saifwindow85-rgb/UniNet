using System.Net.Http.Json;
using Contracts.Requests.StudentRequests;
using Contracts.Responses.StudentResponses;
using Contracts.Results;
using UniNet.Client.Services.Http;

namespace UniNet.Client.Services.Students;

// ============ الطلاب (Students) ============
public class StudentApiService : ApiServiceBase
{
    public StudentApiService(IHttpClientFactory f) : base(f) { }

    // scoped — يُطبَّق نطاق المستخدم؛ يمكن التصفية بالدفعة/الحالة/التفعيل والبحث.
    public Task<ApiResult<PagedResult<StudentDTO>>> GetStudentsAsync(
        int page, int size, string? fullName = null, string? studentNumber = null, int? status = null, bool? isActive = null, int? batchId = null) =>
        Send(api => api.GetAsync($"api/Student?PageNumber={page}&PageSize={size}"
                                 + P("FullName", fullName) + P("StudentNumber", studentNumber)
                                 + P("Status", status) + P("IsActive", isActive) + P("BatchId", batchId)),
             ApiResponse.ReadAsync<PagedResult<StudentDTO>>);

    public Task<ApiResult<StudentDTO>> GetByIdAsync(int id) =>
        Send(api => api.GetAsync($"api/Student/by_id?Id={id}"), ApiResponse.ReadAsync<StudentDTO>);

    // إنشاء/تعديل طالب عادي.
    public Task<ApiResult<StudentDTO>> AddStudentAsync(AddStudentDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/Student/student", dto), ApiResponse.ReadAsync<StudentDTO>);

    public Task<ApiResult<StudentDTO>> UpdateStudentAsync(int id, UpdateStudentDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/Student/student?Id={id}", dto), ApiResponse.ReadAsync<StudentDTO>);

    // إنشاء/تعديل طالب بصلاحية مدير دفعة (BatchAdmin).
    public Task<ApiResult<StudentDTO>> AddBatchAdminAsync(AddStudentDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/Student/batch_admin", dto), ApiResponse.ReadAsync<StudentDTO>);

    public Task<ApiResult<StudentDTO>> UpdateBatchAdminAsync(int id, UpdateStudentDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/Student/batch_admin?Id={id}", dto), ApiResponse.ReadAsync<StudentDTO>);

    // تغيير حالة الطالب.
    public Task<ApiResult<StudentDTO>> UpdateStatusAsync(int studentId, int statusId) =>
        Send(api => api.PutAsync($"api/Student/status?StudentId={studentId}&StudentStatusId={statusId}", null),
             ApiResponse.ReadAsync<StudentDTO>);
}

// ============ حالات الطالب (StudentStatuses) — SA فقط ============
public class StudentStatusApiService : ApiServiceBase
{
    public StudentStatusApiService(IHttpClientFactory f) : base(f) { }

    public Task<ApiResult<PagedResult<StudentStatusDTO>>> GetAllAsync(int page, int size) =>
        Send(api => api.GetAsync($"api/StudentStatus?PageNumber={page}&PageSize={size}"),
             ApiResponse.ReadAsync<PagedResult<StudentStatusDTO>>);

    public Task<ApiResult<StudentStatusDTO>> GetByIdAsync(int id) =>
        Send(api => api.GetAsync($"api/StudentStatus/by_id?Id={id}"), ApiResponse.ReadAsync<StudentStatusDTO>);

    public Task<ApiResult<StudentStatusDTO>> AddAsync(AddUpdateStudentStatusDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/StudentStatus", dto), ApiResponse.ReadAsync<StudentStatusDTO>);

    public Task<ApiResult<StudentStatusDTO>> UpdateAsync(int id, AddUpdateStudentStatusDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/StudentStatus?Id={id}", dto), ApiResponse.ReadAsync<StudentStatusDTO>);
}
