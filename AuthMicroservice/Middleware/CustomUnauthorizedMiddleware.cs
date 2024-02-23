using AuthMicroservice.Dtos;

namespace AuthMicroservice.Middleware
{
    public class CustomUnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomUnauthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task CustomResponse(HttpContext context, string message, int status)
        {


            context.Response.ContentType = "application/json";
            var errorResponse = new GatewayCustomResponse
            {
                Status = status,
                Message = message
            };

            await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(errorResponse));

        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

           

            if (context.Response.StatusCode == 401)
            {
                await CustomResponse(context, "Unauthorized. Please provide valid credentials.", 401);

            }
            else if (context.Response.StatusCode == 502)
            {
                await CustomResponse(context, "Service Down temporarily.", 502);
            }
           
        }
    }
}
