using Microsoft.AspNetCore.Mvc;
using CrudAPI.DTOs;
using CrudAPI.Models;
using CrudAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [Route("lista")]
        public async Task<ActionResult<ApiResponse<List<UsuarioDTO>>>> ListarUsuarios()
        {
            var usuarios = await _usuarioService.ListarUsuarios();
            return Ok(new ApiResponse<List<UsuarioDTO>>(usuarios, "Usuarios obtenidos correctamente"));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ApiResponse<UsuarioDTO>>> ObtenerUsuario(int id)
        {
            var usuario = await _usuarioService.ObtenerUsuarioPorId(id);
            return Ok(new ApiResponse<UsuarioDTO>(usuario, "Usuario obtenido correctamente"));
        }

        [HttpPost]
        [Route("crear")]
        public async Task<ActionResult<ApiResponse<UsuarioDTO>>> CrearUsuario(UsuarioCreacionDTO usuarioDTO)
        {
            var resultado = await _usuarioService.CrearUsuario(usuarioDTO);
            var response = new ApiResponse<UsuarioDTO>(resultado, "Usuario creado correctamente");
            return CreatedAtAction(nameof(ObtenerUsuario), new { id = resultado.Id }, response);
        }

        [HttpPut]
        [Route("actualizar/{id}")]
        public async Task<ActionResult<ApiResponse>> ActualizarUsuario(int id, UsuarioCreacionDTO usuarioDTO)
        {
            await _usuarioService.ActualizarUsuario(id, usuarioDTO);
            return Ok(new ApiResponse("Usuario actualizado correctamente"));
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public async Task<ActionResult<ApiResponse>> EliminarUsuario(int id)
        {
            await _usuarioService.EliminarUsuario(id);
            return Ok(new ApiResponse("Usuario eliminado correctamente"));
        }

        [HttpGet]
        [Route("{id}/estadisticas")]
        public async Task<ActionResult<ApiResponse<UsuarioEstadisticasDTO>>> ObtenerEstadisticas(int id)
        {
            var estadisticas = await _usuarioService.ObtenerEstadisticasUsuario(id);
            return Ok(new ApiResponse<UsuarioEstadisticasDTO>(estadisticas, "Estadísticas obtenidas correctamente"));
        }
    }
}
