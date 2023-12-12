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
using Biblioteca.Vistas.Ventanas;
using System.Windows.Threading;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para ConfirmarReserva.xaml
    /// </summary>
    public partial class ConfirmarReserva : UserControl
    {
        ContentControl contentControl;
        ReservacionesVM lrvm;

        public ConfirmarReserva(ContentControl ccDeReservacion, ReservacionesVM rvm)
        {
            InitializeComponent();
            this.contentControl = ccDeReservacion;
            DataContext = rvm;
            lrvm = rvm;

        }

        private void btnVolverReservas(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Reservaciones(contentControl);
        }

        private void btnConfirmarPrestamo(object sender, RoutedEventArgs e)
        {
            CancelarReserva.IsEnabled = false;
            ConfirmarReservaLector.IsEnabled = false;
            DispatcherTimer localTimer = new DispatcherTimer();
            localTimer.Interval = TimeSpan.FromSeconds(3);
            localTimer.Tick += (s, args) =>
            {
                contentControl.Content = new Reservaciones(contentControl);
                localTimer.Stop();
            };
            localTimer.Start();
        }
    }
}
