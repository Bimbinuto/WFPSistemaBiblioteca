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
    /// Lógica de interacción para ConfirmarPrestamo.xaml
    /// </summary>
    public partial class ConfirmarPrestamo : UserControl
    {
        ContentControl contentControl;

        public ConfirmarPrestamo(ContentControl ccDePrestamos, PrestamosVM pvm)
        {
            InitializeComponent();
            DataContext = pvm;
            this.contentControl = ccDePrestamos;

        }

        private void btnConfirmarEliminacionEjemplar_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Prestamos(contentControl);
        }

        private void btnVolverPrestamos(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Prestamos(contentControl);
        }

        private void btnConfirmarPrestamo(object sender, RoutedEventArgs e)
        {
            CancelarPrestamo.IsEnabled = false;
            ConfirmarPrestamoLector.IsEnabled = false;
            DispatcherTimer localTimer = new DispatcherTimer();
            localTimer.Interval = TimeSpan.FromSeconds(2);
            localTimer.Tick += (s, args) =>
            {
                contentControl.Content = new Prestamos(contentControl);
                localTimer.Stop();
            };
            localTimer.Start();
        }
    }
}
