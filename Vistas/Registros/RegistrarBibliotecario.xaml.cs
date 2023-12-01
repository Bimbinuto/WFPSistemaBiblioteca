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
using Biblioteca.Vistas;
using Biblioteca.Vistas.Ventanas;
using Biblioteca.ModeloDeVista;
using System.Security.Cryptography;
using System.Windows.Threading;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para RegistrarBibliotecario.xaml
    /// </summary>
    public partial class RegistrarBibliotecario : UserControl
    {
        public ContentControl ContentControl;

        public RegistrarBibliotecario(ContentControl DeAdministracion)
        {
            InitializeComponent();
            ContentControl = DeAdministracion;
            DataContext = new BibliotecarioVM();
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            ContentControl.Content = new Administracion(ContentControl);
        }

        private void btnVolverGestionarBibliotecario(object sender, RoutedEventArgs e)
        {
            ContentControl.Content = new GestionarBibliotecario(ContentControl);
        }

        private void btnRegistrarNuevoB(object sender, RoutedEventArgs e)
        {
            registrarNuevoB.IsEnabled = false;
            DispatcherTimer localTimer = new DispatcherTimer();
            localTimer.Interval = TimeSpan.FromSeconds(2);
            localTimer.Tick += (s, args) =>
            {
                ContentControl.Content = new GestionarBibliotecario(ContentControl);
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
