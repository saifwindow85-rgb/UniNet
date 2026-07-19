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
    public interface IUniversityRepository
    {
        public Task<PagedResult<UniversityDTO>> GetAllUniversities(int pageNumber, int pageSize);
        public void Add(University university);
        public Task<bool> Delete(int universityId);
        public Task<UniversityDTO?>GetUniversityDTOById(int universityId);
        public Task<University?>GetUniversityEntityById(int universityId);
        public Task<bool> IsUniversityExists(int universityId);
        public Task<bool> IsUniversityExists(string universityName);
    }
}
