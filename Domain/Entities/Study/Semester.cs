using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Study
{
    public class Semester : BaseEntity
    {
        public int SemesterId { get; set; }

        public string Name { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsCurrent { get; set; }
        public ICollection<SectionSubject> SectionSubjects { get; set; } = new List<SectionSubject>();

    }
}
