using Microsoft.AspNetCore.Mvc;
using CrudAPI.DTOs;
using CrudAPI.Entities;
using CrudAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TareaController : ControllerBase
    {
        private readonly TareaService _tareaService;

        public TareaController(TareaService tareaService)
        {
            _tareaService = tareaService;
        }

        [HttpGet]
        [Route("lista")]
        public async Task<ActionResult<List<TareaDTO>>> ListarTareas()
        {
            try
            {
                return Ok(await _tareaService.ListarTareas());
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<TareaDTO>> ObtenerTarea(int id)
        {
            try
            {
                return Ok(await _tareaService.ObtenerTareaPorId(id));
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Route("crear")]
        public async Task<ActionResult<TareaDTO>> CrearTarea(TareaCreacionDTO tareaDTO)
        {
            try
            {
                var resultado = await _tareaService.CrearTarea(tareaDTO);
                return CreatedAtAction(nameof(ObtenerTarea), new { id = resultado.Id }, resultado);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("actualizar/{id}")]
        public async Task<ActionResult> ActualizarTarea(int id, TareaActualizacionDTO tareaDTO)
        {
            try
            {
                await _tareaService.ActualizarTarea(id, tareaDTO);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("no encontrada"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public async Task<ActionResult> EliminarTarea(int id)
        {
            try
            {
                await _tareaService.EliminarTarea(id);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("no encontrada"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id}/estado")]
        public async Task<ActionResult<TareaDTO>> CambiarEstado(int id, TareaEstadoDTO estadoDTO)
        {
            try
            {
                return Ok(await _tareaService.CambiarEstadoTarea(id, estadoDTO.Estado));
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("no encontrada"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id}/asignar/{usuarioId}")]
        public async Task<ActionResult<TareaDTO>> AsignarUsuario(int id, int usuarioId)
        {
            try
            {
                return Ok(await _tareaService.AsignarTareaAUsuario(id, usuarioId));
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("no encontrada") || ex.Message.Contains("no existe"))
                    return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("paginadas")]
        public async Task<ActionResult<ResultadoPaginadoDTO<TareaDTO>>> ObtenerTareasPaginadas(
            [FromQuery] int pagina = 1, 
            [FromQuery] int registrosPorPagina = 10,
            [FromQuery] string? ordenarPor = null,
            [FromQuery] string? buscar = null)
        {
            try
            {
                var paginacion = new PaginacionDTO
                {
                    Pagina = pagina > 0 ? pagina : 1,
                    RegistrosPorPagina = registrosPorPagina > 0 && registrosPorPagina <= 50 ? registrosPorPagina : 10,
                    OrdenarPor = ordenarPor,
                    Buscar = buscar
                };

                return Ok(await _tareaService.ObtenerTareasPaginadas(paginacion));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
