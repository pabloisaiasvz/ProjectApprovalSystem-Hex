using Application.Request;
using Application.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGeneroService
    {
        Task<GeneroResponse> CreateGenero(GeneroRequest request);
        Task<GeneroResponse> UpdateGenero(GeneroRequest request, int id);
        Task<GeneroResponse> DeleteGenero(int id);
        Task<GeneroResponse> GetGeneroById(int id);
        Task<List<GeneroResponse>> GetAllGeneros();
    }
}
