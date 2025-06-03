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
    public class GeneroService:IGeneroService
    {
        private readonly IGeneroCommand _command;
        private readonly IGeneroQuery _query;
        private readonly IGenericoMapper _genericoMapper;
        public GeneroService(IGeneroCommand command, IGeneroQuery query, IGenericoMapper genericoMapper)
        {
            _command = command;
            _query = query;
            _genericoMapper = genericoMapper;
        }

        public async Task<GeneroResponse> CreateGenero(GeneroRequest request)
        {
            try
            {
                await CheckGeneroNombre(request);
                var genero = new Genero
                {
                    Nombre = request.Nombre
                };
                var result = await _command.CreateGenero(genero);

                return await _genericoMapper.GetGeneroResponse(result);

            }
            catch (ExceptionBadRequest ex)
            {

                throw new ExceptionBadRequest(ex.Message);
            }
        }

        public async Task<GeneroResponse> GetGeneroById(int id)
        {
            try
            {
                await CheckGeneroId(id);
                var genero = await _query.GetGeneroById(id);
                return await _genericoMapper.GetGeneroResponse(genero);
            }
            catch (ExceptionNotFound ex)
            {

                throw new ExceptionNotFound(ex.Message);
            }
        }

        public async Task<GeneroResponse> DeleteGenero(int id)
        {
            try
            {
                await CheckGeneroId(id);
                var generoBorrado = await _command.DeleteGenero(id);
                return await _genericoMapper.GetGeneroResponse(generoBorrado);
            }
            catch (ExceptionNotFound ex)
            {

                throw new ExceptionNotFound(ex.Message);
            }
        }

        public async Task<List<GeneroResponse>> GetAllGeneros()
        {
            var list = await _query.GetAllGeneros();
            return await _genericoMapper.GetAllGenerosResponse(list);
        }

        public async Task<GeneroResponse> UpdateGenero(GeneroRequest request, int id)
        {
            try
            {
                await CheckGeneroNombre(request);
                await CheckGeneroId(id);
                var generoUpdated = await _command.UpdateGenero(request, id);
                return await _genericoMapper.GetGeneroResponse(generoUpdated);
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

        private Task CheckGeneroNombre(GeneroRequest request)
        {
            if (request.Nombre.Contains("string"))
            {
                throw new ExceptionBadRequest("el nombre del genero no puede ser string");
            }
            else
                return Task.CompletedTask;
        }
        private async Task CheckGeneroId(int id)
        {
            if (await _query.GetGeneroById(id) == null)
            {
                throw new ExceptionNotFound("No hay Genero con ese id");
            }
        }


    }
    
}
