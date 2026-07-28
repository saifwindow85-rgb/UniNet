using Contracts.Requests.EmployeeRequests;
using Contracts.Requests.EmployeeRequests.CollegeAdminRequests;
using Contracts.Requests.EmployeeRequests.DepartmentAdminRequests;
using Contracts.Requests.EmployeeRequests.UniversityAdminRequests;
using Contracts.Responses;
using Contracts.Responses.EmployeeResponse;
using Contracts.Results;
using Domain.Entities.Employees;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.EmployeeInterfaces
{
    public interface IEmployeeService
    {
        public Task<PagedResult<EmployeeDTO>>GetEmployees(EmployeeFilter ?filter,EmployeeScope ?scope,int pageNumber,int pageSize);
        public Task<Employee?>GetEntityById(int employeeId);
        public Task<EmployeeDTO?> GetDTOById(int employeeId);

        public Task<AddUpdateServiceResponse<EmployeeDTO>> AddUniversityAdmin(AddUniversityAdminDTO newUniversityAdmin, int currentUserId);
        public Task<AddUpdateServiceResponse<EmployeeDTO>> UpdateUniversityAdmin(int employeeId,UpdateUniversityAdminDTO updatedUniversityAdmin, int currentUserId);

        public Task<AddUpdateServiceResponse<EmployeeDTO>>AddCollegeAdmin(AddCollegeAdminDTO newCollegeAdmin, int currentUserId);
        public Task<AddUpdateServiceResponse<EmployeeDTO>> UpdateCollegeAdmin(int employeeId,UpdateCollegeAdminDTO updatedCollegeAdmin, int currentUserId);
        public Task<AddUpdateServiceResponse<EmployeeDTO>> AddDepartmentAdmin(AddDepartmentAdminDTO newDepartmentAdmin, int currentUserId);
        public Task<AddUpdateServiceResponse<EmployeeDTO>> UpdateDepartmentAdmin(int employeeId, UpdateDepartmentAdminDTO updatedDepartmentAdmin, int currentUserId);
        public  Task<EmployeeAuthorizationInfo?> GetEmployeeAuthorizationInfoAsync(int employeeId);
        public Task<EmployeeAuthorizationInfo?> GetEmployeeAuthorizationInfoByUserIdAsync(int userId);


    }
}
