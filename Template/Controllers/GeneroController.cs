using Application.Exceptions;
using Application.Interfaces;
using Application.Request;
using Application.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Template.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneroController : ControllerBase
    {
        private readonly IGeneroService _generoService;
        public GeneroController(IGeneroService generoService)
        {
            _generoService = generoService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(GeneroResponse), 201)]
        public async Task<IActionResult>CreateGenero(GeneroRequest request)
        {
            try
            {
                var result = await _generoService.CreateGenero(request);
                return new JsonResult(result) { StatusCode = 201};
            }
            catch (ExceptionBadRequest ex)
            {

                return new JsonResult(new ApiError { Message = ex.Message }) { StatusCode = 400 };
            }
        }
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(GeneroResponse), 200)]
        public async Task<IActionResult> UpdateGenero(GeneroRequest request,int id)
        {
            try
            {
                var result = await _generoService.UpdateGenero(request, id);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (ExceptionNotFound ex)
            {

                return new JsonResult(new ApiError { Message = ex.Message }) { StatusCode = 404 };
            }
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GeneroResponse),200)]
        public async Task<IActionResult>GetGeneroById(int id)
        {
            try
            {
                var result = await _generoService.GetGeneroById(id);
                return new JsonResult(result) { StatusCode =200};
            }
            catch (ExceptionNotFound ex)
            {

                return new JsonResult(new ApiError { Message = ex.Message }) {StatusCode = 404 };
            }
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<GeneroResponse>), 200)]
        public async Task<IActionResult> GetAllGeneros()
        {
            var result = await _generoService.GetAllGeneros();
            return new JsonResult(result) { StatusCode = 200 };
        }
        [HttpDelete]
        [ProducesResponseType(typeof(GeneroResponse), 200)]
        public async Task<IActionResult> DeleteGeneroById(int id)
        {
            try
            {
                var result = await _generoService.DeleteGenero(id);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (ExceptionNotFound ex)
            {

                return new JsonResult(new ApiError { Message = ex.Message }) { StatusCode = 404 };
            }
        }
    }
}
