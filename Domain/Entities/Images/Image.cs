using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Images
{
    public class Image
    {
        public int ImageId { get; set; }

        public string FileName { get; set; } = null!;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; }
        public int UploadedByUserId { get; set; }
        public User UploadedByUser { get; set; } = null!;

       public  DateTime UpdatedAt { get; set; }
        public int ?UpdatedByUserId { get; set; }
        public User ?UpdatedByUser { get; set; }
    }
}
