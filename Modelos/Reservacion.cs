using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Biblioteca.Modelos
{
    public class Reservacion
    {
        public string idEjemplar { get; set; }
        public string idUsuario { get; set; }
        public DateTime fechaReserva { get; set; }
    }
}
