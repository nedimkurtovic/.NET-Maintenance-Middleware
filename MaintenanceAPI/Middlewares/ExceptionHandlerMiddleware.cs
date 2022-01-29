using MaintenanceAPI.Exceptions;
using MaintenanceAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MaintenanceAPI.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var clientMessage = string.Empty;
            HttpStatusCode code;

            switch (ex)
            {
                case MaintenanceException:
                    code = HttpStatusCode.ServiceUnavailable;
                    clientMessage = JsonSerializer.Serialize(new CustomError{ Message = ex.Message, StatusCode = (int) code });
                    break;
                case RequestValidationException:
                    code = HttpStatusCode.BadRequest;
                    clientMessage = JsonSerializer.Serialize(new CustomError { Message = ex.Message, StatusCode = (int)code });
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    clientMessage = JsonSerializer.Serialize(new CustomError { Message = "An error has occured!", StatusCode = (int)code });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            
            await context.Response.WriteAsync(clientMessage);
        }
    }
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
