using Contracts.Requests.EmployeeRequests;
using Contracts.Responses.EmployeeResponse;
using Contracts.Results;
using Domain.Interfaces.EmployeeInterfaces;
using Domain.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        public EmployeeService(IUnitOfWorkRepository unitOfWorkRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<PagedResult<EmployeeDTO>> GetEmployees(EmployeeFilter? filter, EmployeeScope? scope, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.EmployeeRepository.GetEmployees(filter, scope, pageNumber, pageSize);
        }
    }
}
