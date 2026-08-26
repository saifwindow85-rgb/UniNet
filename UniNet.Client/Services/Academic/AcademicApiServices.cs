using System.Net.Http.Json;
using Contracts.Requests.AcademicRequests.CollegeRequests;
using Contracts.Requests.AcademicRequests.DepartmentRequests;
using Contracts.Requests.AcademicRequests.BatchRequests;
using Contracts.Requests.AcademicRequests.SectionRequests;
using Contracts.Responses.AcademicResponses.UniversityResponses;
using Contracts.Responses.AcademicResponses.CollegeResponses;
using Contracts.Responses.AcademicResponses.DepartmentResponses;
using Contracts.Responses.AcademicResponses.BatchResponses;
using Contracts.Responses.AcademicResponses.SectionResponses;
using Contracts.Results;
using UniNet.Client.Services.Http;

namespace UniNet.Client.Services.Academic;

// أساس مشترك: العميل المُصادَق + التقاط أخطاء الاتصال.
public abstract class AcademicServiceBase
{
    private readonly IHttpClientFactory _factory;
    protected AcademicServiceBase(IHttpClientFactory factory) => _factory = factory;
    protected HttpClient Api => _factory.CreateClient("Api");

    protected async Task<ApiResult<T>> Send<T>(Func<HttpClient, Task<HttpResponseMessage>> call, Func<HttpResponseMessage, Task<ApiResult<T>>> read)
    {
        try { return await read(await call(Api)); }
        catch (HttpRequestException) { return ApiResult<T>.Fail("تعذّر الاتصال بالخادم.", 0); }
    }

    protected static string Q(string? search) =>
        string.IsNullOrWhiteSpace(search) ? "" : $"&Search={Uri.EscapeDataString(search)}";
}

public class UniversityApiService : AcademicServiceBase
{
    public UniversityApiService(IHttpClientFactory f) : base(f) { }

    public Task<ApiResult<PagedResult<UniversityDTO>>> GetAllAsync(int page, int size, string? search = null) =>
        Send(api => api.GetAsync($"api/University?PageNumber={page}&PageSize={size}{Q(search)}"),
             ApiResponse.ReadAsync<PagedResult<UniversityDTO>>);

    public Task<ApiResult<UniversityDTO>> GetByIdAsync(int id) =>
        Send(api => api.GetAsync($"api/University/by-id?UniversityId={id}"), ApiResponse.ReadAsync<UniversityDTO>);

    public Task<ApiResult<UniversityDTO>> AddAsync(Contracts.Requests.AcademicRequests.UniversityRequests.AddUniversityDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/University", dto), ApiResponse.ReadAsync<UniversityDTO>);

    public Task<ApiResult<UniversityDTO>> UpdateAsync(int id, Contracts.Requests.AcademicRequests.UniversityRequests.UpdateUniversityDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/University?UniversityId={id}", dto), ApiResponse.ReadAsync<UniversityDTO>);

    public Task<ApiResult<bool>> DeleteAsync(int id) =>
        Send(api => api.DeleteAsync($"api/University?UniversityId={id}"), ApiResponse.ReadOkAsync);
}

public class CollegeApiService : AcademicServiceBase
{
    public CollegeApiService(IHttpClientFactory f) : base(f) { }

    // scope-aware: SuperAdmin يمرّر universityId للتصفية؛ المُقيَّد يُطبَّق نطاقه تلقائيًا في الخادم.
    public Task<ApiResult<PagedResult<CollegeDTO>>> GetPerUniversityAsync(int? universityId, int page, int size, string? search = null) =>
        Send(api => api.GetAsync($"api/College/by-universityId?PageNumber={page}&PageSize={size}{Q(search)}"
                                 + (universityId is > 0 ? $"&UniversityId={universityId}" : "")),
             ApiResponse.ReadAsync<PagedResult<CollegeDTO>>);

    public Task<ApiResult<CollegeDTO>> GetByIdAsync(int id) =>
        Send(api => api.GetAsync($"api/College/by-id?CollegeId={id}"), ApiResponse.ReadAsync<CollegeDTO>);

    public Task<ApiResult<CollegeDTO>> AddAsync(AddCollegeDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/College", dto), ApiResponse.ReadAsync<CollegeDTO>);

    public Task<ApiResult<CollegeDTO>> UpdateAsync(int id, UpdateCollegeDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/College?CollegeId={id}", dto), ApiResponse.ReadAsync<CollegeDTO>);

    public Task<ApiResult<bool>> DeleteAsync(int id) =>
        Send(api => api.DeleteAsync($"api/College?CollegeId={id}"), ApiResponse.ReadOkAsync);
}

public class DepartmentApiService : AcademicServiceBase
{
    public DepartmentApiService(IHttpClientFactory f) : base(f) { }

    // للـ SuperAdmin: كل الأقسام. (endpoint مقيّد بـ Super Admin)
    public Task<ApiResult<PagedResult<DepartmentDTO>>> GetAllAsync(int page, int size, string? search = null) =>
        Send(api => api.GetAsync($"api/Department?PageNumber={page}&PageSize={size}{Q(search)}"),
             ApiResponse.ReadAsync<PagedResult<DepartmentDTO>>);

    // scope-aware: يُطبَّق نطاق المستخدم؛ ويمكن تضييقه بكلية مختارة (filter.CollegeId).
    // ملاحظة: الـ endpoint يتطلب CollegeId في الاستعلام (قيد تعاقد) ويتجاهله فعليًا؛ نمرّر المختار أو 1.
    // ملاحظة: الـ endpoint يتطلب CollegeId ويستخدمه أيضًا كفلتر (AcademicFilter.CollegeId).
    // لذا CollegeAdmin يعمل (النطاق يفرض الكلية)، أمّا UniversityAdmin فيُقصَر على الكلية الممرَّرة —
    // لجمع كل أقسام الجامعة استخدم ScopeLookups.DepartmentsAsync بدل تمرير null هنا.
    public Task<ApiResult<PagedResult<DepartmentDTO>>> GetPerCollegeAsync(int? collegeId, int page, int size, string? search = null) =>
        Send(api => api.GetAsync($"api/Department/collegeId?CollegeId={(collegeId is > 0 ? collegeId : 1)}&PageNumber={page}&PageSize={size}{Q(search)}"),
             ApiResponse.ReadAsync<PagedResult<DepartmentDTO>>);

    public Task<ApiResult<DepartmentDTO>> GetByIdAsync(int id) =>
        Send(api => api.GetAsync($"api/Department/by-id?DepartmentId={id}"), ApiResponse.ReadAsync<DepartmentDTO>);

    public Task<ApiResult<DepartmentDTO>> AddAsync(AddDepartmentDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/Department", dto), ApiResponse.ReadAsync<DepartmentDTO>);

    public Task<ApiResult<DepartmentDTO>> UpdateAsync(int id, UpdateDepartmentDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/Department?DepartmentId={id}", dto), ApiResponse.ReadAsync<DepartmentDTO>);

    public Task<ApiResult<bool>> DeleteAsync(int id) =>
        Send(api => api.DeleteAsync($"api/Department?DepartmentId={id}"), ApiResponse.ReadOkAsync);
}

public class BatchApiService : AcademicServiceBase
{
    public BatchApiService(IHttpClientFactory f) : base(f) { }

    public Task<ApiResult<PagedResult<BatchDTO>>> GetAllAsync(int page, int size, string? search = null) =>
        Send(api => api.GetAsync($"api/Batch?PageNumber={page}&PageSize={size}{Q(search)}"),
             ApiResponse.ReadAsync<PagedResult<BatchDTO>>);

    public Task<ApiResult<PagedResult<BatchDTO>>> GetPerDepartmentAsync(int? departmentId, int page, int size, string? search = null) =>
        Send(api => api.GetAsync($"api/Batch/by-departmentId?PageNumber={page}&PageSize={size}{Q(search)}"
                                 + (departmentId is > 0 ? $"&DepartmentId={departmentId}" : "")),
             ApiResponse.ReadAsync<PagedResult<BatchDTO>>);

    public Task<ApiResult<BatchDTO>> GetByIdAsync(int id) =>
        Send(api => api.GetAsync($"api/Batch/by-id?BatchId={id}"), ApiResponse.ReadAsync<BatchDTO>);

    public Task<ApiResult<BatchDTO>> AddAsync(AddBatchDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/Batch", dto), ApiResponse.ReadAsync<BatchDTO>);

    public Task<ApiResult<BatchDTO>> UpdateAsync(int id, UpdateBatchDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/Batch?BatchId={id}", dto), ApiResponse.ReadAsync<BatchDTO>);

    public Task<ApiResult<bool>> DeleteAsync(int id) =>
        Send(api => api.DeleteAsync($"api/Batch?BatchId={id}"), ApiResponse.ReadOkAsync);
}

public class SectionApiService : AcademicServiceBase
{
    public SectionApiService(IHttpClientFactory f) : base(f) { }

    public Task<ApiResult<PagedResult<SectionDTO>>> GetAllAsync(int page, int size, string? search = null) =>
        Send(api => api.GetAsync($"api/Section?PageNumber={page}&PageSize={size}{Q(search)}"),
             ApiResponse.ReadAsync<PagedResult<SectionDTO>>);

    public Task<ApiResult<PagedResult<SectionDTO>>> GetPerBatchAsync(int? batchId, int page, int size, string? search = null) =>
        Send(api => api.GetAsync($"api/Section/batchId?PageNumber={page}&PageSize={size}{Q(search)}"
                                 + (batchId is > 0 ? $"&BatchId={batchId}" : "")),
             ApiResponse.ReadAsync<PagedResult<SectionDTO>>);

    public Task<ApiResult<SectionDTO>> AddAsync(AddSectionDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/Section", dto), ApiResponse.ReadAsync<SectionDTO>);

    public Task<ApiResult<SectionDTO>> UpdateAsync(int id, UpdateSectionDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/Section?SectionId={id}", dto), ApiResponse.ReadAsync<SectionDTO>);

    public Task<ApiResult<bool>> DeleteAsync(int id) =>
        Send(api => api.DeleteAsync($"api/Section?SectionId={id}"), ApiResponse.ReadOkAsync);
}
