using Contracts.Enums;
using Contracts.Requests.StudentRequests;
using Contracts.Responses;
using Contracts.Responses.AcademicResponses.StudentResponses;
using Contracts.Results;
using Domain.Entities.Students;
using Domain.Interfaces.StudentInterfaces.StatusInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.StudentServices
{
    public class StudentStatusService : IStatusService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IValidator<AddUpdateStudentStatusDTO> _validator;

        public StudentStatusService(IUnitOfWorkRepository unitOfWorkRepository, IValidator<AddUpdateStudentStatusDTO> validator)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _validator = validator;
        }

        public async Task<AddUpdateServiceResponse<StudentStatusDTO>> AddStatus(AddUpdateStudentStatusDTO newStatus, int currentUserId)
        {
            var validationResult = await _validator.ValidateAsync(newStatus);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<StudentStatusDTO>.Failure
                    (validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            if(await IsExistsByName(newStatus.Name))
            {
                return AddUpdateServiceResponse<StudentStatusDTO>.AlreadyExists<StudentStatus>();
            }

            var statusEntity = new StudentStatus
            {
                Name = newStatus.Name,
                Description = newStatus.Description,
            };
            _unitOfWorkRepository.StatusRepository.Add(statusEntity);
            await _unitOfWorkRepository.CompleteAsync();
            var statusDTO = await GetDTOById(statusEntity.StatusId);
            return AddUpdateServiceResponse<StudentStatusDTO>.Success(statusDTO!);
        }

        public async Task<StudentStatusDTO?> GetDTOById(int statusId)
        {
            return await _unitOfWorkRepository.StatusRepository.GetDTOById(statusId);
        }

        public async Task<StudentStatusDTO?> GetDTOByName(string Name)
        {
            return await _unitOfWorkRepository.StatusRepository.GetDTOByName(Name);
        }

        public async Task<StudentStatus?> GetEntityById(int statusId)
        {
            return await _unitOfWorkRepository.StatusRepository.GetEntityById(statusId);
        }

        public async Task<PagedResult<StudentStatusDTO>> GetStatuses(int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.StatusRepository.GetStatuses(pageNumber, pageSize);
        }

        public async Task<bool> IsExistsByName(string name)
        {
            return await _unitOfWorkRepository.StatusRepository.IsExistsByName(name);
        }

        public async Task<AddUpdateServiceResponse<StudentStatusDTO>> UpdateStatus(int statusId, AddUpdateStudentStatusDTO updatedStatus, int currentUserId)
        {
            var validationResult = await _validator.ValidateAsync(updatedStatus);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<StudentStatusDTO>.Failure
                  (validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var status = await GetEntityById(statusId);
            if(status == null)
            {
                return AddUpdateServiceResponse<StudentStatusDTO>.ResourceDoesntExist<StudentStatus>();
            }

            if(await IsExistsByName(updatedStatus.Name)&&status.Name != updatedStatus.Name)
            {
                return AddUpdateServiceResponse<StudentStatusDTO>.AlreadyExists<StudentStatus>();
            }

            status.Name = updatedStatus.Name;
            status.Description = updatedStatus.Description;
            await _unitOfWorkRepository.CompleteAsync();

            var statusDTO = await GetDTOById(status.StatusId);
            return AddUpdateServiceResponse<StudentStatusDTO>.Success(statusDTO!);
        }
    }
}
