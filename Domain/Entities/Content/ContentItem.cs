using Domain.Entities.Common;
using Domain.Entities.Enums;
using Domain.Entities.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Content
{
    public abstract class ContentItem : BaseEntity
    {
        public int ContentItemId { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public EnContentScope Scope { get; set; }
        public EncontentType Type { get; set; }
        public Image? Image { get; set; }
    }
}
