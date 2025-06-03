using Application.Interfaces;
using Application.Request;
using Domain.Entities;
using Infrastructrure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructrure.Command
{
    public class ArtistaCommand:IArtistaCommand
    {
        private readonly TemplateContext _context;
        public ArtistaCommand(TemplateContext context)
        {
            _context = context;
        }

        public async Task<Artista> CreateArtista(Artista artista)
        {
            _context.Artistas.Add(artista);
            await _context.SaveChangesAsync();
            return artista;
        }

        public async Task<Artista> DeleteArtista(int id)
        {
            var generoBorrado = await _context.Artistas.FirstOrDefaultAsync(a=>a.ArtistaId== id);
            _context.Artistas.Remove(generoBorrado);
            await _context.SaveChangesAsync();
            return generoBorrado;
        }

        public async Task<Artista> UpdateArtista(ArtistaRequest artista, int id)
        {
            Artista artistaUpdated = await _context.Artistas.FirstOrDefaultAsync(a => a.ArtistaId == id);
            artistaUpdated.Nombre = artista.Nombre;
            await _context.SaveChangesAsync();
            return artistaUpdated;
        }
    }
}
