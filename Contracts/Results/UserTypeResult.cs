using Contracts.Responses.EmployeeResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Results
{
    public class UserTypeResult
    {
        public EmployeeAuthorizationInfo ?EmployeeAuthorizationInfo { get; private set; }

        public static UserTypeResult Employee(EmployeeAuthorizationInfo ?employeeInfo) => new UserTypeResult
        {
            EmployeeAuthorizationInfo = employeeInfo,
        };
        // Student not implemnted yet
    }
}
