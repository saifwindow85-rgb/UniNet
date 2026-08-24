namespace Contracts.Requests.StudyRequestes.StudentResultRequests
{
    public class AddStudentResultDTO
    {
        public int StudentId { get; set; }

        public int SectionSubjectId { get; set; }

        public decimal Midterm { get; set; }

        public decimal Practical { get; set; }

        public decimal Final { get; set; }
    }
}
