using System.Net.Http.Json;
using Contracts.Requests.IdentityRequests.UserRoleRequsets;
using Contracts.Responses.IdentityResponses.UserRoleResponse;
using Contracts.Results;
using UniNet.Client.Services.Http;

namespace UniNet.Client.Services.Identity;

public class UserRoleApiService
{
    private readonly IHttpClientFactory _factory;
    public UserRoleApiService(IHttpClientFactory factory) => _factory = factory;
    private HttpClient Api => _factory.CreateClient("Api");

    public Task<ApiResult<PagedResult<UserRoleDTO>>> GetAllAsync(int page, int size) =>
        Send(api => api.GetAsync($"api/UserRole?PageNumber={page}&PageSize={size}"),
             ApiResponse.ReadAsync<PagedResult<UserRoleDTO>>);

    public Task<ApiResult<PagedResult<UserRoleDTO>>> GetByRoleAsync(int roleId, int page, int size) =>
        Send(api => api.GetAsync($"api/UserRole/roleid?RoleId={roleId}&PageNumber={page}&PageSize={size}"),
             ApiResponse.ReadAsync<PagedResult<UserRoleDTO>>);

    public Task<ApiResult<UserRoleDTO>> AddAsync(AddUserRoleDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/UserRole", dto), ApiResponse.ReadAsync<UserRoleDTO>);

    public Task<ApiResult<bool>> DeleteAsync(int userId, int roleId) =>
        Send(api => api.DeleteAsync($"api/UserRole?UserId={userId}&RoleId={roleId}"), ApiResponse.ReadOkAsync);

    private async Task<ApiResult<T>> Send<T>(Func<HttpClient, Task<HttpResponseMessage>> call, Func<HttpResponseMessage, Task<ApiResult<T>>> read)
    {
        try { return await read(await call(Api)); }
        catch (HttpRequestException) { return ApiResult<T>.Fail("تعذّر الاتصال بالخادم.", 0); }
    }
}
