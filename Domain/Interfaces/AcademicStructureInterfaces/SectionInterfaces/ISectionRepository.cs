using Contracts.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Responses.AcademicResponses.SectionResponses;
using Domain.Entities.Academic_Structure;
using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Requests.AcademicRequests.CommonAcademicRequests;
using Contracts.Requests.RequestParameters;

namespace Domain.Interfaces.AcademicStructureInterfaces.SectionInterfaces
{
    public interface ISectionRepository
    {
        public Task<PagedResult<SectionDTO>> GetAllSections(AcademicFilter?filter,int pageNumber, int pageSize);
        public Task<PagedResult<SectionDTO>>GetSectionsPerBatch(UserScope?scope,AcademicFilter?filter,int pageNumber, int pageSize);
        public Task<SectionDTO?> GetDTOById(int sectionId);
        public Task<Section?>GetEntityById(int sectionId);
        public void Add(Section section);
        public Task<bool> Delete(int sectionId);
        public Task<bool>ExistsById(int sectionId);
        public Task<bool> ExistsByName(int batchId,string name);
        public Task<SectionAuthorizationInfo?> GetSectionAuthorizationInfoAsync(int sectionId);

    }
}
