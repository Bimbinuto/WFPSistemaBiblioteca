using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
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
using MySql.Data.MySqlClient;

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

        //public EstadisticasVM evm = new EstadisticasVM();

        public Estadisticas()
        {
            InitializeComponent();

            LoadDataAsync();


            PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            CantidadPrestamos();
            CantidadPrestamosEsteMes();
            CantidadGeneral();
            CantidadEsteMes();
            DataContext = this;
        }

        private async void LoadDataAsync()
        {
            var results = await GetPrestamosPorMes();

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "2023",
                    Values = new ChartValues<double>(results.Select(r => r.Item2))
                }
            };

            Labels = results.Select(r => r.Item1).ToArray();
            Formatter = value => value.ToString("N");

        }

        public Func<ChartPoint, string> PointLabel { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public async void CantidadPrestamos()
        {
            string consulta = "SELECT usuario.sexo, COUNT(*) as num_prestamos FROM prestamo JOIN transaccion ON prestamo.id_transaccion = transaccion.id_transaccion JOIN usuario ON transaccion.id_usuario = usuario.id_usuario GROUP BY usuario.sexo;";

            using (MySqlConnection cnx = new MySqlConnection(_conexion.cadenaConexion))
            {
                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                await cnx.OpenAsync();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["sexo"].ToString() == "masculino")
                    {
                        PieSeries series = new PieSeries
                        {
                            Title = "Varones",
                            Values = new ChartValues<double> { Convert.ToDouble(reader["num_prestamos"]) },
                            DataLabels = true,
                            LabelPoint = PointLabel
                        };
                        GraficoPastel.Series.Add(series);
                    }
                    else if (reader["sexo"].ToString() == "femenino")
                    {
                        PieSeries series = new PieSeries
                        {
                            Title = "Mujeres",
                            Values = new ChartValues<double> { Convert.ToDouble(reader["num_prestamos"]) },
                            DataLabels = true,
                            LabelPoint = PointLabel
                        };
                        GraficoPastel.Series.Add(series);
                    }
                }

                reader.Close();
                await cnx.CloseAsync();
            }
        }
        public async void CantidadPrestamosEsteMes()
        {
            string consulta = "\r\nSELECT usuario.sexo as sexo, COUNT(*) AS TotalPrestamosEsteMes\r\nFROM prestamo\r\nJOIN transaccion ON prestamo.id_transaccion = transaccion.id_transaccion\r\nJOIN usuario ON transaccion.id_usuario = usuario.id_usuario\r\nWHERE MONTH(prestamo.fecha_prestamo) = MONTH(CURRENT_DATE())\r\nAND YEAR(prestamo.fecha_prestamo) = YEAR(CURRENT_DATE())\r\nGROUP BY usuario.sexo;";

            using (MySqlConnection cnx = new MySqlConnection(_conexion.cadenaConexion))
            {
                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                await cnx.OpenAsync();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["sexo"].ToString() == "masculino")
                    {
                        PieSeries series = new PieSeries
                        {
                            Title = "Varones",
                            Values = new ChartValues<double> { Convert.ToDouble(reader["TotalPrestamosEsteMes"]) },
                            DataLabels = true,
                            LabelPoint = PointLabel
                        };
                        GraficoEsteMes.Series.Add(series);
                    }
                    else if (reader["sexo"].ToString() == "femenino")
                    {
                        PieSeries series = new PieSeries
                        {
                            Title = "Mujeres",
                            Values = new ChartValues<double> { Convert.ToDouble(reader["TotalPrestamosEsteMes"]) },
                            DataLabels = true,
                            LabelPoint = PointLabel
                        };
                        GraficoEsteMes.Series.Add(series);
                    }
                }

                reader.Close();
                await cnx.CloseAsync();
            }
        }
        public async void CantidadEsteMes()
        {
            string consulta = "SELECT COUNT(*) AS TotalPrestamosEsteMes\r\nFROM prestamo\r\nWHERE MONTH(fecha_prestamo) = MONTH(CURRENT_DATE())\r\nAND YEAR(fecha_prestamo) = YEAR(CURRENT_DATE());";
            string total = "";

            using (MySqlConnection cnx = new MySqlConnection(_conexion.cadenaConexion))
            {
                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                await cnx.OpenAsync();

                MySqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

                total = reader["TotalPrestamosEsteMes"].ToString();

                lbTotalEsteMes.Content = total.ToString();

                reader.Close();
                await cnx.CloseAsync();
            }
        }
        public async void CantidadGeneral()
        {
            string consulta = "SELECT COUNT(*) AS TotalGeneral\r\nFROM prestamo;";
            string total = "";

            using (MySqlConnection cnx = new MySqlConnection(_conexion.cadenaConexion))
            {
                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                await cnx.OpenAsync();

                MySqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

                total = reader["TotalGeneral"].ToString();

                lbTotalGeneral.Content = total.ToString();

                reader.Close();
                await cnx.CloseAsync();
            }
        }

        private async Task<List<Tuple<string, double>>> GetPrestamosPorMes()
        {
            var results = new List<Tuple<string, double>>();

            using (var connection = new MySqlConnection(_conexion.cadenaConexion))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"
                    SELECT MONTH(fecha_prestamo) AS Mes, COUNT(*) AS TotalPrestamos
                    FROM prestamo
                    WHERE YEAR(fecha_prestamo) = YEAR(CURRENT_DATE())
                    GROUP BY Mes
                    ORDER BY Mes;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var mes = reader.GetInt32(0);
                            var totalPrestamos = reader.GetInt32(1);

                            results.Add(Tuple.Create(mes.ToString(), (double)totalPrestamos));
                        }
                    }
                }
            }

            return results;
        }


        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }
    }
}
