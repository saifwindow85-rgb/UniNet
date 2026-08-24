using System.Collections.Generic;

namespace Contracts.Responses.StudyResponses.StudentResultResponses
{
    // نتائج طالب في فصل دراسي واحد + المعدّل (مجموع Totals ÷ عدد النتائج).
    // يُستخدم لكشف درجات طالب واحد (مرتّب حسب SemesterId)،
    // ولتقرير كل الطلاب (مرتّب حسب Section => Batch => Department => College => University).
    public class StudentSemesterResultDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public string StudentNumber { get; set; } = null!;

        public int SectionId { get; set; }
        public string SectionName { get; set; } = null!;
        public int BatchId { get; set; }
        public string BatchName { get; set; } = null!;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;

        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = null!;

        public List<StudentResultItemDTO> Results { get; set; } = new List<StudentResultItemDTO>();

        // المعدّل = مجموع Totals ÷ عدد النتائج في هذا الفصل.
        public decimal Grade { get; set; }
    }
}
