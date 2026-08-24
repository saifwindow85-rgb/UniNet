namespace Contracts.Responses.StudyResponses.StudentResultResponses
{
    // صفّ درجة واحد ضمن نتائج الطالب.
    public class StudentResultItemDTO
    {
        public int StudentResultId { get; set; }

        public int SectionSubjectId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;
        public string SubjectCode { get; set; } = null!;
        public string LecturerName { get; set; } = null!;

        public decimal Midterm { get; set; }
        public decimal Practical { get; set; }
        public decimal Final { get; set; }
        public decimal Total { get; set; }
    }
}
