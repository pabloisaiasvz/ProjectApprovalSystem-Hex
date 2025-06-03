using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class AlbumResponse
    {
        public int AlbumId { get; set; }
        public string Nombre { get; set; }
        public GeneroResponse Genero { get; set; }
        public ArtistaResponse Artista { get; set; }
    }
}
