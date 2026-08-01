using Contracts.Common.AuthorizationInfos.StudentAuthorizationInfo;
using Contracts.Requests.RequestParameters;
using Contracts.Requests.StudentRequests;
using Contracts.Responses;
using Contracts.Responses.StudentResponses;
using Contracts.Results;
using Domain.Entities.Students;
using Domain.Interfaces.StudentInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Task<AddUpdateServiceResponse<StudentDTO>> AddBatchAdmin(AddStudentDTO newStudent, int currentUser)
        {
            throw new NotImplementedException();
        }

        public Task<AddUpdateServiceResponse<StudentDTO>> AddStudent(AddStudentDTO newStudent, int currentUser)
        {
            throw new NotImplementedException();
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

        public Task<AddUpdateServiceResponse<StudentDTO>> UpdateBatchAdmin(UpdateStudentDTO updatedStudent, int currentUser)
        {
            throw new NotImplementedException();
        }

        public Task<AddUpdateServiceResponse<StudentDTO>> UpdateStudent(UpdateStudentDTO updatedStudent, int currentUser)
        {
            throw new NotImplementedException();
        }
    }
}
