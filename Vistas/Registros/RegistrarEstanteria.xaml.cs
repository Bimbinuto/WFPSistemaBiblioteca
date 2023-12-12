using Biblioteca.ModeloDeVista;
using Biblioteca.Vistas.Ventanas;
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
using System.Windows.Threading;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para RegistrarEstanteria.xaml
    /// </summary>
    public partial class RegistrarEstanteria : UserControl
    {
        ContentControl contentControl;
        EstanteriaVM evm = new EstanteriaVM();

        public RegistrarEstanteria(ContentControl ccDeGestionarEstanteria)
        {
            InitializeComponent();
            this.contentControl = ccDeGestionarEstanteria;
            DataContext = evm;
        }

        private void btnVolverGestionarLibros(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarEstanteria(contentControl);
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnRegistrarNuevaEstanteria(object sender, RoutedEventArgs e)
        {
            registrarNuevaEstanteria.IsEnabled = false;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            DispatcherTimer timer1 = dispatcherTimer;
            timer1.Interval = TimeSpan.FromSeconds(1.5);
            timer1.Tick += (s, args) =>
            {
                timer1.Stop();
                contentControl.Content = new GestionarEstanteria(contentControl);
            };
            timer1.Start();
        }
    }
}
