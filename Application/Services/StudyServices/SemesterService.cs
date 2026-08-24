using Contracts.Common.Extensions;
using Contracts.Common.Mappers;
using Contracts.Enums;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.SemesterRequests;
using Contracts.Responses;
using Contracts.Responses.StudyResponses.SemesterResponses;
using Contracts.Results;
using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Domain.Entities.Academic_Structure;
using Domain.Entities.Study;
using Domain.Interfaces.StudyInterfaces.SemesterInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.StudyServices
{
    public class SemesterService : ISemesterService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IValidator<AddSemesterDTO> _addValidator;
        private readonly IValidator<UpdateSemesterDTO> _updateValidator;

        public SemesterService(IUnitOfWorkRepository unitOfWorkRepository,
            IValidator<AddSemesterDTO> addValidator, IValidator<UpdateSemesterDTO> updateValidator)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
        }

        public async Task<AddUpdateServiceResponse<SemesterDTO>> AddSemester(UserScope? scope, AddSemesterDTO newSemester, int currentUserId)
        {
            var validationResult = await _addValidator.ValidateAsync(newSemester);
            if (!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<SemesterDTO>.Failure
                    (validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var universityInfo = await _unitOfWorkRepository.UniversityRepository.GetUniversityAuthorizationInfoAsync(newSemester.UniversityId);
            if (universityInfo == null)
            {
                return AddUpdateServiceResponse<SemesterDTO>.ResourceDoesntExist<University>();
            }

            if (!scope.IsWithinScope(universityInfo.ToUniversityInfo()))
            {
                return AddUpdateServiceResponse<SemesterDTO>.ResourceDoesntExist<University>();
            }

            // فصل حالي موجود؟ إن انتهى تاريخه نُنهيه تلقائيًا (يصبح Not Current)، وإلا نمنع فتح فصل جديد.
            var currentSemester = await _unitOfWorkRepository.SemesterRepository.GetCurrentSemesterEntity(newSemester.UniversityId);
            if (currentSemester != null)
            {
                if (currentSemester.EndDate > DateTime.UtcNow)
                {
                    return AddUpdateServiceResponse<SemesterDTO>.CurrentSemesterExists();
                }

                // انتهى تاريخه ⇒ إنهاء تلقائي، ثم تحرير الخانة قبل إضافة الفصل الجديد.
                currentSemester.IsCurrent = false;
                currentSemester.UpdatedAt = DateTime.UtcNow;
                currentSemester.UpdatedByUserId = currentUserId;
                await _unitOfWorkRepository.CompleteAsync();
            }

            if (await IsExistsByName(newSemester.UniversityId, newSemester.Name))
            {
                return AddUpdateServiceResponse<SemesterDTO>.AlreadyExists<Semester>();
            }

            var semesterEntity = new Semester
            {
                Name = newSemester.Name,
                UniversityId = newSemester.UniversityId,
                StartDate = newSemester.StartDate,
                EndDate = newSemester.EndDate,
                IsCurrent = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId,
            };

            _unitOfWorkRepository.SemesterRepository.Add(semesterEntity);
            await _unitOfWorkRepository.CompleteAsync();
            var semesterDto = await GetDTOById(semesterEntity.SemesterId);
            return AddUpdateServiceResponse<SemesterDTO>.Success(semesterDto!);
        }

        public async Task<AddUpdateServiceResponse<SemesterDTO>> UpdateSemester(UserScope? scope, UpdateSemesterDTO updatedSemester, int semesterId, int currentUserId)
        {
            var validationResult = await _updateValidator.ValidateAsync(updatedSemester);
            if (!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<SemesterDTO>.Failure
                    (validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var semesterInfo = await GetSemesterAuthorizationInfoAsync(semesterId);
            if (semesterInfo == null)
            {
                return AddUpdateServiceResponse<SemesterDTO>.ResourceDoesntExist<Semester>();
            }

            if (!scope.IsWithinScope(semesterInfo.ToSemesterInfo()))
            {
                return AddUpdateServiceResponse<SemesterDTO>.ResourceDoesntExist<Semester>();
            }

            var semester = await GetEntityById(semesterId);

            // التعديل مسموح للفصل الحالي فقط.
            if (!semester!.IsCurrent)
            {
                return AddUpdateServiceResponse<SemesterDTO>.SemesterNotCurrent();
            }

            if (semester.Name != updatedSemester.Name && await IsExistsByName(semester.UniversityId, updatedSemester.Name))
            {
                return AddUpdateServiceResponse<SemesterDTO>.AlreadyExists<Semester>();
            }

            semester.Name = updatedSemester.Name;
            semester.StartDate = updatedSemester.StartDate;
            semester.EndDate = updatedSemester.EndDate;
            semester.UpdatedAt = DateTime.UtcNow;
            semester.UpdatedByUserId = currentUserId;

            await _unitOfWorkRepository.CompleteAsync();
            var semesterDto = await GetDTOById(semester.SemesterId);
            return AddUpdateServiceResponse<SemesterDTO>.Success(semesterDto!);
        }

        // الإنهاء اليدوي من UniversityAdmin: يجعل الفصل الحالي Not Current.
        public async Task<AddUpdateServiceResponse<SemesterDTO>> EndSemester(UserScope? scope, int semesterId, int currentUserId)
        {
            var semesterInfo = await GetSemesterAuthorizationInfoAsync(semesterId);
            if (semesterInfo == null)
            {
                return AddUpdateServiceResponse<SemesterDTO>.ResourceDoesntExist<Semester>();
            }

            if (!scope.IsWithinScope(semesterInfo.ToSemesterInfo()))
            {
                return AddUpdateServiceResponse<SemesterDTO>.ResourceDoesntExist<Semester>();
            }

            var semester = await GetEntityById(semesterId);
            if (!semester!.IsCurrent)
            {
                return AddUpdateServiceResponse<SemesterDTO>.SemesterNotCurrent();
            }

            semester.IsCurrent = false;
            semester.UpdatedAt = DateTime.UtcNow;
            semester.UpdatedByUserId = currentUserId;

            await _unitOfWorkRepository.CompleteAsync();
            var semesterDto = await GetDTOById(semester.SemesterId);
            return AddUpdateServiceResponse<SemesterDTO>.Success(semesterDto!);
        }

        public async Task<bool> Delete(int semesterId)
        {
            var semester = await GetEntityById(semesterId);
            if (semester == null)
                return false;

            var result = _unitOfWorkRepository.SemesterRepository.Delete(semester);
            if (result)
                await _unitOfWorkRepository.CompleteAsync();

            return result;
        }

        public async Task<PagedResult<SemesterDTO>> GetAll(SemesterFilterDTO? filter, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.SemesterRepository.GetAll(filter, pageNumber, pageSize);
        }

        public async Task<PagedResult<SemesterDTO>> GetSemestersPerUniversity(UserScope? scope, SemesterFilterDTO? filter, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.SemesterRepository.GetSemestersPerUniversity(scope, filter, pageNumber, pageSize);
        }

        // يعيد الفصل الحالي؛ وإن كان تاريخه قد انتهى يُنهيه تلقائيًا (Not Current) ثم يعيد null.
        public async Task<SemesterDTO?> GetCurrentSemester(int universityId)
        {
            var current = await _unitOfWorkRepository.SemesterRepository.GetCurrentSemesterEntity(universityId);
            if (current == null)
                return null;

            if (current.EndDate <= DateTime.UtcNow)
            {
                current.IsCurrent = false;
                current.UpdatedAt = DateTime.UtcNow;
                await _unitOfWorkRepository.CompleteAsync();
                return null;
            }

            return await GetDTOById(current.SemesterId);
        }

        public async Task<SemesterDTO?> GetDTOById(int semesterId)
        {
            return await _unitOfWorkRepository.SemesterRepository.GetDTOById(semesterId);
        }

        public async Task<DetaieldSemesterDTO?> GetDetaieldSemesterDTOById(int semesterId)
        {
            return await _unitOfWorkRepository.SemesterRepository.GetDetaieldSemesterDTOById(semesterId);
        }

        public async Task<Semester?> GetEntityById(int semesterId)
        {
            return await _unitOfWorkRepository.SemesterRepository.GetEntityById(semesterId);
        }

        public async Task<SemesterAuthorizationInfo?> GetSemesterAuthorizationInfoAsync(int semesterId)
        {
            return await _unitOfWorkRepository.SemesterRepository.GetSemesterAuthorizationInfoAsync(semesterId);
        }

        public async Task<bool> IsExistsById(int semesterId)
        {
            return await _unitOfWorkRepository.SemesterRepository.IsExistsById(semesterId);
        }

        public async Task<bool> IsExistsByName(int universityId, string name)
        {
            return await _unitOfWorkRepository.SemesterRepository.IsExistsByName(universityId, name);
        }
    }
}
