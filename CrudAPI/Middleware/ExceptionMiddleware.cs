using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using CrudAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CrudAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no manejado: {Message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new ApiResponse
            {
                Success = false,
                Message = "Se produjo un error al procesar la solicitud"
            };

            switch (exception)
            {
                case ApplicationException appEx:
                    // Errores de aplicación (validaciones, reglas de negocio)
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = appEx.Message;
                    response.Errors.Add(appEx.Message);
                    break;
                
                case KeyNotFoundException notFoundEx:
                    // Recurso no encontrado
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = notFoundEx.Message;
                    response.Errors.Add(notFoundEx.Message);
                    break;
                
                case UnauthorizedAccessException unauthorizedEx:
                    // Acceso no autorizado
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = unauthorizedEx.Message;
                    response.Errors.Add(unauthorizedEx.Message);
                    break;
                
                default:
                    // Error interno del servidor
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Errors.Add(exception.Message);
                    
                    // En producción, no exponer detalles de errores internos
                    #if DEBUG
                    response.Errors.Add(exception.StackTrace ?? string.Empty);
                    #endif
                    break;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);
            
            await context.Response.WriteAsync(json);
        }
    }
}
