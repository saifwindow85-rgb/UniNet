using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Common.Extensions;
using Contracts.Enums;
using Contracts.Requests.AcademicRequests.BatchRequests;
using Contracts.Requests.AcademicRequests.CommonAcademicRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.AcademicResponses.BatchResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.BatchInterfaces;
using Domain.Interfaces.UnitOfWork;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AcademicServices
{
    public class BatchService : IBatchService
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IValidator<AddBatchDTO> _addValidator;
        private readonly IValidator<UpdateBatchDTO> _updateValidator;
        public BatchService(IUnitOfWorkRepository unitOfWorkRepository,
            IValidator<AddBatchDTO> addValidator,IValidator<UpdateBatchDTO> updateValidator)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
        }
        public async Task<AddUpdateServiceResponse<BatchDTO>> AddBatch(UserScope?scope,AddBatchDTO newBatch, int currentUserId)
        {
            var validationResult = await _addValidator.ValidateAsync(newBatch);
            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<BatchDTO>.Failure(validationResult.Errors
                    .Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var departmentAuthorizationInfo = await _unitOfWorkRepository.DepartmentRepository.GetDepartmentAuthorizationInfoAsync(newBatch.DepartmentId);
            if(departmentAuthorizationInfo == null)
            {
                return AddUpdateServiceResponse<BatchDTO>.ResourceDoesntExist<Department>();
            }

            if(!scope.IsWithinScope(departmentAuthorizationInfo.UniversityId,departmentAuthorizationInfo.CollegeId,departmentAuthorizationInfo.DepartmentId,null))
            {
                return AddUpdateServiceResponse<BatchDTO>.ResourceDoesntExist<Department>();
            }

            if(await ExistsByName(newBatch.DepartmentId,newBatch.BatchName))
            {
                return AddUpdateServiceResponse<BatchDTO>.AlreadyExists<Batch>();
            }

            var batchEntity = new Batch
            {
                DepartmentId = newBatch.DepartmentId,
                Name = newBatch.BatchName,
                BatchYear = newBatch.BatchYear,
                Description = newBatch.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = currentUserId
            };
            _unitOfWorkRepository.BatchRepository.Add(batchEntity);
            await _unitOfWorkRepository.CompleteAsync();
            var batchDTO = await GetDTOById(batchEntity.BatchId);
            return AddUpdateServiceResponse<BatchDTO>.Success(batchDTO!);
        }

        public async Task<bool> Delete(int batchId)
        {
            var result = await _unitOfWorkRepository.BatchRepository.Delete(batchId);
            if (result)
                await _unitOfWorkRepository.CompleteAsync();

            return result;
        }

        public async Task<bool> ExistsById(int batchId)
        {
            return await _unitOfWorkRepository.BatchRepository.ExistsById(batchId);
        }

        public async Task<bool> ExistsByName(int departmentId, string name)
        {
            return await _unitOfWorkRepository.BatchRepository.ExistsByName(departmentId, name); 
        }

        public async Task<PagedResult<BatchDTO>> GetAllBatches(AcademicFilter?filter,int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.BatchRepository.GetAllBatches(filter,pageNumber, pageSize);
        }

        public async Task<BatchAuthorizationInfo?> GetBatchAuthorizationInfoAsync(int batchId)
        {
            return await _unitOfWorkRepository.BatchRepository.GetBatchAuthorizationInfoAsync(batchId);
        }

        public async Task<PagedResult<BatchDTO>> GetBatchesPerDepartment(UserScope?scope,AcademicFilter?filter, int pageNumber, int pageSize)
        {
            return await _unitOfWorkRepository.BatchRepository.GetBatchesPerDepartment(scope,filter, pageNumber, pageSize);
        }

        public async Task<BatchDTO?> GetDTOById(int batchId)
        {
            return await _unitOfWorkRepository.BatchRepository.GetDTOById(batchId);
        }

 
        public async Task<Batch?> GetEntityById(int batchId)
        {
            return await _unitOfWorkRepository.BatchRepository.GetEntityById(batchId);
        }


        public async Task<AddUpdateServiceResponse<BatchDTO>> UpdateBatch(UserScope?scope,int batchId, UpdateBatchDTO updatedBatch, int currentUserId)
        {
            var validationResult = await _updateValidator.ValidateAsync(updatedBatch);

            if(!validationResult.IsValid)
            {
                return AddUpdateServiceResponse<BatchDTO>.Failure(validationResult.Errors
                    .Select(x => $"{x.PropertyName} : {x.ErrorMessage}").ToList(), EnErrorTypes.InvalidData);
            }

            var batchInfo = await GetBatchAuthorizationInfoAsync(batchId);
            if(batchInfo == null)
            {
                return AddUpdateServiceResponse<BatchDTO>.ResourceDoesntExist<Batch>();
            }

            if(!scope.IsWithinScope(batchInfo.UniverseId,batchInfo.CollegeId,batchInfo.DepartmentId,batchInfo.BatchId))
            {
                return AddUpdateServiceResponse<BatchDTO>.ResourceDoesntExist<Batch>();
            }

            var batch = await GetEntityById(batchId);
            if(await ExistsByName(batch!.DepartmentId,updatedBatch.BatchName)&&batch.Name  != updatedBatch.BatchName)
            {
                return AddUpdateServiceResponse<BatchDTO>.AlreadyExists<Batch>();
            }


            batch.Name = updatedBatch.BatchName;
            batch.BatchYear = updatedBatch.BatchYear;
            batch.Description = updatedBatch.Description;
            batch.UpdatedAt = DateTime.UtcNow;
            batch.UpdatedByUserId = currentUserId;

            await _unitOfWorkRepository.CompleteAsync();
            var batchDTO = await GetDTOById(batch.BatchId);
            return AddUpdateServiceResponse<BatchDTO>.Success (batchDTO!);
        }
    }
}
