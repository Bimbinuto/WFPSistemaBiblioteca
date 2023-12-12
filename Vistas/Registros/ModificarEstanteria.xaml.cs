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
    /// Lógica de interacción para ModificarEstanteria.xaml
    /// </summary>
    public partial class ModificarEstanteria : UserControl
    {
        ContentControl contentControl;


        public ModificarEstanteria(ContentControl ccDeGestionarEstanteria, EstanteriaVM evm)
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

        private void btnModificarEstanteria(object sender, RoutedEventArgs e)
        {
            ModificarEstanteriaSelecta.IsEnabled = false;

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
