using Domain.Entities.Academic_Structure;
using Domain.Entities.Common;
using Contracts.Enums;
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

        // ----------------------------------------------------------------------------
        // نطاق النشر: سلسلة الأجداد كاملة لا المستوى الأعمق وحده.
        // منشور موجَّه لدفعة يملأ الأربعة؛ ولكلية يملأ الاثنين الأولين؛ و Public يتركها كلها null.
        // السبب: IsWithinScope و GeneralAuthorizationInfo يقرآن الأربعة مباشرةً، فتصير
        // إسقاطة التفويض بلا أي JOIN — وهذا يهم لأن استعلام الخلاصة هو أسخن استعلام في التطبيق.
        // التناقض بين Scope والأعمدة يمنعه قيد CHECK في ContentItemConfiguration.
        // نفس نمط Employee (UniversityId + CollegeId? + DepartmentId?).
        // ----------------------------------------------------------------------------
        public int? UniversityId { get; set; }
        public University? University { get; set; }

        public int? CollegeId { get; set; }
        public College? College { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? BatchId { get; set; }
        public Batch? Batch { get; set; }

        // كاتب المحتوى هو BaseEntity.CreatedByUserId — لا عمود UserId منفصل.
        // ولا EmployeeId/StudentId: كلاهما مشتقّ من المستخدم عبر User.Employee / User.Student،
        // وتخزينهما يسمح بصف يناقض نفسه (UserId لشخص و EmployeeId لشخص آخر).
        // نوع الكاتب متاح جاهزًا في User.Type.
    }
}
