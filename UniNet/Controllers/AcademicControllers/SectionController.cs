using Contracts.Requests.AcademicRequests.SectionRequests;
using Contracts.Requests.RequestParameters;
using Contracts.Responses;
using Contracts.Responses.AcademicResponses.SectionResponses;
using Contracts.Results;
using Domain.Entities.Academic_Structure;
using Domain.Interfaces.AcademicStructureInterfaces.SectionInterfaces;
using Domain.Interfaces.IdentityInterfaces.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniNet.Extensions;

namespace UniNet.Controllers.AcademicControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly ISectionService _sectionService;
        private readonly ICurrentUserService _currentUserService;

        public SectionController(ISectionService sectionService, ICurrentUserService currentUserService)
        {
            _sectionService = sectionService;
            _currentUserService = currentUserService;
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpGet(Name = "GetAllSections")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<SectionDTO>>> GetAllSections([FromQuery] PagedResultParameters pagedResultParameters)
        {
            var sections = await _sectionService.GetAllSections(pagedResultParameters.PageNumber, pagedResultParameters.PageSize);
            return sections.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpGet("batchId", Name = "GetSectionsPerBatch")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<SectionDTO>>> GetSectionsPerBatch([FromQuery] BatchIdParameter batchIdParameter, [FromQuery] PagedResultParameters pagedResultParameters)
        {
            var sections = await _sectionService.GetSectionsPerBatches(batchIdParameter.BatchId, pagedResultParameters.PageNumber, pagedResultParameters.PageSize);
            return sections.ToPagedActioneResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpGet("by-id", Name = "GetSectionById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SectionDTO>> GetSectionById([FromQuery] SectionIdParameter sectionIdParameter)
        {
            var section = await _sectionService.GetDTOById(sectionIdParameter.SectionId);
            return section.GetResourceEndpoints(sectionIdParameter.SectionId, typeof(Section).Name);
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpGet("by-name", Name = "GetSectionByName")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SectionDTO>> GetSectionByName([FromQuery] BatchIdParameter batchIdParameter, [FromQuery] BaseStringParametre sectionNameParameter)
        {
            var section = await _sectionService.GetDTOByName(batchIdParameter.BatchId, sectionNameParameter.Name);
            return section.GetResourceEndpoints(sectionNameParameter.Name, typeof(Section).Name);
        }


        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpDelete(Name = "DeleteSection")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public async Task<ActionResult> DeleteSection([FromQuery] SectionIdParameter sectionIdParameter)
        {
            var result = await _sectionService.Delete(sectionIdParameter.SectionId);
            return result.ToDeleteActionResult<Section>(sectionIdParameter.SectionId);
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpPost(Name = "CreateSection")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<SectionDTO>>> CreateSection([FromBody]AddSectionDTO newSection)
        {
            var response = await _sectionService.AddSection(newSection,_currentUserService.UserId);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Super Admin,UniversityAdmin,CollegeAdmin,DepartmentAdmin")]
        [HttpPut(Name ="UpdateSection")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AddUpdateServiceResponse<SectionDTO>>> UpdateSection
            ([FromQuery]SectionIdParameter sectionIdParameter,[FromBody] UpdateSectionDTO updatedSection)
        {
            var response = await _sectionService.UpdateSection(sectionIdParameter.SectionId, updatedSection, _currentUserService.UserId);
            return response.ToActionResult();
        }
    }
}
