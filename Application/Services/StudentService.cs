using Contracts.Common.AuthorizationInfos.StudentAuthorizationInfo;
using Contracts.Common.Extensions;
using Contracts.Common.Mappers;
using Contracts.Enums;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudentRequests;
using Contracts.Responses;
using Contracts.Responses.StudentResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Entities.Identity;
using Domain.Entities.Students;
using Domain.Interfaces.StudentInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWorkRepository _unitOfWork;
        private readonly IValidator<AddStudentDTO> _addStudentValidator;
        private readonly IValidator<UpdateStudentDTO> _updateStudentValidator;

        public StudentService(IUnitOfWorkRepository unitOfWork, IValidator<AddStudentDTO> addStudentValidator, IValidator<UpdateStudentDTO> updateStudentValidator)
        {
            _unitOfWork = unitOfWork;
            _addStudentValidator = addStudentValidator;
            _updateStudentValidator = updateStudentValidator;
        }

        public async Task<AddUpdateServiceResponse<StudentDTO>> AddBatchAdmin(UserScope? scope, AddStudentDTO newStudent, int currentUser)
        {
            var role = await _unitOfWork.RoleRepository.GetRoleDTOByRoleName("BatchAdmin");
            if (role == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<Role>();
            }

            var status = await _unitOfWork.StatusRepository.GetDTOByName("Enrolled");
            if (status == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<StudentStatus>();
            }
            return await CreateStudent(scope, newStudent, currentUser, role.RoleId, status.StatusId);
        }

        public async Task<AddUpdateServiceResponse<StudentDTO>> AddStudent(UserScope? scope, AddStudentDTO newStudent, int currentUser)
        {
            var role = await _unitOfWork.RoleRepository.GetRoleDTOByRoleName("Student");
            if (role == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<Role>();
            }

            var status = await _unitOfWork.StatusRepository.GetDTOByName("Enrolled");
            if(status == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<StudentStatus>();
            }
            return await CreateStudent(scope, newStudent, currentUser, role.RoleId, status.StatusId);
        }

        public async Task<bool> ExistsByStudentNumber(string studentNumber)
        {
            return await _unitOfWork.StudentRepository.ExistsByStudentNumber(studentNumber);
        }

        public async Task<StudentDTO?> GetDTOById(int studentId)
        {
            return await _unitOfWork.StudentRepository.GetDTOById(studentId);
        }

        public async Task<StudentDTO?> GetDTOByStudentNumber(string studentNumber)
        {
            return await _unitOfWork.StudentRepository.GetDTOByStudentNumber(studentNumber);
        }

        public async Task<Student?> GetEntityById(int studentId)
        {
            return await _unitOfWork.StudentRepository.GetEntityById(studentId);
        }

        public async Task<StudentAuthorizationInfo?> GetStudentAuthorizationInfoAsync(int studentId)
        {
            return await _unitOfWork.StudentRepository.GetStudentAuthorizationInfoAsync(studentId);
        }

        public async Task<PagedResult<StudentDTO>> GetStudents(UserScope? scope, StudentFilter? filter, int pageNumber, int pageSize)
        {
            return await _unitOfWork.StudentRepository.GetStudents(scope, filter, pageNumber, pageSize);
        }

        public async Task<AddUpdateServiceResponse<StudentDTO>> UpdateBatchAdmin(int studentId,UserScope? scope, UpdateStudentDTO updatedStudent, int currentUser)
        {
            var role = await _unitOfWork.RoleRepository.GetRoleDTOByRoleName("BatchAdmin");
            if (role == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<Role>();
            }

            return await UpdateStudent(studentId, scope, updatedStudent, currentUser, role.RoleId);
        }

        public async Task<AddUpdateServiceResponse<StudentDTO>> UpdateStudent(int studentId,UserScope? scope, UpdateStudentDTO updatedStudent, int currentUser)
        {
            var role = await _unitOfWork.RoleRepository.GetRoleDTOByRoleName("Student");
            if(role == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<Role>();
            }

            var status = await _unitOfWork.StatusRepository.GetDTOByName("Enrolled");
            if (status == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<StudentStatus>();
            }
            return await UpdateStudent(studentId, scope, updatedStudent, currentUser, role.RoleId);
        }

        public async Task<AddUpdateServiceResponse<StudentDTO>> UpdateStudentStatus(UserScope?scope,int studentId, int statusId,int currentUserId)
        {
            var studentInfo = await GetStudentAuthorizationInfoAsync(studentId);
            if(studentInfo == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<Student>();
            }

            if(!scope.IsWithinScope(studentInfo.ToStudentInfo()))
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<Student>();
            }

            var student = await GetEntityById(studentId);
         

            if (!await _unitOfWork.StatusRepository.IsExistsById(statusId))
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<StudentStatus>();
            }

            var user = await _unitOfWork.UserRepository.GetUserEntityById(student!.UserId);
            if(user == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.InvalidRelatedData();
            }

            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedByUserId = currentUserId;

            student.StatusId = statusId;
            await _unitOfWork.CompleteAsync();
            var studentDto = await GetDTOById(studentId);
            return AddUpdateServiceResponse<StudentDTO>.Success(studentDto!);
        }

        private async Task<AddUpdateServiceResponse<StudentDTO>> CreateStudent(UserScope? scope, AddStudentDTO newStudent, int currentUser, int roleId,int statusId)
        {
            var validationResult = await _addStudentValidator.ValidateAsync(newStudent);
            if (!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<StudentDTO>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList(), EnErrorTypes.InvalidData);
            }
            if (await ExistsByStudentNumber(newStudent.StudentNumber))
            {
                return AddUpdateServiceResponse<StudentDTO>.AlreadyExists<Student>();
            }
            if (await _unitOfWork.UserRepository.IsUserExist(newStudent.UserName))
            {
                return AddUpdateServiceResponse<StudentDTO>.AlreadyExists<User>();
            }
            var batchInfo = await _unitOfWork.BatchRepository.GetBatchAuthorizationInfoAsync(newStudent.BatchId);
            if (batchInfo == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<Batch>();
            }
            if (!scope.IsWithinScope(batchInfo.ToBatchInfo()))
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<Batch>();
            }
            if (newStudent.SectionId.HasValue)
            {
                var sectionInfo = await _unitOfWork.SectionRepository.GetSectionAuthorizationInfoAsync(newStudent.SectionId.Value);
                if (sectionInfo!.BatchId != batchInfo.BatchId)
                {
                    return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<Section>();
                }
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var userEntity = new User
                {
                    FullName = newStudent.FullName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(newStudent.Password),
                    UserName = newStudent.UserName,
                    IsActive = newStudent.IsActive,
                    Email = newStudent.Email,
                    PhoneNumber = newStudent.PhoneNumber,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = currentUser,
                    Type = UserType.Student,
                    UniversityId = batchInfo.UniversityId,
                };
                await _unitOfWork.UserRepository.Add(userEntity);
                await _unitOfWork.CompleteAsync();
                var studentEntity = new Student
                {
                    UserId = userEntity.UserId,
                    StudentNumber = newStudent.StudentNumber,
                    BatchId = newStudent.BatchId,
                    SectionId = newStudent.SectionId,
                    StatusId = statusId,
                };
                _unitOfWork.StudentRepository.Add(studentEntity);
                var userRole = new UserRole
                {
                    UserId = userEntity.UserId,
                    RoleId = roleId
                };
                await _unitOfWork.UserRoleRepository.Add(userRole);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
                var studentDTO = await GetDTOById(studentEntity.StudentId);
                return AddUpdateServiceResponse<StudentDTO>.Success(studentDTO!);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task<AddUpdateServiceResponse<StudentDTO>> UpdateStudent(int studentId, UserScope? scope, UpdateStudentDTO updatedStudent, int currentUser, int roleId)
        {
            var validationResult = await _updateStudentValidator.ValidateAsync(updatedStudent);
            if (!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<StudentDTO>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList(), EnErrorTypes.InvalidData);
            }
            var studentEntity = await _unitOfWork.StudentRepository.GetEntityById(studentId);
            if (studentEntity == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<Student>();
            }
            var batchInfo = await _unitOfWork.BatchRepository.GetBatchAuthorizationInfoAsync(studentEntity.BatchId);
            if (batchInfo == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.InvalidRelatedData();
            }
            if (!scope.IsWithinScope(batchInfo.ToBatchInfo()))
            {
                return AddUpdateServiceResponse<StudentDTO>.InvalidRelatedData();
            }
            var user = await _unitOfWork.UserRepository.GetUserEntityById(studentEntity.UserId);
            if (user == null)
            {
                return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<User>();
            }

            if (await _unitOfWork.UserRepository.IsUserExist(updatedStudent.UserName) && user.UserName != updatedStudent.UserName)
            {
                return AddUpdateServiceResponse<StudentDTO>.AlreadyExists<User>();
            }
            if (updatedStudent.SectionId.HasValue)
            {
                var sectionInfo = await _unitOfWork.SectionRepository.GetSectionAuthorizationInfoAsync(updatedStudent.SectionId.Value);
                if (sectionInfo!.BatchId != batchInfo.BatchId)
                {
                    return AddUpdateServiceResponse<StudentDTO>.ResourceDoesntExist<Section>();
                }
            }

            user.FullName = updatedStudent.FullName;
            user.UserName = updatedStudent.UserName;
            user.Email = updatedStudent.Email;
            user.PhoneNumber = updatedStudent.PhoneNumber;
            user.IsActive = updatedStudent.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedByUserId = currentUser;
            studentEntity.SectionId = updatedStudent.SectionId;

            await _unitOfWork.CompleteAsync();
            // Additional logic for updating the student entity can be added here
            // For now, we will just return a success response with the existing student DTO
            var studentDTO = await GetDTOById(studentEntity.StudentId);
            return AddUpdateServiceResponse<StudentDTO>.Success(studentDTO!);
        }

    }
}
