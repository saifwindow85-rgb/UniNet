using Contracts.Common.AuthorizationInfos.ContentAuthorizationInfo;
using Contracts.Enums;
using Contracts.Requests.ContentRequests;
using Contracts.Requests.ImageRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.ContentResponses;
using Contracts.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ContentInterfaces
{
    /// <summary>
    /// خدمة واحدة للنوعين. النوع وسيطٌ لا حقل في الـ DTO: يشتقّه الـ Controller من المسار،
    /// فلا يستطيع عميلٌ نشر إعلان عبر نقطة المنشورات ليتجاوز فحص الأدوار الخاص بها.
    /// </summary>
    public interface IContentService
    {
        Task<PagedResult<ContentFeedItemDTO>> GetFeed(UserScope? viewer, ContentFeedFilterDTO? filter,
            int pageNumber, int pageSize);

        Task<PagedResult<ContentItemDTO>> GetManagedContent(UserScope? actor, ContentFilterDTO? filter,
            int currentUserId, int pageNumber, int pageSize);

        Task<DetaieldContentItemDTO?> GetDetaieldContentItemDTOById(int contentItemId);

        Task<ContentViewInfo?> GetContentViewInfoAsync(int contentItemId);

        Task<ContentManageInfo?> GetContentManageInfoAsync(int contentItemId);

        Task<bool> IsExistsById(int contentItemId);

        Task<AddUpdateServiceResponse<DetaieldContentItemDTO>> AddContent(UserScope? scope, AddContentDTO newContent,
            UploadedFileDTO? file, EnContentType type, int currentUserId);

        Task<AddUpdateServiceResponse<DetaieldContentItemDTO>> UpdateContent(UserScope? scope,
            UpdateContentDTO updatedContent, UploadedFileDTO? file, int contentItemId, int currentUserId);

        Task<bool> Delete(int contentItemId);
    }
}
