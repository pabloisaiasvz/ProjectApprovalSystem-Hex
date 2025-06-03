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
    public class AlbumCommand:IAlbumCommand
    {
        private readonly TemplateContext _context;
        public AlbumCommand(TemplateContext context)
        {
            _context = context;
        }

        public async Task<Album> CreateAlbum(Album album)
        {
            _context.Albumes.Add(album);
            await _context.SaveChangesAsync();
            return album;
        }

        public async Task<Album> DeleteAlbum(int id)
        {
            var albumBorrado = await _context.Albumes.FirstOrDefaultAsync(a=>a.AlbumId == id);
            _context.Albumes.Remove(albumBorrado);
            return albumBorrado;
        }

        public async Task<Album> UpdateAlbum(AlbumRequest album, int id)
        {
            Album albumUpdated = await _context.Albumes.FirstOrDefaultAsync(a => a.AlbumId == id);
            albumUpdated.Nombre= album.Nombre;
            albumUpdated.ArtistaId= album.ArtistaId;
            albumUpdated.GeneroId= album.GeneroId;
            await _context.SaveChangesAsync();
            return albumUpdated;

        }
    }
}
