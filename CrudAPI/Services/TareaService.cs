using Microsoft.EntityFrameworkCore;
using CrudAPI.Context;
using CrudAPI.DTOs;
using CrudAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudAPI.Services
{
    public class TareaService
    {
        private readonly AppDbContext _context;

        public TareaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TareaDTO>> ListarTareas()
        {
            var listaDTO = new List<TareaDTO>();
            var listaDB = await _context.Tareas.Include(t => t.Usuario).ToListAsync();
            
            foreach (var item in listaDB)
            {
                listaDTO.Add(new TareaDTO
                {
                    Id = item.Id,
                    Titulo = item.Titulo,
                    Descripcion = item.Descripcion,
                    FechaCreacion = item.FechaCreacion,
                    FechaVencimiento = item.FechaVencimiento,
                    Estado = item.Estado,
                    UsuarioId = item.UsuarioId,
                    NombreUsuario = item.Usuario?.Nombre ?? "Sin asignar"
                });
            }
            
            return listaDTO;
        }

        public async Task<TareaDTO> ObtenerTareaPorId(int id)
        {
            var tareaDB = await _context.Tareas
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Id == id);
                
            if (tareaDB == null)
                throw new ApplicationException($"Tarea con ID {id} no encontrada");
                
            return new TareaDTO
            {
                Id = tareaDB.Id,
                Titulo = tareaDB.Titulo,
                Descripcion = tareaDB.Descripcion,
                FechaCreacion = tareaDB.FechaCreacion,
                FechaVencimiento = tareaDB.FechaVencimiento,
                Estado = tareaDB.Estado,
                UsuarioId = tareaDB.UsuarioId,
                NombreUsuario = tareaDB.Usuario?.Nombre ?? "Sin asignar"
            };
        }

        public async Task<TareaDTO> CrearTarea(TareaCreacionDTO tareaDTO)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(tareaDTO.Titulo))
                throw new ApplicationException("El título es obligatorio");
                
            if (tareaDTO.FechaVencimiento < DateTime.Now)
                throw new ApplicationException("La fecha de vencimiento debe ser mayor o igual a la fecha actual");
                
            // Verificar si el usuario existe
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == tareaDTO.UsuarioId);
            if (!usuarioExiste)
                throw new ApplicationException($"El usuario con ID {tareaDTO.UsuarioId} no existe");
                
            // Verificar si el usuario tiene menos de 5 tareas pendientes
            var tareasPendientes = await _context.Tareas
                .CountAsync(t => t.UsuarioId == tareaDTO.UsuarioId && t.Estado == EstadoTarea.Pendiente);
                
            if (tareasPendientes >= 5)
                throw new ApplicationException("El usuario no puede tener más de 5 tareas pendientes");

            var tarea = new Tarea
            {
                Titulo = tareaDTO.Titulo,
                Descripcion = tareaDTO.Descripcion,
                FechaCreacion = DateTime.Now,
                FechaVencimiento = tareaDTO.FechaVencimiento,
                Estado = EstadoTarea.Pendiente,
                UsuarioId = tareaDTO.UsuarioId
            };

            await _context.Tareas.AddAsync(tarea);
            await _context.SaveChangesAsync();

            // Cargar el usuario para el DTO
            await _context.Entry(tarea).Reference(t => t.Usuario).LoadAsync();

            return new TareaDTO
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                FechaCreacion = tarea.FechaCreacion,
                FechaVencimiento = tarea.FechaVencimiento,
                Estado = tarea.Estado,
                UsuarioId = tarea.UsuarioId,
                NombreUsuario = tarea.Usuario?.Nombre ?? "Sin asignar"
            };
        }

        public async Task ActualizarTarea(int id, TareaActualizacionDTO tareaDTO)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
                throw new ApplicationException($"Tarea con ID {id} no encontrada");

            // Validaciones
            if (string.IsNullOrWhiteSpace(tareaDTO.Titulo))
                throw new ApplicationException("El título es obligatorio");
                
            if (tareaDTO.FechaVencimiento < DateTime.Now)
                throw new ApplicationException("La fecha de vencimiento debe ser mayor o igual a la fecha actual");

            tarea.Titulo = tareaDTO.Titulo;
            tarea.Descripcion = tareaDTO.Descripcion;
            tarea.FechaVencimiento = tareaDTO.FechaVencimiento;

            _context.Tareas.Update(tarea);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
                throw new ApplicationException($"Tarea con ID {id} no encontrada");

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();
        }

        public async Task<TareaDTO> CambiarEstadoTarea(int id, EstadoTarea nuevoEstado)
        {
            var tarea = await _context.Tareas.Include(t => t.Usuario).FirstOrDefaultAsync(t => t.Id == id);
            if (tarea == null)
                throw new ApplicationException($"Tarea con ID {id} no encontrada");

            // Validar regla de negocio: no se puede marcar como completada si la fecha de vencimiento aún no llegó
            if (nuevoEstado == EstadoTarea.Completada && DateTime.Now < tarea.FechaVencimiento)
                throw new ApplicationException("No se puede marcar como completada una tarea cuya fecha de vencimiento aún no ha llegado");

            tarea.Estado = nuevoEstado;
            _context.Tareas.Update(tarea);
            await _context.SaveChangesAsync();

            return new TareaDTO
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                FechaCreacion = tarea.FechaCreacion,
                FechaVencimiento = tarea.FechaVencimiento,
                Estado = tarea.Estado,
                UsuarioId = tarea.UsuarioId,
                NombreUsuario = tarea.Usuario?.Nombre ?? "Sin asignar"
            };
        }

        public async Task<TareaDTO> AsignarTareaAUsuario(int id, int usuarioId)
        {
            var tarea = await _context.Tareas.Include(t => t.Usuario).FirstOrDefaultAsync(t => t.Id == id);
            if (tarea == null)
                throw new ApplicationException($"Tarea con ID {id} no encontrada");

            // Verificar si el usuario existe
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == usuarioId);
            if (!usuarioExiste)
                throw new ApplicationException($"El usuario con ID {usuarioId} no existe");

            // Verificar si el usuario tiene menos de 5 tareas pendientes
            var tareasPendientes = await _context.Tareas
                .CountAsync(t => t.UsuarioId == usuarioId && t.Estado == EstadoTarea.Pendiente);
                
            if (tareasPendientes >= 5)
                throw new ApplicationException("El usuario no puede tener más de 5 tareas pendientes");

            tarea.UsuarioId = usuarioId;
            _context.Tareas.Update(tarea);
            await _context.SaveChangesAsync();

            // Recargar el usuario
            await _context.Entry(tarea).Reference(t => t.Usuario).LoadAsync();

            return new TareaDTO
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                FechaCreacion = tarea.FechaCreacion,
                FechaVencimiento = tarea.FechaVencimiento,
                Estado = tarea.Estado,
                UsuarioId = tarea.UsuarioId,
                NombreUsuario = tarea.Usuario?.Nombre ?? "Sin asignar"
            };
        }

        public async Task<ResultadoPaginadoDTO<TareaDTO>> ObtenerTareasPaginadas(PaginacionDTO paginacion)
        {
            var query = _context.Tareas.Include(t => t.Usuario).AsQueryable();

            // Aplicar búsqueda si se proporciona
            if (!string.IsNullOrWhiteSpace(paginacion.Buscar))
            {
                query = query.Where(t => t.Titulo.Contains(paginacion.Buscar) || 
                                      (t.Descripcion != null && t.Descripcion.Contains(paginacion.Buscar)));
            }

            // Contar total antes de paginar
            int totalRegistros = await query.CountAsync();

            // Aplicar ordenamiento si se proporciona
            if (!string.IsNullOrWhiteSpace(paginacion.OrdenarPor))
            {
                switch (paginacion.OrdenarPor.ToLower())
                {
                    case "titulo":
                        query = query.OrderBy(t => t.Titulo);
                        break;
                    case "titulo_desc":
                        query = query.OrderByDescending(t => t.Titulo);
                        break;
                    case "fechacreacion":
                        query = query.OrderBy(t => t.FechaCreacion);
                        break;
                    case "fechacreacion_desc":
                        query = query.OrderByDescending(t => t.FechaCreacion);
                        break;
                    case "fechavencimiento":
                        query = query.OrderBy(t => t.FechaVencimiento);
                        break;
                    case "fechavencimiento_desc":
                        query = query.OrderByDescending(t => t.FechaVencimiento);
                        break;
                    case "estado":
                        query = query.OrderBy(t => t.Estado);
                        break;
                    case "estado_desc":
                        query = query.OrderByDescending(t => t.Estado);
                        break;
                    default:
                        query = query.OrderBy(t => t.Id);
                        break;
                }
            }
            else
            {
                // Ordenamiento por defecto
                query = query.OrderBy(t => t.Id);
            }

            // Aplicar paginación
            var tareas = await query
                .Skip((paginacion.Pagina - 1) * paginacion.RegistrosPorPagina)
                .Take(paginacion.RegistrosPorPagina)
                .ToListAsync();

            // Mapear a DTOs
            var tareasDTO = tareas.Select(t => new TareaDTO
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                FechaCreacion = t.FechaCreacion,
                FechaVencimiento = t.FechaVencimiento,
                Estado = t.Estado,
                UsuarioId = t.UsuarioId,
                NombreUsuario = t.Usuario?.Nombre ?? "Sin asignar"
            }).ToList();

            return new ResultadoPaginadoDTO<TareaDTO>
            {
                Items = tareasDTO,
                TotalRegistros = totalRegistros,
                PaginaActual = paginacion.Pagina,
                RegistrosPorPagina = paginacion.RegistrosPorPagina
            };
        }
    }
}
