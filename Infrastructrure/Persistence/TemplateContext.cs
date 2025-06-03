using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructrure.Persistence
{
    public class TemplateContext: DbContext
    {
        public DbSet<Album> Albumes { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Artista> Artistas { get; set; }

        public TemplateContext(DbContextOptions<TemplateContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genero>(entity =>
            {
                entity.ToTable("Genero");
                entity.HasKey(g => g.GeneroId);
                entity.Property(g => g.GeneroId).ValueGeneratedOnAdd();
                entity.Property(g => g.Nombre).HasMaxLength(100).IsRequired();

                entity.HasMany(g => g.Albums)
                .WithOne(a => a.Genero);

            });
            modelBuilder.Entity<Artista>(entity =>
            {
                entity.ToTable("Artista");
                entity.HasKey(a => a.ArtistaId);
                entity.Property(a => a.ArtistaId).ValueGeneratedOnAdd();
                entity.Property(a => a.Nombre).HasMaxLength(100).IsRequired();

                entity.HasMany(a => a.Albums)
                .WithOne(al => al.Artista);

            });
            modelBuilder.Entity<Album>(entity =>
            {
                entity.ToTable("Album");
                entity.HasKey(a => a.AlbumId);
                entity.Property(a => a.AlbumId).ValueGeneratedOnAdd();
                entity.Property(a => a.Nombre).HasMaxLength(100).IsRequired();

                entity.HasOne(a => a.Artista)
                .WithMany(ar => ar.Albums)
                .HasForeignKey(a => a.ArtistaId);

                entity.HasOne(a => a.Genero)
                .WithMany(g => g.Albums)
                .HasForeignKey(a => a.GeneroId);

            });

            modelBuilder.Entity<Genero>().HasData(
                new Genero
                {
                    GeneroId = 1,
                    Nombre = "Pop"
                },
                new Genero
                {
                    GeneroId = 2,
                    Nombre = "Rock"
                },
                new Genero
                {
                    GeneroId = 3,
                    Nombre = "Electronica"
                },
                new Genero
                {
                    GeneroId = 4,
                    Nombre = "Jazz"
                }

                );
            modelBuilder.Entity<Artista>().HasData(
                new Artista
                {
                    ArtistaId = 1,
                    Nombre = "Michael Jackson"
                },
                new Artista
                {
                    ArtistaId = 2,
                    Nombre = "Bruno Mars"
                },
                new Artista
                {
                    ArtistaId = 3,
                    Nombre = "Si lees esto vas a promocionar Proyecto Software"

                },
                new Artista
                {
                    ArtistaId = 4,
                    Nombre = "Se me acabaron las ganas de buscar mas artistas XD"
                }
                );
            modelBuilder.Entity<Album>().HasData(
                new Album
                {
                    AlbumId = 1,
                    Nombre = "Thriller",
                    ArtistaId = 1,
                    GeneroId = 1,
                },
                new Album
                {
                    AlbumId = 2,
                    Nombre = "24k Magic",
                    ArtistaId = 2,
                    GeneroId = 2,
                },
                new Album
                {
                    AlbumId = 3,
                    Nombre = "Usa tu imaginacion para este",
                    ArtistaId = 3,
                    GeneroId = 3,
                },
                new Album
                {
                    AlbumId = 4,
                    Nombre = "Tambien se me fueron las ganas de buscar albumes",
                    ArtistaId = 4,
                    GeneroId = 4,
                }
                );


        }
    }
}
