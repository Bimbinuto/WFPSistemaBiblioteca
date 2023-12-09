using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Modelos
{
    public class UsuarioGlobal
    {
        private static UsuarioGlobal instancia;

        //propiedades:
        public string IdUsuario { get; set; }
        public string NombreUsuarioG { get; set; }
        public string TipoUsuarioG { get; set; }

        //constructor privado:
        private UsuarioGlobal() { }

        //Metodo para obtener la instancia de la clase:
        public static UsuarioGlobal GetInstance()
        {
            if (instancia == null)
            {
                instancia = new UsuarioGlobal();
            }
            return instancia;
        }

        // -- Como establecer los datos:
        //UsuarioGlobal.GetInstance().IdUsuario = "1";
        //UsuarioGlobal.GetInstance().NombreUsuarioG = "nombreDeUsuario";
        //UsuarioGlobal.GetInstance().TipoUsuarioG = "tipoDeUsuario";

        // -- Como obtener los datos:
        //string idUsuario = UsuarioGlobal.GetInstance().IdUsuario;
        //string nombreUsuario = UsuarioGlobal.GetInstance().NombreUsuarioG;
        //string tipoUsuario = UsuarioGlobal.GetInstance().TipoUsuarioG;


    }
}
