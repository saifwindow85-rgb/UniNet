using Contracts.Requests.EmployeeRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.Extensions
{
    public static class EmployeeScopeExtension
    {
        public static bool IsWithinScope(this EmployeeScope?scope,int universityId,int?collegeId = null,int?departmentId =null)
        {
            if (scope == null||(scope.UniversityId == null&&scope.CollegeId == null&&scope.DepartmentId ==null))
                return true;

            if(scope.DepartmentId.HasValue)
                return departmentId == scope.DepartmentId;

            if(scope.CollegeId.HasValue)
                return collegeId == scope.CollegeId;

            return universityId == scope.UniversityId;
        }
    }
}
