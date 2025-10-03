using System;
using CrudAPI.Entities;

namespace CrudAPI.DTOs
{
    public class TareaDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public EstadoTarea Estado { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
    }

    public class TareaCreacionDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int UsuarioId { get; set; }
    }

    public class TareaActualizacionDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaVencimiento { get; set; }
    }

    public class TareaEstadoDTO
    {
        public EstadoTarea Estado { get; set; }
    }

    public class TareaAsignacionDTO
    {
        public int UsuarioId { get; set; }
    }
}
