using Biblioteca.ModeloDeVista;
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
    /// Lógica de interacción para EliminarLibro.xaml
    /// </summary>
    public partial class EliminarLibro : UserControl
    {
        ContentControl contentControl;

        public EliminarLibro(ContentControl ccDeGestionarLibros, LibroVM lvm)
        {
            InitializeComponent();
            contentControl = ccDeGestionarLibros;
            DataContext = lvm;
        }

        private void btnVolverGestionar(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarLibro(contentControl);
        }

        private void btnConfirmarEliminacion_Click(object sender, RoutedEventArgs e)
        {
            btnCancelarEliminacion.IsEnabled = false;
            btnConfirmarEliminacion.IsEnabled = false;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            DispatcherTimer timer1 = dispatcherTimer;
            timer1.Interval = TimeSpan.FromSeconds(3);
            timer1.Tick += (s, args) =>
            {
                timer1.Stop();
                contentControl.Content = new GestionarLibro(contentControl);
            };
            timer1.Start();
        }
    }
}
