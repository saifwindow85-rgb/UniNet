using Contracts.Common.AuthorizationInfos.ContentAuthorizationInfo;
using Contracts.Enums;
using Contracts.Requests.ContentRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses.ContentResponses;
using Contracts.Results;
using DataAccessLayer.Dbcontext;
using DataAccessLayer.Extensions;
using Domain.Entities.Content;
using Domain.Interfaces.ContentInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.ContentRepository
{
    public class ContentRepository : IContentRepository
    {
        private readonly AppDbcontext _context;

        // c.Image != null يُترجَم إلى EXISTS على الجدول التابع — لا رحلة ثانية ولا Include.
        // ولا يُستعمل ?. في أي إسقاطة: أشجار التعبير لا تقبله (CS8072)، والبديل ثلاثيّ صريح.
        private readonly Expression<Func<ContentItem, ContentFeedItemDTO>> ToFeedDto = c => new ContentFeedItemDTO
        {
            ContentItemId = c.ContentItemId,
            Title = c.Title,
            Body = c.Body,
            Type = c.Type,
            AuthorName = c.CreatedByUser.FullName,
            CreatedAt = c.CreatedAt,
            HasImage = c.Image != null,
        };

        private readonly Expression<Func<ContentItem, ContentItemDTO>> ToDto = c => new ContentItemDTO
        {
            ContentItemId = c.ContentItemId,
            Title = c.Title,
            Body = c.Body,
            Type = c.Type,
            Scope = c.Scope,
            UniversityId = c.UniversityId,
            CollegeId = c.CollegeId,
            DepartmentId = c.DepartmentId,
            BatchId = c.BatchId,
            CreatedByUserId = c.CreatedByUserId,
            AuthorName = c.CreatedByUser.FullName,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            HasImage = c.Image != null,
        };

        private readonly Expression<Func<ContentItem, DetaieldContentItemDTO>> ToDetaieldDto = c => new DetaieldContentItemDTO
        {
            ContentItemId = c.ContentItemId,
            Title = c.Title,
            Body = c.Body,
            Type = c.Type,
            Scope = c.Scope,
            UniversityId = c.UniversityId,
            UniversityName = c.University == null ? null : c.University.Name,
            CollegeId = c.CollegeId,
            CollegeName = c.College == null ? null : c.College.Name,
            DepartmentId = c.DepartmentId,
            DepartmentName = c.Department == null ? null : c.Department.Name,
            BatchId = c.BatchId,
            BatchName = c.Batch == null ? null : c.Batch.Name,
            HasImage = c.Image != null,
            ImageOriginalFileName = c.Image == null ? null : c.Image.OriginalFileName,
            CreatedAt = c.CreatedAt,
            CreatedByUserId = c.CreatedByUserId,
            CreatedByUserName = c.CreatedByUser.UserName,
            UpdatedAt = c.UpdatedAt,
            UpdatedByUserId = c.UpdatedByUserId,
            UpdatedByUserName = c.UpdatedByUser == null ? null : c.UpdatedByUser.UserName,
        };

        private readonly Expression<Func<ContentItem, ContentViewInfo>> ToViewInfo = c => new ContentViewInfo
        {
            Scope = c.Scope,
            UniversityId = c.UniversityId,
            CollegeId = c.CollegeId,
            DepartmentId = c.DepartmentId,
            BatchId = c.BatchId,
        };

        private readonly Expression<Func<ContentItem, ContentManageInfo>> ToManageInfo = c => new ContentManageInfo
        {
            Scope = c.Scope,
            UniversityId = c.UniversityId,
            CollegeId = c.CollegeId,
            DepartmentId = c.DepartmentId,
            BatchId = c.BatchId,
            CreatedByUserId = c.CreatedByUserId,
        };

        public ContentRepository(AppDbcontext context)
        {
            _context = context;
        }

        public void Add(ContentItem contentItem)
        {
            // DbSet الجذر يقبل النوع المشتق ويكتب المميّز الصحيح بنفسه
            _context.ContentItems.Add(contentItem);
        }

        public bool Delete(ContentItem contentItem)
        {
            if (contentItem == null)
                return false;

            _context.ContentItems.Remove(contentItem);
            return true;
        }

        public async Task<ContentItem?> GetEntityById(int contentItemId)
        {
            return await _context.ContentItems.FindAsync(contentItemId);
        }

        public async Task<bool> IsExistsById(int contentItemId)
        {
            return await _context.ContentItems.AnyAsync(c => c.ContentItemId == contentItemId);
        }

        public async Task<DetaieldContentItemDTO?> GetDetaieldContentItemDTOById(int contentItemId)
        {
            return await _context.ContentItems.AsNoTracking()
                .Where(c => c.ContentItemId == contentItemId)
                .Select(ToDetaieldDto)
                .SingleOrDefaultAsync();
        }

        public async Task<ContentViewInfo?> GetContentViewInfoAsync(int contentItemId)
        {
            return await _context.ContentItems.AsNoTracking()
                .Where(c => c.ContentItemId == contentItemId)
                .Select(ToViewInfo)
                .SingleOrDefaultAsync();
        }

        public async Task<ContentManageInfo?> GetContentManageInfoAsync(int contentItemId)
        {
            return await _context.ContentItems.AsNoTracking()
                .Where(c => c.ContentItemId == contentItemId)
                .Select(ToManageInfo)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedResult<ContentFeedItemDTO>> GetFeed(UserScope? viewer, ContentFeedFilterDTO? filter,
            int pageNumber, int pageSize)
        {
            filter ??= new ContentFeedFilterDTO();
            viewer ??= new UserScope();

            var query = _context.ContentItems.AsNoTracking().AsQueryable();

            if (filter.Type.HasValue)
                query = query.Where(c => c.Type == filter.Type.Value);

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(c => EF.Functions.Like(c.Title, $"%{filter.Title}%"));

            if (!viewer.IsGlobal)
            {
                // صفر بديلًا عن null، وهذا ليس تجميلًا:
                // EF يحوّل "= @p" إلى "IS NULL" حين تكون قيمة الوسيط null، فيولّد شكل SQL
                // مختلفًا لكل تركيبة من تركيبات العدم الستّ عشرة ويُشظّي ذاكرة الخطط.
                // كل المفاتيح IDENTITY(1,1) فلا صفّ يساوي صفرًا، والمقارنة بـ int غير قابل
                // للعدم تُبقي شكل الاستعلام واحدًا وصحّته مستقلة عن قيد CHECK.
                int universityId = viewer.UniversityId ?? 0;
                int collegeId = viewer.CollegeId ?? 0;
                int departmentId = viewer.DepartmentId ?? 0;
                int batchId = viewer.BatchId ?? 0;

                // شرط Scope في كل فرع حمّالٌ لا زائد: سلسلة الأجداد منزَّلة، فمنشور الدفعة
                // يحمل CollegeId أيضًا. بحذف الشرط يقرأ كل عضو في الكلية محتوى كل قسم
                // وكل دفعة داخلها. هذا هو الفرق بين مُسنَد الجمهور ومُسنَد الاحتواء.
                query = query.Where(c =>
                       c.Scope == EnContentScope.Public
                    || (c.Scope == EnContentScope.University && c.UniversityId == universityId)
                    || (c.Scope == EnContentScope.College && c.CollegeId == collegeId)
                    || (c.Scope == EnContentScope.Department && c.DepartmentId == departmentId)
                    || (c.Scope == EnContentScope.Batch && c.BatchId == batchId));
            }

            // ToPagedResultAsync يطبّق Skip/Take بلا أي ترتيب خاص به، فالترتيب مسؤولية المستودع.
            // والفاصل بالمفتاح الأساسي إلزامي لا تجميلي: المُغذّي يمنح كل صفوف المحتوى القيمة
            // now نفسها، فالترتيب بـ CreatedAt وحده يجعل الصفحات تُكرِّر وتُسقِط عناصر من يومها الأول.
            return await query
                .OrderByDescending(c => c.CreatedAt)
                .ThenByDescending(c => c.ContentItemId)
                .Select(ToFeedDto)
                .ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<PagedResult<ContentItemDTO>> GetManagedContent(UserScope? actor, ContentFilterDTO? filter,
            int currentUserId, int pageNumber, int pageSize)
        {
            filter ??= new ContentFilterDTO();
            actor ??= new UserScope();

            var query = _context.ContentItems.AsNoTracking().AsQueryable();

            if (filter.Type.HasValue)
                query = query.Where(c => c.Type == filter.Type.Value);

            if (filter.Scope.HasValue)
                query = query.Where(c => c.Scope == filter.Scope.Value);

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(c => EF.Functions.Like(c.Title, $"%{filter.Title}%"));

            if (filter.MineOnly)
                query = query.Where(c => c.CreatedByUserId == currentUserId);

            // مُسنَد الاحتواء التراكمي — نمط GetSubjectsPerDepartment نفسه.
            // معاكس تمامًا لمُسنَد الخلاصة: هناك OR على مستوى واحد مطابق، وهنا AND على
            // كل مستوًى يملكه الفاعل. المحتوى العام لا يظهر لغير المسؤول العام، وهذا مقصود:
            // من لا يستطيع إدارته لا يُدرَج له في قائمة الإدارة.
            if (!actor.IsGlobal)
            {
                // فشل مغلق. الحارس ليس زائدًا: الفروع الأربعة أدناه كلها مشروطة، ففاعلٌ
                // ليس عامًّا وبلا أي مطالبة نطاق لا يُفعِّل أيًّا منها — فلا يُطبَّق أي Where
                // ويعود جدول ContentItems كاملًا.
                if (!actor.UniversityId.HasValue && !actor.CollegeId.HasValue
                    && !actor.DepartmentId.HasValue && !actor.BatchId.HasValue)
                {
                    return new PagedResult<ContentItemDTO>
                    {
                        Data = new List<ContentItemDTO>(),
                        TotalRecords = 0,
                        TotalPages = 0,
                        CurrentPage = pageNumber,
                    };
                }

                // مرآة CanManageContent في SQL، وشرط الملكية جزء منها لا إضافة:
                // بعد أن صار الفاعل ينشر لمستوى أعلى من مستواه (جمهور لا سقف)، تحمل
                // سلسلة منشوره null في الأعمدة الأعمق من هدفه — فيُقصيه التضييق التراكمي
                // عن قائمته هو نفسه. الخيار (أ): الكاتب يُدير ما كتب، ومن فوق مستوى
                // المنشور يُديره أيضًا، ومن دونه لا.
                bool hasUniversity = actor.UniversityId.HasValue;
                bool hasCollege = actor.CollegeId.HasValue;
                bool hasDepartment = actor.DepartmentId.HasValue;
                bool hasBatch = actor.BatchId.HasValue;

                int universityId = actor.UniversityId ?? 0;
                int collegeId = actor.CollegeId ?? 0;
                int departmentId = actor.DepartmentId ?? 0;
                int batchId = actor.BatchId ?? 0;

                query = query.Where(c =>
                       c.CreatedByUserId == currentUserId
                    || ((!hasUniversity || c.UniversityId == universityId)
                     && (!hasCollege || c.CollegeId == collegeId)
                     && (!hasDepartment || c.DepartmentId == departmentId)
                     && (!hasBatch || c.BatchId == batchId)));
            }
            else
            {
                // المسؤول العام وحده يملك ترشيح النطاق يدويًا
                if (filter.UniversityId.HasValue)
                    query = query.Where(c => c.UniversityId == filter.UniversityId);

                if (filter.CollegeId.HasValue)
                    query = query.Where(c => c.CollegeId == filter.CollegeId);

                if (filter.DepartmentId.HasValue)
                    query = query.Where(c => c.DepartmentId == filter.DepartmentId);

                if (filter.BatchId.HasValue)
                    query = query.Where(c => c.BatchId == filter.BatchId);
            }

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .ThenByDescending(c => c.ContentItemId)
                .Select(ToDto)
                .ToPagedResultAsync(pageNumber, pageSize);
        }
    }
}
