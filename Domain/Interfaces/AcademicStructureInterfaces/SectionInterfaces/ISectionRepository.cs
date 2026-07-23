using Contracts.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Responses.AcademicResponses.SectionResponses;
using Domain.Entities.Academic_Structure;

namespace Domain.Interfaces.AcademicStructureInterfaces.SectionInterfaces
{
    public interface ISectionRepository
    {
        public Task<PagedResult<SectionDTO>> GetAllSections(int pageNumber, int pageSize);
        public Task<PagedResult<SectionDTO>>GetSectionsPerBatch(int batchId,int pageNumber, int pageSize);
        public Task<SectionDTO?> GetDTOById(int sectionId);
        public Task<SectionDTO?>GetDTOByName(int batchId,string name);
        public Task<Section?>GetEntityById(int sectionId);
        public Task<Section?>GetEntityByName(int batchId,string name);
        public void Add(Section section);
        public Task<bool> Delete(int sectionId);
        public Task<bool>ExistsById(int sectionId);
        public Task<bool> ExistsByName(int batchId,string name);

    }
}
