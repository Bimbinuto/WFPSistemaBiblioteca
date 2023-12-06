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
    /// Lógica de interacción para GestionarEstudiante.xaml
    /// </summary>
    public partial class GestionarEstudiante : UserControl
    {
        ContentControl contentControl;
        EstudianteVM evm = new EstudianteVM();

        public GestionarEstudiante(ContentControl ccDeAdministracion)
        {
            InitializeComponent();
            contentControl = ccDeAdministracion;
            DataContext = evm;
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnEditarBibliotecario(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ModificarEstudiante(contentControl, evm);
        }

        private void btnEliminarBibliotecario(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new EliminarEstudiante(contentControl, evm);
        }

        private void btnRegistrarNuevoEstudiante(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new RegistrarEstudiante(contentControl);
        }
    }
}
