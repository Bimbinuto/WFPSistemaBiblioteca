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

namespace Biblioteca.ModeloDeVista
{
    public class HistorialVM : INotifyPropertyChanged
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

                ElementosSeleccionados = $"ID Ejemplar: {value["id_ejemplar"].ToString()}\n" +
                                         $"Titulo: {value["titulo"].ToString()}\n" +
                                         $"Disponibilidad: {value["disponibilidad"].ToString()} \n" +
                                         $"Usuario: {value["NombreUsuario"].ToString()} \n";
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
        public DataTable InicioT
        {
            get => _inicioT;
            set
            {
                _inicioT = value;
                OnPropertyChanged(nameof(InicioT));
            }
        }

        public HistorialVM()
        {
            CargarTablaHistorialAsync();
        }

        private async void CargarTablaHistorialAsync()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT usuario.nombres, libro.titulo, prestamo.fecha_prestamo, prestamo.fecha_max_devolucion, prestamo.tipo_prestamo\r\nFROM prestamo\r\nJOIN transaccion ON prestamo.id_transaccion = transaccion.id_transaccion\r\nJOIN usuario ON transaccion.id_usuario = usuario.id_usuario\r\nJOIN ejemplar ON transaccion.id_ejemplar = ejemplar.id_ejemplar\r\nJOIN libro ON ejemplar.id_libro = libro.id_libro\r\n" +
                                  "WHERE usuario.id_usuario = @ID; ";

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@ID", $"{UsuarioGlobal.GetInstance().IdUsuario.ToString()}");

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
