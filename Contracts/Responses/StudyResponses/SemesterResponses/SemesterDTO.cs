using System;

namespace Contracts.Responses.StudyResponses.SemesterResponses
{
    public class SemesterDTO
    {
        public int SemesterId { get; set; }

        public string Name { get; set; } = null!;

        public int UniversityId { get; set; }
        public string UniversityName { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsCurrent { get; set; }
    }
}
