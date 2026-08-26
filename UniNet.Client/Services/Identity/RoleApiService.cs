using System.Net.Http.Json;
using Contracts.Requests.IdentityRequests.RoleRequests;
using Contracts.Responses.IdentityResponses.RoleResponses;
using Contracts.Results;
using UniNet.Client.Services.Http;

namespace UniNet.Client.Services.Identity;

public class RoleApiService
{
    private readonly IHttpClientFactory _factory;
    public RoleApiService(IHttpClientFactory factory) => _factory = factory;
    private HttpClient Api => _factory.CreateClient("Api");

    public Task<ApiResult<PagedResult<RoleDTO>>> GetRolesAsync(int page, int size) =>
        Send(async api => await api.GetAsync($"api/Role?PageNumber={page}&PageSize={size}"),
             ApiResponse.ReadAsync<PagedResult<RoleDTO>>);

    public Task<ApiResult<RoleDTO>> AddRoleAsync(AddRoleDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/Role", dto), ApiResponse.ReadAsync<RoleDTO>);

    public Task<ApiResult<RoleDTO>> UpdateRoleAsync(int roleId, AddRoleDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/Role?RoleId={roleId}", dto), ApiResponse.ReadAsync<RoleDTO>);

    public Task<ApiResult<bool>> DeleteRoleAsync(int roleId) =>
        Send(api => api.DeleteAsync($"api/Role?RoleId={roleId}"), ApiResponse.ReadOkAsync);

    private async Task<ApiResult<T>> Send<T>(Func<HttpClient, Task<HttpResponseMessage>> call, Func<HttpResponseMessage, Task<ApiResult<T>>> read)
    {
        try { return await read(await call(Api)); }
        catch (HttpRequestException) { return ApiResult<T>.Fail("تعذّر الاتصال بالخادم.", 0); }
    }
}
