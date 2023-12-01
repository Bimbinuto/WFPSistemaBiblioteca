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

namespace Biblioteca.Vistas.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Prestamos.xaml
    /// </summary>
    public partial class Prestamos : UserControl
    {
        public ContentControl ContentControl;

        public Prestamos(ContentControl ccDelMenu)
        {
            InitializeComponent();
            DataContext = new PrestamosVM();
            ContentControl = ccDelMenu;
        }

        private void btnEditarClick(object sender, RoutedEventArgs e)
        {
            ContentControl.Content = new ModificarPrestamo(ContentControl);
        }

        private void btnEliminarClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrestarClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
