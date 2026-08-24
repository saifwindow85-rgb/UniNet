using Contracts.Common.Extensions;
using Contracts.Common.Mappers;
using Contracts.Enums;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SectionSubjectRequests;
using Contracts.Responses;
using Contracts.Responses.StudyResponses.SectionSubjectResponses;
using Contracts.Results;
using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Domain.Entities.Academic_Structure;
using Domain.Entities.Study;
using Domain.Interfaces.StudyInterfaces.SectionSubjectInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.StudyServices
{
    public class SectionSubjectService : ISectionSubjectService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IValidator<AddSectionSubjectDTO> _addValidator;
        private readonly IValidator<UpdateSectionSubjectDTO> _updateValidator;

        public SectionSubjectService(IUnitOfWorkRepository unitOfWorkRepository,
            IValidator<AddSectionSubjectDTO> addValidator, IValidator<UpdateSectionSubjectDTO> updateValidator)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
        }

        public async Task<AddUpdateServiceResponse<SectionSubjectDTO>> AddSectionSubject(UserScope? scope, AddSectionSubjectDTO newSectionSubject, int currentUserId)
        {
            var validationResult = await _addValidator.ValidateAsync(newSectionSubject);
            if (!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<SectionSubjectDTO>.Failure
                    (validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            // 1) الشعبة موجودة وضمن نطاق المستخدم.
            var sectionInfo = await _unitOfWorkRepository.SectionRepository.GetSectionAuthorizationInfoAsync(newSectionSubject.SectionId);
            if (sectionInfo == null)
            {
                return AddUpdateServiceResponse<SectionSubjectDTO>.ResourceDoesntExist<Section>();
            }

            if (!scope.IsWithinScope(sectionInfo.ToSectionInfo()))
            {
                return AddUpdateServiceResponse<SectionSubjectDTO>.ResourceDoesntExist<Section>();
            }

            // 2) المادة موجودة، ويجب أن تكون تابعة لنفس قسم الشعبة.
            var subjectInfo = await _unitOfWorkRepository.SubjectRepository.GetSubjectAuthorizationInfoAsync(newSectionSubject.SubjectId);
            if (subjectInfo == null)
            {
                return AddUpdateServiceResponse<SectionSubjectDTO>.ResourceDoesntExist<Subject>();
            }

            if (subjectInfo.DepartmentId != sectionInfo.DepartmentId)
            {
                return AddUpdateServiceResponse<SectionSubjectDTO>.InvalidRelatedData();
            }

            // 3) الفصل الدراسي موجود، ويجب أن يكون تابعًا لنفس جامعة الشعبة.
            var semesterInfo = await _unitOfWorkRepository.SemesterRepository.GetSemesterAuthorizationInfoAsync(newSectionSubject.SemesterId);
            if (semesterInfo == null)
            {
                return AddUpdateServiceResponse<SectionSubjectDTO>.ResourceDoesntExist<Semester>();
            }

            if (semesterInfo.UniversityId != sectionInfo.UniversityId)
            {
                return AddUpdateServiceResponse<SectionSubjectDTO>.InvalidRelatedData();
            }

            // 4) عدم تكرار الإسناد (نفس المادة لنفس الشعبة في نفس الفصل).
            if (await _unitOfWorkRepository.SectionSubjectRepository.IsAlreadyAssigned(newSectionSubject.SectionId, newSectionSubject.SubjectId, newSectionSubject.SemesterId))
            {
                return AddUpdateServiceResponse<SectionSubjectDTO>.AlreadyExists<SectionSubject>();
            }

            var sectionSubjectEntity = new SectionSubject
            {
                SectionId = newSectionSubject.SectionId,
                SubjectId = newSectionSubject.SubjectId,
                SemesterId = newSectionSubject.SemesterId,
                LecturerName = newSectionSubject.LecturerName,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId,
            };

            _unitOfWorkRepository.SectionSubjectRepository.Add(sectionSubjectEntity);
            await _unitOfWorkRepository.CompleteAsync();
            var sectionSubjectDto = await GetDTOById(sectionSubjectEntity.SectionSubjectId);
            return AddUpdateServiceResponse<SectionSubjectDTO>.Success(sectionSubjectDto!);
        }

        public async Task<AddUpdateServiceResponse<SectionSubjectDTO>> UpdateSectionSubject(UserScope? scope, UpdateSectionSubjectDTO updatedSectionSubject, int sectionSubjectId, int currentUserId)
        {
            var validationResult = await _updateValidator.ValidateAsync(updatedSectionSubject);
            if (!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<SectionSubjectDTO>.Failure
                    (validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var sectionSubjectInfo = await GetSectionSubjectAuthorizationInfoAsync(sectionSubjectId);
            if (sectionSubjectInfo == null)
            {
                return AddUpdateServiceResponse<SectionSubjectDTO>.ResourceDoesntExist<SectionSubject>();
            }

            if (!scope.IsWithinScope(sectionSubjectInfo.ToSectionSubjectInfo()))
            {
                return AddUpdateServiceResponse<SectionSubjectDTO>.ResourceDoesntExist<SectionSubject>();
            }

            var sectionSubject = await GetEntityById(sectionSubjectId);

            sectionSubject!.LecturerName = updatedSectionSubject.LecturerName;
            sectionSubject.UpdatedAt = DateTime.UtcNow;
            sectionSubject.UpdatedByUserId = currentUserId;

            await _unitOfWorkRepository.CompleteAsync();
            var sectionSubjectDto = await GetDTOById(sectionSubject.SectionSubjectId);
            return AddUpdateServiceResponse<SectionSubjectDTO>.Success(sectionSubjectDto!);
        }

        public async Task<bool> Delete(int sectionSubjectId)
        {
            var sectionSubject = await GetEntityById(sectionSubjectId);
            if (sectionSubject == null)
                return false;

            var result = _unitOfWorkRepository.SectionSubjectRepository.Delete(sectionSubject);
            if (result)
                await _unitOfWorkRepository.CompleteAsync();

            return result;
        }

        public async Task<PagedResult<SectionSubjectDTO>> GetAll(SectionSubjectFilterDTO? filter, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.SectionSubjectRepository.GetAll(filter, pageNumber, pageSize);
        }

        public async Task<PagedResult<SectionSubjectDTO>> GetSectionSubjectsPerScope(UserScope? scope, SectionSubjectFilterDTO? filter, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.SectionSubjectRepository.GetSectionSubjectsPerScope(scope, filter, pageNumber, pageSize);
        }

        public async Task<SectionSubjectDTO?> GetDTOById(int sectionSubjectId)
        {
            return await _unitOfWorkRepository.SectionSubjectRepository.GetDTOById(sectionSubjectId);
        }

        public async Task<DetaieldSectionSubjectDTO?> GetDetaieldSectionSubjectDTOById(int sectionSubjectId)
        {
            return await _unitOfWorkRepository.SectionSubjectRepository.GetDetaieldSectionSubjectDTOById(sectionSubjectId);
        }

        public async Task<SectionSubject?> GetEntityById(int sectionSubjectId)
        {
            return await _unitOfWorkRepository.SectionSubjectRepository.GetEntityById(sectionSubjectId);
        }

        public async Task<SectionSubjectAuthorizationInfo?> GetSectionSubjectAuthorizationInfoAsync(int sectionSubjectId)
        {
            return await _unitOfWorkRepository.SectionSubjectRepository.GetSectionSubjectAuthorizationInfoAsync(sectionSubjectId);
        }

        public async Task<bool> IsExistsById(int sectionSubjectId)
        {
            return await _unitOfWorkRepository.SectionSubjectRepository.IsExistsById(sectionSubjectId);
        }
    }
}
