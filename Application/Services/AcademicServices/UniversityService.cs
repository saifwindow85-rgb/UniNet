using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Enums;
using Contracts.Requests.AcademicRequests.CommonAcademicRequests;
using Contracts.Requests.AcademicRequests.UniversityRequests;
using Contracts.Responses;
using Contracts.Responses.AcademicResponses.UniversityResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.UniversityInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AcademicServices
{
    public class UniversityService : IUniversityService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IValidator<AddUniversityDTO> _addValidator;
        private readonly IValidator<UpdateUniversityDTO> _updateValidator;

        public UniversityService
            (IUnitOfWorkRepository unitOfWorkRepository,IValidator<AddUniversityDTO>addValidator,IValidator<UpdateUniversityDTO> updateValidator)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
        }
        public async Task<AddUpdateServiceResponse<UniversityDTO>> AddUniversity(AddUniversityDTO newUniversity, int currentUserId)
        {
            var validationResult = await _addValidator.ValidateAsync(newUniversity);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<UniversityDTO>.Failure(validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList()
                    , EnErrorTypes.InvalidData);
            }

            if(await IsUniversityExists(newUniversity.UniversityName))
            {
                return AddUpdateServiceResponse<UniversityDTO>.AlreadyExists<University>();
            }

            var universityEntity = new University
            {
                Name = newUniversity.UniversityName,
                Description = newUniversity.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId,
            };
            _unitOfWorkRepository.UniversityRepository.Add(universityEntity);
            await _unitOfWorkRepository.CompleteAsync();
            var universityDTO = await FindUniversityDTOById(universityEntity.UniversityId);
            return AddUpdateServiceResponse<UniversityDTO>.Success(universityDTO!);
        }

        public async Task<bool> Delete(int universityId)
        {
            var result = await _unitOfWorkRepository.UniversityRepository.Delete(universityId);
            if (result)
                await _unitOfWorkRepository.CompleteAsync();

            return result;
        }

        public async Task<UniversityDTO?> FindUniversityDTOById(int UniversityId)
        {
            return await _unitOfWorkRepository.UniversityRepository.GetUniversityDTOById(UniversityId);
        }

        public async Task<University?> FindUniversityEntityById(int universityId)
        {
            return await _unitOfWorkRepository.UniversityRepository.GetUniversityEntityById(universityId);
        }

        public async Task<PagedResult<UniversityDTO>> GetAllUniversities(AcademicFilter?filter,int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.UniversityRepository.GetAllUniversities(filter,pageNumber, pageSize);
        }

        public Task<UniversityAuthorizationInfo?> GetUniversityAuthorizationInfoAsync(int universityId)
        {
            return _unitOfWorkRepository.UniversityRepository.GetUniversityAuthorizationInfoAsync(universityId);
        }

        public async Task<bool> IsUniversityExists(int universityId)
        {
            return await _unitOfWorkRepository.UniversityRepository.IsUniversityExists(universityId);
        }

        public async Task<bool> IsUniversityExists(string universityName)
        {
            return await _unitOfWorkRepository.UniversityRepository.IsUniversityExists(universityName);
        }

        public async Task<AddUpdateServiceResponse<UniversityDTO>> UpdateUniversity(int updatedUinversityId, UpdateUniversityDTO updatedUniversity, int currentUserId)
        {
            var validationResult = await _updateValidator.ValidateAsync(updatedUniversity);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<UniversityDTO>.Failure(validationResult.Errors.Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList()
                    , EnErrorTypes.InvalidData);
            }

            var university = await FindUniversityEntityById(updatedUinversityId);
            if(university == null)
            {
                return AddUpdateServiceResponse<UniversityDTO>.ResourceDoesntExist<University>();
            }

            if(await IsUniversityExists(updatedUniversity.UniversityName) && university.Name != updatedUniversity.UniversityName)
            {
                return AddUpdateServiceResponse<UniversityDTO>.AlreadyExists<University>();
            }
            university.Name = updatedUniversity.UniversityName;
            university.Description = updatedUniversity.Description;
            university.UpdatedAt = DateTime.UtcNow;
            university.UpdatedByUserId = currentUserId;
            await _unitOfWorkRepository.CompleteAsync();
            var uinversityDTO = await FindUniversityDTOById(university.UniversityId);
            return AddUpdateServiceResponse<UniversityDTO>.Success(uinversityDTO!);
        }
    }
}
