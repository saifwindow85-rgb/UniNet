using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.ImageRequests
{
    public class UploadedFileDTO
    {
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Length { get; set; }
        public Stream Content { get; set; } = null!;
    }
}
