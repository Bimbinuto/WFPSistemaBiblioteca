using Biblioteca.BDConexion;
using Biblioteca.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Biblioteca.ModeloDeVista
{
    public class DocenteVM : INotifyPropertyChanged
    {
        private DocenteM _docente = new DocenteM();
        public ICommand RegistrarCommand { get; set; }
        public ICommand ModificarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private Timer _timerBusqueda;

        private Conexion conexion = new Conexion();
        public string _resultado;


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
