using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.DTOs.User_Token_DTOs
{
    public class TokenUserInfoDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int? UniversityId { get; set; }
        public int? CollegeId { get; set; }
        public int ? DepartmentId { get; set; }
        public int? BatchId { get; set; }
        public int? StudentId { get; set; }
        public int? EmployeeId { get; set; }
        public UserType Type { get; set; }
        public List<string>UserRoles { get; set; } = new List<string>();
    }
}
