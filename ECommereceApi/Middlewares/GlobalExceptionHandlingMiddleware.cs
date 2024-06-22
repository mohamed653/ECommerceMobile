using ECommereceApi.Middlewares.Domains;
using Serilog;
using System.Net;

namespace ECommereceApi.Middlewares
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {

        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);

            }
            catch (Exception ex)
            {
                

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var errorDetails = new ErrorDetails()
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Internal Server Error."
                }.ToString();
                await context.Response.WriteAsync(errorDetails);

                // Logging the json response
                _logger.LogError(ex, ex.Message);
                Log.Error(errorDetails);

            }
        }
    }
}
