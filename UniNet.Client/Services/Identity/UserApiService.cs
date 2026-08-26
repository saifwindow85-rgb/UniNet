using System.Net.Http.Json;
using Contracts.Requests.UserRequests;
using Contracts.Responses.IdentityResponses;
using UniNet.Client.Services.Http;

namespace UniNet.Client.Services.Identity;

public class UserApiService
{
    private readonly IHttpClientFactory _factory;
    public UserApiService(IHttpClientFactory factory) => _factory = factory;
    private HttpClient Api => _factory.CreateClient("Api");

    public Task<ApiResult<UserDTO>> GetUserByIdAsync(int userId) =>
        Send(api => api.GetAsync($"api/User/Id?UserId={userId}"), ApiResponse.ReadAsync<UserDTO>);

    public Task<ApiResult<UserDTO>> AddUserAsync(AddUserDTO dto) =>
        Send(api => api.PostAsJsonAsync("api/User", dto), ApiResponse.ReadAsync<UserDTO>);

    public Task<ApiResult<UserDTO>> UpdateUserAsync(int userId, UpdateUserDTO dto) =>
        Send(api => api.PutAsJsonAsync($"api/User?UserId={userId}", dto), ApiResponse.ReadAsync<UserDTO>);

    private async Task<ApiResult<T>> Send<T>(Func<HttpClient, Task<HttpResponseMessage>> call, Func<HttpResponseMessage, Task<ApiResult<T>>> read)
    {
        try { return await read(await call(Api)); }
        catch (HttpRequestException) { return ApiResult<T>.Fail("تعذّر الاتصال بالخادم.", 0); }
    }
}
