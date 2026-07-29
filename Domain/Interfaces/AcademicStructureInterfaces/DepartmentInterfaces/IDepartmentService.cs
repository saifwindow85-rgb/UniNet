using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Requests.AcademicRequests.DepartmentRequests;
using Contracts.Responses;
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
    public  interface IDepartmentService
    {
        public Task<PagedResult<DepartmentDTO>> GetAllDepartments(int pageNumber, int pageSize);
        public Task<PagedResult<DepartmentDTO>>GetDepartmentsPerCollege(int collegeId,int pageNumber,int pageSize);
        public Task<DepartmentDTO?> GetDTOById(int departmentId);
        public Task<Department?>GetEntityById(int departmentId);
        public Task<DepartmentDTO?> GetDTOByName(int collegeId, string name);
        public Task<Department?>GetEntityByName(int departmentId, string name);
        public Task<AddUpdateServiceResponse<DepartmentDTO>> AddDepartment(AddDepartmentDTO newDepartment, int currentUserId);
        public Task<AddUpdateServiceResponse<DepartmentDTO>>UpdateDepartment(int departmentId,UpdateDepartmentDTO updatedDepartment,int currentUserId);
        public Task<bool> ExistsById(int departmentId);
        public Task<bool>ExistsByName(int collegeId,string name);
        public Task<bool> Delete(int departmentId);
        public Task<DepartmentAuthorizationInfo?> GetDepartmentAuthorizationInfoAsync(int departmentId);
        public Task<DepartmentAuthorizationInfo?> GetDepartmentAuthorizationInfoByNameAsync(int collegeId, string departmentName);

    }
}
