using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Common.Extensions;
using Contracts.Enums;
using Contracts.Requests.AcademicRequests.SectionRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.AcademicResponses.SectionResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.SectionInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AcademicServices
{
    public class SectionService : ISectionService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IValidator<AddSectionDTO> _addValidator;
        private readonly IValidator<UpdateSectionDTO> _updateValidator;
        public SectionService(IUnitOfWorkRepository unitOfWorkRepository,
            IValidator<AddSectionDTO>addValidator,IValidator<UpdateSectionDTO>updateValidator)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
        }
        public async Task<AddUpdateServiceResponse<SectionDTO>> AddSection(UserScope?scope,AddSectionDTO newSection, int currentUserId)
        {
            var validationResult = await _addValidator.ValidateAsync(newSection);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<SectionDTO>.Failure(validationResult.
                    Errors.Select(e => e.ErrorMessage).ToList(), EnErrorTypes.InvalidData);
            }

            var batchInfo = await _unitOfWorkRepository.BatchRepository.GetBatchAuthorizationInfoAsync(newSection.BatchId);
            if(batchInfo == null)
            {
                return AddUpdateServiceResponse<SectionDTO>.ResourceDoesntExist<Batch>();

            }

            if(!scope.IsWithinScope(batchInfo.UniverseId,batchInfo.CollegeId,batchInfo.DepartmentId))
            {
                return AddUpdateServiceResponse<SectionDTO>.ResourceDoesntExist<Batch>();
            }

            if (await ExistsByName(newSection.BatchId,newSection.SectionName))
            {
                return AddUpdateServiceResponse<SectionDTO>.AlreadyExists<Section>();
            }

            var sectionEntity = new Section
            {
                BatchId = newSection.BatchId,
                Name = newSection.SectionName,
                CreatedByUserId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };
            _unitOfWorkRepository.SectionRepository.Add(sectionEntity);
            await _unitOfWorkRepository.CompleteAsync();
            var sectionDTO = await GetDTOById(sectionEntity.SectionId);
            return AddUpdateServiceResponse<SectionDTO>.Success(sectionDTO!);
        }

        public async Task<bool> Delete(int sectionId)
        {
            var result  = await _unitOfWorkRepository.SectionRepository.Delete(sectionId);
            if (result)
              await  _unitOfWorkRepository.CompleteAsync();

            return result;
        }

        public Task<bool> ExistsById(int sectionId)
        {
            return _unitOfWorkRepository.SectionRepository.ExistsById(sectionId);
        }

        public Task<bool> ExistsByName(int batchId, string name)
        {
            return _unitOfWorkRepository.SectionRepository.ExistsByName(batchId, name);
        }

        public Task<PagedResult<SectionDTO>> GetAllSections(int pageNumber, int pageSize)
        {
            return _unitOfWorkRepository.SectionRepository.GetAllSections(pageNumber, pageSize);
        }

        public Task<SectionDTO?> GetDTOById(int sectionId)
        {
            return _unitOfWorkRepository.SectionRepository.GetDTOById(sectionId);
        }

        public Task<SectionDTO?> GetDTOByName(int batchId, string name)
        {
            return _unitOfWorkRepository.SectionRepository.GetDTOByName(batchId, name);
        }

        public Task<Section?> GetEntityById(int sectionId)
        {
            return _unitOfWorkRepository.SectionRepository.GetEntityById(sectionId);
        }

        public Task<Section?> GetEntityByName(int batchId, string name)
        {
            return _unitOfWorkRepository.SectionRepository.GetEntityByName(batchId, name);
        }

        public async Task<SectionAuthorizationInfo?> GetSectionAuthorizationInfoAsync(int sectionId)
        {
            return await _unitOfWorkRepository.SectionRepository.GetSectionAuthorizationInfoAsync(sectionId);
        }

        public Task<PagedResult<SectionDTO>> GetSectionsPerBatches(int batchId, int pageNumber, int pageSize)
        {
            return _unitOfWorkRepository.SectionRepository.GetSectionsPerBatch(batchId, pageNumber, pageSize);
        }

        public async Task<AddUpdateServiceResponse<SectionDTO>> UpdateSection(UserScope?scope,int SectionId, UpdateSectionDTO updatedSection, int currentUserId)
        {
            var validationResult = await _updateValidator.ValidateAsync(updatedSection);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<SectionDTO>.Failure(validationResult.
                    Errors.Select(e => e.ErrorMessage).ToList(), EnErrorTypes.InvalidData);
            }

            var sectionInfo = await GetSectionAuthorizationInfoAsync(SectionId);
            if(sectionInfo == null)
            {
                return AddUpdateServiceResponse<SectionDTO>.ResourceDoesntExist<Section>();
            }

            if(!scope.IsWithinScope(sectionInfo.UniversityId,sectionInfo.CollegeId,sectionInfo.DepartmentId,sectionInfo.BatchId))
            {
                return AddUpdateServiceResponse<SectionDTO>.ResourceDoesntExist<Section>();
            }


            var  section = await GetEntityById(SectionId);


            if(await ExistsByName(section!.BatchId, updatedSection.SectionName)&& section.Name != updatedSection.SectionName)
            {
                return AddUpdateServiceResponse<SectionDTO>.AlreadyExists<Section>();
            }

            section.Name = updatedSection.SectionName;
            section.UpdatedAt = DateTime.UtcNow;
            section.UpdatedByUserId = currentUserId;
            await _unitOfWorkRepository.CompleteAsync();
            var sectionDTO = await GetDTOById(section.SectionId);
            return AddUpdateServiceResponse<SectionDTO>.Success(sectionDTO!);
        }
    }
}
