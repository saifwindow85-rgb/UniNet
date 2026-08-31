using Contracts.Common;
using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.ContentResponses
{
    // الاسم يحتفظ بخطأ البيت الإملائي Detaield (DetaieldSubjectDTO و DetaieldStudentResultDTO):
    // الاتساق مع ثمانية نظائر أنفع من تصحيح حرف في التاسع وحده.
    public class DetaieldContentItemDTO : DTOsBaseEntity
    {
        public int ContentItemId { get; set; }

        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        public EnContentType Type { get; set; }

        public EnContentScope Scope { get; set; }

        public int? UniversityId { get; set; }

        public string? UniversityName { get; set; }

        public int? CollegeId { get; set; }

        public string? CollegeName { get; set; }

        public int? DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public int? BatchId { get; set; }

        public string? BatchName { get; set; }

        public bool HasImage { get; set; }

        public string? ImageOriginalFileName { get; set; }
    }
}
