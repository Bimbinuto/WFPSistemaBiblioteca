using Biblioteca.ModeloDeVista;
using Biblioteca.Vistas.Ventanas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para RegistrarDevolucion.xaml
    /// </summary>
    public partial class RegistrarDevolucion : UserControl
    {
        ContentControl contentControl;

        public RegistrarDevolucion(ContentControl ccDeControl, ControlLibrosVM clvm)
        {
            InitializeComponent();
            contentControl = ccDeControl;
            DataContext = clvm;
        }

        private void btnRegistrarTransaccion(object sender, RoutedEventArgs e)
        {
            registrarTransaccion.IsEnabled = false;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            DispatcherTimer timer1 = dispatcherTimer;
            timer1.Interval = TimeSpan.FromSeconds(1.7);
            timer1.Tick += (s, args) =>
            {
                timer1.Stop();
                contentControl.Content = new ControlLibros(contentControl);
            };
            timer1.Start();
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnVolverReasignacion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ControlLibros(contentControl);
        }
    }
}
