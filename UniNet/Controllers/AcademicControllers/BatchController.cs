using Contracts.Requests.RequestParameters;
using Contracts.Responses.AcademicResponses.BatchResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.BatchInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;
using Contracts.Common.Messages;
using Contracts.Responses;
using Contracts.Requests.AcademicRequests.BatchRequests;

namespace UniNet.Controllers.AcademicControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly IBatchService _batchService;
        private readonly ICurrentUserService _currentUserService;
        public BatchController(IBatchService batchService, ICurrentUserService currentUserService)
        {
            _batchService = batchService;
            _currentUserService = currentUserService;
        }


        [Authorize]
        [HttpGet(Name ="GetAllBatches")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<BatchDTO>>> GetBatches([FromQuery]PagedResultParameters @pagedResultParameters)
        {
            var batches = await _batchService.GetAllBatches(pagedResultParameters.PageNumber, @pagedResultParameters.PageSize);
            return batches.ToPagedActioneResult();
        }

        [Authorize]
        [HttpGet("by-departmentId",Name = "GetBatchesPerDepartment")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<BatchDTO>>> GetBatchesPerDepartment
            ([FromQuery] PagedResultParameters @pagedResultParameters, [FromQuery]DepartmentIdParameter @departmentIdParameter)
        {
            var batches = await _batchService.GetBatchesPerDepartment(departmentIdParameter.DepartmentId,pagedResultParameters.PageNumber, @pagedResultParameters.PageSize);
            return batches.ToPagedActioneResult();
        }

        [Authorize]
        [HttpGet("by-id", Name = "GetBatchById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BatchDTO>> GetBatchById([FromQuery]BatchIdParameter @batchIdParameter)
        {
            var batch = await _batchService.GetDTOById(batchIdParameter.BatchId);
            return batch.GetResourceEndpoints(batchIdParameter.BatchId, typeof(Batch).Name);
        }

        [Authorize]
        [HttpGet("by-name", Name = "GetBatchByName")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BatchDTO>> GetBatchByName
            ([FromQuery]DepartmentIdParameter @departmentIdParameter,[FromQuery] BaseStringParametre @stringparameter)
        {
            var batch = await _batchService.GetDTOByName(departmentIdParameter.DepartmentId,stringparameter.Name);
            return batch.GetResourceEndpoints(stringparameter.Name, typeof(Batch).Name);
        }

        [Authorize]
        [HttpDelete(Name = "DeleteBatch")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete([FromQuery]BatchIdParameter @batchIdParameter)
        {
            var result = await _batchService.Delete(batchIdParameter.BatchId);
          return result.ToDeleteActionResult<Batch>(batchIdParameter.BatchId);
        }

        [Authorize]
        [HttpPost(Name = "AddBatch")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<BatchDTO>>> AddBatch([FromBody]AddBatchDTO newBatch)
        {
            var response = await _batchService.AddBatch(newBatch, _currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPut(Name = "UpdateBatch")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<BatchDTO>>> UpdateBatch
            ([FromQuery]BatchIdParameter @batchIdParameter, [FromBody]UpdateBatchDTO updatedBatch)
        {
            var response = await _batchService.UpdateBatch(batchIdParameter.BatchId,updatedBatch, _currentUserService.UserId);
            return response.ToActionResult();
        }
    }
}
