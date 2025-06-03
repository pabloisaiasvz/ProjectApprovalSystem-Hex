using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Artista
    {
        public int ArtistaId {  get; set; }
        public string Nombre { get; set; }

        public List<Album>Albums { get; set; }
    }
}
