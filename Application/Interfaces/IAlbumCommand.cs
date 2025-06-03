using Application.Request;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAlbumCommand
    {
        Task<Album> CreateAlbum(Album album);
        Task<Album> UpdateAlbum(AlbumRequest album, int id);
        Task<Album> DeleteAlbum(int id);
    }
}
