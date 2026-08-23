using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Common.Extensions;
using Contracts.Common.Mappers;
using Contracts.Enums;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SubjectRequests;
using Contracts.Responses;
using Contracts.Responses.StudyResponses.SubjectResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Entities.Study;
using Domain.Interfaces.StudyInterfaces.SubjectInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.StudyServices
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IValidator<AddSubjectDTO> _addValidator;
        private readonly IValidator<UpdateSubjectDTO> _updateValidator;

        public SubjectService(IUnitOfWorkRepository unitOfWorkRepository,
            IValidator<AddSubjectDTO>addValidator,IValidator<UpdateSubjectDTO>updateValidator)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
        }
        public async Task<AddUpdateServiceResponse<SubjectDTO>> AddSubject(UserScope? scope, AddSubjectDTO newSubject, int currentUserId)
        {
            var validationResult = await _addValidator.ValidateAsync(newSubject);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<SubjectDTO>.Failure
                    (validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var departmentInfo = await _unitOfWorkRepository.DepartmentRepository.GetDepartmentAuthorizationInfoAsync(newSubject.DepartmentId);
            if(departmentInfo == null)
            {
                return AddUpdateServiceResponse<SubjectDTO>.ResourceDoesntExist<Department>();
            }

            if (!scope.IsWithinScope(departmentInfo.ToDepartmentInfo()))
            {
                return AddUpdateServiceResponse<SubjectDTO>.ResourceDoesntExist<Department>();
            }

            if(await IsExistsByName(departmentInfo.DepartmentId,newSubject.Name))
            {
                return AddUpdateServiceResponse<SubjectDTO>.AlreadyExists<Subject>();
            }

            var subjectEntity = new Subject
            {
                Name = newSubject.Name,
                Code = newSubject.Code,
                Description = newSubject.Description,
                DepartmentId = newSubject.DepartmentId,
                CreditHours = newSubject.CreditHours,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId,
            };

            _unitOfWorkRepository.SubjectRepository.Add(subjectEntity);
            await _unitOfWorkRepository.CompleteAsync();
            var subjectDto = await GetDTOById(subjectEntity.SubjectId);
            return AddUpdateServiceResponse<SubjectDTO>.Success(subjectDto!);
        }

        public async Task<bool> Delete(int subjectId)
        {
            var subject = await GetEntityById(subjectId);
            if (subject == null)
                return false;

            var result = _unitOfWorkRepository.SubjectRepository.Delete(subject);
            if (result)
                await _unitOfWorkRepository.CompleteAsync();

            return result;
        }

        public async Task<PagedResult<SubjectDTO>> GetAll(SubjectFilterDTO? filter, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.SubjectRepository.GetAll(filter, pageNumber, pageSize);
        }

        public async Task<DetaieldSubjectDTO?> GetDetaieldSubjectDTOById(int subjectId)
        {
            return await _unitOfWorkRepository.SubjectRepository.GetDetaieldSubjectDTOById(subjectId);
        }

        public async Task<SubjectDTO?> GetDTOById(int subjectId)
        {
            return await _unitOfWorkRepository.SubjectRepository.GetDTOById(subjectId);
        }

        public async Task<Subject?> GetEntityById(int subjectId)
        {
            return await _unitOfWorkRepository.SubjectRepository.GetEntityById(subjectId);
        }

        public async Task<SubjectAuthorizationInfo?> GetSubjectAuthorizationInfoAsync(int subjecId)
        {
            return await _unitOfWorkRepository.SubjectRepository.GetSubjectAuthorizationInfoAsync(subjecId);
        }

        public async Task<PagedResult<SubjectDTO>> GetSubjectsPerDepartments(UserScope? scope, SubjectFilterDTO? filter, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.SubjectRepository.GetSubjectsPerDepartment(scope, filter, pageNumber, pageSize);
        }

        public async Task<bool> IsExistsById(int subjectId)
        {
            return await _unitOfWorkRepository.SubjectRepository.IsExistsById(subjectId);
        }

        public async Task<bool> IsExistsByName(int departmentId, string name)
        {
           return await _unitOfWorkRepository.SubjectRepository.IsExistsByName(departmentId, name);
        }

        public async Task<AddUpdateServiceResponse<SubjectDTO>> UpdateSubject(UserScope? scope, UpdateSubjectDTO updatedSubject, int subjectId, int currentUserId)
        {
            var validationResult = await _updateValidator.ValidateAsync(updatedSubject);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<SubjectDTO>.Failure
                   (validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var subjectInfo = await GetSubjectAuthorizationInfoAsync(subjectId);
            if (subjectInfo == null)
            {
                return AddUpdateServiceResponse<SubjectDTO>.ResourceDoesntExist<Subject>();
            }

            if(!scope.IsWithinScope(subjectInfo.ToSubjectInfo()))
            {
                return AddUpdateServiceResponse<SubjectDTO>.ResourceDoesntExist<Subject>();
            }

            var subject = await GetEntityById(subjectId);

            subject!.Name = updatedSubject.Name;
            subject.Description = updatedSubject.Description;
            subject.Code = updatedSubject.Code;
            subject.CreditHours = updatedSubject.CreditHours;
            subject.UpdatedAt = DateTime.UtcNow;
            subject.UpdatedByUserId = currentUserId;

            await _unitOfWorkRepository.CompleteAsync();
            var subjectDto = await GetDTOById(subject.SubjectId);
            return  AddUpdateServiceResponse<SubjectDTO>.Success(subjectDto!);
        }

    }
}
