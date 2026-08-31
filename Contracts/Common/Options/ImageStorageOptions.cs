using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.Options
{
    public class ImageStorageOptions
    {
        public string RootPath { get; set; } = null!;
        public long MaxFileSizeInBytes { get; set; } = 5 *1024 *1024;
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".webp" };
        public string[] AllowedContentTypes { get; set; } = { "image/jpeg", "image/png", "image/webp" };
    }
}
