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
    /// Lógica de interacción para EliminarEstudiante.xaml
    /// </summary>
    public partial class EliminarEstudiante : UserControl
    {
        ContentControl contentControl;
        EstudianteVM localevm;

        public EliminarEstudiante(ContentControl ccDeGestionarEstudiante, EstudianteVM evm)
        {
            InitializeComponent();
            localevm = evm;
            contentControl = ccDeGestionarEstudiante;
            DataContext = evm;
        }

        private void btnConfirmarEliminacion_Click(object sender, RoutedEventArgs e)
        {
            btnCancelarEliminacion.IsEnabled = false;
            btnConfirmarEliminacion.IsEnabled = false;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            DispatcherTimer timer1 = dispatcherTimer;
            timer1.Interval = TimeSpan.FromSeconds(1.5);
            timer1.Tick += (s, args) =>
            {
                timer1.Stop();
                contentControl.Content = new GestionarBibliotecario(contentControl);
            };
            timer1.Start();
        }

        private void btnVolverGestionar(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarEstudiante(contentControl);
        }
    }
}
