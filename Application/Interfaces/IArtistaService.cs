using Application.Request;
using Application.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IArtistaService
    {
        Task<ArtistaResponse>CreateArtista(ArtistaRequest request);
        Task<ArtistaResponse>UpdateArtista(ArtistaRequest request, int id);
        Task<ArtistaResponse> DeleteArtista(int id);
        Task<ArtistaResponse> GetArtistaById(int id);
        Task<List<ArtistaResponse>> GetAllArtistas();
    }
}
