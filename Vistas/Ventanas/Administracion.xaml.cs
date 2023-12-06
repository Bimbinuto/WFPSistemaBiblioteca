using Biblioteca.Vistas.Registros;
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

namespace Biblioteca.Vistas.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Administracion.xaml
    /// </summary>
    public partial class Administracion : UserControl
    {
        public ContentControl ContentControl;

        public Administracion(ContentControl ccDelMenu)
        {
            InitializeComponent();
            ContentControl = ccDelMenu;
        }

        private void btnAbrirRegistrarBibliotecario(object sender, RoutedEventArgs e)
        {
            ContentControl.Content = new GestionarBibliotecario(ContentControl);
        }

        private void btnGestionarDocente(object sender, RoutedEventArgs e)
        {
            ContentControl.Content = new GestionarDocente(ContentControl);
        }

        private void btnGestionarEstudiante(object sender, RoutedEventArgs e)
        {
            ContentControl.Content = new GestionarEstudiante(ContentControl);
        }

        private void btnGestionarLibros(object sender, RoutedEventArgs e)
        {
            ContentControl.Content = new GestionarLibro(ContentControl);
        }
    }
}
