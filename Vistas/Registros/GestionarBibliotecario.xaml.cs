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

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para GestionarBibliotecario.xaml
    /// </summary>
    public partial class GestionarBibliotecario : UserControl
    {
        ContentControl contentControl;
        BibliotecarioVM vm = new BibliotecarioVM();

        public GestionarBibliotecario(ContentControl ccDeAdministracion)
        {
            InitializeComponent();
            contentControl = ccDeAdministracion;
            DataContext = vm;
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnEditarBibliotecario(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ModificarBibliotecario(contentControl, vm);
        }

        private void btnEliminarBibliotecario(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new EliminarBibliotecario(contentControl, vm);
        }

        private void btnRegistrarNuevoBibliotecario(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new RegistrarBibliotecario(contentControl);
        }
    }
}
