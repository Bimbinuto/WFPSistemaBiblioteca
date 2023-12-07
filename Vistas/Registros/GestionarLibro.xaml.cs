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
    /// Lógica de interacción para GestionarLibro.xaml
    /// </summary>
    public partial class GestionarLibro : UserControl
    {
        ContentControl contentControl;
        LibroVM lvm = new LibroVM();

        public GestionarLibro(ContentControl ccDeAdministracion)
        {
            InitializeComponent();
            contentControl = ccDeAdministracion;
            DataContext = lvm;
        }

        private void btnEditarLibro(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ObservarEjemplar(contentControl, lvm);
        }

        private void btnEliminarLibro(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new EliminarLibro(contentControl, lvm);
        }

        private void btnRegistrarEjemplar(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new RegistrarEjemplar(contentControl, lvm);
        }

        private void btnRegistrarNuevoLibro(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new RegistrarLibro(contentControl);
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }
    }
}
