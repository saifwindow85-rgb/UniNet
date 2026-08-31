using System.Net.Http.Headers;
using Contracts.Enums;
using Contracts.Requests.ContentRequests;
using Contracts.Responses.ContentResponses;
using Contracts.Results;
using Microsoft.AspNetCore.Components.Forms;
using UniNet.Client.Services.Http;

namespace UniNet.Client.Services.Content;

// بايتات صورة + نوعها — ما يحتاجه AuthImage ليبني Object URL.
public record ImageBytes(byte[] Bytes, string ContentType);

public class ContentApiService : ApiServiceBase
{
    // يطابق ImageStorage:MaxFileSizeInBytes في الخادم (5 ميجابايت).
    // OpenReadStream في Blazor يرفض أي ملف أكبر من الحد المُمرَّر، فنمنع الرفع الفاشل مبكرًا.
    public const long MaxImageBytes = 5 * 1024 * 1024;

    public ContentApiService(IHttpClientFactory f) : base(f) { }

    // ---------------------------------------------------------------- قراءة

    // الخلاصة: ما يقع المستخدم داخل جمهوره. لا معاملات نطاق — نطاقه يأتي من مطالباته في الخادم.
    public Task<ApiResult<PagedResult<ContentFeedItemDTO>>> GetFeedAsync(
        int page, int size, string? title = null, EnContentType? type = null) =>
        Send(api => api.GetAsync($"api/Content/feed?PageNumber={page}&PageSize={size}"
                                 + P("Title", title)
                                 + (type is null ? "" : $"&Type={(int)type}")),
             ApiResponse.ReadAsync<PagedResult<ContentFeedItemDTO>>);

    // قائمة الإدارة: ما يستطيع هذا المسؤول تعديله وحذفه — سؤال معاكس للخلاصة، ونقطة مختلفة.
    public Task<ApiResult<PagedResult<ContentItemDTO>>> GetManagedAsync(
        int page, int size, string? title = null, EnContentType? type = null,
        EnContentScope? scope = null, bool mineOnly = false) =>
        Send(api => api.GetAsync($"api/Content?PageNumber={page}&PageSize={size}"
                                 + P("Title", title)
                                 + (type is null ? "" : $"&Type={(int)type}")
                                 + (scope is null ? "" : $"&Scope={(int)scope}")
                                 + (mineOnly ? "&MineOnly=true" : "")),
             ApiResponse.ReadAsync<PagedResult<ContentItemDTO>>);

    public Task<ApiResult<DetaieldContentItemDTO>> GetByIdAsync(int contentItemId) =>
        Send(api => api.GetAsync($"api/Content/{contentItemId}"),
             ApiResponse.ReadAsync<DetaieldContentItemDTO>);

    // ---------------------------------------------------------------- الصورة

    // تُجلب بالرمز ثم تُسلَّم كبايتات؛ تحويلها إلى عنوان يصلح لـ img يتم في AuthImage.
    public async Task<ApiResult<ImageBytes>> GetImageAsync(int contentItemId)
    {
        try
        {
            var response = await Api.GetAsync($"api/Content/{contentItemId}/image");
            if (!response.IsSuccessStatusCode)
                return ApiResult<ImageBytes>.Fail("تعذّر جلب الصورة.", (int)response.StatusCode);

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/*";
            return ApiResult<ImageBytes>.Ok(new ImageBytes(bytes, contentType));
        }
        catch (HttpRequestException)
        {
            return ApiResult<ImageBytes>.Fail("تعذّر الاتصال بالخادم.", 0);
        }
    }

    // ---------------------------------------------------------------- كتابة

    // النوع يُحدَّد بالمسار لا بحقل في النموذج — نفس ما يفرضه الخادم.
    public Task<ApiResult<DetaieldContentItemDTO>> AddAsync(
        EnContentType type, AddContentDTO dto, IBrowserFile? image)
    {
        var route = type == EnContentType.Announcement ? "announcement" : "post";

        return Send(api =>
        {
            var form = BuildForm(dto.Title, dto.Body, image);
            form.Add(new StringContent(((int)dto.Scope).ToString()), "Scope");
            if (dto.TargetId is int target)
                form.Add(new StringContent(target.ToString()), "TargetId");

            return api.PostAsync($"api/Content/{route}", form);
        }, ApiResponse.ReadAsync<DetaieldContentItemDTO>);
    }

    // RemoveImage هو الحالة الثالثة: بدونه يبقى غياب الملف غامضًا بين إبقاء الصورة وحذفها.
    public Task<ApiResult<DetaieldContentItemDTO>> UpdateAsync(
        int contentItemId, UpdateContentDTO dto, IBrowserFile? image)
    {
        return Send(api =>
        {
            var form = BuildForm(dto.Title, dto.Body, image);
            form.Add(new StringContent(dto.RemoveImage.ToString().ToLowerInvariant()), "RemoveImage");
            return api.PutAsync($"api/Content/{contentItemId}", form);
        }, ApiResponse.ReadAsync<DetaieldContentItemDTO>);
    }

    public Task<ApiResult<bool>> DeleteAsync(int contentItemId) =>
        Send(api => api.DeleteAsync($"api/Content/{contentItemId}"), ApiResponse.ReadOkAsync);

    private static MultipartFormDataContent BuildForm(string title, string body, IBrowserFile? image)
    {
        var form = new MultipartFormDataContent
        {
            { new StringContent(title), "Title" },
            { new StringContent(body), "Body" },
        };

        if (image is not null)
        {
            var stream = new StreamContent(image.OpenReadStream(MaxImageBytes));
            stream.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
            form.Add(stream, "Image", image.Name);
        }

        return form;
    }
}
