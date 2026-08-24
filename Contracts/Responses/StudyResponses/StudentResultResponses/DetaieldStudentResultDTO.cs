using Contracts.Common;

namespace Contracts.Responses.StudyResponses.StudentResultResponses
{
    public class DetaieldStudentResultDTO : DTOsBaseEntity
    {
        public int StudentResultId { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public string StudentNumber { get; set; } = null!;

        public int SectionSubjectId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;
        public string SubjectCode { get; set; } = null!;
        public string LecturerName { get; set; } = null!;

        public int SectionId { get; set; }
        public string SectionName { get; set; } = null!;

        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = null!;

        public decimal Midterm { get; set; }
        public decimal Practical { get; set; }
        public decimal Final { get; set; }
        public decimal Total { get; set; }
    }
}
