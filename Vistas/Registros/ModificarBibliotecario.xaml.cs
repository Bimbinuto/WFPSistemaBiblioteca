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
    /// Lógica de interacción para ModificarBibliotecario.xaml
    /// </summary>
    public partial class ModificarBibliotecario : UserControl
    {
        ContentControl contentControl;

        public ModificarBibliotecario(ContentControl ccDeGestionar, BibliotecarioVM isnBvm)
        {
            InitializeComponent();
            contentControl = ccDeGestionar;
            DataContext = isnBvm;
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnVolverGestionarBibliotecario(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarBibliotecario(contentControl);
        }

        private void btnModificarBibliotecario(object sender, RoutedEventArgs e)
        {
            botonModificar.IsEnabled = false;
            DispatcherTimer localTimer = new DispatcherTimer();
            localTimer.Interval = TimeSpan.FromSeconds(2);
            localTimer.Tick += (s, args) =>
            {
                contentControl.Content = new GestionarBibliotecario(contentControl);
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
