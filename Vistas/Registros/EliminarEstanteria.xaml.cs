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
using System.Windows.Threading;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para EliminarEstanteria.xaml
    /// </summary>
    public partial class EliminarEstanteria : UserControl
    {
        ContentControl contentControl;

        public EliminarEstanteria(ContentControl ccDeGestionarEstanteria, EstanteriaVM evm)
        {
            InitializeComponent();
            this.contentControl = ccDeGestionarEstanteria;
            DataContext = evm;
        }

        private void btnConfirmarEliminacion_Click(object sender, RoutedEventArgs e)
        {
            btnCancelarEliminacion.IsEnabled = false;
            btnConfirmarEliminacion.IsEnabled = false;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            DispatcherTimer timer1 = dispatcherTimer;
            timer1.Interval = TimeSpan.FromSeconds(3);
            timer1.Tick += (s, args) =>
            {
                timer1.Stop();
                contentControl.Content = new GestionarEstanteria(contentControl);
            };
            timer1.Start();
        }

        private void btnVolverGestionar(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarEstanteria(contentControl);
        }
    }
}
