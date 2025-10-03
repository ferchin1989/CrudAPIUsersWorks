using System;

namespace CrudAPI.Entities
{
    public enum EstadoTarea
    {
        Pendiente = 0,
        EnProgreso = 1,
        Completada = 2
    }

    public class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public EstadoTarea Estado { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
