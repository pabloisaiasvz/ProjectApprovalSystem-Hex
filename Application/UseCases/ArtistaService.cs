using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.IMappers;
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
    public class ArtistaService:IArtistaService
    {
        private readonly IArtistaQuery _query;
        private readonly IArtistaCommand _command;
        private readonly IGenericoMapper _genericoMapper;
        public ArtistaService(IArtistaQuery query, IArtistaCommand command, IGenericoMapper genericoMapper)
        {
            _query = query;
            _command = command;
            _genericoMapper = genericoMapper;
        }

        public async Task<ArtistaResponse> CreateArtista(ArtistaRequest request)
        {
            try
            {
                await CheckArtistaNombre(request);
                var artista = new Artista
                {
                    Nombre = request.Nombre
                };
                var result = await _command.CreateArtista(artista);

                return await _genericoMapper.GetArtistaResponse(result);

            }
            catch (ExceptionBadRequest ex)
            {

                throw new ExceptionBadRequest(ex.Message);
            }
        }

        public async Task<ArtistaResponse> DeleteArtista(int id)
        {
            try
            {
                await CheckArtistaId(id);
                var artistaBorrado = await _command.DeleteArtista(id);
                return await _genericoMapper.GetArtistaResponse(artistaBorrado);
            }
            catch (ExceptionNotFound ex)
            {

                throw new ExceptionNotFound(ex.Message);
            }
        }

        public async Task<ArtistaResponse> GetArtistaById(int id)
        {
            try
            {
                await CheckArtistaId(id);
                var artista = await _query.GetArtistaById(id);
                return await _genericoMapper.GetArtistaResponse(artista);
            }
            catch (ExceptionNotFound ex)
            {

                throw new ExceptionNotFound(ex.Message);
            }
        }

        public async Task<List<ArtistaResponse>> GetAllArtistas()
        {
            var list = await _query.GetAllArtistas();
            return await _genericoMapper.GetAllArtistasResponse(list);
        }

        public async Task<ArtistaResponse> UpdateArtista(ArtistaRequest request, int id)
        {
            try
            {
                await CheckArtistaNombre(request);
                await CheckArtistaId(id);
                var artistaUpdated = await _command.UpdateArtista(request, id);
                return await _genericoMapper.GetArtistaResponse(artistaUpdated);
            }
            catch (ExceptionBadRequest ex)
            {

                throw new ExceptionBadRequest(ex.Message);
            }
            catch(ExceptionNotFound ex)
            {
                throw new ExceptionNotFound(ex.Message);
            }
        }

        private Task CheckArtistaNombre(ArtistaRequest request)
        {
            if (request.Nombre.Contains("string"))
            {
                throw new ExceptionBadRequest("el nombre del artista no puede ser string");
            }
            else
                return Task.CompletedTask;
        }
        private async Task CheckArtistaId(int id)
        {
            if (await _query.GetArtistaById(id)==null)
            {
                throw new ExceptionNotFound("No hay Artista con ese id");
            }
        }


    }
}
