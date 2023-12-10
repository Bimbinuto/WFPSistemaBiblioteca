using Biblioteca.BDConexion;
using Biblioteca.ViewModel;
using Biblioteca.Vistas.Ventanas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Biblioteca.Vistas;
using Biblioteca.Vistas.Registros;
using System.Management.Instrumentation;
using Biblioteca.Efectos;
using Biblioteca.ModeloDeVista;
using Biblioteca.Modelos;

/*TODO se esta usando Mysql.Data Framework (el que pesa menos)*/

namespace Biblioteca.Ventanas
{
    public partial class Menu : Window
    {
        // esperar a usar despues
        //LoginVM log;

        private DispatcherTimer timer;

        //Colocar despues el 'LoginVM log' como parametro al constructor OK
        public Menu()
        {
            BaseDeDatosVM bvm = new BaseDeDatosVM();
            InitializeComponent();
            ccPrincipal.Content = new Inicio();

            //ccPrincipal.Content = new Administracion(ccPrincipal);
            startclock();
            //this.log = log;
            //movableGrid = bvm.EstaConectado;

            //se activa con el LoginVM log;
            //txtUsuarioGlobal.DataContext = log;
            //txtTipoUsuarioGlobal.DataContext = log;

            txtUsuarioGlobal.Content = UsuarioGlobal.GetInstance().NombreUsuarioG;
            txtTipoUsuarioGlobal.Content = UsuarioGlobal.GetInstance().TipoUsuarioG;

            if (UsuarioGlobal.GetInstance().TipoUsuarioG == "Estudiante")
            {
                rbMenuAdministracion.Visibility = Visibility.Collapsed;
                rbMenuReservaciones.Visibility = Visibility.Collapsed;
                rbMenuReportes.Visibility = Visibility.Collapsed;
                rbMenuEstadisticas.Visibility = Visibility.Collapsed;
                rbMenuNotas.Visibility = Visibility.Collapsed;
            }
            else if (UsuarioGlobal.GetInstance().TipoUsuarioG == "Docente")
            {
                rbMenuAdministracion.Visibility = Visibility.Collapsed;
                rbMenuReportes.Visibility = Visibility.Collapsed;
                rbMenuEstadisticas.Visibility = Visibility.Collapsed;
                rbMenuNotas.Visibility = Visibility.Collapsed;
            }
        }

        #region [base de datos y tiempo]eventos

        private void startclock()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += tickevent;
            timer.Start();
        }

        private void tickevent(object sender, EventArgs e)
        {
            lbTimer.Content = DateTime.Now.ToString(@"HH:mm");
        }

        #endregion eventos

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        #region [Test] movimiento de la ventana respecto a la barra superior
        //movable Grid

        //MouseLeftButtonDown="Ventana_MouseLeftButtonDown"
        //        MouseLeftButtonUp="Ventana_MouseLeftButtonUp"
        //        MouseMove="Ventana_MouseMove"

        private bool isDragging = false;
        private Point anchorPoint;

        private void Ventana_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(this);
            if (mousePosition.X < movableGrid.ActualWidth)
            {
                isDragging = true;
                anchorPoint = mousePosition;
                this.Cursor = Cursors.SizeAll;
            }
        }

        private void Ventana_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Ventana_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var currentPoint = e.GetPosition(this);
                var differenceX = currentPoint.X - anchorPoint.X;
                var differenceY = currentPoint.Y - anchorPoint.Y;
                this.Left += differenceX;
                this.Top += differenceY;
            }
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new Efectos.WindowBlureffect(this, Efectos.AccentState.ACCENT_ENABLE_BLURBEHIND) { BlurOpacity = 100 };
        }

        private void btnInicio(object sender, RoutedEventArgs e)
        {
            ccPrincipal.Content = new Inicio();
        }

        private void btnAdministracion(object sender, RoutedEventArgs e)
        {
            ccPrincipal.Content = new Administracion(ccPrincipal);
        }

        private void btnPrestamos(object sender, RoutedEventArgs e)
        {
            ccPrincipal.Content = new Prestamos(ccPrincipal);
        }

        private void btnReporte(object sender, RoutedEventArgs e)
        {
            ccPrincipal.Content = new Reportes();
        }

        private void btnEstadisticas(object sender, RoutedEventArgs e)
        {
            ccPrincipal.Content = new Estadisticas();
        }

        private void btnAcercaDe(object sender, RoutedEventArgs e)
        {
            ccPrincipal.Content = new AcercaSoftware();
        }

        private void btnMenuClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnConfiguracion(object sendere, RoutedEventArgs e)
        {
            ccPrincipal.Content = new UserControlTest();
        }

        private void btnNotasDesarrollo(object sender, RoutedEventArgs e)
        {
            ccPrincipal.Content = new NotasDeDesarrollo();
        }

        private void btnCerrarSesion(object sender, RoutedEventArgs e)
        {
            Login nuevo = new Login();
            nuevo.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

    }
}
