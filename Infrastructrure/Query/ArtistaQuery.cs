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
    public class ArtistaQuery:IArtistaQuery
    {
        private readonly TemplateContext _context;
        public ArtistaQuery(TemplateContext context)
        {
            _context = context;
        }

        public async Task<List<Artista>> GetAllArtistas()
        {
            return await _context.Artistas.ToListAsync();
        }

        public Task<Artista> GetArtistaById(int id)
        {
            return _context.Artistas.FirstOrDefaultAsync(a => a.ArtistaId == id);
        }
    }
}
