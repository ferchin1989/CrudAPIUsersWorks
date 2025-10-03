using Microsoft.EntityFrameworkCore;
using CrudAPI.Context;
using CrudAPI.DTOs;
using CrudAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrudAPI.Services
{
    public class UsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UsuarioDTO>> ListarUsuarios()
        {
            var listaDTO = new List<UsuarioDTO>();
            var listaDB = await _context.Usuarios.ToListAsync();
            
            foreach (var item in listaDB)
            {
                listaDTO.Add(new UsuarioDTO
                {
                    Id = item.Id,
                    Nombre = item.Nombre,
                    Email = item.Email
                });
            }
            
            return listaDTO;
        }

        public async Task<UsuarioDTO> ObtenerUsuarioPorId(int id)
        {
            var usuarioDB = await _context.Usuarios.FindAsync(id);
            
            if (usuarioDB == null)
                throw new ApplicationException($"Usuario con ID {id} no encontrado");
                
            return new UsuarioDTO
            {
                Id = usuarioDB.Id,
                Nombre = usuarioDB.Nombre,
                Email = usuarioDB.Email
            };
        }

        public async Task<UsuarioDTO> CrearUsuario(UsuarioCreacionDTO usuarioDTO)
        {
            // Validar email con regex
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(usuarioDTO.Email, pattern))
                throw new ApplicationException("El formato del email no es válido");

            // Verificar si ya existe un usuario con ese email
            var existeEmail = await _context.Usuarios.AnyAsync(u => u.Email == usuarioDTO.Email);
            if (existeEmail)
                throw new ApplicationException($"Ya existe un usuario con el email {usuarioDTO.Email}");

            var usuario = new Usuario
            {
                Nombre = usuarioDTO.Nombre,
                Email = usuarioDTO.Email
            };

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };
        }

        public async Task ActualizarUsuario(int id, UsuarioCreacionDTO usuarioDTO)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                throw new ApplicationException($"Usuario con ID {id} no encontrado");

            // Validar email con regex
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(usuarioDTO.Email, pattern))
                throw new ApplicationException("El formato del email no es válido");

            // Verificar si ya existe otro usuario con ese email
            var existeEmail = await _context.Usuarios.AnyAsync(u => u.Email == usuarioDTO.Email && u.Id != id);
            if (existeEmail)
                throw new ApplicationException($"Ya existe otro usuario con el email {usuarioDTO.Email}");

            usuario.Nombre = usuarioDTO.Nombre;
            usuario.Email = usuarioDTO.Email;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                throw new ApplicationException($"Usuario con ID {id} no encontrado");

            // Verificar si el usuario tiene tareas asignadas
            var tieneTareas = await _context.Tareas.AnyAsync(t => t.UsuarioId == id);
            if (tieneTareas)
                throw new ApplicationException("No se puede eliminar el usuario porque tiene tareas asignadas");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<UsuarioEstadisticasDTO> ObtenerEstadisticasUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                throw new ApplicationException($"Usuario con ID {id} no encontrado");

            var tareas = await _context.Tareas.Where(t => t.UsuarioId == id).ToListAsync();
            int totalTareas = tareas.Count;
            int tareasCompletadas = tareas.Count(t => t.Estado == EstadoTarea.Completada);
            double porcentajeCompletado = totalTareas > 0 ? (double)tareasCompletadas / totalTareas * 100 : 0;

            return new UsuarioEstadisticasDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                TotalTareas = totalTareas,
                TareasCompletadas = tareasCompletadas,
                PorcentajeCompletado = Math.Round(porcentajeCompletado, 2)
            };
        }
    }
}
