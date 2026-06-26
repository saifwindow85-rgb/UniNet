using Domain.Entities.Academic_Structure;
using Domain.Entities.Identity;
using Domain.Entities.Study;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Students
{
    public class Student
    {
        public int StudentId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string StudentNumber { get; set; } = null!;

        public int BatchId { get; set; }
        public Batch Batch { get; set; } = null!;
        public int SectionId { get; set; }
        public Section Section { get; set; } = null!;
        public int StatusId { get; set; }
        public StudentStatus Status { get; set; } = null!;

        public DateTime EnrollmentDate { get; set; }
        public ICollection<StudentResult> StudentResults { get; set; } = new List<StudentResult>();
    }
}
