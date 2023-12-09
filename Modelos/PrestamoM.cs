using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Modelos
{
    public class PrestamoM
    {
        public string idEjemplar { get; set; }
        public string idUsuario { get; set; }
        public DateTime fechaPrestamo { get; set; }
        public DateTime fechaDevolucion { get; set; }
        public string tipoPrestamo { get; set; }
    }
}
