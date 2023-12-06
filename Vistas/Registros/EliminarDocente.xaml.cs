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
using Biblioteca.ModeloDeVista;
using Biblioteca.Vistas.Ventanas;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para EliminarDocente.xaml
    /// </summary>
    public partial class EliminarDocente : UserControl
    {
        ContentControl contentControl;
        DocenteVM localdvm = new DocenteVM();

        public EliminarDocente(ContentControl ccDeGestionarDocente, DocenteVM dvm)
        {
            InitializeComponent();
            DataContext = dvm;

            contentControl = ccDeGestionarDocente;
            localdvm = dvm;
        }

        private void btnVolverGestionar(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarDocente(contentControl);
        }

        private void btnConfirmarEliminacion(object sender, RoutedEventArgs e)
        {
            btnCancelar.IsEnabled = false;
            btnConfirmar.IsEnabled = false;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            DispatcherTimer timer1 = dispatcherTimer;
            timer1.Interval = TimeSpan.FromSeconds(1.5);
            timer1.Tick += (s, args) =>
            {
                timer1.Stop();
                contentControl.Content = new GestionarDocente(contentControl);
            };
            timer1.Start();
        }
    }
}
