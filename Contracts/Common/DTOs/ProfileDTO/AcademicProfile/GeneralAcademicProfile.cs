using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.DTOs.ProfileDTO.AcademicProfile
{
    public class GeneralAcademicProfile
    {
        public string UniversityName { get; set; } = null!;
        public string Name { get; set; } = null!; // Name of College Or Department Or Batch 
        public int TotalStudents { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        // Only for batch 
        public int?BatchYear { get; set; }
    }
}
