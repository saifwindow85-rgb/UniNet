using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Responses.AcademicResponses.DepartmentResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.AcademicStructureInterfaces.DepartmentInterfaces
{
    public interface IDepartmentRepository
    {
        public Task<PagedResult<DepartmentDTO>> GetAllDepartments(int pageNumber, int pageSize);
        public Task<PagedResult<DepartmentDTO>>GetDepartmentsPerCollege(int collegeId, int pageNumber, int pageSize);
        public Task<DepartmentDTO?> GetDTOById(int departmentId);
        public Task<Department?>GetEntityById(int departmentId);
        public Task<DepartmentDTO?> GetDTOByName(int collegeId, string name);
        public Task<Department?> GetEntityByName(int collegeId, string name);
        public void Add(Department department);
        public Task<bool> Delete(int DepartmentId);
        public Task<bool> ExistsById(int departmentId);
        public Task<bool> ExistsByName(int collegeId, string name);
        public Task<DepartmentAuthorizationInfo?> GetDepartmentAuthorizationInfoAsync(int departmentId);
    }
}
