using Contracts.Common.AuthorizationInfos.ContentAuthorizationInfo;
using Contracts.Requests.ContentRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses.ContentResponses;
using Contracts.Results;
using Domain.Entities.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.ContentInterfaces
{
    /// <summary>
    /// مستودع واحد للهرم كله (ContentItem و Post و Announcement) لا ثلاثة.
    /// TPH يضع الأنواع الثلاثة في جدول ContentItems الواحد، و IQueryable متغاير،
    /// فإسقاطة Expression مشتركة تعمل على DbSet المشتق وتُصدِر شرط المميّز تلقائيًا.
    /// ثلاثة مستودعات كانت ستُثلِّث إسقاطات متطابقة بلا أي مكسب في الاستعلام.
    /// </summary>
    public interface IContentRepository
    {
        void Add(ContentItem contentItem);

        bool Delete(ContentItem contentItem);

        Task<ContentItem?> GetEntityById(int contentItemId);

        Task<bool> IsExistsById(int contentItemId);

        Task<DetaieldContentItemDTO?> GetDetaieldContentItemDTOById(int contentItemId);

        /// <summary>يُغذّي ContentViewPolicy.</summary>
        Task<ContentViewInfo?> GetContentViewInfoAsync(int contentItemId);

        /// <summary>يُغذّي ContentManagePolicy — نوع مختلف عن السابق عمدًا.</summary>
        Task<ContentManageInfo?> GetContentManageInfoAsync(int contentItemId);

        /// <summary>ماذا يرى هذا المستخدم؟ مُسنَد جمهور.</summary>
        Task<PagedResult<ContentFeedItemDTO>> GetFeed(UserScope? viewer, ContentFeedFilterDTO? filter,
            int pageNumber, int pageSize);

        /// <summary>ماذا يُدير هذا المسؤول؟ مُسنَد احتواء — سؤال معاكس، واستعلام منفصل.</summary>
        Task<PagedResult<ContentItemDTO>> GetManagedContent(UserScope? actor, ContentFilterDTO? filter,
            int currentUserId, int pageNumber, int pageSize);
    }
}
