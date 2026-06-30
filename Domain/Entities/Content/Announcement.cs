using Domain.Entities.Common;
using Domain.Entities.Enums;
using Domain.Entities.Images;

namespace Domain.Entities.Content
{
    public class Announcement : BaseEntity
    {
        public int AnnouncementId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;
        public EnContentType Type { get; set; }
        public int? ImageId { get; set; }
        public Image? Image { get; set; }
    }
}
