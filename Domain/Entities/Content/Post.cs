using Domain.Entities.Common;
using Domain.Entities.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Content
{
    public class Post : BaseEntity
    {
        public int PostId { get; set; }


        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;
        public int ?ImageId { get; set; }
        public Image?Image { get; set; }
    }


}
