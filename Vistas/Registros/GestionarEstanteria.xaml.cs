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
    /// Lógica de interacción para GestionarEstanteria.xaml
    /// </summary>
    public partial class GestionarEstanteria : UserControl
    {
        ContentControl contentControl;
        EstanteriaVM evm = new EstanteriaVM();

        public GestionarEstanteria(ContentControl ccDeAdministracion)
        {
            InitializeComponent();
            this.contentControl = ccDeAdministracion;
            DataContext = evm;
        }

        private void btnEditarEstanteria(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ModificarEstanteria(contentControl, evm);
        }

        private void btnEliminarEstanteria(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new EliminarEstanteria(contentControl, evm);
        }

        private void btnRegistrarNuevoEstanteria(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new RegistrarEstanteria(contentControl);
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }
    }
}
