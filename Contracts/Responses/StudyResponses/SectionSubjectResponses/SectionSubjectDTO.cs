namespace Contracts.Responses.StudyResponses.SectionSubjectResponses
{
    public class SectionSubjectDTO
    {
        public int SectionSubjectId { get; set; }

        public int SectionId { get; set; }
        public string SectionName { get; set; } = null!;

        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;
        public string SubjectCode { get; set; } = null!;

        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = null!;

        public string LecturerName { get; set; } = null!;
    }
}
