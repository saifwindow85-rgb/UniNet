using Application.Extensions;
using Contracts.Common;
using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Common.AuthorizationInfos.EmployeeAuthorizationInfo;
using Contracts.Common.Extensions;
using Contracts.Common.Mappers;
using Contracts.Enums;
using Contracts.Requests.EmployeeRequests;
using Contracts.Requests.EmployeeRequests.CollegeAdminRequests;
using Contracts.Requests.EmployeeRequests.DepartmentAdminRequests;
using Contracts.Requests.EmployeeRequests.UniversityAdminRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.EmployeeResponse;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
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

        public async Task<AddUpdateServiceResponse<EmployeeDTO>> AddCollegeAdmin(UserScope? scope, AddCollegeAdminDTO newCollegeAdmin, int currentUserId)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<AddCollegeAdminDTO>>();
            var validationResult = await validator.ValidateAsync(newCollegeAdmin);

            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.Failure(validationResult
                    .Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var collegeInfo = await _unitOfWorkRepository.CollegeRepository.GetCollegeAuthorizationInfo(newCollegeAdmin.CollegeId);
            if(collegeInfo == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.InvalidRelatedData();
            }
            if(!scope.IsWithinScope(collegeInfo.ToCollegeInfo()))
            {
                return AddUpdateServiceResponse<EmployeeDTO>.ResourceDoesntExist<College>();
                // not returning 403 because we dont want the end user to know that resource exists even
            }
            var role = await _unitOfWorkRepository.RoleRepository.GetRoleDTOByRoleName("CollegeAdmin");
            if(role == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.InvalidRelatedData();
            }
            if(await _unitOfWorkRepository.UserRepository.IsUserExist(newCollegeAdmin.UserName))
            {
                return AddUpdateServiceResponse<EmployeeDTO>.AlreadyExists<User>();
            }

            var employeeEntity = await CreateEmployee(newCollegeAdmin, currentUserId, collegeInfo.UniversityId, role.RoleId, collegeInfo.CollegeId);
            var employeeDTO = await GetDTOById(employeeEntity.EmployeeId);
            return AddUpdateServiceResponse<EmployeeDTO>.Success(employeeDTO!);
        }

        public async Task<AddUpdateServiceResponse<EmployeeDTO>> AddDepartmentAdmin(UserScope?scope,AddDepartmentAdminDTO newDepartmentAdmin, int currentUserId)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<AddDepartmentAdminDTO>>();
            var validationResult = await validator.ValidateAsync(newDepartmentAdmin);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.Failure(validationResult
                    .Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }
            var departmentInfo = await _unitOfWorkRepository.DepartmentRepository.GetDepartmentAuthorizationInfoAsync(newDepartmentAdmin.DepartmentId);
            if(departmentInfo == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.InvalidRelatedData();
            }
            if(!scope.IsWithinScope(departmentInfo.ToDepartmentInfo()))
            {
                return AddUpdateServiceResponse<EmployeeDTO>.ResourceDoesntExist<Department>();
                // not returning 403 because we dont want the end user to know that resource exists even
            }
            if (await _unitOfWorkRepository.UserRepository.IsUserExist(newDepartmentAdmin.UserName))
            {
                return AddUpdateServiceResponse<EmployeeDTO>.AlreadyExists<User>();
            }

            var role = await _unitOfWorkRepository.RoleRepository.GetRoleDTOByRoleName("DepartmentAdmin");
            if(role == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.InvalidRelatedData();
            }
            var employeeEntity = await CreateEmployee(newDepartmentAdmin, currentUserId, departmentInfo.UniversityId,
                role.RoleId, departmentInfo.CollegeId, departmentInfo.DepartmentId);
            var employeeDTO = await GetDTOById(employeeEntity.EmployeeId);
            return AddUpdateServiceResponse<EmployeeDTO>.Success(employeeDTO!);
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

            if(! await _unitOfWorkRepository.UniversityRepository.IsUniversityExists(newUniversityAdmin.UniversityId))
            {
                return AddUpdateServiceResponse<EmployeeDTO>.InvalidRelatedData();
            }
            var role = await _unitOfWorkRepository.RoleRepository.GetRoleDTOByRoleName("UniversityAdmin");
            if(role == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.InvalidRelatedData();
            }
            if(await _unitOfWorkRepository.UserRepository.IsUserExist(newUniversityAdmin.UserName))
            {
                return AddUpdateServiceResponse<EmployeeDTO>.AlreadyExists<User>();
            }

            var employeeEntity = await CreateEmployee(newUniversityAdmin, currentUserId, newUniversityAdmin.UniversityId,role.RoleId);
            var employeeDTO = await GetDTOById(employeeEntity.EmployeeId);
            return AddUpdateServiceResponse<EmployeeDTO>.Success(employeeDTO!);
        }

        public async Task<EmployeeDTO?> GetDTOById(int employeeId)
        {
           return await _unitOfWorkRepository.EmployeeRepository.GetDTOById(employeeId);
        }

        public async Task<EmployeeAuthorizationInfo?> GetEmployeeAuthorizationInfoAsync(int employeeId)
        {
            return await _unitOfWorkRepository.EmployeeRepository.GetEmployeeAuthorizationInfoAsync(employeeId);
        }

        public async Task<EmployeeAuthorizationInfo?> GetEmployeeAuthorizationInfoByUserIdAsync(int userId)
        {
            return await _unitOfWorkRepository.EmployeeRepository.GetEmployeeAuthorizationInfoByUserIdAsync(userId);
        }

        public async Task<PagedResult<EmployeeDTO>> GetEmployees(EmployeeFilter? filter, UserScope? scope, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.EmployeeRepository.GetEmployees(filter, scope, pageNumber, pageSize);
        }

        public async Task<Employee?> GetEntityById(int employeeId)
        {
            return await _unitOfWorkRepository.EmployeeRepository.GetById(employeeId);
        }


        public async Task<AddUpdateServiceResponse<EmployeeDTO>> UpdateCollegeAdmin(UserScope?scope,int employeeId, UpdateCollegeAdminDTO updatedCollegeAdmin, int currentUserId)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UpdateCollegeAdminDTO>>();
            var validationResult = await validator.ValidateAsync(updatedCollegeAdmin);

            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.Failure(validationResult
                   .Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }
            var collegeInfo = await _unitOfWorkRepository.CollegeRepository.GetCollegeAuthorizationInfo(updatedCollegeAdmin.CollegeId);
            if(collegeInfo == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.InvalidRelatedData();
            }
            if(!scope.IsWithinScope(collegeInfo.ToCollegeInfo()))
            {
                return AddUpdateServiceResponse<EmployeeDTO>.ResourceDoesntExist<College>();
                // not returning 403 because we dont want the end user to know that resource exists even
            }

            var employeeInfo = await GetEmployeeAuthorizationInfoAsync(employeeId);
            if(employeeInfo == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.ResourceDoesntExist<Employee>();
            }

            if (!scope.IsWithinScope(employeeInfo.ToEmployeeInfo()))
            {
                return AddUpdateServiceResponse<EmployeeDTO>.ResourceDoesntExist<Employee>();
            }

            var employee = await GetEntityById(employeeId);
         

            var user = await _unitOfWorkRepository.UserRepository.GetUserEntityById(employee!.UserId);
            if(user == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.ResourceDoesntExist<User>();
            }
            if(await _unitOfWorkRepository.UserRepository.IsUserExist(updatedCollegeAdmin.UserName)&&user.UserName != updatedCollegeAdmin.UserName)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.AlreadyExists<User>();
            }
            employee.CollegeId = collegeInfo.CollegeId;
            employee.UniversityId = collegeInfo.UniversityId;
            await UpdateEmployee(user,   updatedCollegeAdmin, currentUserId);
            var employeeDTO = await GetDTOById(employee.EmployeeId);
            return AddUpdateServiceResponse<EmployeeDTO>.Success(employeeDTO!);
        }

        public async Task<AddUpdateServiceResponse<EmployeeDTO>> UpdateDepartmentAdmin(UserScope?scope,int employeeId, UpdateDepartmentAdminDTO updatedDepartmentAdmin, int currentUserId)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UpdateDepartmentAdminDTO>>();
            var validationResult = await validator.ValidateAsync(updatedDepartmentAdmin);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.Failure(validationResult
                 .Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var departmentInfo = await _unitOfWorkRepository.DepartmentRepository.GetDepartmentAuthorizationInfoAsync(updatedDepartmentAdmin.DepartmentId);
            if(departmentInfo == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.InvalidRelatedData();
            }
            if(!scope.IsWithinScope(departmentInfo.ToDepartmentInfo()))
            {
                return AddUpdateServiceResponse<EmployeeDTO>.ResourceDoesntExist<Department>();
                // not returning 403 because we dont want the end user to know that resource exists even
            }

            var employeeInfo = await GetEmployeeAuthorizationInfoAsync(employeeId);
            if(employeeInfo == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.ResourceDoesntExist<Employee>();
            }

            if(!scope.IsWithinScope(employeeInfo.ToEmployeeInfo()))
            {
                return AddUpdateServiceResponse<EmployeeDTO>.ResourceDoesntExist<Employee>();
            }

            var employee = await GetEntityById(employeeId);

            var user = await _unitOfWorkRepository.UserRepository.GetUserEntityById(employee!.UserId);
            if(user == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.ResourceDoesntExist<User>();
            }

            if(await _unitOfWorkRepository.UserRepository.IsUserExist(updatedDepartmentAdmin.UserName)&&user.UserName!=updatedDepartmentAdmin.UserName)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.AlreadyExists<User>();
            }
            employee.DepartmentId = departmentInfo.DepartmentId;
            employee.CollegeId = departmentInfo.CollegeId;
            employee.UniversityId = departmentInfo.UniversityId;
            await UpdateEmployee(user, updatedDepartmentAdmin, currentUserId);
            var employeeDTO = await GetDTOById(employee.EmployeeId);
            return AddUpdateServiceResponse<EmployeeDTO>.Success(employeeDTO!);
        }

        public async Task<AddUpdateServiceResponse<EmployeeDTO>> UpdateUniversityAdmin(int employeeId, UpdateUniversityAdminDTO updatedUniversityAdmin, int currentUserId)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UpdateUniversityAdminDTO>>();
            var validationResult = await validator.ValidateAsync(updatedUniversityAdmin);

            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.Failure(validationResult
                    .Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var employee = await GetEntityById(employeeId);
            if(employee == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.ResourceDoesntExist<Employee>();
            }

            var user = await _unitOfWorkRepository.UserRepository.GetUserEntityById(employee.UserId);
            if(user == null)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.ResourceDoesntExist<User>();
            }

            if(await _unitOfWorkRepository.UserRepository.IsUserExist(updatedUniversityAdmin.UserName)&& user.UserName != updatedUniversityAdmin.UserName)
            {
                return AddUpdateServiceResponse<EmployeeDTO>.AlreadyExists<User>();
            }

            await UpdateEmployee(user, updatedUniversityAdmin, currentUserId);
            var employeeDTO = await GetDTOById(employee.EmployeeId);
            return AddUpdateServiceResponse<EmployeeDTO>.Success(employeeDTO!);
        }


        private async Task<Employee> CreateEmployee(BaseAddEmployeeOrStudentDTO dto,int currentUserId,int universityId, int roleId,int? collegeId = null, int? departmentId = null)
        {
            await _unitOfWorkRepository.BeginTransactionAsync();
            try
            {
                var userEntity = new User
                {
                    FullName = dto.FullName,
                    UserName = dto.UserName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = currentUserId,
                    UniversityId = universityId,
                    Type = UserType.Employee,
                };
                await _unitOfWorkRepository.UserRepository.Add(userEntity);
                await _unitOfWorkRepository.CompleteAsync();

                var employeeEntity = new Employee
                {
                    UserId = userEntity.UserId,
                    UniversityId = universityId,
                    CollegeId = collegeId,
                    DepartmentId = departmentId
                };

                 _unitOfWorkRepository.EmployeeRepository.Add(employeeEntity);

                var userRole = new UserRole
                {
                    UserId = userEntity.UserId,
                    RoleId = roleId
                };

                await _unitOfWorkRepository.UserRoleRepository.Add(userRole);
                await _unitOfWorkRepository.CompleteAsync();
                await _unitOfWorkRepository.CommitTransactionAsync();
                return employeeEntity;
            }
            catch(Exception)
            {
                await _unitOfWorkRepository.RollbackTransactionAsync();
                throw;
            }
          
        }
        private async Task UpdateEmployee(User user,BaseUpdateEmployeeOrStudentDTO dto, int currentUserId)
        {          

            user.FullName = dto.FullName;
            user.UserName = dto.UserName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Email = dto.Email;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedByUserId = currentUserId;
            await _unitOfWorkRepository.CompleteAsync();

        }


    }
}
