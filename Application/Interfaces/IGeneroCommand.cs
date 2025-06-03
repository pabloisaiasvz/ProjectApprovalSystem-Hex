using Application.Request;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGeneroCommand
    {
        Task<Genero> CreateGenero(Genero genero);
        Task<Genero> UpdateGenero(GeneroRequest genero,int id);
        Task<Genero> DeleteGenero(int id);
    }
}
