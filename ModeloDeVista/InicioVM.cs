using Biblioteca.BDConexion;
using Biblioteca.Vistas.Ventanas;
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
    public class InicioVM : INotifyPropertyChanged
    {
        private Conexion conexion = new Conexion();
        public event PropertyChangedEventHandler PropertyChanged;

        private DataRowView _filaSeleccionada;
        private string _elementosSeleccionados;
        public DataRowView FilaSeleccionada
        {
            get => _filaSeleccionada;
            set
            {
                _filaSeleccionada = value;
                OnPropertyChanged(nameof(FilaSeleccionada));

                if (value != null)
                {
                    ElementosSeleccionados = $"ID Ejemplar: {value["id_ejemplar"].ToString()}\n" +
                                             $"Titulo: {value["titulo"].ToString()}\n" +
                                             $"Disponibilidad: {value["disponibilidad"].ToString()} \n" +
                                             $"Usuario: {value["NombreUsuario"].ToString()} \n";
                }
                else
                {
                    ElementosSeleccionados = "sin eventos";
                }
            }
        }
        public string ElementosSeleccionados
        {
            get => _elementosSeleccionados;
            set
            {
                _elementosSeleccionados = value;
                OnPropertyChanged(nameof(ElementosSeleccionados));
            }
        }

        private DataTable _inicioT;
        private DateTime _fechaBusqueda;
        public DataTable InicioT
        {
            get => _inicioT;
            set
            {
                _inicioT = value;
                OnPropertyChanged(nameof(InicioT));
            }
        }
        public DateTime FechaBusqueda
        {
            get => _fechaBusqueda;
            set
            {
                _fechaBusqueda = value;
                OnPropertyChanged(nameof(FechaBusqueda));

                CargarTablaLibrosAsync();
            }
        }

        public InicioVM()
        {
            CargarTablaLibrosAsync();
        }

        private async void CargarTablaLibrosAsync()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                if (FechaBusqueda == null)
                {
                    FechaBusqueda = DateTime.Now;
                }

                string consulta = "SELECT e.id_ejemplar, l.titulo, e.disponibilidad, MAX(p.id_prestamo) AS UltimoPrestamo, CONCAT(MAX(u.nombres), ' ', MAX(u.ap_paterno), ' ', MAX(u.ap_materno)) AS NombreUsuario\r\nFROM ejemplar e\r\nJOIN libro l ON e.id_libro = l.id_libro\r\nJOIN transaccion t ON e.id_ejemplar = t.id_ejemplar\r\nJOIN usuario u ON t.id_usuario = u.id_usuario\r\nJOIN prestamo p ON t.id_transaccion = p.id_transaccion\r\n" +
                                  "WHERE e.disponibilidad IN ('prestado', 'reservado') AND p.fecha_max_devolucion = @FECHA\r\n" +
                                  "GROUP BY e.id_ejemplar, l.titulo, e.disponibilidad; ";

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@FECHA", $"{FechaBusqueda.ToString("yyyy-MM-dd")}");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                InicioT = dt;
                await cnx.CloseAsync();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
