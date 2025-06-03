using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.IMappers;
using Application.Mappers;
using Application.Request;
using Application.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class AlbumService:IAlbumService
    {
        private readonly IAlbumQuery _query;
        private readonly IAlbumCommand _command;
        private readonly IAlbumMapper _mapper;
        private readonly IGeneroService _generoService;
        private readonly IArtistaService _artistaService;
        public AlbumService(IAlbumQuery query, IAlbumCommand command, IAlbumMapper mapper, IGeneroService generoService, IArtistaService artistaService)
        {
            _query = query;
            _command = command;
            _mapper = mapper;
            _generoService = generoService;
            _artistaService = artistaService;
        }

        public async Task<AlbumResponse> CreateAlbum(AlbumRequest request)
        {
            try
            {
                await CheckAlbumNombre(request);
                var genero = new Album
                {
                    Nombre = request.Nombre
                };
                var result = await _command.CreateAlbum(genero);

                return await _mapper.GetAlbumResponse(await _query.GetAlbumById(request.GeneroId));

            }
            catch (ExceptionBadRequest ex)
            {

                throw new ExceptionBadRequest(ex.Message);
            }
        }

        public async Task<AlbumResponse> DeleteAlbum(int id)
        {
            try
            {
                await CheckAlbumId(id);
                var generoBorrado = await _command.DeleteAlbum(id);
                return await _mapper.GetAlbumResponse(await _query.GetAlbumById(id));
            }
            catch (ExceptionNotFound ex)
            {

                throw new ExceptionNotFound(ex.Message);
            }
        }

        public async Task<AlbumResponse> GetAlbumById(int id)
        {
            try
            {
                await CheckAlbumId(id);
                var genero = await _query.GetAlbumById(id);
                return await _mapper.GetAlbumResponse(await _query.GetAlbumById(id));
            }
            catch (ExceptionNotFound ex)
            {

                throw new ExceptionNotFound(ex.Message);
            }
        }

        public async Task<List<AlbumResponse>> GetAllAlbums()
        {
            var list = await _query.GetAllAlbums();
            return await _mapper.GetAllAlbums(list);
        }

        public async Task<AlbumResponse> UpdateAlbum(AlbumRequest request, int id)
        {
            try
            {
                await CheckAlbumNombre(request);
                await CheckAlbumId(id);
                var albumUpdated = await _command.UpdateAlbum(request, id);
                return await _mapper.GetAlbumResponse(await _query.GetAlbumById(id));
            }
            catch (ExceptionBadRequest ex)
            {

                throw new ExceptionBadRequest(ex.Message);
            }
            catch (ExceptionNotFound ex)
            {
                throw new ExceptionNotFound(ex.Message);
            }
        }

        private Task CheckAlbumNombre(AlbumRequest request)
        {
            if (request.Nombre.Contains("string"))
            {
                throw new ExceptionBadRequest("el nombre del Album no puede ser string");
            }
            else
                return Task.CompletedTask;
        }
        private async Task CheckAlbumId(int id)
        {
            if (await _query.GetAlbumById(id) == null)
            {
                throw new ExceptionNotFound("No hay Album con ese id");
            }
        }
    }
}
