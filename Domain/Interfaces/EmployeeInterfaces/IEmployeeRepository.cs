using Contracts.Requests.EmployeeRequests;
using Contracts.Responses.EmployeeResponse;
using Contracts.Results;
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
    }
}
