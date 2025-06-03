using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string Nombre { get; set; }
        
        public Artista Artista { get; set; }
        public int ArtistaId { get; set; }
        public Genero Genero { get; set; }
        public int GeneroId {  get; set; }
    }
}
