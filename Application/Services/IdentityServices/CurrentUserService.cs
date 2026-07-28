using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
namespace Application.Services.IdentityServices
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId { get
            { var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    throw new UnauthorizedAccessException("User is not authenticated.");
                return int.Parse(userId);
            } }

        public string UserName
        {
            get
            {
                var userName = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
                if (userName == null)
                    throw new UnauthorizedAccessException("User is not authenticated.");
                return userName;
            }
        }

        public int? UniversityId 
        {
            get
            {
                var universityIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UniversityId")?.Value;
                if (universityIdClaim == null)
                    return null;
                return int.Parse(universityIdClaim);
            }
        }

        public int? CollegeId
        {
            get
            {
                var collegeIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("CollegeId")?.Value;
                if (collegeIdClaim == null)
                    return null;

                return int.Parse(collegeIdClaim);
            }
        }

        public int? DepartmentId
        {
            get
            {
                var departmentIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("DepartmentId")?.Value;
                if (departmentIdClaim == null)
                    return null;
                return int.Parse(departmentIdClaim);
            }
        }

        public int? BatchId
        {
            get
            {
                var batchIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("BatchId")?.Value;
                if (batchIdClaim == null)
                    return null;

                return int.Parse(batchIdClaim);
            }
        }
    }
}
