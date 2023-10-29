using Biblioteca.BDConexion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.ViewModel
{

    public class BaseDeDatosVM : INotifyPropertyChanged
    {
        private bool _estaConectado = false;
        public event PropertyChangedEventHandler PropertyChanged;
        Conexion cnn;

        public BaseDeDatosVM()
        {
            //cnn = new Conexion();
        }

        public bool EstaConectado
        {
            get { return _estaConectado; }
            set
            {
                if (_estaConectado != value)
                {
                    _estaConectado = value;
                    OnPropertyChanged("EstaConectado");
                }
            }
        }

        public void estadoDeLaConexion()
        {
            cnn = new Conexion();
            EstaConectado = cnn.comprobarConexion();
        }

        protected virtual void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
