using Contracts.Enums;
using Contracts.Requests.EmployeeRequests;
using Contracts.Requests.EmployeeRequests.CollegeAdminRequests;
using Contracts.Requests.EmployeeRequests.DepartmentAdminRequests;
using Contracts.Requests.EmployeeRequests.UniversityAdminRequests;
using Contracts.Responses;
using Contracts.Responses.EmployeeResponse;
using Contracts.Results;
using Domain.Entities.Employees;
using Domain.Entities.Identity;
using Domain.Interfaces.EmployeeInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Application.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IServiceProvider _serviceProvider;




        public EmployeeService(IUnitOfWorkRepository unitOfWorkRepository,IServiceProvider serviceProvider)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _serviceProvider = serviceProvider;
        }

        public Task<AddUpdateServiceResponse<EmployeeDTO>> AddCollegeAdmin(AddCollegeAdminDTO newCollegeAdmin, int currentUserId)
        {
            throw new NotImplementedException();
        }

        public Task<AddUpdateServiceResponse<EmployeeDTO>> AddDepartmentAdmin(AddDepartmentAdminDTO newDepartmentAdmin, int currentUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<AddUpdateServiceResponse<EmployeeDTO>> AddUniversityAdmin(AddUniversityAdminDTO newUniversityAdmin, int currentUserId)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<AddUniversityAdminDTO>>();
            var validationResult = await validator.ValidateAsync(newUniversityAdmin);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.Failure(validationResult
                    .Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            if(await _unitOfWorkRepository.UserRepository.IsUserExsist(newUniversityAdmin.UserName))
            {
                return AddUpdateServiceResponse<EmployeeDTO>.AlreadyExists<Employee>();
            }

            await _unitOfWorkRepository.BeginTransactionAsync();
            try
            {
                var userEntity = new User
                {
                    FullName = newUniversityAdmin.FullName,
                    UserName = newUniversityAdmin.UserName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUniversityAdmin.Password),
                    PhoneNumber = newUniversityAdmin.PhoneNumber,
                    Email = newUniversityAdmin.Email,
                    IsActive = newUniversityAdmin.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = currentUserId,
                    UniversityId = newUniversityAdmin.UniversityId,
                };

                await _unitOfWorkRepository.UserRepository.Add(userEntity);

                await _unitOfWorkRepository.CompleteAsync();
                var employeeEntity = new Employee
                {
                    UserId = userEntity.UserId,
                    UniversityId = newUniversityAdmin.UniversityId,
                };
                _unitOfWorkRepository.EmployeeRepository.Add(employeeEntity);
                await _unitOfWorkRepository.CompleteAsync();

                var role = await _unitOfWorkRepository.RoleRepository.GetRoleDTOByRoleName("UniversityAdmin");
                var userRole = new UserRole
                {
                    UserId = userEntity.UserId,
                    RoleId = role!.RoleId
                };
                await _unitOfWorkRepository.UserRoleRepository.Add(userRole);
                await _unitOfWorkRepository.CompleteAsync();

                await _unitOfWorkRepository.CommitTransactionAsync();

                var employeeDTO = await GetDTOById(employeeEntity.EmployeeId);
                return AddUpdateServiceResponse<EmployeeDTO>.Success(employeeDTO!);
            }
            catch(Exception ex) 
            {
                await _unitOfWorkRepository.RollbackTransactionAsync();
                throw new Exception($"An error occurred while adding the university admin: {ex.Message}", ex);
            }
        }

        public async Task<EmployeeDTO?> GetDTOById(int employeeId)
        {
           return await _unitOfWorkRepository.EmployeeRepository.GetDTOById(employeeId);
        }

        public async Task<PagedResult<EmployeeDTO>> GetEmployees(EmployeeFilter? filter, EmployeeScope? scope, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.EmployeeRepository.GetEmployees(filter, scope, pageNumber, pageSize);
        }

        public async Task<Employee?> GetEntityById(int employeeId)
        {
            return await _unitOfWorkRepository.EmployeeRepository.GetById(employeeId);
        }

        public Task<AddUpdateServiceResponse<EmployeeDTO>> UpdateCollegeAdmin(int employeeId, UpdateCollegeAdminDTO updatedCollegeAdmin, int currentUserId)
        {
            throw new NotImplementedException();
        }

        public Task<AddUpdateServiceResponse<EmployeeDTO>> UpdateDepartmentAdmin(int employeeId, UpdateDepartmentAdminDTO updatedDepartmentAdmin, int currentUserId)
        {
            throw new NotImplementedException();
        }

        public Task<AddUpdateServiceResponse<EmployeeDTO>> UpdateUniversityAdmin(int employeeId, UpdateUniversityAdminDTO updatedUniversityAdmin, int currentUserId)
        {
            throw new NotImplementedException();
        }
    }
}
