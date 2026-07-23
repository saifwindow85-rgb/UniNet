using Contracts.Requests.AcademicRequests.SectionRequests;
using Contracts.Responses;
using Contracts.Responses.AcademicResponses.SectionResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.AcademicStructureInterfaces.SectionInterfaces
{
    public interface ISectionService
    {
        public Task<PagedResult<SectionDTO>> GetAllSections(int pageNumber, int pageSize);
        public Task<PagedResult<SectionDTO>>GetSectionsPerBatches(int batchId,int pageNumber, int pageSize);
        public Task<SectionDTO?> GetDTOById(int sectionId);
        public Task<SectionDTO?>GetDTOByName(int batchId,string name);
        public Task<Section?>GetEntityById(int sectionId);
        public Task<Section?>GetEntityByName(int batchId,string name);
        public Task<bool> ExistsById(int sectionId);
        public Task<bool> ExistsByName(int batchId, string name);
        public Task<bool>Delete(int sectionId);
        public Task<AddUpdateServiceResponse<SectionDTO>> AddSection(AddSectionDTO newSection, int currentUserId);
        public Task<AddUpdateServiceResponse<SectionDTO>>UpdateSection(int SectionId,UpdateSectionDTO updatedSection,int currentUserId);
    }
}
