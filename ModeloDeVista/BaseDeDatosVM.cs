using Biblioteca.BDConexion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Biblioteca.ViewModel
{

    public class BaseDeDatosVM : INotifyPropertyChanged
    {
        private bool _estaConectado;
        public event PropertyChangedEventHandler PropertyChanged;
        private Conexion cnn;

        public BaseDeDatosVM()
        {
            EstaConectado = false;
            cnn = new Conexion();
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += async (sender, e) => await estadoDeLaConexionAsync();
            timer.Start();
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

        //Prueba continua permanente (Optima)
        public async Task estadoDeLaConexionAsync()
        {
            bool conectado = await cnn.comprobarConexionAsync();
            EstaConectado = conectado ? true : false;
        }

        protected virtual void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
