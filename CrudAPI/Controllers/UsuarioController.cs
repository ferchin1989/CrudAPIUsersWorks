using Microsoft.AspNetCore.Mvc;
using CrudAPI.DTOs;
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
        public async Task<ActionResult<List<UsuarioDTO>>> ListarUsuarios()
        {
            try
            {
                return Ok(await _usuarioService.ListarUsuarios());
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<UsuarioDTO>> ObtenerUsuario(int id)
        {
            try
            {
                return Ok(await _usuarioService.ObtenerUsuarioPorId(id));
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Route("crear")]
        public async Task<ActionResult<UsuarioDTO>> CrearUsuario(UsuarioCreacionDTO usuarioDTO)
        {
            try
            {
                var resultado = await _usuarioService.CrearUsuario(usuarioDTO);
                return CreatedAtAction(nameof(ObtenerUsuario), new { id = resultado.Id }, resultado);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("actualizar/{id}")]
        public async Task<ActionResult> ActualizarUsuario(int id, UsuarioCreacionDTO usuarioDTO)
        {
            try
            {
                await _usuarioService.ActualizarUsuario(id, usuarioDTO);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("no encontrado"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public async Task<ActionResult> EliminarUsuario(int id)
        {
            try
            {
                await _usuarioService.EliminarUsuario(id);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("no encontrado"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id}/estadisticas")]
        public async Task<ActionResult<UsuarioEstadisticasDTO>> ObtenerEstadisticas(int id)
        {
            try
            {
                return Ok(await _usuarioService.ObtenerEstadisticasUsuario(id));
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("no encontrado"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
