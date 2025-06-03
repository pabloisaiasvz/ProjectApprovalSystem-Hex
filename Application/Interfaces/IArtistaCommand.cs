using Application.Request;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IArtistaCommand
    {
        Task<Artista> CreateArtista(Artista artista);
        Task<Artista> UpdateArtista(ArtistaRequest artista, int id);
        Task<Artista> DeleteArtista(int id);
    }
}
