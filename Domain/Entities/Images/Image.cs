using Domain.Entities.Common;
using Domain.Entities.Content;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Images
{
    public class Image : BaseEntity
    {
        public int ImageId { get; set; }
        public string OriginalFileName { get; set; } = null!;
        public string StoredFileName { get; set; } = null!;
        public long FileSize { get; set; }
        public string RelativePath { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public int ContentItemId { get; set; }
        public ContentItem ContentItem { get; set; } = null!;
    }
}
