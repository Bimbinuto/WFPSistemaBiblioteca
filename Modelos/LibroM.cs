using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Modelos
{
    public class LibroM
    {
        public string isbn { get; set; } //
        public string titulo { get; set; }
        public DateTime fecha_edicion { get; set; }
        public string editorial { get; set; }
        public string id_proveedor { get; set; }
        public string id_categoria { get; set; }
        public string id_materia { get; set; }
        public DateTime fecha_disposicion { get; set; }
        public string disponibilidad { get; set; }
        public string tipo_ejemplar { get; set; }
        public string estado { get; set; }
        public string descripcion { get; set; }
        public string id_estanteria { get; set; }
    }
}
