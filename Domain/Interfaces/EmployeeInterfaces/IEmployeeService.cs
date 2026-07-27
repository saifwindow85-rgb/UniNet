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
    public interface IEmployeeService
    {
        public Task<PagedResult<EmployeeDTO>>GetEmployees(EmployeeFilter ?filter,EmployeeScope ?scope,int pageNumber,int pageSize);
    }
}
