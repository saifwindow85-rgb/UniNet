namespace Contracts.Requests.StudyRequestes.SemesterRequests
{
    public class SemesterFilterDTO
    {
        public string? Name { get; set; }

        public int? UniversityId { get; set; }

        public bool? IsCurrent { get; set; }
    }
}
