using Contracts.Common.AuthorizationInfos.EmployeeAuthorizationInfo;
using Contracts.Common.AuthorizationInfos.StudentAuthorizationInfo;
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
        public int? AdminId { get; private set; }


        public static UserTypeResult Employee(EmployeeAuthorizationInfo ?employeeInfo) => new UserTypeResult
        {
            EmployeeAuthorizationInfo = employeeInfo,
        };

        public static UserTypeResult Student(StudentAuthorizationInfo? studentInfo) => new UserTypeResult
        {
            StudentAuthorizationInfo = studentInfo,
        };
        public static UserTypeResult SystemAdmin(int? AdminId) => null;
    }
}
