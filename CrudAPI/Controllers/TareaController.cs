using Microsoft.AspNetCore.Mvc;
using CrudAPI.DTOs;
using CrudAPI.Entities;
using CrudAPI.Models;
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
        public async Task<ActionResult<ApiResponse<List<TareaDTO>>>> ListarTareas()
        {
            var tareas = await _tareaService.ListarTareas();
            return Ok(new ApiResponse<List<TareaDTO>>(tareas, "Tareas obtenidas correctamente"));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ApiResponse<TareaDTO>>> ObtenerTarea(int id)
        {
            var tarea = await _tareaService.ObtenerTareaPorId(id);
            return Ok(new ApiResponse<TareaDTO>(tarea, "Tarea obtenida correctamente"));
        }

        [HttpPost]
        [Route("crear")]
        public async Task<ActionResult<ApiResponse<TareaDTO>>> CrearTarea(TareaCreacionDTO tareaDTO)
        {
            var resultado = await _tareaService.CrearTarea(tareaDTO);
            var response = new ApiResponse<TareaDTO>(resultado, "Tarea creada correctamente");
            return CreatedAtAction(nameof(ObtenerTarea), new { id = resultado.Id }, response);
        }

        [HttpPut]
        [Route("actualizar/{id}")]
        public async Task<ActionResult<ApiResponse>> ActualizarTarea(int id, TareaActualizacionDTO tareaDTO)
        {
            await _tareaService.ActualizarTarea(id, tareaDTO);
            return Ok(new ApiResponse("Tarea actualizada correctamente"));
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public async Task<ActionResult<ApiResponse>> EliminarTarea(int id)
        {
            await _tareaService.EliminarTarea(id);
            return Ok(new ApiResponse("Tarea eliminada correctamente"));
        }

        [HttpPut]
        [Route("{id}/estado")]
        public async Task<ActionResult<ApiResponse<TareaDTO>>> CambiarEstado(int id, TareaEstadoDTO estadoDTO)
        {
            var tarea = await _tareaService.CambiarEstadoTarea(id, estadoDTO.Estado);
            return Ok(new ApiResponse<TareaDTO>(tarea, "Estado de tarea actualizado correctamente"));
        }

        [HttpPut]
        [Route("{id}/asignar/{usuarioId}")]
        public async Task<ActionResult<ApiResponse<TareaDTO>>> AsignarUsuario(int id, int usuarioId)
        {
            var tarea = await _tareaService.AsignarTareaAUsuario(id, usuarioId);
            return Ok(new ApiResponse<TareaDTO>(tarea, "Tarea asignada correctamente"));
        }

        [HttpGet]
        [Route("paginadas")]
        public async Task<ActionResult<ApiResponse<ResultadoPaginadoDTO<TareaDTO>>>> ObtenerTareasPaginadas(
            [FromQuery] int pagina = 1, 
            [FromQuery] int registrosPorPagina = 10,
            [FromQuery] string? ordenarPor = null,
            [FromQuery] string? buscar = null)
        {
            var paginacion = new PaginacionDTO
            {
                Pagina = pagina > 0 ? pagina : 1,
                RegistrosPorPagina = registrosPorPagina > 0 && registrosPorPagina <= 50 ? registrosPorPagina : 10,
                OrdenarPor = ordenarPor,
                Buscar = buscar
            };

            var resultado = await _tareaService.ObtenerTareasPaginadas(paginacion);
            return Ok(new ApiResponse<ResultadoPaginadoDTO<TareaDTO>>(resultado, "Tareas paginadas obtenidas correctamente"));
        }
    }
}
