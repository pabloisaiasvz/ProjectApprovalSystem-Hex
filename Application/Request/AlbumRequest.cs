using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request
{
    public class AlbumRequest
    {
        public string Nombre { get; set; } 
        public int ArtistaId { get; set; }
        public int GeneroId { get; set; }
    }
}
