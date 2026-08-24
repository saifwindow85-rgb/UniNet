namespace Contracts.Requests.StudyRequestes.StudentResultRequests
{
    // فلتر احترافي: يجمع الفلترة بالمعرّفات الدقيقة والبحث النصّي (اسم المادة، اسم الطالب، رقمه...).
    public class StudentResultFilterDTO
    {
        // فلترة دقيقة بالمعرّفات
        public int? StudentId { get; set; }
        public int? SectionSubjectId { get; set; }
        public int? SubjectId { get; set; }
        public int? SemesterId { get; set; }
        public int? SectionId { get; set; }

        // بحث نصّي (Contains)
        public string? StudentName { get; set; }
        public string? StudentNumber { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
        public string? LecturerName { get; set; }
    }
}
