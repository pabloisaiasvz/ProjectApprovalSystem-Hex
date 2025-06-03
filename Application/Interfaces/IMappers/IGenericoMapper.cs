using Application.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IMappers
{
    public interface IGenericoMapper
    {
        Task<ArtistaResponse>GetArtistaResponse(Artista artista);
        Task<List<ArtistaResponse>> GetAllArtistasResponse(List<Artista> artistas);
        Task<GeneroResponse> GetGeneroResponse(Genero genero);
        Task<List<GeneroResponse>> GetAllGenerosResponse(List<Genero> generos);
    }
}
