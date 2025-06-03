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
    public class AlbumController : ControllerBase
    {
        private IAlbumService _albumService;
        public AlbumController(IAlbumService albumService)
        {
            _albumService = albumService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(AlbumResponse), 201)]
        public async Task<IActionResult> CreateAlbum(AlbumRequest request)
        {
            try
            {
                var result = await _albumService.CreateAlbum(request);
                return new JsonResult(result) { StatusCode = 201 };
            }
            catch (ExceptionBadRequest ex)
            {

                return new JsonResult(new ApiError { Message = ex.Message }) { StatusCode = 400 };
            }
        }
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AlbumResponse), 200)]
        public async Task<IActionResult> UpdateAlbum(AlbumRequest request, int id)
        {
            try
            {
                var result = await _albumService.UpdateAlbum(request, id);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (ExceptionNotFound ex)
            {

                return new JsonResult(new ApiError { Message = ex.Message }) { StatusCode = 404 };
            }
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AlbumResponse), 200)]
        public async Task<IActionResult> GetAlbumById(int id)
        {
            try
            {
                var result = await _albumService.GetAlbumById(id);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (ExceptionNotFound ex)
            {

                return new JsonResult(new ApiError { Message = ex.Message }) { StatusCode = 404 };
            }
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<AlbumResponse>), 200)]
        public async Task<IActionResult> GetAllGeneros()
        {
            var result = await _albumService.GetAllAlbums();
            return new JsonResult(result) { StatusCode = 200 };
        }
        [HttpDelete]
        [ProducesResponseType(typeof(ArtistaResponse), 200)]
        public async Task<IActionResult> DeleteGeneroById(int id)
        {
            try
            {
                var result = await _albumService.DeleteAlbum(id);
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (ExceptionNotFound ex)
            {

                return new JsonResult(new ApiError { Message = ex.Message }) { StatusCode = 404 };
            }
        }

    }
}
