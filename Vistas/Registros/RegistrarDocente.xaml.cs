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
using Biblioteca.ModeloDeVista;
using Biblioteca.Vistas.Ventanas;
using Biblioteca.Vistas.Registros;
using System.Windows.Threading;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para RegistrarDocente.xaml
    /// </summary>
    public partial class RegistrarDocente : UserControl
    {
        ContentControl contentControl;

        public RegistrarDocente(ContentControl ccDeGestionarDocente)
        {
            InitializeComponent();
            contentControl = ccDeGestionarDocente;
            DataContext = new DocenteVM();
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnVolverGestionarDocente(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarDocente(contentControl);
        }

        private void btnRegistrarNuevoD(object sender, RoutedEventArgs e)
        {
            registrarNuevoD.IsEnabled = false;
            DispatcherTimer localTimer = new DispatcherTimer();
            localTimer.Interval = TimeSpan.FromSeconds(2);
            localTimer.Tick += (s, args) =>
            {
                contentControl.Content = new GestionarDocente(contentControl);
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
