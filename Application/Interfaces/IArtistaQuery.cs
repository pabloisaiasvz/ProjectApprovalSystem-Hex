using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IArtistaQuery
    {
        Task<Artista> GetArtistaById(int id);
        Task<List<Artista>> GetAllArtistas();
    }
}
