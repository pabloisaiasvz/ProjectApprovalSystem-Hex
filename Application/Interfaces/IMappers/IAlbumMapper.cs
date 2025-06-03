using Application.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IMappers
{
    public interface IAlbumMapper
    {
        Task<AlbumResponse> GetAlbumResponse(Album album);
        Task<List<AlbumResponse>> GetAllAlbums(List<Album> Albumes);
    }
}
