using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Requests.AcademicRequests.CommonAcademicRequests;
using Contracts.Requests.AcademicRequests.UniversityRequests;
using Contracts.Responses;
using Contracts.Responses.AcademicResponses.UniversityResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.AcademicStructureInterfaces.UniversityInterfaces
{
    public interface IUniversityService
    {
        public Task<PagedResult<UniversityDTO>> GetAllUniversities(AcademicFilter?filter,int pageNumber,int pageSize);
        public Task<UniversityDTO?> FindUniversityDTOById(int UniversityId);
        public Task<University?> FindUniversityEntityById(int universityId);
        public Task<bool> Delete(int universityId);
        public Task<bool>IsUniversityExists(int universityId);
        public Task<bool> IsUniversityExists(string universityName);
        public Task<AddUpdateServiceResponse<UniversityDTO>> AddUniversity(AddUniversityDTO newUniversity, int currentUserId);
        public Task<AddUpdateServiceResponse<UniversityDTO>>UpdateUniversity(int updatedUinversityId,UpdateUniversityDTO updatedUniversity, int currentUserId);
        public Task<UniversityAuthorizationInfo?> GetUniversityAuthorizationInfoAsync(int universityId);

    }
}
