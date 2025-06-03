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
    public class AlbumMapper:IAlbumMapper
    {
        private readonly IGenericoMapper _genericoMapper;

        public AlbumMapper(IGenericoMapper genericoMapper)
        {
            _genericoMapper = genericoMapper;
        }

        public async Task<AlbumResponse> GetAlbumResponse(Album album)
        {
            var response = new AlbumResponse
            {
                AlbumId = album.AlbumId,
                Nombre = album.Nombre,
                Artista = await _genericoMapper.GetArtistaResponse(album.Artista),
                Genero = await _genericoMapper.GetGeneroResponse(album.Genero),
            };
            return response;
        }

        public async Task<List<AlbumResponse>> GetAllAlbums(List<Album> Albumes)
        {
            List<AlbumResponse> lista = new List<AlbumResponse>();
            foreach (var album in Albumes) 
            {
                var response = new AlbumResponse
                {
                    AlbumId = album.AlbumId,
                    Nombre = album.Nombre,
                    Artista = await _genericoMapper.GetArtistaResponse(album.Artista),
                    Genero = await _genericoMapper.GetGeneroResponse(album.Genero),
                };
                lista.Add(response);
            }
            return lista;
        }
    }
}
