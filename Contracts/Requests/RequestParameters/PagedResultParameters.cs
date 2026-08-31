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

    public class UserIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "UserId  Format!/UserId Must Be > 0")]
        public int UserId { get; set; }
    }

    public class   SectionIdParameter 
    {
        [Range(1, int.MaxValue, ErrorMessage = "SectionId  Format!/SectionId Must Be > 0")]
        public int SectionId { get; set; }
    }

    public class EmployeeIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeId  Format!/EmployeeId Must Be > 0")]
        public int EmployeeId { get; set; }
    }

    public class StudentIdParameter 
    {
        [Range(1, int.MaxValue, ErrorMessage = "StudentId  Format!/StudentId Must Be > 0")]
        public int StudentId { get; set; }
    }

    public class StudentStatusIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "StudentStatusId  Format!/StudentStatusId Must Be > 0")]
        public int StudentStatusId { get; set; }
    }

    public class SubjectIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "SubjectId  Format!/SubjectId Must Be > 0")]
        public int SubjectId { get; set; }
    }

    public class SemesterIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "SemesterId  Format!/SemesterId Must Be > 0")]
        public int SemesterId { get; set; }
    }

    public class SectionSubjectIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "SectionSubjectId  Format!/SectionSubjectId Must Be > 0")]
        public int SectionSubjectId { get; set; }
    }

    public class StudentResultIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "StudentResultId  Format!/StudentResultId Must Be > 0")]
        public int StudentResultId { get; set; }
    }

    public class ContentItemIdParameter
    {
        [Range(1, int.MaxValue, ErrorMessage = "ContentItemId  Format!/ContentItemId Must Be > 0")]
        public int ContentItemId { get; set; }
    }
}
