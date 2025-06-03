using Application.Interfaces;
using Domain.Entities;
using Infrastructrure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructrure.Query
{
    public class GeneroQuery:IGeneroQuery
    {
        private readonly TemplateContext _context;
        public GeneroQuery(TemplateContext context)
        {
            _context = context;
        }

        public async Task<List<Genero>> GetAllGeneros()
        {
            return await _context.Generos.ToListAsync();
        }

        public async Task<Genero> GetGeneroById(int id)
        {
            return await _context.Generos.FirstOrDefaultAsync(g => g.GeneroId == id);
        }
    }
}
