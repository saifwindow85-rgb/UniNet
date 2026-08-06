using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Responses.AcademicResponses.CollegeResponses;
using Contracts.Requests.AcademicRequests.CollegeRequests;
using Contracts.Requests.RequestParameters;

namespace Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces
{
    public interface ICollegeRepository
    {
        public Task<PagedResult<CollegeDTO>> GetAllColleges(CollegeFilter?filter,int pageNumber, int pageSize);
        public Task<PagedResult<CollegeDTO>> GetAllCollegesPerUniversity(UserScope?scope,CollegeFilter?filter,int pageNumber, int pageSize);

        public void Add(College college);
        public Task<bool> Delete(int collegeId);
        public Task<bool> IsCollegeExists(int collegeId);
        public Task<bool> IsCollegeExists(int universityId,string collegeName);
        public Task<CollegeDTO?> GetCollegeDTOById(int collegeId);
        public Task<College?> GetCollegeEntityById(int collegeId);
        public Task<CollegeAuthorizationInfo?> GetCollegeAuthorizationInfo(int collegeId);


    }
}
