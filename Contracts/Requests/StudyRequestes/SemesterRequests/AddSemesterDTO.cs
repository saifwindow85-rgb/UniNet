using System;

namespace Contracts.Requests.StudyRequestes.SemesterRequests
{
    public class AddSemesterDTO
    {
        public string Name { get; set; } = null!;

        public int UniversityId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
