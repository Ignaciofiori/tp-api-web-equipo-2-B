using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modelo
{
    public class Imagen
    {
        public int Id { get; set; }
        public int IdArticulo { get; set; } //se relaciona con Articulos
        public string ImagenURL { get; set; } 
    }
}
