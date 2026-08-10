using Contracts.Common.AuthorizationInfos;
using Contracts.Requests.RequestParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.Extensions
{
    public static class EmployeeScopeExtension
    {
        public static bool IsWithinScope(this UserScope?scope,GeneralAuthorizationInfo info)
        {
            if (scope == null|| scope.IsGlobal)
                return true;

            if(scope.BatchId.HasValue)
                return info.BatchId == scope.BatchId;

            if (scope.DepartmentId.HasValue)
                return info.DepartmentId == scope.DepartmentId;

            if(scope.CollegeId.HasValue)
                return info.CollegeId == scope.CollegeId;

            return info.UniversityId == scope.UniversityId;
        }
    }
}
