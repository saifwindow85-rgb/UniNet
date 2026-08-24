using System;

namespace Contracts.Requests.StudyRequestes.SemesterRequests
{
    public class UpdateSemesterDTO
    {
        public string Name { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
