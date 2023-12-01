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
    /// Lógica de interacción para ModificarPrestamo.xaml
    /// </summary>
    public partial class ModificarPrestamo : UserControl
    {
        public ContentControl contentControl;

        public ModificarPrestamo(ContentControl ccDePrestamo)
        {
            InitializeComponent();
            DataContext = new PrestamosVM();
            contentControl = ccDePrestamo;
        }

        private void btnVolverPrestamo(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Prestamos(contentControl);
        }
    }
}
