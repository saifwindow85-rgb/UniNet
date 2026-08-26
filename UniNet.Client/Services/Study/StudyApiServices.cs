using System.Net.Http.Json;
using Contracts.Requests.StudyRequestes.SubjectRequests;
using Contracts.Requests.StudyRequestes.SemesterRequests;
using Contracts.Requests.StudyRequestes.SectionSubjectRequests;
using Contracts.Requests.StudyRequestes.StudentResultRequests;
using Contracts.Responses.StudyResponses.SubjectResponses;
using Contracts.Responses.StudyResponses.SemesterResponses;
using Contracts.Responses.StudyResponses.SectionSubjectResponses;
using Contracts.Responses.StudyResponses.StudentResultResponses;
using Contracts.Results;
using UniNet.Client.Services.Http;

namespace UniNet.Client.Services.Study;

// ============ المواد (Subjects) ============
public class SubjectApiService : ApiServiceBase
{
    public SubjectApiService(IHttpClientFactory f) : base(f) { }

    // SA فقط — كل المواد.
    public Task<ApiResult<PagedResult<SubjectDTO>>> GetAllAsync(int page, int size, string? code = null, string? name = null, int? deptId = null) =>
        Send(api => api.GetAsync($"api/Subject?PageNumber={page}&PageSize={size}{P("Code", code)}{P("Name", name)}{P("DepartmentId", deptId)}"),
             ApiResponse.ReadAsync<PagedResult<SubjectDTO>>);

    // scoped — يُطبَّق نطاق المستخدم؛ يمكن تضييقه بقسم.
    public Task<ApiResult<PagedResult<SubjectDTO>>> GetPerDepartmentAsync(int page, int size, string? code = null, string? name = null, int? deptId = null) =>
        Send(api => api.GetAsync($"api/Subject/by-departmentId?PageNumber={page}&PageSize={size}{P("Code", code)}{P("Name", name)}{P("DepartmentId", deptId)}"),
             ApiResponse.ReadAsync<PagedResult<SubjectDTO>>);

    public Task<ApiResult<DetaieldSubjectDTO>> GetByIdAsync(int id) =>
        Send(api => api.GetAsync($"api/Subject/by-id?SubjectId={id}"), ApiResponse.ReadAsync<DetaieldSubjectDTO>);

    public Task<ApiResult<SubjectDTO>> AddAsync(AddSubjectDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/Subject", dto), ApiResponse.ReadAsync<SubjectDTO>);

    public Task<ApiResult<SubjectDTO>> UpdateAsync(int id, UpdateSubjectDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/Subject?SubjectId={id}", dto), ApiResponse.ReadAsync<SubjectDTO>);

    public Task<ApiResult<bool>> DeleteAsync(int id) =>
        Send(api => api.DeleteAsync($"api/Subject?SubjectId={id}"), ApiResponse.ReadOkAsync);
}

// ============ الفصول الدراسية (Semesters) ============
public class SemesterApiService : ApiServiceBase
{
    public SemesterApiService(IHttpClientFactory f) : base(f) { }

    public Task<ApiResult<PagedResult<SemesterDTO>>> GetAllAsync(int page, int size, string? name = null, int? universityId = null, bool? isCurrent = null) =>
        Send(api => api.GetAsync($"api/Semester?PageNumber={page}&PageSize={size}{P("Name", name)}{P("UniversityId", universityId)}{P("IsCurrent", isCurrent)}"),
             ApiResponse.ReadAsync<PagedResult<SemesterDTO>>);

    public Task<ApiResult<PagedResult<SemesterDTO>>> GetPerUniversityAsync(int page, int size, string? name = null, int? universityId = null, bool? isCurrent = null) =>
        Send(api => api.GetAsync($"api/Semester/by-universityId?PageNumber={page}&PageSize={size}{P("Name", name)}{P("UniversityId", universityId)}{P("IsCurrent", isCurrent)}"),
             ApiResponse.ReadAsync<PagedResult<SemesterDTO>>);

    public Task<ApiResult<DetaieldSemesterDTO>> GetByIdAsync(int id) =>
        Send(api => api.GetAsync($"api/Semester/by-id?SemesterId={id}"), ApiResponse.ReadAsync<DetaieldSemesterDTO>);

    public Task<ApiResult<SemesterDTO>> GetCurrentAsync(int universityId) =>
        Send(api => api.GetAsync($"api/Semester/current?UniversityId={universityId}"), ApiResponse.ReadAsync<SemesterDTO>);

    public Task<ApiResult<SemesterDTO>> AddAsync(AddSemesterDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/Semester", dto), ApiResponse.ReadAsync<SemesterDTO>);

    public Task<ApiResult<SemesterDTO>> UpdateAsync(int id, UpdateSemesterDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/Semester?SemesterId={id}", dto), ApiResponse.ReadAsync<SemesterDTO>);

    public Task<ApiResult<SemesterDTO>> EndAsync(int id) =>
        Send(api => api.PutAsync($"api/Semester/end?SemesterId={id}", null), ApiResponse.ReadAsync<SemesterDTO>);

    public Task<ApiResult<bool>> DeleteAsync(int id) =>
        Send(api => api.DeleteAsync($"api/Semester?SemesterId={id}"), ApiResponse.ReadOkAsync);
}

// ============ ربط المادة بالشعبة (SectionSubjects) ============
public class SectionSubjectApiService : ApiServiceBase
{
    public SectionSubjectApiService(IHttpClientFactory f) : base(f) { }

    public Task<ApiResult<PagedResult<SectionSubjectDTO>>> GetAllAsync(int page, int size, int? sectionId = null, int? subjectId = null, int? semesterId = null, string? lecturer = null) =>
        Send(api => api.GetAsync($"api/SectionSubject?PageNumber={page}&PageSize={size}{P("SectionId", sectionId)}{P("SubjectId", subjectId)}{P("SemesterId", semesterId)}{P("LecturerName", lecturer)}"),
             ApiResponse.ReadAsync<PagedResult<SectionSubjectDTO>>);

    public Task<ApiResult<PagedResult<SectionSubjectDTO>>> GetPerScopeAsync(int page, int size, int? sectionId = null, int? subjectId = null, int? semesterId = null, string? lecturer = null) =>
        Send(api => api.GetAsync($"api/SectionSubject/scoped?PageNumber={page}&PageSize={size}{P("SectionId", sectionId)}{P("SubjectId", subjectId)}{P("SemesterId", semesterId)}{P("LecturerName", lecturer)}"),
             ApiResponse.ReadAsync<PagedResult<SectionSubjectDTO>>);

    public Task<ApiResult<DetaieldSectionSubjectDTO>> GetByIdAsync(int id) =>
        Send(api => api.GetAsync($"api/SectionSubject/by-id?SectionSubjectId={id}"), ApiResponse.ReadAsync<DetaieldSectionSubjectDTO>);

    public Task<ApiResult<SectionSubjectDTO>> AddAsync(AddSectionSubjectDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/SectionSubject", dto), ApiResponse.ReadAsync<SectionSubjectDTO>);

    public Task<ApiResult<SectionSubjectDTO>> UpdateAsync(int id, UpdateSectionSubjectDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/SectionSubject?SectionSubjectId={id}", dto), ApiResponse.ReadAsync<SectionSubjectDTO>);

    public Task<ApiResult<bool>> DeleteAsync(int id) =>
        Send(api => api.DeleteAsync($"api/SectionSubject?SectionSubjectId={id}"), ApiResponse.ReadOkAsync);
}

// ============ نتائج الطلاب (StudentResults) ============
public class StudentResultApiService : ApiServiceBase
{
    public StudentResultApiService(IHttpClientFactory f) : base(f) { }

    // SA فقط — كل النتائج مُفصّلة ومُصفّحة.
    public Task<ApiResult<PagedResult<DetaieldStudentResultDTO>>> GetAllAsync(int page, int size, StudentResultFilterDTO filter) =>
        Send(api => api.GetAsync($"api/StudentResult?PageNumber={page}&PageSize={size}{FilterQuery(filter)}"),
             ApiResponse.ReadAsync<PagedResult<DetaieldStudentResultDTO>>);

    public Task<ApiResult<DetaieldStudentResultDTO>> GetByIdAsync(int id) =>
        Send(api => api.GetAsync($"api/StudentResult/by-id?StudentResultId={id}"), ApiResponse.ReadAsync<DetaieldStudentResultDTO>);

    // كشف درجات طالب واحد — مجمّع حسب الفصل مع المعدّل.
    public Task<ApiResult<List<StudentSemesterResultDTO>>> GetPerStudentAsync(int studentId, int? semesterId = null) =>
        Send(api => api.GetAsync($"api/StudentResult/student?StudentId={studentId}{P("SemesterId", semesterId)}"),
             ApiResponse.ReadAsync<List<StudentSemesterResultDTO>>);

    // تقرير كل الطلاب ضمن النطاق — مجمّع حسب الطالب/الفصل.
    public Task<ApiResult<List<StudentSemesterResultDTO>>> GetReportAsync(StudentResultFilterDTO filter) =>
        Send(api => api.GetAsync($"api/StudentResult/report?_=1{FilterQuery(filter)}"),
             ApiResponse.ReadAsync<List<StudentSemesterResultDTO>>);

    public Task<ApiResult<DetaieldStudentResultDTO>> AddAsync(AddStudentResultDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/StudentResult", dto), ApiResponse.ReadAsync<DetaieldStudentResultDTO>);

    public Task<ApiResult<DetaieldStudentResultDTO>> UpdateAsync(int id, UpdateStudentResultDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/StudentResult?StudentResultId={id}", dto), ApiResponse.ReadAsync<DetaieldStudentResultDTO>);

    public Task<ApiResult<bool>> DeleteAsync(int id) =>
        Send(api => api.DeleteAsync($"api/StudentResult?StudentResultId={id}"), ApiResponse.ReadOkAsync);

    private static string FilterQuery(StudentResultFilterDTO f) =>
        P("StudentId", f.StudentId) + P("SectionSubjectId", f.SectionSubjectId) + P("SubjectId", f.SubjectId)
        + P("SemesterId", f.SemesterId) + P("SectionId", f.SectionId)
        + P("StudentName", f.StudentName) + P("StudentNumber", f.StudentNumber)
        + P("SubjectName", f.SubjectName) + P("SubjectCode", f.SubjectCode) + P("LecturerName", f.LecturerName);
}
