using Contracts.Common.Messages;
using Contracts.Enums;
using Contracts.Responses;
using Contracts.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
                    Detail = response.Errors!.FirstOrDefault() ?? "The requested resource was not found."
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
                    StatusCode = 409,
                    Errors = response.Errors
                }),

                _ => new ObjectResult(new { Status = 500, Title = "Server Error" }) { StatusCode = 500 }
            };
        }


        public static ActionResult<PagedResult<T>> ToPagedActioneResult<T>(this PagedResult<T> pagedResult)
        {
            return pagedResult.Data != null ? new OkObjectResult(pagedResult) : new NotFoundObjectResult(new { Status = 404, Title = "No Result Found ?" });
        }

        public static ActionResult<T>GetResourceEndpoints<T>(this T? resource,int Id,string EntityName)
        {
            return resource != null ? new OkObjectResult(resource) : new NotFoundObjectResult(ErrorMessages.NotFound(EntityName, Id));
        }
        public static ActionResult<T> GetResourceEndpoints<T>(this T? resource, string name, string EntityName)
        {
            return resource != null ? new OkObjectResult(resource) : new NotFoundObjectResult(ErrorMessages.NotFound(EntityName, name));
        }

        public static ActionResult AuthorizationInfoNotFound()
        {
            return new NotFoundObjectResult("No Result Found!");
        }
        public static ActionResult ToDeleteActionResult<TEntity>( this bool result,int Id)
        {
            if (!result)
                return new NotFoundObjectResult($"Deletaion faield/{typeof(TEntity).Name} with Id = {Id} NotFound!");

            return new NoContentResult();
        }
    }
}
