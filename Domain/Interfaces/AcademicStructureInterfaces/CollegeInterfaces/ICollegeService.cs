using Contracts.Requests.AcademicRequests.CollegeRequests;
using Contracts.Responses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Contracts.Common.AuthorizationInfos.AcademicInfos;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Responses.AcademicResponses.CollegeResponses;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.AcademicRequests.CommonAcademicRequests;

namespace Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces
{
    public interface ICollegeService
    {
        public Task<PagedResult<CollegeDTO>>GetColleges(AcademicFilter?filter,int pageNumber, int pageSize);
        public Task<PagedResult<CollegeDTO>>GetCollegesPerUniversity(UserScope?scope,AcademicFilter?filter,int pageNumber, int pageSize);
        public Task<CollegeDTO?> GetCollegeDTOById(int collegeId);
        public Task<College?>GetCollegeEntityById(int collegeId);
        public Task<bool> Delete(int collegeId);
        public Task<bool>IsCollegeExists(int collegeId);
        public Task<bool> IsCollegeExists(int universityId,string collegeName);
        public Task<AddUpdateServiceResponse<CollegeDTO>> AddCollege(UserScope?scope,AddCollegeDTO newCollege, int currentUserId);
        public Task<AddUpdateServiceResponse<CollegeDTO>> UpdateCollege(UserScope?scop,int collegeId, UpdateCollegeDTO updatedCollege, int currentUserId);
        public Task<CollegeAuthorizationInfo?> GetCollegeAuthorizationInfo(int collegeId);
    }
}
