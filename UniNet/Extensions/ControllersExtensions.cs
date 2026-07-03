using Contracts.Enums;
using Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace UniNet.Extensions
{
    public static  class ControllersExtensions
    {
        public static ActionResult<AddUpdateServiceResponse<T>> ToActionResult<T>(this AddUpdateServiceResponse<T> response)
        {
            if (response.IsSuccess)
            {
                return response.Data != null ? new OkObjectResult(response.Data) : new NoContentResult();
            }

            return response.ErrorType switch
            {
                EnErrorTypes.NotFound => new NotFoundObjectResult(new
                {
                    Status = 404,
                    Title = "Not Found",
                    Detail = response.Errors.FirstOrDefault() ?? "The requested resource was not found."
                }),

                EnErrorTypes.InvalidData => new BadRequestObjectResult(new
                {
                    Status = 400,
                    Title = "Validation Error",
                    Errors = response.Errors
                }),

                EnErrorTypes.InvalidRefrenceData => new BadRequestObjectResult(new
                {
                    Status = 400,
                    Title = "Invalid Related Entity",
                    Errors = response.Errors
                }),

                EnErrorTypes.InvalidAuthenticatedUserId => new UnauthorizedObjectResult(new
                {
                    StatusCode = 401,
                    Message = response.ErrorMessage
                }),

                EnErrorTypes.ExistedResource => new BadRequestObjectResult(new
                {
                    StatusCode = 400,
                    Errors = response.Errors
                }),

                _ => new ObjectResult(new { Status = 500, Title = "Server Error" }) { StatusCode = 500 }
            };
        }


        //public static ActionResult<PagedResult<T>> ToPagedActioneResult<T>(this PagedResult<T> pagedResult)
        //{
        //    return pagedResult.Data != null ? new ObjectResult(pagedResult) : new NotFoundObjectResult(new { Status = 404, Title = "No Result Found ?" });
        //}
    }
}
