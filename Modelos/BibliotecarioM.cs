using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Modelos
{
    public class BibliotecarioM
    {
        public long? ci { get; set; }
        public string nombres { get; set; }
        public string apPaterno { get; set; }
        public string apMaterno { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public string direccion { get; set; }
        public long? telefono { get; set; }
        public string correo { get; set; }
        public string cuenta { get; set; }
        public string contrasena { get; set; }
        public string sexo { get; set; }
        public DateTime fechaContrato { get; set; }
    }
}
