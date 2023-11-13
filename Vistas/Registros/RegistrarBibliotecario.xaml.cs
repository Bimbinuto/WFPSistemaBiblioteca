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
using Biblioteca.Vistas;
using Biblioteca.Vistas.Ventanas;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para RegistrarBibliotecario.xaml
    /// </summary>
    public partial class RegistrarBibliotecario : UserControl
    {
        public ContentControl ContentControl;

        public RegistrarBibliotecario(ContentControl DeAdministracion)
        {
            InitializeComponent();
            ContentControl = DeAdministracion;
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            ContentControl.Content = new Administracion(ContentControl);
        }
    }
}
