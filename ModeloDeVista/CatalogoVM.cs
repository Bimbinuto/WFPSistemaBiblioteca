using Biblioteca.BDConexion;
using Biblioteca.Modelos;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Biblioteca.ModeloDeVista
{
    public class CatalogoVM : INotifyPropertyChanged
    {
        private Conexion conexion = new Conexion();
        private Timer _timerBusqueda;
        public event PropertyChangedEventHandler PropertyChanged;

        private DataTable _catalogo;
        public DataTable Catalogo
        {
            get => _catalogo;
            set
            {
                _catalogo = value;
                OnPropertyChanged(nameof(Catalogo));
            }
        }

        private string _filtroL = "Nombres";
        private string _busquedaL;

        public string FiltroL
        {
            get => _filtroL;
            set
            {
                _filtroL = value;
                OnPropertyChanged(nameof(FiltroL));

                CargarTablaLibrosAsync();
            }
        }
        public string BusquedaL
        {
            get => _busquedaL;
            set
            {
                _busquedaL = value;
                OnPropertyChanged(nameof(BusquedaL));
                _timerBusqueda.Stop();
                _timerBusqueda.Start();
            }
        }

        public CatalogoVM()
        {
            _timerBusqueda = new System.Timers.Timer(650);
            _timerBusqueda.Elapsed += (sender, e) => Task.Run(() => CargarTablaLibrosAsync());
            _timerBusqueda.AutoReset = false;

            CargarTablaLibrosAsync();
        }

        private async void CargarTablaLibrosAsync()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT e.codigo AS CodigoEjemplar,\r\nl.titulo AS TituloLibro,\r\nm.nombre AS MateriaLibro,\r\nc.nombre AS CategoriaLibro,\r\nGROUP_CONCAT(DISTINCT a.nombres) AS AutoresLibro,\r\nCOUNT(p.id_prestamo) AS VecesPrestado\r\nFROM ejemplar e\r\nJOIN libro l ON e.id_libro = l.id_libro\r\nJOIN libro_autor la ON l.id_libro = la.id_libro\r\nJOIN autor a ON la.id_autor = a.id_autor\r\nJOIN materia m ON l.id_materia = m.id_materia\r\nJOIN categoria c ON l.id_categoria = c.id_categoria\r\nJOIN transaccion t ON e.id_ejemplar = t.id_ejemplar\r\nJOIN prestamo p ON t.id_transaccion = p.id_transaccion\r\nWHERE l.titulo LIKE @busquedaB\r\n";

                if (FiltroL == "Libro")
                {
                    consulta += "AND (e.tipo_ejemplar = 'original' OR e.tipo_ejemplar = 'copia') \r\n";
                }
                else if (FiltroL == "Tesis")
                {
                    consulta += "AND e.tipo_ejemplar = 'tesis'\r\n ";
                }

                consulta += "GROUP BY e.codigo, l.titulo, m.nombre, c.nombre; ";

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@busquedaB", $"%{BusquedaL}%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                Catalogo = dt;
                await cnx.CloseAsync();
            }
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
