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
using Biblioteca.Vistas.Registros;
using Biblioteca.Vistas.Ventanas;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para RegistrarNuevoAutor.xaml
    /// </summary>
    public partial class RegistrarNuevoAutor : UserControl
    {
        ContentControl contentControl;
        LibroVM llvm;

        public RegistrarNuevoAutor(ContentControl ccDeAsignarAutor, LibroVM lvm)
        {
            InitializeComponent();
            this.contentControl = ccDeAsignarAutor;
            DataContext = lvm;
            llvm = lvm;
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnVolverGestionarLibros(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarLibro(contentControl);
        }

        private void btnVolverAsginarAutores(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new RegistrarAutor(contentControl, llvm);
        }

        private void btnRegistrarNuevoAutor(object sender, RoutedEventArgs e)
        {

        }
    }
}
