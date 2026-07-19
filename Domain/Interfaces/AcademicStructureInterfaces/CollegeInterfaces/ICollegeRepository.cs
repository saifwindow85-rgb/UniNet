using Contracts.Responses.CollegeResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces
{
    public interface ICollegeRepository
    {
        public Task<PagedResult<CollegeDTO>> GetAllColleges(int pageNumber, int pageSize);
        public Task<PagedResult<CollegeDTO>> GetAllCollegesPerUniversity(int universityId,int pageNumber, int pageSize);

        public void Add(College college);
        public Task<bool> Delete(int collegeId);
        public Task<bool> IsCollegeExists(int universityId,int collegeId);
        public Task<bool> IsCollegeExists(int universityId,string collegeName);
        public Task<CollegeDTO?> GetCollegeDTOById(int collegeId);
        public Task<College?> GetCollegeEntityById(int collegeId);
        public Task<CollegeDTO?> GetCollegeDTOByName(int universityId,string collegeName);
        public Task<College?>GetCollegeEntityByName(int universityId,string collegeName);

    }
}
