using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Biblioteca.BDConexion;


namespace Biblioteca.ModeloDeVista
{
    public class EstadisticasVM : INotifyPropertyChanged
    {
        private Conexion _conexion = new Conexion();
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
