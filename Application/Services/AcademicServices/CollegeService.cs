using Contracts.Enums;
using Contracts.Requests.AcademicRequests.CollegeRequests;
using Contracts.Responses;
using Contracts.Responses.CollegeResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AcademicServices
{
    public class CollegeService : ICollegeService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IValidator<AddCollegeDTO> _addValidator;
        private readonly IValidator<UpdateCollegeDTO> _updateCollegeValidator;
        public CollegeService(IUnitOfWorkRepository unitOfWorkRepository
            , IValidator<AddCollegeDTO>addValidator,IValidator<UpdateCollegeDTO>updateValidator)
        {
           _unitOfWorkRepository = unitOfWorkRepository;
            _addValidator = addValidator;
            _updateCollegeValidator = updateValidator;
        }
        public async Task<AddUpdateServiceResponse<CollegeDTO>> AddCollege(AddCollegeDTO newCollege, int currentUserId)
        {
            var validationResult = await _addValidator.ValidateAsync(newCollege);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<CollegeDTO>.Failure(validationResult.Errors.
                    Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            if(await IsCollegeExists(newCollege.UniversityId,newCollege.CollegeName))
            {
                return AddUpdateServiceResponse<CollegeDTO>.AlreadyExists<College>();
            }

            var collegeEntity = new College
            {
                Name = newCollege.CollegeName,
                Description = newCollege.Description,
                UniversityId = newCollege.UniversityId, // Just for now user can enter universityId until authentication get Completed
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId,

            };
            _unitOfWorkRepository.CollegeRepository.Add(collegeEntity);
            await _unitOfWorkRepository.CompleteAsync();
            var collegeDTO = await GetCollegeDTOById(collegeEntity.CollegeId);
            return AddUpdateServiceResponse<CollegeDTO>.Success(collegeDTO!);
        }

        public async Task<bool> Delete(int collegeId)
        {
            var result = await _unitOfWorkRepository.CollegeRepository.Delete(collegeId);
            if (result)
                await _unitOfWorkRepository.CompleteAsync();
            return result;
        }

        public Task<CollegeDTO?> GetCollegeDTOById(int collegeId)
        {
            throw new NotImplementedException();
        }

        public Task<CollegeDTO?> GetCollegeDTOByName(string collegeName)
        {
            throw new NotImplementedException();
        }

        public Task<College?> GetCollegeEntityById(int collegeId)
        {
            throw new NotImplementedException();
        }

        public Task<College?> GetCollegeEntityByName(string collegeName)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<CollegeDTO>> GetColleges(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<CollegeDTO>> GetCollegesPerUniversity(int universityId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCollegeExists(int universityId, int collegeId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCollegeExists(int universityId, string collegeName)
        {
            throw new NotImplementedException();
        }

        public Task<AddUpdateServiceResponse<CollegeDTO>> UpdateCollege(int collegeId, UpdateCollegeDTO updatedCollege, int currentUserId)
        {
            throw new NotImplementedException();
        }
    }
}
