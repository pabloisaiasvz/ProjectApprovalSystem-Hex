using Application.Interfaces.IMappers;
using Application.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class GenericoMapper : IGenericoMapper
    {
        public Task<List<ArtistaResponse>> GetAllArtistasResponse(List<Artista> artistas)
        {
            List<ArtistaResponse> lista = new List<ArtistaResponse>();
            foreach (var artista in artistas)
            {
                var response = new ArtistaResponse
                {
                    ArtistaId = artista.ArtistaId,
                    Nombre = artista.Nombre,
                };
                lista.Add(response);
            }
            return Task.FromResult(lista);
        }

        public Task<List<GeneroResponse>> GetAllGenerosResponse(List<Genero> generos)
        {
            List<GeneroResponse> lista = new List<GeneroResponse>();
            foreach (var genero in generos)
            {
                var response = new GeneroResponse
                {
                    GeneroId = genero.GeneroId,
                    Nombre = genero.Nombre
                };
                lista.Add(response);
            }
            return Task.FromResult(lista);  
        }

        public Task<ArtistaResponse> GetArtistaResponse(Artista artista)
        {
            var response = new ArtistaResponse
            {
                ArtistaId = artista.ArtistaId,
                Nombre = artista.Nombre,
            };
            return Task.FromResult(response);
        }

        public Task<GeneroResponse> GetGeneroResponse(Genero genero)
        {
            var response = new GeneroResponse
            {
                GeneroId = genero.GeneroId,
                Nombre = genero.Nombre
            };
            return Task.FromResult(response);
        }
    }
}
