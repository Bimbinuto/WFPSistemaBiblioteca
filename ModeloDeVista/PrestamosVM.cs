using Biblioteca.BDConexion;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Timers;

namespace Biblioteca.ModeloDeVista
{
    //Para la venta de prestamos
    public partial class PrestamosVM : INotifyPropertyChanged
    {
        private Conexion conexion = new Conexion();
        private System.Timers.Timer _timer;

        public ICommand ModificarCommand { get; private set; }
        public ICommand EliminarCommand { get; private set; }
        public ICommand PrestamoCommand { get; private set; }

        private string _idEjemplar;
        private string _idUsuario;
        private DateTime _fechaPrestamo;
        private DateTime _fechaDevolucion;
        private string _tipoPrestamo;

        private DataTable _ejemplares;
        private DataRowView _filaSeleccionada;
        private string _elementosFilaSeleccionada;
        private string _busqueda;
        private string _filtro;

        private string _idEjemplarSel;
        private string _codigoSel;
        private string _disponibilidadSel;

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
            get => _filaSeleccionada; set
            {
                _filaSeleccionada = value;
                OnPropertyChanged(nameof(FilaSeleccionada));

                if (value != null)
                {
                    var id_ejemplar = value["id_ejemplar"].ToString();
                    var codigo = value["codigo"].ToString();
                    var titulo = value["titulo"].ToString();
                    var autores = value["autores"].ToString();
                    var disponibilidad = value["disponibilidad"].ToString();

                    _idEjemplarSel = id_ejemplar;
                    _codigoSel = codigo;
                    _disponibilidadSel = disponibilidad;

                    ElementosFilaSeleccionada = $"{_idEjemplarSel}, {_codigoSel}, {_disponibilidadSel}"; // $"{codigo},  {titulo},  {autores},  {disponibilidad} - ID: {id_ejemplar}";

                }
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
        public string Busqueda
        {
            get => _busqueda;
            set
            {
                _busqueda = value;
                OnPropertyChanged(nameof(Busqueda));
                //CargarTablaAsync();
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
        public string IdEjemplar
        {
            get => _idEjemplar;
            set
            {
                _idEjemplar = value;
                OnPropertyChanged(nameof(IdEjemplar));
            }
        }
        public string IdUsuario
        {
            get => _idUsuario;
            set
            {
                _idUsuario = value;
                OnPropertyChanged(nameof(IdUsuario));
            }
        }
        public DateTime FechaPrestamo
        {
            get => _fechaPrestamo;
            set
            {
                _fechaPrestamo = value;
                OnPropertyChanged(nameof(FechaPrestamo));
            }
        }
        public DateTime FechaDevolucion
        {
            get => _fechaDevolucion;
            set
            {
                _fechaDevolucion = value;
                OnPropertyChanged(nameof(FechaDevolucion));
            }
        }
        public string TipoPrestamo
        {
            get => _tipoPrestamo;
            set
            {
                _tipoPrestamo = value;
                OnPropertyChanged(nameof(TipoPrestamo));
            }
        }


        public PrestamosVM()
        {
            ModificarCommand = new RelayCommand(Modificar, PuedeModificar);
            PrestamoCommand = new RelayCommand(Prestamo, PuedeRealizarPrestamo);

            _timer = new System.Timers.Timer(650);
            _timer.Elapsed += (sender, e) => Task.Run(() => CargarTablaAsync());
            _timer.AutoReset = false;

            CargarTablaAsync();
            CargarLectoresAsync();
        }

        private async void CargarTablaAsync()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT e.id_ejemplar, e.codigo, l.titulo, a.nombres, GROUP_CONCAT(a.nombres) AS autores, e.disponibilidad\r\n" +
                                  "FROM ejemplar e\r\n" +
                                  "JOIN libro l ON e.id_libro = l.id_libro\r\n" +
                                  "JOIN libro_autor la ON l.id_libro = la.id_autor\r\n" +
                                  "JOIN autor a ON la.id_autor = a.id_autor \r\n" +
                                  "WHERE l.titulo LIKE @busqueda\r\n" +
                                  "AND e.disponibilidad = 'disponible' ";

                if (Filtro == "Tesis")
                {
                    consulta += "AND e.tipo_ejemplar = 'tesis' ";
                }
                else if (Filtro == "Libro")
                {
                    consulta += "AND (e.tipo_ejemplar = 'original' OR e.tipo_ejemplar = 'copia') ";
                }

                consulta += "GROUP BY e.id_ejemplar, e.codigo, l.titulo, e.disponibilidad;";

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@busqueda", $"%{Busqueda}%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                Ejemplares = dt;
                await cnx.CloseAsync();
            }
        }

        private void Modificar(object parameter)
        {
            ElementosFilaSeleccionada = "Pulsado Modificar";
        }

        private void Prestamo(object parameter)
        {
            ElementosFilaSeleccionada = "pulsado Realizar prestamo";
        }

        private bool PuedeModificar(object parameter)
        {
            if (_disponibilidadSel == "prestado" || _disponibilidadSel == "reservado")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private bool PuedeRealizarPrestamo(object parameter)
        {
            if (_disponibilidadSel == "disponible")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //Para la ventana de Modificar Prestamo
    public partial class PrestamosVM
    {
        //private string _idPrestamoM;
        //private string _idusuarioM;
        //private string _idEjemplarM;
        //private string _fecPresM;
        //private string _fecDevM;
        //private string _TipoPres;


        private DataTable _lectores;
        private string _filtrarLectorM = "Carnet de identidad";
        private string _buscarLectorM;

        public DataTable Lectores
        {
            get => _lectores;
            set
            {
                _lectores = value;
                OnPropertyChanged(nameof(Lectores));
            }
        }
        public string FiltrarLectorM
        {
            get => _filtrarLectorM;
            set
            {
                _filtrarLectorM = value;
                OnPropertyChanged(nameof(FiltrarLectorM));
            }
        }
        public string BuscarLectorM
        {
            get => _buscarLectorM;
            set
            {
                _buscarLectorM = value;
                OnPropertyChanged(nameof(BuscarLectorM));
                CargarLectoresAsync();
                //_timer.Stop();
                //_timer.Start();
            }
        }


        public async void CargarLectoresAsync()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT u.id_usuario, u.ci, u.nombres, u.ap_paterno, u.ap_materno\r\n" +
                    "FROM usuario u\r\nJOIN lector l ON u.id_usuario = l.id_usuario\r\n" +
                    "LEFT JOIN docente d ON l.id_lector = d.id_lector\r\n" +
                    "LEFT JOIN estudiante e ON l.id_lector = e.id_lector\r\n" +
                    "WHERE (d.id_docente IS NOT NULL OR e.id_estudiante IS NOT NULL) ";

                if (string.IsNullOrEmpty(FiltrarLectorM) || FiltrarLectorM == "Carnet de identidad")
                {
                    consulta += "AND u.ci LIKE @busqueda";
                }
                else if (FiltrarLectorM == "Nombres")
                {
                    consulta += "AND u.nombres LIKE @busqueda";
                }
                else if (FiltrarLectorM == "Apellido Paterno")
                {
                    consulta += "AND u.ap_paterno LIKE @busqueda";
                }
                else if (FiltrarLectorM == "Apellido Materno")
                {
                    consulta += "AND u.ap_materno LIKE @busqueda";
                }

                consulta += ";";

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@busqueda", $"%{BuscarLectorM}%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                Lectores = dt;
                await cnx.CloseAsync();
            }
        }


    }
}
