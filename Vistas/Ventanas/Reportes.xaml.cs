
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

namespace Biblioteca.Vistas.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Reporte.xaml
    /// </summary>
    public partial class Reportes : UserControl
    {
        public ReportesVM rvm = new ReportesVM();

        public Reportes()
        {
            InitializeComponent();
            DataContext = rvm;
        }

        private void btnGenerarReporte(object sender, RoutedEventArgs e)
        {
            rvm.GenerarReporte();
        }
    }
}