using Application.Exceptions;
using Application.Interfaces;
using Application.Request;
using Application.Response;
using Application.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Template.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistaController : ControllerBase
    {
        private readonly IArtistaService _artistaService;
        public ArtistaController(IArtistaService artistaService)
        {
            _artistaService = artistaService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(ArtistaResponse), 201)]
        public async Task<IActionResult> CreateArtista(ArtistaRequest request)
        {
            try
            {
                var result = await _artistaService.CreateArtista(request);
                return new JsonResult(result) { StatusCode = 201 };
            }
            catch (ExceptionBadRequest ex)
            {

                return new JsonResult(new ApiError { Message = ex.Message }) { StatusCode = 400 };
            }
        }
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ArtistaResponse), 200)]
        public async Task<IActionResult> UpdateArtista(ArtistaRequest request, int id)
        {
            try
            {
                var result = await _artistaService.UpdateArtista(request, id);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (ExceptionNotFound ex)
            {

                return new JsonResult(new ApiError { Message = ex.Message }) { StatusCode = 404 };
            }
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ArtistaResponse), 200)]
        public async Task<IActionResult> GetArtistaById(int id)
        {
            try
            {
                var result = await _artistaService.GetArtistaById(id);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (ExceptionNotFound ex)
            {

                return new JsonResult(new ApiError { Message = ex.Message }) { StatusCode = 404 };
            }
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<ArtistaResponse>), 200)]
        public async Task<IActionResult> GetAllGeneros()
        {
            var result = await _artistaService.GetAllArtistas();
            return new JsonResult(result) { StatusCode = 200 };
        }
        [HttpDelete]
        [ProducesResponseType(typeof(ArtistaResponse), 200)]
        public async Task<IActionResult> DeleteGeneroById(int id)
        {
            try
            {
                var result = await _artistaService.DeleteArtista(id);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (ExceptionNotFound ex)
            {

                return new JsonResult(new ApiError { Message = ex.Message }) { StatusCode = 404 };
            }
        }
    }
}
