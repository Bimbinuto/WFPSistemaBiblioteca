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
    /// Lógica de interacción para ModificarEstudiante.xaml
    /// </summary>
    public partial class ModificarEstudiante : UserControl
    {
        ContentControl contentControl;

        public ModificarEstudiante(ContentControl ccDeGestionarEstudiante, EstudianteVM cevm)
        {
            InitializeComponent();
            contentControl = ccDeGestionarEstudiante;
            DataContext = cevm;
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
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

        private void btnModificarE(object sender, RoutedEventArgs e)
        {
            botonModificarE.IsEnabled = false;
            DispatcherTimer localTimer = new DispatcherTimer();
            localTimer.Interval = TimeSpan.FromSeconds(2);
            localTimer.Tick += (s, args) =>
            {
                contentControl.Content = new GestionarEstudiante(contentControl);
                localTimer.Stop();
            };
            localTimer.Start();
        }

        private void btnVolverGestionarEstudiante(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarEstudiante(contentControl);
        }
    }
}
