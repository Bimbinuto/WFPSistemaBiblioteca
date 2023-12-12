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
using System.Windows;
using System.Windows.Input;

namespace Biblioteca.ModeloDeVista
{
    public partial class ReservacionesVM : INotifyPropertyChanged
    {
        private Conexion conexion = new Conexion();
        private System.Timers.Timer _timer;
        private Reservacion reserva = new Reservacion();
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand RegistrarReservaCommand { get; private set; }

        private string _idEjemplarSel;
        private string _tituloSel;
        private DataTable _ejemplares;
        private DataRowView _filaSeleccionada;
        private string _elementosFilaSeleccionada;
        private string _busqueda;
        private string _filtro;
        private string _resultadReserva;

        private string Titulo
        {
            get => _tituloSel;
            set
            {
                _tituloSel = value;
                OnPropertyChanged(nameof(Titulo));
            }
        }
        public DataTable Ejemplares
        {
            get => _ejemplares;
            set
            {
                _ejemplares = value;
                OnPropertyChanged(nameof(Ejemplares));
            }
        }
        public DataRowView FilaSeleccionada
        {
            get => _filaSeleccionada;
            set
            {
                _filaSeleccionada = value;
                OnPropertyChanged(nameof(FilaSeleccionada));

                _idEjemplarSel = value["id_ejemplar"].ToString();
                _tituloSel = value["titulo"].ToString();

                ElementosFilaSeleccionada = $"ID Ejemplar: {_idEjemplarSel}\nTitulo: {_tituloSel}\nID Usuario: {UsuarioGlobal.GetInstance().IdUsuario}";
            }
        }
        public string Busqueda
        {
            get => _busqueda;
            set
            {
                _busqueda = value;
                OnPropertyChanged(nameof(Busqueda));
                _timer.Stop();
                _timer.Start();
            }
        }
        public string Filtro
        {
            get => _filtro;
            set
            {
                _filtro = value;
                OnPropertyChanged(nameof(Filtro));
                CargarTablaAsync();
            }
        }
        public string ElementosFilaSeleccionada
        {
            get => _elementosFilaSeleccionada;
            set
            {
                _elementosFilaSeleccionada = value;
                OnPropertyChanged(nameof(ElementosFilaSeleccionada));
            }
        }
        public string ResultadoReserva
        {
            get => _resultadReserva;
            set
            {
                _resultadReserva = value;
                OnPropertyChanged(nameof(ResultadoReserva));
            }
        }

        public ReservacionesVM()
        {
            RegistrarReservaCommand = new AsyncRelayCommand(RegistrarReserva, PuedeRegistrarReserva);

            _timer = new System.Timers.Timer(650);
            _timer.Elapsed += (sender, e) => Task.Run(() => CargarTablaAsync());
            _timer.AutoReset = false;

            CargarTablaAsync();
        }

        public async void CargarTablaAsync()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT e.id_ejemplar, e.codigo, l.titulo, GROUP_CONCAT(a.nombres) AS autores, e.disponibilidad\r\n" +
                                  "FROM ejemplar e\r\n" +
                                  "JOIN libro l ON e.id_libro = l.id_libro\r\n" +
                                  "JOIN libro_autor la ON l.id_libro = la.id_libro\r\n" +
                                  "JOIN autor a ON la.id_autor = a.id_autor \r\n" +
                                  "WHERE l.titulo LIKE @busqueda\r\n" +
                                  "AND e.disponibilidad = 'prestado'\r\n ";

                if (Filtro == "Tesis")
                {
                    consulta += "AND e.tipo_ejemplar = 'tesis'\r\n ";
                }
                else if (Filtro == "Libro")
                {
                    consulta += "AND (e.tipo_ejemplar = 'original' OR e.tipo_ejemplar = 'copia') \r\n";
                }

                consulta += " GROUP BY e.id_ejemplar, e.codigo, l.titulo, e.disponibilidad;";

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@busqueda", $"%{Busqueda}%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                Ejemplares = dt;
                await cnx.CloseAsync();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //REGISTRAR RESERVACIONES
    public partial class ReservacionesVM
    {
        //private bool fechaReservaCorrecto = true;

        //public DateTime FechaPrestamo
        //{
        //    get => reserva.fechaReserva;
        //    set
        //    {
        //        reserva.fechaReserva = value;
        //        OnPropertyChanged(nameof(FechaPrestamo));
        //    }
        //}

        private async Task RegistrarReserva(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("registrar_reserva", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_ejemplar", Int32.Parse(_idEjemplarSel));
                        cmd.Parameters.AddWithValue("@pid_usuario", Int32.Parse(UsuarioGlobal.GetInstance().IdUsuario));
                        cmd.Parameters.AddWithValue("@pfecha_reserva", DateTime.Now);

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        ResultadoReserva = (string)cmd.Parameters["@resultado"].Value;

                        await cnx.CloseAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            //MessageBox.Show($"nombres: {Nombres} \n Apellido paterno: {ApPaterno} \n Apellido materno: {ApMaterno} \n Fechde nacimiento: {FechaNacimiento} \n Direccion: {Direccion} \n Telefono: {Telefono} \n Correo: {Correo} \n Cuenta: {Cuenta} \n Contraseña: {Contrasena} \n Sexo: {Sexo} \n Fecha de contrato: {FechaContrato}");
        }

        private bool PuedeRegistrarReserva(object parameter)
        {
            return true;
        }
    }
}
