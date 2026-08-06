using Contracts.Enums;
using Contracts.Requests.AcademicRequests.CollegeRequests;
using Contracts.Responses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.CollegeInterfaces;
using Domain.Interfaces.UnitOfWork;
using Contracts.Common.AuthorizationInfos.AcademicInfos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Responses.AcademicResponses.CollegeResponses;
using Contracts.Requests.RequestParameters;
using Contracts.Common.Extensions;

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
        public async Task<AddUpdateServiceResponse<CollegeDTO>> AddCollege(UserScope?scope,AddCollegeDTO newCollege, int currentUserId)
        {
            var validationResult = await _addValidator.ValidateAsync(newCollege);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<CollegeDTO>.Failure(validationResult.Errors.
                    Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var UniversityInfo = await _unitOfWorkRepository.UniversityRepository.GetUniversityAuthorizationInfoAsync(newCollege.UniversityId);
            if(UniversityInfo == null)
            {
                return AddUpdateServiceResponse<CollegeDTO>.ResourceDoesntExist<University>();
            }

            if(!scope.IsWithinScope(UniversityInfo.UniversityId))
            {
                return AddUpdateServiceResponse<CollegeDTO>.ResourceDoesntExist<University>();
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

        public async Task<CollegeAuthorizationInfo?> GetCollegeAuthorizationInfo(int collegeId)
        {
            return await  _unitOfWorkRepository.CollegeRepository.GetCollegeAuthorizationInfo(collegeId);
        }

     

        public async Task<CollegeDTO?> GetCollegeDTOById(int collegeId)
        {
            return await _unitOfWorkRepository.CollegeRepository.GetCollegeDTOById(collegeId);
        }

        public async Task<College?> GetCollegeEntityById(int collegeId)
        {
            return await _unitOfWorkRepository.CollegeRepository.GetCollegeEntityById(collegeId);
        }

        public async Task<PagedResult<CollegeDTO>> GetColleges(CollegeFilter?filter,int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.CollegeRepository.GetAllColleges(filter,pageNumber, pageSize);
        }

        public async Task<PagedResult<CollegeDTO>> GetCollegesPerUniversity(UserScope?scope,CollegeFilter?filter, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.CollegeRepository.GetAllCollegesPerUniversity(scope,filter, pageNumber, pageSize);
        }

        public async Task<bool> IsCollegeExists( int collegeId)
        {
            return await _unitOfWorkRepository.CollegeRepository.IsCollegeExists( collegeId);
        }

        public async Task<bool> IsCollegeExists(int universityId, string collegeName)
        {
            return await _unitOfWorkRepository.CollegeRepository.IsCollegeExists(universityId, collegeName);
        }

        public async Task<AddUpdateServiceResponse<CollegeDTO>> UpdateCollege(UserScope?scope,int collegeId, UpdateCollegeDTO updatedCollege, int currentUserId)
        {
            var validationResult = await _updateCollegeValidator.ValidateAsync(updatedCollege);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<CollegeDTO>.Failure(validationResult.Errors.
                                   Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var collegeInfo = await GetCollegeAuthorizationInfo(collegeId);
            if(collegeInfo == null)
            {
                return AddUpdateServiceResponse<CollegeDTO>.ResourceDoesntExist<College>();

            }

            if(!scope.IsWithinScope(collegeInfo.UniversityId,collegeInfo.CollegeId))
            {
                return AddUpdateServiceResponse<CollegeDTO>.ResourceDoesntExist<College>();

            }
            var college = await GetCollegeEntityById(collegeId);
            if(college == null)
            {
                return AddUpdateServiceResponse<CollegeDTO>.ResourceDoesntExist<College>();
            }

            if(await IsCollegeExists(college.UniversityId,updatedCollege.CollegeName) && college.Name != updatedCollege.CollegeName)
            {
                return AddUpdateServiceResponse<CollegeDTO>.AlreadyExists<College>();
            }

            college.Name = updatedCollege.CollegeName;
            college.Description = updatedCollege.Description;
            college.UpdatedAt = DateTime.UtcNow;
            college.UpdatedByUserId = currentUserId;
            await _unitOfWorkRepository.CompleteAsync();
            var collegeDTO = await GetCollegeDTOById(college.CollegeId);
            return AddUpdateServiceResponse<CollegeDTO>.Success(collegeDTO!);
        }
    }
}
