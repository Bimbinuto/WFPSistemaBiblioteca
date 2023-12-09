using Biblioteca.ModeloDeVista;
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

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para RegistrarAutor.xaml
    /// </summary>
    public partial class RegistrarAutor : UserControl
    {
        ContentControl contentControl;
        LibroVM llvm;

        public RegistrarAutor(ContentControl ccDeGestionarLibro, LibroVM lvm)
        {
            InitializeComponent();
            contentControl = ccDeGestionarLibro;
            DataContext = lvm;
            llvm = lvm;
            lvm.CargarTablaAutoresAsync();
        }

        private void btnVolverGestionarLibros(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarLibro(contentControl);
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnRegistrarAutor(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new RegistrarNuevoAutor(contentControl, llvm);
        }

        private void btnAsignarAutor(object sender, RoutedEventArgs e)
        {

        }
    }
}
