using Contracts.Requests.AcademicRequests.CollegeRequests;
using Contracts.Responses;
using Contracts.Responses.CollegeResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Contracts.Common.AuthorizationInfos.AcademicInfos;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces
{
    public interface ICollegeService
    {
        public Task<PagedResult<CollegeDTO>>GetColleges(int pageNumber, int pageSize);
        public Task<PagedResult<CollegeDTO>>GetCollegesPerUniversity(int universityId,int pageNumber, int pageSize);
        public Task<CollegeDTO?> GetCollegeDTOById(int collegeId);
        public Task<College?>GetCollegeEntityById(int collegeId);
        public Task<CollegeDTO?> GetCollegeDTOByName(int universityId,string collegeName);
        public Task<College?>GetCollegeEntityByName(int universityId, string collegeName);
        public Task<bool> Delete(int collegeId);
        public Task<bool>IsCollegeExists(int collegeId);
        public Task<bool> IsCollegeExists(int universityId,string collegeName);
        public Task<AddUpdateServiceResponse<CollegeDTO>> AddCollege(AddCollegeDTO newCollege, int currentUserId);
        public Task<AddUpdateServiceResponse<CollegeDTO>> UpdateCollege(int collegeId, UpdateCollegeDTO updatedCollege, int currentUserId);
        public Task<CollegeAuthorizationInfo?> GetCollegeAuthorizationInfo(int collegeId);
        public Task<CollegeAuthorizationInfo?> GetCollegeAuthorizationInfo(int universityID, string collegeName);
    }
}
