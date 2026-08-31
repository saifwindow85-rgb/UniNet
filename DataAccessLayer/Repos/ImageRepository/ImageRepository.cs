using Contracts.Common.AuthorizationInfos.ContentAuthorizationInfo;
using Contracts.Responses.ImageResponses;
using DataAccessLayer.Dbcontext;
using Domain.Entities.Images;
using Domain.Interfaces.ImageInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos.ImageRepository
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbcontext _context;

        // إسقاطة واحدة تجلب بيانات البثّ وجمهور المحتوى معًا.
        // أعمدة النطاق مخزَّنة على ContentItem نفسه، فهذه ضمّة واحدة فقط —
        // لا سلسلة Batch→Department→College→University كما لو خُزِّن المستوى الأعمق وحده.
        private readonly Expression<Func<Image, ImageFileDTO>> ToFileInfo = i => new ImageFileDTO
        {
            ImageId = i.ImageId,
            RelativePath = i.RelativePath,
            ContentType = i.ContentType,
            OriginalFileName = i.OriginalFileName,
            ViewInfo = new ContentViewInfo
            {
                Scope = i.ContentItem.Scope,
                UniversityId = i.ContentItem.UniversityId,
                CollegeId = i.ContentItem.CollegeId,
                DepartmentId = i.ContentItem.DepartmentId,
                BatchId = i.ContentItem.BatchId,
            }
        };

        public ImageRepository(AppDbcontext context)
        {
            _context = context;
        }

        public async Task<ImageFileDTO?> GetFileInfoByContentItemIdAsync(int contentItemId)
        {
            return await _context.Images.AsNoTracking()
                .Where(i => i.ContentItemId == contentItemId)
                .Select(ToFileInfo)
                .SingleOrDefaultAsync();
        }

        public async Task<Image?> GetByContentItemIdAsync(int contentItemId)
        {
            return await _context.Images.SingleOrDefaultAsync(i => i.ContentItemId == contentItemId);
        }
    }
}
