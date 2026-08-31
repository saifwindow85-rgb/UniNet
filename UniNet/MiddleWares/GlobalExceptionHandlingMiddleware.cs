using Contracts.Exceptions;

namespace UniNet.MiddleWares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DeleteRestrictedException ex)
            {
                // Warning لا Error: هذا قيد عمل متوقَّع (المستخدم حاول حذف مورد له تبعيات)، وليس عطلاً في الكود
                _logger.LogWarning(ex, "Delete restricted on {Method} {Path}", context.Request.Method, context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    message = ex.Message
                });
            }
            catch (ConstraintViolationException ex)
            {
                // قيد بيانات خولف في كتابة — خطأ طلب لا عطل خادم
                _logger.LogWarning(ex, "Constraint violation on {Method} {Path}", context.Request.Method, context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    status = 400,
                    title = "Constraint Violation",
                    message = ex.Message
                });
            }
            catch (DuplicateResourceException ex)
            {
                _logger.LogWarning(ex, "Duplicate resource on {Method} {Path}", context.Request.Method, context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    status = 409,
                    title = "Duplicate Resource",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                // التفاصيل الحقيقية تُسجَّل هنا فقط؛ رسالة العميل تبقى عامة كما هي — لا كشف لتفاصيل داخلية
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "An unexpected error occurred"
                });
            }
        }
    }
}
