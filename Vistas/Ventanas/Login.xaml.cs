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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Biblioteca.ModeloDeVista;
using Biblioteca.Modelos;

namespace Biblioteca.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>

    public partial class Login : Window
    {
        LoginVM logVM;

        public Login()
        {
            InitializeComponent();

            logVM = new LoginVM(this);
            string entero = logVM.IDUsuario;


            txtUsername.DataContext = logVM;
            txtPassword.DataContext = logVM;
            btnAcceder.DataContext = logVM;
            txtBienvenido.DataContext = logVM;
            txtTipoUsuario.DataContext = logVM;
            lbResultado.DataContext = logVM;
            //lbResultado.Content = "";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new Efectos.WindowBlureffect(this, Efectos.AccentState.ACCENT_ENABLE_BLURBEHIND) { BlurOpacity = 100 };
        }
        private void BtnCerrarLogin(object sender, EventArgs e) => this.Close();
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            watermarkTextBlock.Visibility = string.IsNullOrEmpty(txtPassword.Password) ? Visibility.Visible : Visibility.Collapsed;

            if (((PasswordBox)sender).DataContext != null)
            {
                ((dynamic)((PasswordBox)sender).DataContext).Password = ((PasswordBox)sender).Password;
            }

        }

        public void Comenzar()
        {
            Storyboard sb = this.FindResource("animacionLogin") as Storyboard;
            if (sb != null)
            {
                sb.Begin();
            }
        }

        // TODO arreglar el error del boton que acceder que cuando se pulsa varias veces, repite la animacion varias veces y crear varios Menus
        public void btnLogin(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show($"ID: {UsuarioGlobal.GetInstance().IdUsuario}\n Nombre: {UsuarioGlobal.GetInstance().NombreUsuarioG}\n Tipo: {UsuarioGlobal.GetInstance().TipoUsuarioG}");

            string resultado = logVM.Resultado.ToString();

            if (resultado == "OK")
            {
                lbResultado.Foreground = new SolidColorBrush(Colors.Green);
                btnAcceder.IsEnabled = false;
            }
        }
    }
}
