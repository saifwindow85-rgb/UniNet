using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses
{
    public class AddUpdateServiceResponse <T>
    {
        public T? Data { get; private set; }
        public bool IsSuccess { get; private set; }
        public List<string>? Errors { get; private set; }
        public EnErrorTypes? ErrorType { get; private set; }
        public string ErrorMessage { get; private set; }

        public static AddUpdateServiceResponse<T> Success(T data) => new AddUpdateServiceResponse<T>
        {
            Data = data,
            IsSuccess = true
        };

        public static AddUpdateServiceResponse<T> Failure(List<string> errors, EnErrorTypes errorType) => new AddUpdateServiceResponse<T>
        {
            Errors = errors,
            ErrorType = errorType,
            IsSuccess = false
        };

        public static AddUpdateServiceResponse<T> InvalidRelatedData() => new AddUpdateServiceResponse<T>
        {
            IsSuccess = false,
            ErrorType = EnErrorTypes.InvalidData,
            Errors = new List<string>() { "The operation failed due to invalid related data. One or more referenced records do not exist" }

        };

        public static AddUpdateServiceResponse<T> ExistedResource<TEntity>() => new AddUpdateServiceResponse<T>
        {
            IsSuccess = false,
            ErrorType = EnErrorTypes.ExistedResource,
            Errors = new List<string>() { $"This {typeof(TEntity).Name} Already Exist!" }
        };
        public static AddUpdateServiceResponse<T> InValidUserId(EnErrorTypes errorType) => new AddUpdateServiceResponse<T>
        {
            ErrorMessage = "Unauthorized",
            ErrorType = errorType
        };
    }
}
