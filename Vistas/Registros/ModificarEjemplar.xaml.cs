using System;
using System.Collections.Generic;
using System.Data;
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
using Biblioteca.ModeloDeVista;
using Biblioteca.Vistas.Ventanas;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para ModificarEjemplar.xaml
    /// </summary>
    public partial class ModificarEjemplar : UserControl
    {
        ContentControl contentControl;
        LibroVM llvm;

        public ModificarEjemplar(ContentControl ccDeVerEjemplares, LibroVM lvm)
        {
            InitializeComponent();
            contentControl = ccDeVerEjemplares;
            DataContext = lvm;
            llvm = lvm;
        }

        private void btnVolverVerEjemplares(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ObservarEjemplar(contentControl, llvm);
        }

        private void btnVolverGestionarLibros(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new GestionarLibro(contentControl);
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnModificarEjemplar(object sender, RoutedEventArgs e)
        {
            ModificarEjemplarActual.IsEnabled = false;
            ModificarEjemplarActual.IsEnabled = false;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            DispatcherTimer timer1 = dispatcherTimer;
            timer1.Interval = TimeSpan.FromSeconds(1.7);
            timer1.Tick += (s, args) =>
            {
                timer1.Stop();
                contentControl.Content = new ObservarEjemplar(contentControl, this.llvm);
            };
            timer1.Start();
        }

        private void DatePicker_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }
        private void DatePicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = true;
            }
        }
    }
}
