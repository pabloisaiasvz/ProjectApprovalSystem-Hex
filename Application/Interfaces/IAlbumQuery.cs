using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAlbumQuery
    {
        Task<Album> GetAlbumById(int id);
        Task<List<Album>> GetAllAlbums();
    }
}
