using Contracts.Common.AuthorizationInfos.StudentAuthorizationInfo;
using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using Contracts.Common.Extensions;
using Contracts.Common.Mappers;
using Contracts.Enums;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudyRequestes.StudentResultRequests;
using Contracts.Responses;
using Contracts.Responses.StudyResponses.StudentResultResponses;
using Contracts.Results;
using Domain.Entities.Students;
using Domain.Entities.Study;
using Domain.Interfaces.StudyInterfaces.StudentResultInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.StudyServices
{
    public class StudentResultService : IStudentResultService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IValidator<AddStudentResultDTO> _addValidator;
        private readonly IValidator<UpdateStudentResultDTO> _updateValidator;

        public StudentResultService(IUnitOfWorkRepository unitOfWorkRepository,
            IValidator<AddStudentResultDTO> addValidator, IValidator<UpdateStudentResultDTO> updateValidator)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
        }

        public async Task<AddUpdateServiceResponse<DetaieldStudentResultDTO>> AddStudentResult(UserScope? scope, AddStudentResultDTO newStudentResult, int currentUserId)
        {
            var validationResult = await _addValidator.ValidateAsync(newStudentResult);
            if (!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<DetaieldStudentResultDTO>.Failure
                    (validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            // 1) الإسناد (SectionSubject) موجود وضمن نطاق المستخدم.
            var sectionSubjectInfo = await _unitOfWorkRepository.SectionSubjectRepository.GetSectionSubjectAuthorizationInfoAsync(newStudentResult.SectionSubjectId);
            if (sectionSubjectInfo == null)
            {
                return AddUpdateServiceResponse<DetaieldStudentResultDTO>.ResourceDoesntExist<SectionSubject>();
            }

            if (!scope.IsWithinScope(sectionSubjectInfo.ToSectionSubjectInfo()))
            {
                return AddUpdateServiceResponse<DetaieldStudentResultDTO>.ResourceDoesntExist<SectionSubject>();
            }

            // 2) الطالب موجود.
            var student = await _unitOfWorkRepository.StudentRepository.GetEntityById(newStudentResult.StudentId);
            if (student == null)
            {
                return AddUpdateServiceResponse<DetaieldStudentResultDTO>.ResourceDoesntExist<Student>();
            }

            // 3) الحرج: يجب أن يكون الطالب في نفس شعبة الإسناد.
            if (sectionSubjectInfo.SectionId != student.SectionId)
            {
                return AddUpdateServiceResponse<DetaieldStudentResultDTO>.InvalidRelatedData();
            }

            // 4) عدم تكرار تسجيل نتيجة للطالب في نفس الإسناد.
            if (await _unitOfWorkRepository.StudentResultRepository.IsAlreadyRecorded(newStudentResult.StudentId, newStudentResult.SectionSubjectId))
            {
                return AddUpdateServiceResponse<DetaieldStudentResultDTO>.AlreadyExists<StudentResult>();
            }

            var studentResultEntity = new StudentResult
            {
                StudentId = newStudentResult.StudentId,
                SectionSubjectId = newStudentResult.SectionSubjectId,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId,
            };
            studentResultEntity.SetGrades(newStudentResult.Midterm, newStudentResult.Practical, newStudentResult.Final);

            _unitOfWorkRepository.StudentResultRepository.Add(studentResultEntity);
            await _unitOfWorkRepository.CompleteAsync();
            var dto = await GetDetaieldStudentResultDTOById(studentResultEntity.StudentResultId);
            return AddUpdateServiceResponse<DetaieldStudentResultDTO>.Success(dto!);
        }

        public async Task<AddUpdateServiceResponse<DetaieldStudentResultDTO>> UpdateStudentResult(UserScope? scope, UpdateStudentResultDTO updatedStudentResult, int studentResultId, int currentUserId)
        {
            var validationResult = await _updateValidator.ValidateAsync(updatedStudentResult);
            if (!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<DetaieldStudentResultDTO>.Failure
                    (validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var info = await GetStudentResultAuthorizationInfoAsync(studentResultId);
            if (info == null)
            {
                return AddUpdateServiceResponse<DetaieldStudentResultDTO>.ResourceDoesntExist<StudentResult>();
            }

            if (!scope.IsWithinScope(info.ToStudentResultInfo()))
            {
                return AddUpdateServiceResponse<DetaieldStudentResultDTO>.ResourceDoesntExist<StudentResult>();
            }

            var studentResult = await GetEntityById(studentResultId);
            studentResult!.SetGrades(updatedStudentResult.Midterm, updatedStudentResult.Practical, updatedStudentResult.Final);
            studentResult.UpdatedAt = DateTime.UtcNow;
            studentResult.UpdatedByUserId = currentUserId;

            await _unitOfWorkRepository.CompleteAsync();
            var dto = await GetDetaieldStudentResultDTOById(studentResult.StudentResultId);
            return AddUpdateServiceResponse<DetaieldStudentResultDTO>.Success(dto!);
        }

        public async Task<bool> Delete(int studentResultId)
        {
            var studentResult = await GetEntityById(studentResultId);
            if (studentResult == null)
                return false;

            var result = _unitOfWorkRepository.StudentResultRepository.Delete(studentResult);
            if (result)
                await _unitOfWorkRepository.CompleteAsync();

            return result;
        }

        public async Task<PagedResult<DetaieldStudentResultDTO>> GetAll(StudentResultFilterDTO? filter, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.StudentResultRepository.GetAll(filter, pageNumber, pageSize);
        }

        public async Task<DetaieldStudentResultDTO?> GetDetaieldStudentResultDTOById(int studentResultId)
        {
            return await _unitOfWorkRepository.StudentResultRepository.GetDetaieldStudentResultDTOById(studentResultId);
        }

        public async Task<StudentResult?> GetEntityById(int studentResultId)
        {
            return await _unitOfWorkRepository.StudentResultRepository.GetEntityById(studentResultId);
        }

        public async Task<StudentResultAuthorizationInfo?> GetStudentResultAuthorizationInfoAsync(int studentResultId)
        {
            return await _unitOfWorkRepository.StudentResultRepository.GetStudentResultAuthorizationInfoAsync(studentResultId);
        }

        public async Task<StudentAuthorizationInfo?> GetStudentAuthorizationInfoAsync(int studentId)
        {
            return await _unitOfWorkRepository.StudentResultRepository.GetStudentAuthorizationInfoAsync(studentId);
        }

        public async Task<bool> IsExistsById(int studentResultId)
        {
            return await _unitOfWorkRepository.StudentResultRepository.IsExistsById(studentResultId);
        }

        public async Task<List<StudentSemesterResultDTO>> GetStudentResults(int studentId, StudentResultFilterDTO? filter)
        {
            return await _unitOfWorkRepository.StudentResultRepository.GetStudentResults(studentId, filter);
        }

        public async Task<List<StudentSemesterResultDTO>> GetAllStudentsResults(UserScope? scope, StudentResultFilterDTO? filter)
        {
            return await _unitOfWorkRepository.StudentResultRepository.GetAllStudentsResults(scope, filter);
        }
    }
}
