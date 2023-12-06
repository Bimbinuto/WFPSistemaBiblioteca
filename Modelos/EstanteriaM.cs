using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Modelos
{
    public class EstanteriaM
    {
        public int id_estanteria { get; set; }
        public string codigo_ubicacion { get; set; }
        public int capacidad { get; set; }
        public int capacidad_original { get; set; }
    }
}
