namespace Contracts.Requests.StudyRequestes.SectionSubjectRequests
{
    public class AddSectionSubjectDTO
    {
        public int SectionId { get; set; }

        public int SubjectId { get; set; }

        public int SemesterId { get; set; }

        public string LecturerName { get; set; } = null!;
    }
}
