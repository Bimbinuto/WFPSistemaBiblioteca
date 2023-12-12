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
using Biblioteca.Vistas.Registros;
using Biblioteca.Modelos;

namespace Biblioteca.Vistas.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Reservaciones.xaml
    /// </summary>
    public partial class Reservaciones : UserControl
    {
        ContentControl contentControl;
        ReservacionesVM rvm = new ReservacionesVM();

        public Reservaciones(ContentControl ccDelMenu)
        {
            InitializeComponent();
            this.contentControl = ccDelMenu;
            DataContext = rvm;
            rvm.CargarTablaAsync();

            if (UsuarioGlobal.GetInstance().TipoUsuarioG == "Bibliotecario")
            {
                TablaReservaciones.IsEnabled = false;
            }
        }

        private void btnReservarClick(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ConfirmarReserva(contentControl, rvm);
        }
    }
}
