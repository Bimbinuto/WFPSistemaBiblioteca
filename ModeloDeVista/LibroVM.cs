using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Biblioteca.BDConexion;
using Biblioteca.Modelos;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Biblioteca.ModeloDeVista
{
    public partial class LibroVM : INotifyPropertyChanged
    {
        private LibroM _libro = new LibroM();

        public ICommand AsignarAutorCommand { get; set; }
        public ICommand RegistrarNuevoAutorCommand { get; set; }
        public ICommand RegistrarCommand { get; set; }
        public ICommand ModificarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public ICommand RegistrarEjemplarCommand { get; set; }
        public ICommand ModificarEjemplarCommand { get; set; }
        public ICommand EliminarEjemplarCommand { get; set; }

        private Timer _timerBusqueda;
        public event PropertyChangedEventHandler PropertyChanged;

        private Conexion conexion = new Conexion();
        public string _resultado;

        private string _filtroL = "Nombres";
        private string _busquedaL;
        private DataTable _libroT;
        private DataTable _ejemplarT;
        private DataTable _autorT;

        //para los combobox
        private ObservableCollection<Proveedor> _iProveedor;
        private ObservableCollection<CategoriaM> _iCategoria;
        private ObservableCollection<MateriaM> _iMateria;
        private ObservableCollection<EstanteriaM> iEstanteria;

        private Proveedor _selProveedor;
        private CategoriaM _selCategoria;
        private MateriaM _selMateria;
        private EstanteriaM _selEstanteria;

        //property changed de los combobox
        public ObservableCollection<Proveedor> IProveedor
        {
            get => _iProveedor;
            set
            {
                _iProveedor = value;
                OnPropertyChanged(nameof(IProveedor));
            }
        }
        public ObservableCollection<CategoriaM> ICategoria
        {
            get => _iCategoria;
            set
            {
                _iCategoria = value;
                OnPropertyChanged(nameof(ICategoria));
            }
        }
        public ObservableCollection<MateriaM> IMateria
        {
            get => _iMateria;
            set
            {
                _iMateria = value;
                OnPropertyChanged(nameof(IMateria));
            }
        }
        public ObservableCollection<EstanteriaM> IEstanteria
        {
            get => iEstanteria;
            set
            {
                iEstanteria = value;
                OnPropertyChanged(nameof(IEstanteria));
            }
        }

        // seleccion de los combobox (SeletedItem)
        public Proveedor EProveedor
        {
            get => _selProveedor;
            set
            {
                _selProveedor = value;
                OnPropertyChanged(nameof(EProveedor));

                IdProveedor = value.id_proveedor.ToString();
            }
        }
        public CategoriaM ECategoria
        {
            get => _selCategoria;
            set
            {
                _selCategoria = value;
                OnPropertyChanged(nameof(ECategoria));

                IdCategoria = value.id_categoria.ToString();
            }
        }
        public MateriaM EMateria
        {
            get => _selMateria;
            set
            {
                _selMateria = value;
                OnPropertyChanged(nameof(EMateria));

                IdMateria = value.id_materia.ToString();
            }
        }
        public EstanteriaM EEstanteria
        {
            get => _selEstanteria;
            set
            {
                _selEstanteria = value;
                OnPropertyChanged(nameof(EEstanteria));

                IdEstanteria = value.id_estanteria.ToString();
            }
        }

        //filtros de busqueda
        public string FiltroL
        {
            get => _filtroL;
            set
            {
                _filtroL = value;
                OnPropertyChanged(nameof(FiltroL));
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
        public DataTable LibroT
        {
            get => _libroT;
            set
            {
                _libroT = value;
                OnPropertyChanged(nameof(LibroT));
            }
        }
        public DataTable EjemplarT
        {
            get => _ejemplarT;
            set
            {
                if (_ejemplarT != value)
                {
                    _ejemplarT = value;
                    OnPropertyChanged(nameof(EjemplarT));
                }

            }
        }
        public DataTable AutorT
        {
            get => _autorT;
            set
            {
                _autorT = value;
                OnPropertyChanged(nameof(AutorT));
            }
        }

        private DataRowView _filaSeleccionadaL;
        private DataRowView _filaSeleccionadaEjemplar;
        private DataRowView _filaSeleccionadaA;
        private string _elementosSeleccionadosL;
        private string _elementosSeleccionadoEjemplar;
        private string _elementosSeleccionadosA;
        //private string _idEjemplarSeleccionado;

        public string ElementosSeleccionadosL
        {
            get => _elementosSeleccionadosL;
            set
            {
                _elementosSeleccionadosL = value;
                OnPropertyChanged("ElementosSeleccionadosL");
            }
        }
        public string ElementosSeleccionadosEjemplar
        {
            get => _elementosSeleccionadoEjemplar;
            set
            {
                _elementosSeleccionadoEjemplar = value;
                OnPropertyChanged(nameof(ElementosSeleccionadosEjemplar));
            }
        }
        public string ElementosSeleccionadosA
        {
            get => _elementosSeleccionadosA;
            set
            {
                _elementosSeleccionadosA = value;
                OnPropertyChanged(nameof(ElementosSeleccionadosA));
            }
        }
        public DataRowView FilaSeleccionadaL
        {
            get => _filaSeleccionadaL;
            set
            {
                _filaSeleccionadaL = value;
                OnPropertyChanged(nameof(_filaSeleccionadaL));

                if (value != null)
                {
                    _idLibroSel = value["id_libro"].ToString();
                    _tituloSel = value["titulo"].ToString();

                    ElementosSeleccionadosL = $"ID: {_idLibroSel} \n" +
                                              $"Titulo: {_tituloSel}";
                }
            }
        }
        public DataRowView FilaSeleccionadaEjemplar
        {
            get => _filaSeleccionadaEjemplar;
            set
            {
                _filaSeleccionadaEjemplar = value;
                OnPropertyChanged(nameof(FilaSeleccionadaEjemplar));

                if (value != null)
                {
                    _idEjemplarSel = value["id_ejemplar"].ToString();
                    _codigoEjemplar = value["codigo"].ToString();
                    _descripcionEjemplar = value["descripcion"].ToString();

                    ElementosSeleccionadosEjemplar = $"ID Ejemplar: {_idEjemplarSel} \n" +
                                                     $"Codigo: {_codigoEjemplar}\n" +
                                                     $"Descripcion: {_descripcionEjemplar}";
                }
            }
        }
        public DataRowView FilaSeleccionadaA
        {
            get => _filaSeleccionadaA;
            set
            {
                _filaSeleccionadaA = value;
                OnPropertyChanged(nameof(FilaSeleccionadaA));

                if (value != null)
                {
                    _idAutor = value["id_autor"].ToString();

                    ElementosSeleccionadosA = $"ID autor: {_idAutor}\n";
                }
            }
        }
        public string IDLibroSeleccionado
        {
            get => _idLibroSel;
            set
            {
                _idLibroSel = value;
                OnPropertyChanged(nameof(IDLibroSeleccionado));
            }
        }


        public LibroVM()
        {
            AsignarAutorCommand = new AsyncRelayCommand(AsignarAutor, PuedeAsignarAutor);
            RegistrarCommand = new AsyncRelayCommand(Registrar, PuedeRegistrar);
            RegistrarNuevoAutorCommand = new AsyncRelayCommand(RegistrarNuevoAutor, PuedeRegistrarNuevoAutor);
            //ModificarCommand = new AsyncRelayCommand(Modificar, PuedeModificar);
            EliminarCommand = new AsyncRelayCommand(Eliminar, PuedeEliminar);
            RegistrarEjemplarCommand = new AsyncRelayCommand(RegistrarEjemplar, PuedeRegistrarEjemplar);
            ModificarEjemplarCommand = new AsyncRelayCommand(ModificarEjemplar, PuedeModificarEjemplar);
            EliminarEjemplarCommand = new AsyncRelayCommand(EliminarEjemplar, PuedeEliminarEjemplar);

            FechaDisposicion = new DateTime(2000, 1, 1);
            FechaEdicion = new DateTime(2000, 1, 1);

            _timerBusqueda = new System.Timers.Timer(650);
            _timerBusqueda.Elapsed += (sender, e) => Task.Run(() => CargarTablaLibrosAsync());
            _timerBusqueda.AutoReset = false;

            _timerAutor = new System.Timers.Timer(650);
            _timerAutor.Elapsed += (sender, e) => Task.Run(() => CargarTablaAutoresAsync());
            _timerAutor.AutoReset = false;

            FiltroA = "Nombres";

            CargarTablaLibrosAsync();
            CargarListaProveedorASync();
            CargarListaCategoriaASync();
            CargarListaMateriaASync();
            CargarListaEstanteriaASync();
            CargarListaEjemplaresAsync();
            CargarTablaAutoresAsync();
        }

        private async void CargarTablaLibrosAsync()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT l.id_libro, l.isbn, l.titulo, l.fecha_edicion, l.editorial, l.grado_recomendacion, l.id_proveedor, l.id_categoria, l.id_materia\r\n" +
                                  "FROM libro l\r\n";

                if (FiltroL == "ISBN")
                {
                    consulta += "WHERE l.isbn LIKE @busquedaB ";
                }
                else if (FiltroL == "Titulo")
                {
                    consulta += "WHERE l.titulo LIKE @busquedaB ";
                }
                else if (FiltroL == "Editorial")
                {
                    consulta += "WHERE l.editorial LIKE @busquedaB ";
                }

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@busquedaB", $"%{BusquedaL}%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                LibroT = dt;
                await cnx.CloseAsync();
            }
        }

        public async void CargarListaProveedorASync()
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    string query = "SELECT id_proveedor, nombre, contacto FROM proveedor";
                    MySqlCommand cmd = new MySqlCommand(query, cnx);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        IProveedor = new ObservableCollection<Proveedor>();
                        while (await reader.ReadAsync())
                        {
                            var Proveedor = new Proveedor
                            {
                                id_proveedor = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                contacto = reader.GetString(2)
                            };
                            IProveedor.Add(Proveedor);
                        }
                    }

                    await cnx.CloseAsync();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public async void CargarListaCategoriaASync()
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    string query = "SELECT id_categoria, nombre, descripcion FROM categoria";
                    MySqlCommand cmd = new MySqlCommand(query, cnx);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        ICategoria = new ObservableCollection<CategoriaM>();
                        while (await reader.ReadAsync())
                        {
                            var CategoriaM = new CategoriaM
                            {
                                id_categoria = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                descripcion = reader.GetString(2)
                            };
                            ICategoria.Add(CategoriaM);
                        }
                    }

                    await cnx.CloseAsync();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public async void CargarListaMateriaASync()
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    string query = "SELECT id_materia, sigla, nombre FROM materia";
                    MySqlCommand cmd = new MySqlCommand(query, cnx);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        IMateria = new ObservableCollection<MateriaM>();
                        while (await reader.ReadAsync())
                        {
                            var MateriaM = new MateriaM
                            {
                                id_materia = reader.GetInt32(0),
                                sigla = reader.GetString(1),
                                nombre = reader.GetString(2)
                            };
                            IMateria.Add(MateriaM);
                        }
                    }

                    await cnx.CloseAsync();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public async void CargarListaEstanteriaASync()
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    string query = "SELECT id_estanteria, codigo_ubicacion, capacidad, capacidad_original FROM estanteria";
                    MySqlCommand cmd = new MySqlCommand(query, cnx);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        IEstanteria = new ObservableCollection<EstanteriaM>();
                        while (await reader.ReadAsync())
                        {
                            var EstanteriaM = new EstanteriaM
                            {
                                id_estanteria = reader.GetInt32(0),
                                codigo_ubicacion = reader.GetString(1),
                                capacidad = reader.GetInt32(2),
                                capacidad_original = reader.GetInt32(3)
                            };
                            IEstanteria.Add(EstanteriaM);
                        }
                    }

                    await cnx.CloseAsync();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public async void CargarListaEjemplaresAsync()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT e.id_ejemplar, e.codigo, e.fecha_disposicion, e.disponibilidad, e.tipo_ejemplar, e.estado,  e.descripcion, e.activo , es.codigo_ubicacion\r\n" +
                                  "FROM ejemplar e\r\n" +
                                  "JOIN estanteria es ON e.id_estanteria = es.id_estanteria\r\n" +
                                  "WHERE e.activo = 1\r\n";

                if (!String.IsNullOrEmpty(IDLibroSeleccionado))
                {
                    consulta += "AND e.id_libro LIKE @idlibro ";
                }

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@idlibro", $"%{IDLibroSeleccionado.ToString()}%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                EjemplarT = dt;
                await cnx.CloseAsync();

            }
        }

        public async void CargarTablaAutoresAsync()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT id_autor, nombres, ap_paterno, ap_materno\r\n" +
                                  "FROM autor WHERE\r\n";

                if (FiltroA == "Nombres")
                {
                    consulta += " nombres LIKE @Busqueda AND\r\n";
                }
                else if (FiltroA == "Apellido paterno")
                {
                    consulta += " ap_paterno LIKE @Busqueda AND\r\n";
                }
                else if (FiltroA == "Apellido materno")
                {
                    consulta += " ap_materno LIKE @Busqueda AND\r\n";
                }

                consulta += " NOT EXISTS (\r\nSELECT 1\r\nFROM libro_autor\r\nWHERE autor.id_autor = libro_autor.id_autor\r\nAND libro_autor.id_libro = @idLibro\r\n);";


                //consulta += "";


                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@Busqueda", $"%{BusquedaA}%");
                cmd.Parameters.AddWithValue("@idLibro", $"{IDLibroSeleccionado.ToString()}");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                AutorT = dt;
                await cnx.CloseAsync();

            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //REGISTRAR AUTOR
    public partial class LibroVM
    {
        private Timer _timerAutor;

        private string _idAutor;

        private string _nombres;
        private string _apPaterno;
        private string _apMaterno;

        private string _nombresVacio;
        private string _apPaternoVacio;
        private string _apMaternoVacio;

        private bool nombresCorrecto;
        private bool apPaternoCorrecto;
        private bool apMaternoCorrecto;

        public string NombresAutorVacio
        {
            get => _nombresVacio;
            set
            {
                _nombresVacio = value;
                OnPropertyChanged(nameof(NombresAutorVacio));
            }
        }
        public string ApPaternoAutorVacio
        {
            get => _apPaternoVacio;
            set
            {
                _apPaternoVacio = value;
                OnPropertyChanged(nameof(ApPaternoAutorVacio));
            }
        }
        public string ApMaternoAutorVacio
        {
            get => _apMaternoVacio;
            set
            {
                _apMaternoVacio = value;
                OnPropertyChanged(nameof(ApMaternoAutorVacio));
            }
        }

        private string _filtroA;
        private string _busquedaA;
        private string _resultadoAutor;
        private string _resultadoNuevoAutor;

        public string FiltroA
        {
            get => _filtroA;
            set
            {
                _filtroA = value;
                OnPropertyChanged(nameof(FiltroA));
            }
        }
        public string BusquedaA
        {
            get => _busquedaA;
            set
            {
                _busquedaA = value;
                OnPropertyChanged(nameof(BusquedaA));
                _timerAutor.Stop();
                _timerAutor.Start();
            }
        }
        public string ResultadoAutor
        {
            get => _resultadoAutor;
            set
            {
                _resultadoAutor = value;
                OnPropertyChanged(nameof(ResultadoAutor));
            }
        }
        public string ResultadoNuevoAutor
        {
            get => _resultadoNuevoAutor;
            set
            {
                _resultadoNuevoAutor = value;
                OnPropertyChanged(nameof(ResultadoNuevoAutor));
            }
        }

        private async Task AsignarAutor(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("registrar_autor_por_libro", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_libro", Int32.Parse(_idLibroSel));
                        cmd.Parameters.AddWithValue("@pid_autor", Int32.Parse(_idAutor));

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;


                        await cmd.ExecuteNonQueryAsync();

                        ResultadoAutor = (string)cmd.Parameters["@resultado"].Value;

                        CargarTablaAutoresAsync();

                        DispatcherTimer dispatcherTimer = new DispatcherTimer();
                        DispatcherTimer timerAutor = dispatcherTimer;
                        timerAutor.Interval = TimeSpan.FromSeconds(3);
                        timerAutor.Tick += (s, args) =>
                        {
                            ResultadoAutor = "";
                        };
                        timerAutor.Start();



                        await cnx.CloseAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            //MessageBox.Show($"nombres: {Nombres} \n Apellido paterno: {ApPaterno} \n Apellido materno: {ApMaterno} \n Fechde nacimiento: {FechaNacimiento} \n Direccion: {Direccion} \n Telefono: {Telefono} \n Correo: {Correo} \n Cuenta: {Cuenta} \n Contraseña: {Contrasena} \n Sexo: {Sexo} \n Fecha de contrato: {FechaContrato}");
        }

        private bool PuedeAsignarAutor(object parameter)
        {
            return true;
        }

        public string NombresAutor
        {
            get => _nombres;
            set
            {
                _nombres = value;
                OnPropertyChanged(nameof(NombresAutor));

                if (string.IsNullOrEmpty(value))
                {
                    NombresAutorVacio = "No puede estar vacio o contener espacios";
                    nombresCorrecto = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\s]+$";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        NombresAutorVacio = "No puede contener números ni caracteres especiales";
                        nombresCorrecto = false;
                    }
                    else
                    {
                        NombresAutorVacio = "";
                        nombresCorrecto = true;
                    }
                }
            }
        }
        public string ApPaternoAutor
        {
            get => _apPaterno;
            set
            {
                _apPaterno = value;
                OnPropertyChanged(nameof(ApPaternoAutor));

                if (string.IsNullOrEmpty(value))
                {
                    ApPaternoAutorVacio = "No puede estar vacio o contener espacios";
                    apPaternoCorrecto = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\s]+$";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        ApPaternoAutorVacio = "No puede contener números ni caracteres especiales";
                        apPaternoCorrecto = false;
                    }
                    else
                    {
                        ApPaternoAutorVacio = "";
                        apPaternoCorrecto = true;
                    }
                }
            }
        }
        public string ApMaternoAutor
        {
            get => _apMaterno;
            set
            {
                _apMaterno = value;
                OnPropertyChanged(nameof(ApMaternoAutor));

                if (string.IsNullOrEmpty(value))
                {
                    ApMaternoAutorVacio = "No puede estar vacio o contener espacios";
                    apMaternoCorrecto = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\s]+$";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        ApMaternoAutorVacio = "No puede contener números ni caracteres especiales";
                        apMaternoCorrecto = false;
                    }
                    else
                    {
                        ApMaternoAutorVacio = "";
                        apMaternoCorrecto = true;
                    }
                }
            }
        }

        private async Task RegistrarNuevoAutor(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("registrar_autor", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pnombres", NombresAutor);
                        cmd.Parameters.AddWithValue("@pap_paterno", ApPaternoAutor);
                        cmd.Parameters.AddWithValue("@pap_materno", ApMaternoAutor);

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;


                        await cmd.ExecuteNonQueryAsync();

                        ResultadoNuevoAutor = (string)cmd.Parameters["@resultado"].Value;

                        await cnx.CloseAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            //MessageBox.Show($"nombres: {Nombres} \n Apellido paterno: {ApPaterno} \n Apellido materno: {ApMaterno} \n Fechde nacimiento: {FechaNacimiento} \n Direccion: {Direccion} \n Telefono: {Telefono} \n Correo: {Correo} \n Cuenta: {Cuenta} \n Contraseña: {Contrasena} \n Sexo: {Sexo} \n Fecha de contrato: {FechaContrato}");
        }

        private bool PuedeRegistrarNuevoAutor(object parameter)
        {
            return nombresCorrecto && apPaternoCorrecto && apMaternoCorrecto;
        }

    }


    //REGISTRAR EJEMPLARES:
    public partial class LibroVM
    {
        private string _idLibroSel = "";
        private string _tituloSel = "";

        public DateTime FechaDisposicionEjemplar
        {
            get => _libro.fecha_disposicion;
            set
            {
                _libro.fecha_disposicion = value;
                OnPropertyChanged(nameof(FechaDisposicionEjemplar));

                if (value > DateTime.Now)
                {
                    FechaDisposicionVacio = "La fecha no puede ser superior al dia de hoy";
                    fechaDisposicionCorrecto = false;
                }
                else
                {
                    FechaDisposicionVacio = "";
                    fechaDisposicionCorrecto = true;
                }
            }
        }
        public string DisponibilidadEjemplar
        {
            get => _libro.disponibilidad;
            set
            {
                _libro.disponibilidad = value;
                OnPropertyChanged(nameof(DisponibilidadEjemplar));

                if (String.IsNullOrEmpty(value))
                {
                    DisponibilidadVacio = "No puede estar vacio";
                    disponibilidadCorrecto = false;
                }
                else
                {
                    DisponibilidadVacio = "";
                    disponibilidadCorrecto = true;
                }
            }
        }
        public string TipoEjemplarEjemplar
        {
            get => _libro.tipo_ejemplar;
            set
            {
                _libro.tipo_ejemplar = value;
                OnPropertyChanged(nameof(TipoEjemplarEjemplar));

                if (String.IsNullOrEmpty(value))
                {
                    TipoEjemplarVacio = "No puede estar vacio";
                    tipoEjemplarCorrecto = false;
                }
                else
                {
                    TipoEjemplarVacio = "";
                    tipoEjemplarCorrecto = true;
                }
            }
        }
        public string EstadoEjemplar
        {
            get => _libro.estado;
            set
            {
                _libro.estado = value;
                OnPropertyChanged(nameof(EstadoEjemplar));

                if (String.IsNullOrEmpty(value))
                {
                    EstadoVacio = "No puede estar vacio";
                    estadoCorrecto = false;
                }
                else
                {
                    EstadoVacio = "";
                    estadoCorrecto = true;
                }
            }
        }
        public string DescripcionEjemplar
        {
            get => _libro.descripcion;
            set
            {
                _libro.descripcion = value;
                OnPropertyChanged(nameof(DescripcionEjemplar));

                if (String.IsNullOrEmpty(value))
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
        public string IdEstanteriaEjemplar
        {
            get => _libro.id_estanteria;
            set
            {
                _libro.id_estanteria = value;
                OnPropertyChanged(nameof(IdEstanteriaEjemplar));

                if (String.IsNullOrEmpty(value))
                {
                    IdEstanteriaVacio = "No puede estar vacio";
                    idEstanteriaCorrecto = false;
                }
                else
                {
                    IdEstanteriaVacio = "";
                    idEstanteriaCorrecto = true;
                }
            }
        }

        private async Task RegistrarEjemplar(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("registrar_ejemplar", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_libro", _idLibroSel);
                        cmd.Parameters.AddWithValue("@pfecha_disposicion", FechaDisposicionEjemplar);
                        cmd.Parameters.AddWithValue("@pdisponibilidad", DisponibilidadEjemplar);
                        cmd.Parameters.AddWithValue("@ptipo_ejemplar", TipoEjemplarEjemplar);
                        cmd.Parameters.AddWithValue("@pestado", EstadoEjemplar);
                        cmd.Parameters.AddWithValue("@pdescripcion", DescripcionEjemplar);
                        cmd.Parameters.AddWithValue("@pid_estanteria", Int64.Parse(IdEstanteriaEjemplar));

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;


                        await cmd.ExecuteNonQueryAsync();

                        Resultado = (string)cmd.Parameters["@resultado"].Value;

                        await cnx.CloseAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            //MessageBox.Show($"nombres: {Nombres} \n Apellido paterno: {ApPaterno} \n Apellido materno: {ApMaterno} \n Fechde nacimiento: {FechaNacimiento} \n Direccion: {Direccion} \n Telefono: {Telefono} \n Correo: {Correo} \n Cuenta: {Cuenta} \n Contraseña: {Contrasena} \n Sexo: {Sexo} \n Fecha de contrato: {FechaContrato}");
        }

        private bool PuedeRegistrarEjemplar(object parameter)
        {
            return fechaDisposicionCorrecto &&
                   disponibilidadCorrecto &&
                   tipoEjemplarCorrecto &&
                   estadoCorrecto &&
                   descripcionCorrecto &&
                   idEstanteriaCorrecto;
        }
    }


    //MODIFICAR EJEMPLAR
    public partial class LibroVM
    {
        private string _idEjemplarSel = "";
        private string _codigoEjemplar = "";
        private string _descripcionEjemplar = "";

        private async Task ModificarEjemplar(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("modificar_ejemplar", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_ejemplar", _idEjemplarSel);
                        cmd.Parameters.AddWithValue("@pfecha_disposicion", FechaDisposicionEjemplar);
                        cmd.Parameters.AddWithValue("@pdisponibilidad", DisponibilidadEjemplar);
                        cmd.Parameters.AddWithValue("@ptipo_ejemplar", TipoEjemplarEjemplar);
                        cmd.Parameters.AddWithValue("@pestado", EstadoEjemplar);
                        cmd.Parameters.AddWithValue("@pdescripcion", DescripcionEjemplar);
                        cmd.Parameters.AddWithValue("@pactivo", true);
                        cmd.Parameters.AddWithValue("@pid_libro", _idLibroSel);
                        cmd.Parameters.AddWithValue("@pid_estanteria", Int64.Parse(IdEstanteriaEjemplar));



                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;


                        await cmd.ExecuteNonQueryAsync();

                        Resultado = (string)cmd.Parameters["@resultado"].Value;

                        await cnx.CloseAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            //MessageBox.Show($"nombres: {Nombres} \n Apellido paterno: {ApPaterno} \n Apellido materno: {ApMaterno} \n Fechde nacimiento: {FechaNacimiento} \n Direccion: {Direccion} \n Telefono: {Telefono} \n Correo: {Correo} \n Cuenta: {Cuenta} \n Contraseña: {Contrasena} \n Sexo: {Sexo} \n Fecha de contrato: {FechaContrato}");
        }

        private bool PuedeModificarEjemplar(object parameter)
        {
            return fechaDisposicionCorrecto &&
                   disponibilidadCorrecto &&
                   tipoEjemplarCorrecto &&
                   estadoCorrecto &&
                   descripcionCorrecto &&
                   idEstanteriaCorrecto;
        }
    }

    // ELIMINAR INFORMACION DEL LIBRO
    public partial class LibroVM
    {
        private bool ejemplarElimCorrecto;
        private string _resultadoEjemplarEliminacion;

        public string ElementoSelEliminarEjemplar
        {
            get => _idLibroSel;
            set
            {
                _idLibroSel = value;
                OnPropertyChanged(nameof(ElementoSelEliminarEjemplar));

                if (value != null)
                {
                    ejemplarElimCorrecto = true;
                }
                else
                {
                    ejemplarElimCorrecto = false;
                }
            }
        }
        public string ResultadoEliminacionEjemplar
        {
            get => _resultadoEjemplarEliminacion;
            set
            {
                _resultadoEjemplarEliminacion = value;
                OnPropertyChanged(nameof(ResultadoEliminacionEjemplar));


            }
        }

        private async Task EliminarEjemplar(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("eliminar_ejemplar", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_ejemplar", Int64.Parse(_idEjemplarSel));

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        ResultadoEliminacionEjemplar = (string)cmd.Parameters["@resultado"].Value;

                        await cnx.CloseAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private bool PuedeEliminarEjemplar(object parameter)
        {
            return !ejemplarElimCorrecto;
        }
    }


    // REGISTRAR LIBRO NUEVO
    public partial class LibroVM
    {
        //Notify de os Binding del alerta del textbox
        private string _isbnVacio = "";
        private string _tituloVacio = "";
        private string _fechaEdicionVacio = "";
        private string _editorialVacio = "";
        private string _idProveedorVacio = "";
        private string _idCategoriaVacio = "";
        private string _idMateriaVacio = "";
        private string _fechaDisposicionVacio = "";
        private string _disponibilidadVacio = "";
        private string _tipoEjemplarVacio = "";
        private string _estadoVacio = "";
        private string _descripcionVacio = "";
        private string _idEstanteriaVacio = "";

        //control de texto del textbox correcto; para registrar
        private bool isbnCorrecto;
        private bool tituloCorrecto;
        private bool fechaEdicionCorrecto;
        private bool editorialCorrecto;
        private bool idProveedorCorrecto;
        private bool idCategoriaCorrecto;
        private bool idMateriaCorrecto;
        private bool fechaDisposicionCorrecto;
        private bool disponibilidadCorrecto;
        private bool tipoEjemplarCorrecto;
        private bool estadoCorrecto;
        private bool descripcionCorrecto;
        private bool idEstanteriaCorrecto;

        public string IsbnVacio
        {
            get => _isbnVacio;
            set
            {
                _isbnVacio = value;
                OnPropertyChanged(nameof(IsbnVacio));
            }
        }
        public string TituloVacio
        {
            get => _tituloVacio;
            set
            {
                _tituloVacio = value;
                OnPropertyChanged(nameof(TituloVacio));
            }
        }
        public string FechaEdicionVacio
        {
            get => _fechaEdicionVacio;
            set
            {
                _fechaEdicionVacio = value;
                OnPropertyChanged(nameof(FechaEdicionVacio));
            }
        }
        public string EditorialVacio
        {
            get => _editorialVacio;
            set
            {
                _editorialVacio = value;
                OnPropertyChanged(EditorialVacio);
            }
        }
        public string IdProveedorVacio
        {
            get => _idProveedorVacio;
            set
            {
                _idProveedorVacio = value;
                OnPropertyChanged(nameof(IdProveedorVacio));
            }
        }
        public string IdCategoriaVacio
        {
            get => _idCategoriaVacio;
            set
            {
                _idCategoriaVacio = value;
                OnPropertyChanged(nameof(IdCategoriaVacio));
            }
        }
        public string IdMateriaVacio
        {
            get => _idMateriaVacio;
            set
            {
                _idMateriaVacio = value;
                OnPropertyChanged(nameof(IdMateriaVacio));
            }
        }
        public string FechaDisposicionVacio
        {
            get => _fechaDisposicionVacio;
            set
            {
                _fechaDisposicionVacio = value;
                OnPropertyChanged(nameof(FechaDisposicionVacio));
            }
        }
        public string DisponibilidadVacio
        {
            get => _disponibilidadVacio;
            set
            {
                _disponibilidadVacio = value;
                OnPropertyChanged(nameof(DisponibilidadVacio));
            }
        }
        public string TipoEjemplarVacio
        {
            get => _tipoEjemplarVacio;
            set
            {
                _tipoEjemplarVacio = value;
                OnPropertyChanged(nameof(TipoEjemplarVacio));
            }
        }
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
        public string IdEstanteriaVacio
        {
            get => _idEstanteriaVacio;
            set
            {
                _idEstanteriaVacio = value;
                OnPropertyChanged(nameof(IdEstanteriaVacio));
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

        // control de los textbox para el libro, Por Binding
        public string Isbn
        {
            get => _libro.isbn;
            set
            {
                _libro.isbn = value;
                OnPropertyChanged(nameof(Isbn));

                if (String.IsNullOrEmpty(value))
                {
                    IsbnVacio = "No puede estar vacio";
                    isbnCorrecto = false;
                }
                else
                {
                    IsbnVacio = "";
                    isbnCorrecto = true;
                }
            }
        }
        public string Titulo
        {
            get => _libro.titulo;
            set
            {
                _libro.titulo = value;
                OnPropertyChanged(nameof(Titulo));

                if (String.IsNullOrEmpty(value))
                {
                    TituloVacio = "No puede estar vacio";
                    tituloCorrecto = false;
                }
                else
                {
                    TituloVacio = "";
                    tituloCorrecto = true;
                }
            }
        }
        public DateTime FechaEdicion
        {
            get => _libro.fecha_edicion;
            set
            {
                _libro.fecha_edicion = value;
                OnPropertyChanged(nameof(FechaEdicion));

                if (value > DateTime.Now)
                {
                    FechaEdicionVacio = "La fecha no puede ser superior al dia de hoy";
                    fechaEdicionCorrecto = false;
                }
                else
                {
                    FechaEdicionVacio = "";
                    fechaEdicionCorrecto = true;
                }
            }
        }
        public string Editorial
        {
            get => _libro.editorial;
            set
            {
                _libro.editorial = value;
                OnPropertyChanged(nameof(Editorial));

                if (String.IsNullOrEmpty(value))
                {
                    EditorialVacio = "No puede estar vacío";
                    editorialCorrecto = false;
                }
                else if (!Regex.IsMatch(value, "^[a-zA-Z0-9 ]*$"))
                {
                    EditorialVacio = "Contiene caracteres especiales";
                    editorialCorrecto = false;
                }
                else
                {
                    EditorialVacio = "";
                    editorialCorrecto = true;
                }
            }
        }
        public string IdProveedor
        {
            get => _libro.id_proveedor;
            set
            {
                _libro.id_proveedor = value;
                OnPropertyChanged(nameof(IdProveedor));


                if (String.IsNullOrEmpty(value))
                {
                    IdProveedorVacio = "No puede estar vacio";
                    idProveedorCorrecto = false;
                }
                else
                {
                    IdProveedorVacio = "";
                    idProveedorCorrecto = true;
                }

                //if (String.IsNullOrEmpty(value))
                //{
                //    IdProveedorVacio = "No puede estar vacio";
                //    idProveedorCorrecto = false;
                //}
                //else
                //{
                //    string patternPositivo = @"^[1-9][0-9]*$"; // Expresión regular que acepta solo números positivos (no cero)
                //    string patternNegativo = @"^-"; // Expresión regular que detecta números negativos
                //    string patternEspecial = @"[^0-9]"; // Expresión regular que detecta caracteres no numéricos

                //    if (Regex.IsMatch(value, patternNegativo))
                //    {
                //        IdProveedorVacio = "No puede haber campos negativos";
                //        idProveedorCorrecto = false;
                //    }
                //    else if (Regex.IsMatch(value, patternPositivo))
                //    {
                //        IdProveedorVacio = "El valor debe ser un número positivo";
                //        idProveedorCorrecto = false;
                //    }
                //    else if (Regex.IsMatch(value, patternEspecial))
                //    {
                //        IdProveedorVacio = "No puede contener caracteres especiales";
                //        idProveedorCorrecto = false;
                //    }
                //    else
                //    {
                //        IdProveedorVacio = "";
                //        idProveedorCorrecto = true;
                //    }
                //}
            }
        }
        public string IdCategoria
        {
            get => _libro.id_categoria;
            set
            {
                _libro.id_categoria = value;
                OnPropertyChanged(nameof(IdCategoria));

                if (String.IsNullOrEmpty(value))
                {
                    IdCategoriaVacio = "No puede estar vacio";
                    idCategoriaCorrecto = false;
                }
                else
                {
                    IdCategoriaVacio = "";
                    idCategoriaCorrecto = true;
                }
                //else
                //{
                //    string patternPositivo = @"^[1-9][0-9]*$"; // Expresión regular que acepta solo números positivos (no cero)
                //    string patternNegativo = @"^-"; // Expresión regular que detecta números negativos
                //    string patternEspecial = @"[^0-9]"; // Expresión regular que detecta caracteres no numéricos

                //    if (Regex.IsMatch(value, patternNegativo))
                //    {
                //        IdCategoriaVacio = "No puede haber campos negativos";
                //        idCategoriaCorrecto = false;
                //    }
                //    else if (!Regex.IsMatch(value, patternPositivo))
                //    {
                //        IdCategoriaVacio = "El valor debe ser un número positivo";
                //        idCategoriaCorrecto = false;
                //    }
                //    else if (Regex.IsMatch(value, patternEspecial))
                //    {
                //        IdCategoriaVacio = "No puede contener caracteres especiales";
                //        idCategoriaCorrecto = false;
                //    }
                //    else
                //    {
                //        IdCategoriaVacio = "";
                //        idCategoriaCorrecto = true;
                //    }
                //}
            }
        }
        public string IdMateria
        {
            get => _libro.id_materia;
            set
            {
                _libro.id_materia = value;
                OnPropertyChanged(nameof(IdMateria));

                if (String.IsNullOrEmpty(value))
                {
                    IdMateriaVacio = "No puede estar vacio";
                    idMateriaCorrecto = false;
                }
                else
                {
                    IdMateriaVacio = "";
                    idMateriaCorrecto = true;
                }
                //else
                //{
                //    string patternPositivo = @"^[1-9][0-9]*$"; // Expresión regular que acepta solo números positivos (no cero)
                //    string patternNegativo = @"^-"; // Expresión regular que detecta números negativos
                //    string patternEspecial = @"[^0-9]"; // Expresión regular que detecta caracteres no numéricos

                //    if (Regex.IsMatch(value, patternNegativo))
                //    {
                //        IdMateriaVacio = "No puede haber campos negativos";
                //        idMateriaCorrecto = false;
                //    }
                //    else if (!Regex.IsMatch(value, patternPositivo))
                //    {
                //        IdMateriaVacio = "El valor debe ser un número positivo";
                //        idMateriaCorrecto = false;
                //    }
                //    else if (Regex.IsMatch(value, patternEspecial))
                //    {
                //        IdMateriaVacio = "No puede contener caracteres especiales";
                //        idMateriaCorrecto = false;
                //    }
                //    else
                //    {
                //        IdMateriaVacio = "";
                //        idMateriaCorrecto = true;
                //    }
                //}
            }
        }
        public DateTime FechaDisposicion
        {
            get => _libro.fecha_disposicion;
            set
            {
                _libro.fecha_disposicion = value;
                OnPropertyChanged(nameof(FechaDisposicion));

                if (value > DateTime.Now)
                {
                    FechaDisposicionVacio = "La fecha no puede ser superior al dia de hoy";
                    fechaDisposicionCorrecto = false;
                }
                else
                {
                    FechaDisposicionVacio = "";
                    fechaDisposicionCorrecto = true;
                }
            }
        }
        public string Disponibilidad
        {
            get => _libro.disponibilidad;
            set
            {
                _libro.disponibilidad = value;
                OnPropertyChanged(nameof(Disponibilidad));

                if (String.IsNullOrEmpty(value))
                {
                    DisponibilidadVacio = "No puede estar vacio";
                    disponibilidadCorrecto = false;
                }
                else
                {
                    DisponibilidadVacio = "";
                    disponibilidadCorrecto = true;
                }
            }
        }
        public string TipoEjemplar
        {
            get => _libro.tipo_ejemplar;
            set
            {
                _libro.tipo_ejemplar = value;
                OnPropertyChanged(nameof(TipoEjemplar));

                if (String.IsNullOrEmpty(value))
                {
                    TipoEjemplarVacio = "No puede estar vacio";
                    tipoEjemplarCorrecto = false;
                }
                else
                {
                    TipoEjemplarVacio = "";
                    tipoEjemplarCorrecto = true;
                }
            }
        }
        public string Estado
        {
            get => _libro.estado;
            set
            {
                _libro.estado = value;
                OnPropertyChanged(nameof(Estado));

                if (String.IsNullOrEmpty(value))
                {
                    EstadoVacio = "No puede estar vacio";
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
            get => _libro.descripcion;
            set
            {
                _libro.descripcion = value;
                OnPropertyChanged(nameof(Descripcion));

                if (String.IsNullOrEmpty(value))
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
        public string IdEstanteria
        {
            get => _libro.id_estanteria;
            set
            {
                _libro.id_estanteria = value;
                OnPropertyChanged(nameof(IdEstanteria));

                if (String.IsNullOrEmpty(value))
                {
                    IdEstanteriaVacio = "No puede estar vacio";
                    idEstanteriaCorrecto = false;
                }
                else
                {
                    IdEstanteriaVacio = "";
                    idEstanteriaCorrecto = true;
                }

            }
        }

        private async Task Registrar(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("registrar_libro", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pisbn", Isbn);
                        cmd.Parameters.AddWithValue("@ptitulo", Titulo);
                        cmd.Parameters.AddWithValue("@pfecha_edicion", FechaEdicion);
                        cmd.Parameters.AddWithValue("@peditorial", Editorial);
                        cmd.Parameters.AddWithValue("@pid_proveedor", Int64.Parse(IdProveedor));
                        cmd.Parameters.AddWithValue("@pid_categoria", Int64.Parse(IdCategoria));
                        cmd.Parameters.AddWithValue("@pid_materia", Int64.Parse(IdMateria));
                        cmd.Parameters.AddWithValue("@pfecha_disposicion", FechaDisposicion);
                        cmd.Parameters.AddWithValue("@pdisponibilidad", Disponibilidad);
                        cmd.Parameters.AddWithValue("@ptipo_ejemplar", TipoEjemplar);
                        cmd.Parameters.AddWithValue("@pestado", Estado);
                        cmd.Parameters.AddWithValue("@pdescripcion", Descripcion);
                        cmd.Parameters.AddWithValue("@pid_estanteria", Int64.Parse(IdEstanteria));

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;


                        await cmd.ExecuteNonQueryAsync();

                        Resultado = (string)cmd.Parameters["@resultado"].Value;

                        await cnx.CloseAsync();
                    }

                    /*using(MySqlCommand cmd = new MySqlCommand("registrar_autor_por_libro", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_libro", );
                        cmd.Parameters.AddWithValue("@pid_autor", Int32.Parse(IdAutor));
                    }*/
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            //MessageBox.Show($"nombres: {Nombres} \n Apellido paterno: {ApPaterno} \n Apellido materno: {ApMaterno} \n Fechde nacimiento: {FechaNacimiento} \n Direccion: {Direccion} \n Telefono: {Telefono} \n Correo: {Correo} \n Cuenta: {Cuenta} \n Contraseña: {Contrasena} \n Sexo: {Sexo} \n Fecha de contrato: {FechaContrato}");
        }

        private bool PuedeRegistrar(object parameter)
        {
            return isbnCorrecto &&
                   tituloCorrecto &&
                   fechaEdicionCorrecto &&
                   editorialCorrecto &&
                   idProveedorCorrecto &&
                   idCategoriaCorrecto &&
                   idMateriaCorrecto &&
                   fechaDisposicionCorrecto &&
                   disponibilidadCorrecto &&
                   tipoEjemplarCorrecto &&
                   estadoCorrecto &&
                   descripcionCorrecto &&
                   idEstanteriaCorrecto;
        }
    }


    // EDITAR INFORMACION DEL LIBRO
    public partial class libroVM
    {

    }



    // ELIMINAR INFORMACION DEL LIBRO
    public partial class LibroVM
    {
        private bool elementoElimCorrecto;
        private string _resultadoEliminacion;

        public string ElementoSelEliminarLibro
        {
            get => _idLibroSel;
            set
            {
                _idLibroSel = value;
                OnPropertyChanged(nameof(ElementoSelEliminarLibro));

                if (value != null)
                {
                    elementoElimCorrecto = true;
                }
                else
                {
                    elementoElimCorrecto = false;
                }
            }
        }
        public string ResultadoEliminacionLibro
        {
            get => _resultadoEliminacion;
            set
            {
                _resultadoEliminacion = value;
                OnPropertyChanged(nameof(ResultadoEliminacionLibro));
            }
        }


        private async Task Eliminar(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("eliminar_libro", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_libro", Int64.Parse(ElementoSelEliminarLibro));

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        ResultadoEliminacionLibro = (string)cmd.Parameters["@resultado"].Value;

                        await cnx.CloseAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private bool PuedeEliminar(object parameter)
        {
            return !elementoElimCorrecto;
        }
    }
}
