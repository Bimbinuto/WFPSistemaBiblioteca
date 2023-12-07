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
    /// Lógica de interacción para ObservarEjemplar.xaml
    /// </summary>
    public partial class ObservarEjemplar : UserControl
    {
        ContentControl contentControl;
        LibroVM llvm;

        public ObservarEjemplar(ContentControl contentControl, LibroVM lvm)
        {
            InitializeComponent();
            this.contentControl = contentControl;
            DataContext = lvm;
            llvm = lvm;
            llvm.CargarListaEjemplaresAsync();
        }

        public void RefrescarTabla()
        {
            TablaEjemplares.Items.Refresh();
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnVolverGestionarLibros(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarLibro(contentControl);
        }

        private void btnEditarEjemplar(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ModificarEjemplar(contentControl, llvm);
        }

        private void btnEliminarEjemplar(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new EliminarEjemplar(contentControl, llvm);
        }

        private void btnRefrescar(object sender, RoutedEventArgs e)
        {
            llvm.CargarListaEjemplaresAsync();
        }
    }
}
