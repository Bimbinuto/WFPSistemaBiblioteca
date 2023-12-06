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
using Biblioteca.Vistas.Ventanas;
using Biblioteca.ModeloDeVista;
using System.Windows.Threading;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para RegistrarEstudiante.xaml
    /// </summary>
    public partial class RegistrarEstudiante : UserControl
    {
        ContentControl contentControl;

        public RegistrarEstudiante(ContentControl ccDeGestionarEstudiante)
        {
            InitializeComponent();
            contentControl = ccDeGestionarEstudiante;
            DataContext = new EstudianteVM();
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnVolverGestionarEstudiante(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarEstudiante(contentControl);
        }

        private void btnRegistrarNuevoE(object sender, RoutedEventArgs e)
        {
            registrarNuevoE.IsEnabled = false;
            DispatcherTimer localTimer = new DispatcherTimer();
            localTimer.Interval = TimeSpan.FromSeconds(2);
            localTimer.Tick += (s, args) =>
            {
                contentControl.Content = new GestionarEstudiante(contentControl);
                localTimer.Stop();
            };
            localTimer.Start();
        }

        private void DatePicker_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void DatePicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = true;
            }
        }
    }
}
