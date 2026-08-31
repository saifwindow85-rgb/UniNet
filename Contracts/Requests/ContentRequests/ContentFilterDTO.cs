using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.ContentRequests
{
    /// <summary>مرشِّح القائمة الإدارية: المسؤول يرى النطاق فيصحّ أن يرشِّح به.</summary>
    public class ContentFilterDTO
    {
        public string? Title { get; set; }

        public EnContentType? Type { get; set; }

        public EnContentScope? Scope { get; set; }

        public int? UniversityId { get; set; }

        public int? CollegeId { get; set; }

        public int? DepartmentId { get; set; }

        public int? BatchId { get; set; }

        public bool MineOnly { get; set; }
    }
}
