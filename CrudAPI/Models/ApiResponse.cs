using System;
using System.Collections.Generic;

namespace CrudAPI.Models
{
    /// <summary>
    /// Clase base para todas las respuestas de la API
    /// </summary>
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public ApiResponse()
        {
            Success = true;
            Message = string.Empty;
            Errors = new List<string>();
        }

        public ApiResponse(string message)
        {
            Success = true;
            Message = message;
            Errors = new List<string>();
        }

        public ApiResponse(bool success, string message)
        {
            Success = success;
            Message = message;
            Errors = new List<string>();
        }
    }

    /// <summary>
    /// Respuesta genérica que incluye datos
    /// </summary>
    /// <typeparam name="T">Tipo de datos que se devuelven</typeparam>
    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        public ApiResponse() : base()
        {
            Data = default;
        }

        public ApiResponse(T data) : base()
        {
            Success = true;
            Message = "Operación exitosa";
            Data = data;
        }

        public ApiResponse(T data, string message) : base(message)
        {
            Data = data;
        }

        public ApiResponse(bool success, string message) : base(success, message)
        {
            Data = default;
        }
    }
}
