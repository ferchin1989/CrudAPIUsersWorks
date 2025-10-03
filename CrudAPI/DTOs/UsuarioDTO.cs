using System.Collections.Generic;

namespace CrudAPI.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UsuarioCreacionDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UsuarioEstadisticasDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int TotalTareas { get; set; }
        public int TareasCompletadas { get; set; }
        public double PorcentajeCompletado { get; set; }
    }

    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        public int RegistrosPorPagina { get; set; } = 10;
        public string? OrdenarPor { get; set; }
        public string? Buscar { get; set; }
    }

    public class ResultadoPaginadoDTO<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalPaginas => (TotalRegistros + RegistrosPorPagina - 1) / RegistrosPorPagina;
    }
}
