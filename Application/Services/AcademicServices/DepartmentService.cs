using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Common.Extensions;
using Contracts.Enums;
using Contracts.Requests.AcademicRequests.DepartmentRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.AcademicResponses.DepartmentResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.DepartmentInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AcademicServices
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWorkRepository _unitOfWork;
        private readonly IValidator<AddDepartmentDTO> _addValidator;
        private readonly IValidator<UpdateDepartmentDTO> _updateValidator;
        public DepartmentService(IUnitOfWorkRepository unitOfWork
            , IValidator<AddDepartmentDTO>addValidator, IValidator<UpdateDepartmentDTO> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
        }
        public async Task<AddUpdateServiceResponse<DepartmentDTO>> AddDepartment(UserScope?scope,AddDepartmentDTO newDepartment, int currentUserId)
        {
            var validationResult = await _addValidator.ValidateAsync(newDepartment);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<DepartmentDTO>.Failure
                    (validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(),EnErrorTypes.InvalidData);
            }


            var collegeInfo = await _unitOfWork.CollegeRepository.GetCollegeAuthorizationInfo(newDepartment.CollegeId);
            if(collegeInfo == null)
            {
                return AddUpdateServiceResponse<DepartmentDTO>.ResourceDoesntExist<Department>();
            }
            
            if(!scope.IsWithinScope(collegeInfo.UniversityId,collegeInfo.CollegeId))
            {
                return AddUpdateServiceResponse<DepartmentDTO>.ResourceDoesntExist<Department>();
            }

            if(await ExistsByName(newDepartment.CollegeId,newDepartment.DepartmentName))
            {
                return AddUpdateServiceResponse<DepartmentDTO>.AlreadyExists<Department>();
            }
            var departmentEntity = new Department
            {
               CollegeId = newDepartment.CollegeId,
                Name = newDepartment.DepartmentName,
                Description = newDepartment.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId
            };
            _unitOfWork.DepartmentRepository.Add(departmentEntity);
            await _unitOfWork.CompleteAsync();
            var departmentDTO = await GetDTOById(departmentEntity.DepartmentId);
            return AddUpdateServiceResponse<DepartmentDTO>.Success(departmentDTO!);
        }

        public async Task<bool> Delete(int departmentId)
        {
            var result = await _unitOfWork.DepartmentRepository.Delete(departmentId);
            if (result)
                await _unitOfWork.CompleteAsync();

            return result;
        }

        public async Task<bool> ExistsById(int departmentId)
        {
            return await _unitOfWork.DepartmentRepository.ExistsById(departmentId);
        }

        public async Task<bool> ExistsByName(int collegeId, string name)
        {
            return await _unitOfWork.DepartmentRepository.ExistsByName(collegeId, name);
        }

        public async Task<PagedResult<DepartmentDTO>> GetAllDepartments(int pageNumber, int pageSize)
        {
            return await _unitOfWork.DepartmentRepository.GetAllDepartments(pageNumber, pageSize); 
        }

        public async Task<DepartmentAuthorizationInfo?> GetDepartmentAuthorizationInfoAsync(int departmentId)
        {
            return await _unitOfWork.DepartmentRepository.GetDepartmentAuthorizationInfoAsync(departmentId);
        }


        public async Task<PagedResult<DepartmentDTO>> GetDepartmentsPerCollege(int collegeId, int pageNumber, int pageSize)
        {
            return await _unitOfWork.DepartmentRepository.GetDepartmentsPerCollege(collegeId, pageNumber, pageSize);
        }

        public async Task<DepartmentDTO?> GetDTOById(int departmentId)
        {
            return await _unitOfWork.DepartmentRepository.GetDTOById(departmentId);
        }

        public async Task<DepartmentDTO?> GetDTOByName(int collegeId, string name)
        {
            return await _unitOfWork.DepartmentRepository.GetDTOByName(collegeId, name);
        }

        public async Task<Department?> GetEntityById(int departmentId)
        {
            return await _unitOfWork.DepartmentRepository.GetEntityById(departmentId);
        }

        public async Task<Department?> GetEntityByName(int departmentId, string name)
        {
            return await _unitOfWork.DepartmentRepository.GetEntityByName(departmentId, name);
        }

        public async Task<AddUpdateServiceResponse<DepartmentDTO>> UpdateDepartment(UserScope?scope,int departmentId, UpdateDepartmentDTO updatedDepartment, int currentUserId)
        {
            var validationResult = await _updateValidator.ValidateAsync(updatedDepartment);

            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<DepartmentDTO>.Failure
                   (validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var departmentInfo = await GetDepartmentAuthorizationInfoAsync(departmentId);
            if(departmentInfo == null)
            {
                return AddUpdateServiceResponse<DepartmentDTO>.AlreadyExists<Department>();
            }

            if(!scope.IsWithinScope(departmentInfo.UniversityId,departmentInfo.CollegeId,departmentInfo.DepartmentId))
            {
                return AddUpdateServiceResponse<DepartmentDTO>.AlreadyExists<Department>();
            }


            var department = await GetEntityById(departmentId);

            if(await ExistsByName(department!.CollegeId,updatedDepartment.DepartmentName)&& department.Name != updatedDepartment.DepartmentName)
            {
                return AddUpdateServiceResponse<DepartmentDTO>.AlreadyExists<Department>();
            }
            department.Name = updatedDepartment.DepartmentName;
            department.Description = updatedDepartment.Description;
            department.UpdatedAt = DateTime.UtcNow;
            department.UpdatedByUserId = currentUserId;

            await _unitOfWork.CompleteAsync();
            var departmentDTO = await GetDTOById(department.DepartmentId);
            return AddUpdateServiceResponse<DepartmentDTO>.Success(departmentDTO!);
        }
    }
}
