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
    public class AlbumQuery:IAlbumQuery
    {
        private readonly TemplateContext _context;
        public AlbumQuery(TemplateContext context)
        {
            _context = context;
        }

        public async Task<Album> GetAlbumById(int id)
        {
            return await _context.Albumes
                .Include(a => a.Artista)
                .Include(a => a.Genero)
                .FirstOrDefaultAsync(a => a.AlbumId == id);
        }

        public async Task<List<Album>> GetAllAlbums()
        {
            return await _context.Albumes
                .Include(a => a.Artista)
                .Include(a => a.Genero)
                .ToListAsync();
        }
    }
}
