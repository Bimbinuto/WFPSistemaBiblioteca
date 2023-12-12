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
using Biblioteca.Vistas.Registros;
using Biblioteca.ModeloDeVista;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para ControlLibros.xaml
    /// </summary>
    public partial class ControlLibros : UserControl
    {
        ContentControl contentControl;
        ControlLibrosVM clvm = new ControlLibrosVM();

        public ControlLibros(ContentControl ccDeAdministracion)
        {
            InitializeComponent();
            this.contentControl = ccDeAdministracion;
            DataContext = clvm;
        }

        private void btnEditarDisponiblidad(object sender, RoutedEventArgs e)
        {
            this.contentControl.Content = new RegistrarDevolucion(contentControl, clvm);
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            this.contentControl.Content = new Administracion(contentControl);
        }
    }
}
