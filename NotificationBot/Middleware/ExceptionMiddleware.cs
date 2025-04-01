
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace NotificationsBot.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                ProblemDetails problem = new()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Type = "Bad Request",
                    Title = "Bad Request",
                    Detail = "Something went wrong"
                };

                string json = JsonSerializer.Serialize(problem);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
