using Biblioteca.BDConexion;
using Biblioteca.ViewModel;
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

/*TODO se esta usando Mysql.Data Framework (el que pesa menos)*/

namespace Biblioteca.Ventanas
{
    public partial class Menu : Window
    {
        private DispatcherTimer timer;
        public Menu()
        {
            InitializeComponent();
            frPagePrincipal.Content = new PageInicio();
            startclock();
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
            frPagePrincipal.Content = new PageInicio();
        }

        private void btnAdministracion(object sender, RoutedEventArgs e)
        {
            frPagePrincipal.Content = new PageAdministracion();
        }


        private void btnPrestamosYReservaciones(object sender, RoutedEventArgs e)
        {
            frPagePrincipal.Content = new PrestamosYReservaciones();

        }

        private void btnAcercaDe(object sender, RoutedEventArgs e)
        {
            frPagePrincipal.Content = new PageAcerca();
        }

        private void btnMenuClose(object sender, RoutedEventArgs e)
        {
            //Login nuevo = new Login();
            //nuevo.Show();
            this.Close();
        }

    }
}
