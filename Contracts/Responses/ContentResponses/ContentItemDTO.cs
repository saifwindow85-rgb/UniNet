using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses.ContentResponses
{
    /// <summary>
    /// صفّ القائمة الإدارية: يضيف ما يحتاجه المسؤول ولا يحتاجه القارئ —
    /// النطاق ومعرّفاته والكاتب وتاريخ التعديل.
    /// </summary>
    public class ContentItemDTO
    {
        public int ContentItemId { get; set; }

        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        public EnContentType Type { get; set; }

        public EnContentScope Scope { get; set; }

        public int? UniversityId { get; set; }

        public int? CollegeId { get; set; }

        public int? DepartmentId { get; set; }

        public int? BatchId { get; set; }

        public int CreatedByUserId { get; set; }

        public string AuthorName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool HasImage { get; set; }
    }
}
