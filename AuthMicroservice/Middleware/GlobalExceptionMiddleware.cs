using AuthMicroservice.Dtos;
using AuthMicroservice.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AuthMicroservice.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {

            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {

                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            context.Response.ContentType = "application/json";
            

            if (exception is BadRequestException)
            {
                context.Response.StatusCode = 401;
            }
            else if(exception is NotFoundException)
            {
                context.Response.StatusCode = 404;
            }
            else 
            {
                context.Response.StatusCode = 500;
            }


            var errorResponse = new
            GatewayCustomResponse()
            {
                Status = context.Response.StatusCode,
                Message = exception.Message,
                
            };
            await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(errorResponse)); ;
        }
    }
}
