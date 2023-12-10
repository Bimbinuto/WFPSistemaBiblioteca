using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Biblioteca.BDConexion;
using Biblioteca.ModeloDeVista;

using LiveCharts;
using LiveCharts.Wpf;


namespace Biblioteca.Vistas.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Estadisticas.xaml
    /// </summary>
    public partial class Estadisticas : UserControl
    {
        private Conexion _conexion = new Conexion();
        public ObservableCollection<string> Dias { get; set; }
        public ObservableCollection<int> Cantidades { get; set; }


        public Estadisticas()
        {
            InitializeComponent();

            //PointLabel = chartPoint =>
            //    string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            Dias = new ObservableCollection<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };
            Cantidades = new ObservableCollection<int> { 5, 10, 8, 15, 12 };

            DataContext = this;
        }

        //public Func<ChartPoint, string> PointLabel { get; set; }

        //private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        //{
        //    var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

        //    //clear selected slice.
        //    foreach (PieSeries series in chart.Series)
        //        series.PushOut = 0;

        //    var selectedSeries = (PieSeries)chartpoint.SeriesView;
        //    selectedSeries.PushOut = 8;
        //}
    }
}
