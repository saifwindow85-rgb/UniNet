using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Requests.RequestParameters
{
    public class PagedResultParameters
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageSize  Format!/PageSize Must Be > 0")]
        public int PageSize { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "PageNumber  Format!/PageNumber Must Be > 0")]
        public int PageNumber { get; set; }
    }

    public class DepartmentIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId  Format!/DepartmentId Must Be > 0")]
        public int DepartmentId { get; set; }
    }

    public class CollegeIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "CollegeId  Format!/CollegeId Must Be > 0")]
        public int CollegeId { get; set; }
    }

    public class BatchIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "BatchId  Format!/BatchId Must Be > 0")]
        public int BatchId { get; set; }
    }

    public class UniversityIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "UniversityId  Format!/UniversityId Must Be > 0")]
        public int UniversityId { get; set; }
    }


    public class RoleIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "RoleId  Format!/RoleId Must Be > 0")]
        public int RoleId { get; set; }
    }

}
