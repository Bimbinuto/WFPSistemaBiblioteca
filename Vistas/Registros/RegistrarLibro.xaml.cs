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

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para RegistrarLibro.xaml
    /// </summary>
    public partial class RegistrarLibro : UserControl
    {
        ContentControl contentControl;

        public RegistrarLibro(ContentControl ccDeGestionaLibro)
        {
            InitializeComponent();
            contentControl = ccDeGestionaLibro;
            DataContext = new LibroVM();
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnVolverGestionarLibros(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarLibro(contentControl);
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

        private void btnRegistrarNuevoL(object sender, RoutedEventArgs e)
        {

        }
    }
}
