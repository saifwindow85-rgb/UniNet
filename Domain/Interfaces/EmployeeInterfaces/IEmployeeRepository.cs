using Contracts.Requests.EmployeeRequests;
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
    public interface IEmployeeRepository
    {
        public Task<PagedResult<EmployeeDTO>> GetEmployees(EmployeeFilter? employeeFilter, EmployeeScope? employeeScope, int pageNumber, int pageSize);
        public void Add(Employee employee);

        public Task<EmployeeDTO?>GetDTOById(int employeeId);
        public Task<Employee?> GetById(int employeeId);
        public Task<EmployeeAuthorizationInfo?> GetEmployeeAuthorizationInfoAsync(int employeeId);
        public Task<EmployeeAuthorizationInfo?> GetEmployeeAuthorizationInfoByUserIdAsync(int userId);

    }
}
