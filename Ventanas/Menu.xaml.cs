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
    /// Lógica de interacción para Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public Menu(Login login)
        {
            InitializeComponent();
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

        private void btnInicio(object sender, RoutedEventArgs e)
        {
            frPageInicio.Content = new PageInicio();
        }

        private void btnAdministracion(object sender, RoutedEventArgs e)
        {
            frPageInicio.Content = new PageAdministracion();
        }

        private void btnMenuClose(object sender, RoutedEventArgs e)
        {
            Login nuevo = new Login();
            nuevo.Show();
            this.Close();
        }

        private void frPageInicio_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }
    }
}
