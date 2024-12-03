using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApplication1.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); // Pass the request to the next middleware
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex); // Handle exception if one occurs
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500 for generic error

            var response = new { message = "An error occurred while processing your request.", details = exception.Message };

            return context.Response.WriteAsJsonAsync(response); // Return error details in response body
        }
    }
}
