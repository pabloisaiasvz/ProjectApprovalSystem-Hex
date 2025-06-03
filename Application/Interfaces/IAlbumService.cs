using Application.Request;
using Application.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAlbumService
    {
        Task<AlbumResponse> CreateAlbum(AlbumRequest request);
        Task<AlbumResponse> UpdateAlbum(AlbumRequest request, int id);
        Task<AlbumResponse> DeleteAlbum(int id);
        Task<AlbumResponse> GetAlbumById(int id);
        Task<List<AlbumResponse>> GetAllAlbums();
    }
}
