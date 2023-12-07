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
    /// Lógica de interacción para EliminarEjemplar.xaml
    /// </summary>
    public partial class EliminarEjemplar : UserControl
    {
        ContentControl contentControl;
        LibroVM llvm;

        public EliminarEjemplar(ContentControl ccDeVerEjemplar, LibroVM lvm)
        {
            InitializeComponent();
            contentControl = ccDeVerEjemplar;
            DataContext = lvm;
            llvm = lvm;
        }

        private void btnVolverGestionar(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ObservarEjemplar(contentControl, llvm);
        }

        private void btnConfirmarEliminacionEjemplar_Click(object sender, RoutedEventArgs e)
        {
            btnCancelarEliminacionEjemplar.IsEnabled = false;
            btnConfirmarEliminacionEjemplar.IsEnabled = false;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            DispatcherTimer timer1 = dispatcherTimer;
            timer1.Interval = TimeSpan.FromSeconds(3);
            timer1.Tick += (s, args) =>
            {
                timer1.Stop();
                contentControl.Content = new ObservarEjemplar(contentControl, llvm);
                llvm.ResultadoEliminacionEjemplar = "";
            };
            timer1.Start();
        }
    }
}
