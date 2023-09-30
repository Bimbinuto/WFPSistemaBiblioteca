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
using System.Windows.Shapes;

namespace Biblioteca.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>

    public partial class Login : Window
    {
        private int intentos = 0;

        public Login() => InitializeComponent();

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

        private void btnLogin(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUsername.Text) || String.IsNullOrEmpty(txtPassword.Text))
            {
                lbTest.Content = "complete campos";
                intentos++;
                if (intentos >= 3)
                    this.Close();
            }
            else
            {
                if (txtUsername.Text == "@username" && txtPassword.Text == "@password")
                {
                    Menu nuevo = new Menu(this);
                    nuevo.Show();
                    this.Close();
                }
            }
        }
    }
}
