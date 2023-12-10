
using Biblioteca.ModeloDeVista;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CefSharp;
using CefSharp.Wpf;
using iTextSharp.text.pdf.parser;
using System.Windows.Threading;

namespace Biblioteca.Vistas.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Reporte.xaml
    /// </summary>
    public partial class Reportes : System.Windows.Controls.UserControl
    {
        public ReportesVM rvm = new ReportesVM();

        public Reportes()
        {
            InitializeComponent();

            DataContext = rvm;

            if (!Cef.IsInitialized)
            {
                Cef.Initialize(new CefSettings());
            }

            string path = @"C:\Users\HP\source\repos\Biblioteca\Resources\plantilla.html";

            Navegador.BrowserSettings.BackgroundColor = (uint)System.Drawing.Color.White.ToArgb();
            // Carga el archivo HTML local
            Navegador.Address = path;
        }

        private async void btnGenerarReporte(object sender, RoutedEventArgs e)
        {
            bool espera = await rvm.GenerarReporte();

            if (espera)
            {
                DispatcherTimer dispatcherTimer = new DispatcherTimer();
                DispatcherTimer timer1 = dispatcherTimer;
                timer1.Interval = TimeSpan.FromSeconds(3);
                timer1.Tick += (s, args) =>
                {
                    timer1.Stop();
                    lbGeneracionCorrecta.Content = "";
                };
                timer1.Start();
            }


        }
    }
}