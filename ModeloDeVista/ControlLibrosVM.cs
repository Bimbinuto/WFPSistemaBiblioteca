using Biblioteca.BDConexion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Timers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Vistas.Ventanas;
using MySql.Data.MySqlClient;
using System.Windows.Input;
using Biblioteca.Modelos;
using System.Windows;

namespace Biblioteca.ModeloDeVista
{
    public class ControlLibrosVM : INotifyPropertyChanged
    {
        private Conexion _conexion = new Conexion();
        private Timer _timerBusqueda;
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand RegistrarTransaccionCommand { get; set; }

        private bool estadoCorrecto;
        private bool descripcionCorrecto;

        private string _estadoVacio;
        private string _descripcionVacio;

        public string EstadoVacio
        {
            get => _estadoVacio;
            set
            {
                _estadoVacio = value;
                OnPropertyChanged(nameof(EstadoVacio));
            }
        }
        public string DescripcionVacio
        {
            get => _descripcionVacio;
            set
            {
                _descripcionVacio = value;
                OnPropertyChanged(nameof(DescripcionVacio));
            }
        }

        private string _idEjemplar;
        private string _estado;
        private string _descripcion;
        private string _resultado;

        public string IDEjemplar
        {
            get => _idEjemplar;
            set
            {
                _idEjemplar = value;
                OnPropertyChanged(nameof(IDEjemplar));
            }
        }
        public string Estado
        {
            get => _estado;
            set
            {
                _estado = value;
                OnPropertyChanged(nameof(Estado));

                if (String.IsNullOrEmpty(value))
                {
                    EstadoVacio = "no puede estar vacio";
                    estadoCorrecto = false;
                }
                else
                {
                    EstadoVacio = "";
                    estadoCorrecto = true;
                }
            }
        }
        public string Descripcion
        {
            get => _descripcion;
            set
            {
                _descripcion = value;
                OnPropertyChanged(nameof(Descripcion));

                if (String.IsNullOrWhiteSpace(value))
                {
                    DescripcionVacio = "No puede estar vacio";
                    descripcionCorrecto = false;
                }
                else
                {
                    DescripcionVacio = "";
                    descripcionCorrecto = true;
                }
            }
        }
        public string Resultado
        {
            get => _resultado;
            set
            {
                _resultado = value;
                OnPropertyChanged(nameof(Resultado));
            }
        }

        private DataTable _controlT;
        private string _filtro;
        private string _busqueda;
        private DataRowView _filaSeleccionada;
        private string _elementosSeleccionados;

        public DataTable ControlT
        {
            get => _controlT;
            set
            {
                _controlT = value;
                OnPropertyChanged(nameof(ControlT));
            }
        }
        public string Filtro
        {
            get => _filtro;
            set
            {
                _filtro = value;
                OnPropertyChanged(nameof(Filtro));

                CargarTransacciones();
            }
        }
        public string Busqueda
        {
            get => _busqueda;
            set
            {
                _busqueda = value;
                OnPropertyChanged(nameof(Busqueda));
                _timerBusqueda.Stop();
                _timerBusqueda.Start();
            }
        }

        public DataRowView FilaSeleccionada
        {
            get => _filaSeleccionada;
            set
            {
                _filaSeleccionada = value;
                OnPropertyChanged(nameof(FilaSeleccionada));

                IDEjemplar = value["id_ejemplar"].ToString();

                ElementosSeleccionados = $"ID Ejemplar: {IDEjemplar}\n ID Usuario: {UsuarioGlobal.GetInstance().IdUsuario}";
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

        public ControlLibrosVM()
        {
            RegistrarTransaccionCommand = new AsyncRelayCommand(RegistrarTransaccion, PuedeRegistrarTransaccion);

            _timerBusqueda = new System.Timers.Timer(650);
            _timerBusqueda.Elapsed += (sender, e) => Task.Run(() => CargarTransacciones());
            _timerBusqueda.AutoReset = false;

            EstadoVacio = "Seleccione";

            CargarTransacciones();
        }

        public async void CargarTransacciones()
        {
            using (MySqlConnection cnx = new MySqlConnection(_conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT e.id_ejemplar, l.titulo, e.disponibilidad, MAX(p.id_prestamo) AS UltimoPrestamo, CONCAT(MAX(u.nombres), ' ', MAX(u.ap_paterno), ' ', MAX(u.ap_materno)) AS NombreUsuario\r\nFROM ejemplar e\r\nJOIN libro l ON e.id_libro = l.id_libro\r\nJOIN transaccion t ON e.id_ejemplar = t.id_ejemplar\r\nJOIN usuario u ON t.id_usuario = u.id_usuario\r\nJOIN prestamo p ON t.id_transaccion = p.id_transaccion\r\nWHERE e.disponibilidad IN ('prestado', 'reservado')\r\n";

                if (Filtro == "Titulo")
                {
                    consulta += " AND l.titulo LIKE @busquedaB \r\n";
                }
                else if (Filtro == "Nombre")
                {
                    consulta += " AND u.nombres LIKE @busquedaB\r\n ";
                }
                else if (Filtro == "Apellido paterno")
                {
                    consulta += " AND u.ap_paterno LIKE @busquedaB\r\n ";
                }
                else if (Filtro == "Apellido materno")
                {
                    consulta += " AND u.ap_materno LIKE @busquedaB\r\n ";
                }

                consulta += "GROUP BY e.id_ejemplar, l.titulo, e.disponibilidad; ";

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@busquedaB", $"%{Busqueda}%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                ControlT = dt;
                await cnx.CloseAsync();
            }
        }

        public async Task RegistrarTransaccion(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(_conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("devolver_libro", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_ejemplar", Int16.Parse(IDEjemplar));
                        cmd.Parameters.AddWithValue("@pestado", Estado);
                        cmd.Parameters.AddWithValue("@pdescripcion", Descripcion);

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;


                        await cmd.ExecuteNonQueryAsync();

                        Resultado = (string)cmd.Parameters["@resultado"].Value;

                        await cnx.CloseAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private bool PuedeRegistrarTransaccion(object parameter)
        {
            return estadoCorrecto && descripcionCorrecto;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
