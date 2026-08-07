using Contracts.Common.AuthorizationInfos.EmployeeAuthorizationInfo;
using Contracts.Common.AuthorizationInfos.StudentAuthorizationInfo;
using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Results
{
    public class UserTypeResult
    {
        public EmployeeAuthorizationInfo ?EmployeeAuthorizationInfo { get; private set; }
        public StudentAuthorizationInfo? StudentAuthorizationInfo { get; private set; }
        public UserType Type { get; private set; }
        public int? AdminId { get; private set; }


        public static UserTypeResult Employee(EmployeeAuthorizationInfo ?employeeInfo) => new UserTypeResult
        {
            EmployeeAuthorizationInfo = employeeInfo,
            Type = UserType.Employee,
        };

        public static UserTypeResult Student(StudentAuthorizationInfo? studentInfo) => new UserTypeResult
        {
            StudentAuthorizationInfo = studentInfo,
            Type = UserType.Student,
        };
        public static UserTypeResult SystemAdmin(int? AdminId) => new UserTypeResult
        {
            Type = UserType.SystemAdmin,
        };
    }
}
