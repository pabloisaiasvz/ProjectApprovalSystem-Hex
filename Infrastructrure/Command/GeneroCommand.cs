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
    public class GeneroCommand:IGeneroCommand
    {
        private readonly TemplateContext _context;
        public GeneroCommand(TemplateContext context)
        {
            _context = context;
        }

        public async Task<Genero> CreateGenero(Genero genero)
        {
            _context.Generos.Add(genero);
            await _context.SaveChangesAsync();
            return genero;
        }

        public async Task<Genero> DeleteGenero(int id)
        {
            var generoBorrado = await _context.Generos.FirstOrDefaultAsync(g => g.GeneroId == id);
            _context.Generos.Remove(generoBorrado);
            await _context.SaveChangesAsync();
            return generoBorrado;
        }

        public async Task<Genero> UpdateGenero(GeneroRequest genero, int id)
        {
            Genero generoUpdated = await _context.Generos.FirstOrDefaultAsync(g=>g.GeneroId==id);
            generoUpdated.Nombre = genero.Nombre;
            await _context.SaveChangesAsync();
            return generoUpdated;
        }
    }
}
