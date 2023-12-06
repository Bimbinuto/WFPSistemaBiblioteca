using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Biblioteca.BDConexion;
using Biblioteca.Modelos;
using MySql.Data.MySqlClient;
using Biblioteca.Vistas.Registros;
using System.Windows.Input;
using System.Data;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Windows;
using System.Collections.ObjectModel;

namespace Biblioteca.ModeloDeVista
{
    public partial class EstudianteVM : INotifyPropertyChanged
    {
        private EstudianteM _estudiante = new EstudianteM();
        public ICommand RegistrarCommand { get; set; }
        public ICommand ModificarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private Timer _timerBusqueda;

        private EncriptadoUnico encriptar = new EncriptadoUnico();
        private string _anteriorPass;
        private Conexion conexion = new Conexion();
        public string _resultado;

        private string _filtroE = "Nombres";
        private string _busquedaE;
        private DataTable _estudianteT;
        private ObservableCollection<CarreraM> _items;
        private CarreraM _elementoItem;

        public ObservableCollection<CarreraM> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }
        public CarreraM ElementoItem
        {
            get => _elementoItem;
            set
            {
                _elementoItem = value;
                OnPropertyChanged(nameof(ElementoItem));

                _id_carrera = value.id_carrera;
            }
        }
        private DataRowView _filaSeleccionadaE;
        private string _elementosSeleccionadosE;

        public string FiltroE
        {
            get => _filtroE;
            set
            {
                _filtroE = value;
                OnPropertyChanged(nameof(FiltroE));
            }
        }
        public string BusquedaE
        {
            get => _busquedaE;
            set
            {
                _busquedaE = value;
                OnPropertyChanged(nameof(BusquedaE));
                //CargarTablaBibliotecariosAsync();
                _timerBusqueda.Stop();
                _timerBusqueda.Start();
            }
        }
        public DataTable EstudianteT
        {
            get => _estudianteT;
            set
            {
                _estudianteT = value;
                OnPropertyChanged(nameof(EstudianteT));
            }
        }
        public DataRowView FilaSeleccionadaE
        {
            get => _filaSeleccionadaE;
            set
            {
                _filaSeleccionadaE = value;
                OnPropertyChanged(nameof(FilaSeleccionadaE));

                if (value != null)
                {
                    _idEstudianteSel = value["id_estudiante"].ToString();
                    _isCISel = value["ci"].ToString();
                    _isNombreSel = value["nombres"].ToString();
                    _isApPaternoSel = value["ap_paterno"].ToString();
                    _isApMaternoSel = value["ap_materno"].ToString();
                    _isFechaNacSel = Convert.ToDateTime(value["fecha_nacimiento"]);
                    _isDireccionSel = value["direccion"].ToString();
                    _isTelefonoSel = value["telefono"].ToString();
                    _isCorreoSel = value["correo"].ToString();
                    _isCuentaSel = value["cuenta"].ToString();
                    _isContrasenaSel = value["constrasena"].ToString();
                    _isSexoSel = value["sexo"].ToString();
                    _isCarreraSel = value["codigo"].ToString();
                    _isSemetreIngresoSel = value["semestre_ingreso"].ToString();

                    _anteriorPass = _isContrasenaSel;

                    ElementosSeleccionadosE = $"Elementos seleccionados: \n" +
                                              $"ID: {_idEstudianteSel} \n" +
                                              $"CI: {_isCISel} \n" +
                                              $"Nombres: {_isNombreSel} \n" +
                                              $"Apellido Paterno: {_isApPaternoSel} \n" +
                                              $"Apellido Materno: {_isApMaternoSel} \n" +
                                              $"Fecha de Nacimiento: {_isFechaNacSel} \n" +
                                              $"Dirección: {_isDireccionSel} \n" +
                                              $"Teléfono: {_isTelefonoSel} \n" +
                                              $"Correo: {_isCorreoSel} \n" +
                                              $"Cuenta: {_isCuentaSel} \n" +
                                              $"Contraseña: {_isContrasenaSel} \n" +
                                              $"Sexo: {_isSexoSel} \n" +
                                              $"Carrera: {_isCarreraSel}\n" +
                                              $"ID carrera: {value["id_carrera"].ToString()}\n" +
                                              $"Nombre carrera: {value["nombre"].ToString()}\n" +
                                              $"Semestre Ingreso: {value["semestre_ingreso"].ToString()}\n";
                }
            }
        }
        public string ElementosSeleccionadosE
        {
            get => _elementosSeleccionadosE;
            set
            {
                _elementosSeleccionadosE = value;
                OnPropertyChanged("ElementosSeleccionadosE");
            }
        }

        // CONSTRUCTOR
        public EstudianteVM()
        {
            FechaNacimiento = new DateTime(2000, 1, 1);

            RegistrarCommand = new AsyncRelayCommand(Registrar, PuedeRegistrar);
            ModificarCommand = new AsyncRelayCommand(Modificar, PuedeModificar);
            EliminarCommand = new AsyncRelayCommand(Eliminar, PuedeEliminar);

            _timerBusqueda = new System.Timers.Timer(650);
            _timerBusqueda.Elapsed += (sender, e) => Task.Run(() => CargarTablaBibliotecariosAsync());
            _timerBusqueda.AutoReset = false;

            CargarTablaBibliotecariosAsync();
            CargarListaCarrera();
        }

        private async void CargarTablaBibliotecariosAsync()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT e.id_estudiante, u.ci, u.nombres, u.ap_paterno, u.ap_materno, u.fecha_nacimiento, u.direccion, u.telefono, u.correo, u.cuenta, u.constrasena, u.sexo, e.semestre_ingreso , l.id_carrera, c.codigo, c.nombre\r\n" +
                                  "FROM estudiante e\r\n" +
                                  "JOIN lector l ON e.id_lector = l.id_lector\r\n" +
                                  "JOIN usuario u ON l.id_usuario = u.id_usuario\r\n" +
                                  "JOIN carrera c ON l.id_carrera = c.id_carrera\r\n";

                if (FiltroE == "Carnet de identidad")
                {
                    consulta += "WHERE u.ci LIKE @busquedaB ";
                }
                else if (FiltroE == "Nombres")
                {
                    consulta += "WHERE u.nombres LIKE @busquedaB ";
                }
                else if (FiltroE == "Apellido paterno")
                {
                    consulta += "WHERE u.ap_paterno LIKE @busquedaB ";
                }
                else if (FiltroE == "Apellido materno")
                {
                    consulta += "WHERE u.ap_materno LIKE @busquedaB ";
                }
                else if (FiltroE == "Cuenta")
                {
                    consulta += "WHERE u.cuenta LIKE @busquedaB ";
                }

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@busquedaB", $"%{BusquedaE}%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                EstudianteT = dt;
                await cnx.CloseAsync();
            }
        }

        public async void CargarListaCarrera()
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    string query = "SELECT id_carrera, codigo, nombre FROM carrera";
                    MySqlCommand cmd = new MySqlCommand(query, cnx);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        Items = new ObservableCollection<CarreraM>();
                        while (await reader.ReadAsync())
                        {
                            var carreraM = new CarreraM
                            {
                                id_carrera = reader.GetInt32(0),
                                codigo = reader.GetString(1),
                                nombre = reader.GetString(2)
                            };
                            Items.Add(carreraM);
                        }
                    }

                    await cnx.CloseAsync();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    //MODIFICAR ESTUDIANTE
    public partial class EstudianteVM
    {
        private string _idEstudianteSel = "";
        private string _isCISel = "";
        private string _isNombreSel = "";
        private string _isApPaternoSel = "";
        private string _isApMaternoSel = "";
        private DateTime _isFechaNacSel;
        private string _isDireccionSel = "";
        private string _isTelefonoSel = "";
        private string _isCorreoSel = "";
        private string _isCuentaSel = "";
        private string _isContrasenaSel = "";
        //private string _isActivoSel = "";
        private string _isSexoSel = "";
        //private string _isIdBibliotecarioSel = "";
        //private string _isIdUsuarioSel = "";
        private string _isCarreraSel = "";
        private string _isSemetreIngresoSel = "";

        private string _isCIMod = "";
        private string _isNombreMod = "";
        private string _isApPaternoMod = "";
        private string _isApMaternoMod = "";
        private string _isFechaNacMod = "";
        private string _isDireccionMod = "";
        private string _isTelefonoMod = "";
        private string _isCorreoMod = "";
        private string _isCuentaMod = "";
        private string _isContrasenaMod = "";
        //private string _isActivoMod = "";
        private string _isSexoMod = "";
        //private string _isIdBibliotecarioMod = "";
        //private string _isIdUsuarioMod = "";
        private string _isCarreraMod = "";
        private string _isSemestreIngresoMod = "";
        private string _isResultadoMod = "";

        //control de texto correcto; para MODIFICAR
        private bool ciCorrectoMod = true;
        private bool nombreCorrectoMod = true;
        private bool apPaternoCorrectoMod = true;
        private bool apMaternoCorrectoMod = true;
        private bool fechaNacimientoCorrectoMod = true;
        private bool direccionCorrectoMod = true;
        private bool telefonoCorrectoMod = true;
        private bool correoCorrectoMod = true;
        private bool cuentaCorrectoMod = true;
        private bool contrasenaCorrectoMod = true;
        private bool sexoCorrectoMod = true;
        private bool carreraCorrectoMod = true;
        private bool semestreIngresoCorrectoMod = true;

        //control de los textBox enlazados; para MODIFICAR
        public string IsCIVacioMod
        {
            get => _isCIMod;
            set
            {
                _isCIMod = value;
                OnPropertyChanged(nameof(IsCIVacioMod));
            }
        }
        public string IsNombreVacioMod
        {
            get => _isNombreMod;
            set
            {
                _isNombreMod = value;
                OnPropertyChanged(nameof(IsNombreVacioMod));
            }
        }
        public string IsApPaternoVacioMod
        {
            get => _isApPaternoMod;
            set
            {
                _isApPaternoMod = value;
                OnPropertyChanged(nameof(IsApPaternoVacioMod));
            }
        }
        public string IsApMaternoVacioMod
        {
            get => _isApMaternoMod;
            set
            {
                _isApMaternoMod = value;
                OnPropertyChanged(nameof(IsApMaternoVacioMod));
            }
        }
        public string IsFechaVacioMod
        {
            get => _isFechaNacMod;
            set
            {
                _isFechaNacMod = value;
                OnPropertyChanged(nameof(IsFechaVacioMod));
            }
        }
        public string IsDireccionVacioMod
        {
            get => _isDireccionMod;
            set
            {
                _isDireccionMod = value;
                OnPropertyChanged(nameof(IsDireccionVacioMod));
            }
        }
        public string IsTelefonoVacioMod
        {
            get => _isTelefonoMod;
            set
            {
                _isTelefonoMod = value;
                OnPropertyChanged(nameof(IsTelefonoVacioMod));
            }
        }
        public string IsCorreoVacioMod
        {
            get => _isCorreoMod;
            set
            {
                _isCorreoMod = value;
                OnPropertyChanged(nameof(IsCorreoVacioMod));
            }
        }
        public string IsCuentaVacioMod
        {
            get => _isCuentaMod;
            set
            {
                _isCuentaMod = value;
                OnPropertyChanged(nameof(IsCuentaVacioMod));
            }
        }
        public string IsContrasenaVacioMod
        {
            get => _isContrasenaMod;
            set
            {
                _isContrasenaMod = value;
                OnPropertyChanged(nameof(IsContrasenaVacioMod));
            }
        }
        public string IsSexoVacioMod
        {
            get => _isSexoMod;
            set
            {
                _isSexoMod = value;
                OnPropertyChanged(nameof(IsSexoVacioMod));
            }
        }
        public string IsCarreraVacioMod
        {
            get => _isCarreraMod;
            set
            {
                _isCarreraMod = value;
                OnPropertyChanged(nameof(IsCarreraVacioMod));
            }
        }
        public string IsSemestreIngresoVacioMod
        {
            get => _isSemestreIngresoMod;
            set
            {
                _isSemestreIngresoMod = value;
                OnPropertyChanged(nameof(IsSemestreIngresoVacioMod));
            }
        }
        public string IsResultadoMod
        {
            get => _isResultadoMod;
            set
            {
                _isResultadoMod = value;
                OnPropertyChanged(nameof(IsResultadoMod));
            }
        }

        //control de los textbox; para MODIFICAR
        public string CIMod
        {
            get => _isCISel;
            set
            {
                _isCISel = value;
                OnPropertyChanged(nameof(CIMod));

                if (string.IsNullOrEmpty(value))
                {
                    IsCIVacioMod = "No puede estar vacio";
                    ciCorrectoMod = false;
                }
                else if (value == "0")
                {
                    IsCIVacioMod = "El valor no puede ser cero";
                    ciCorrectoMod = false;
                }
                else
                {
                    string patternPositivo = @"^[1-9][0-9]*$"; // Expresión regular que acepta solo números positivos (no cero)
                    string patternNegativo = @"^-"; // Expresión regular que detecta números negativos
                    string patternEspecial = @"[^0-9]"; // Expresión regular que detecta caracteres no numéricos

                    if (Regex.IsMatch(value, patternNegativo))
                    {
                        IsCIVacioMod = "No puede haber campos negativos";
                        ciCorrectoMod = false;
                    }
                    else if (!Regex.IsMatch(value, patternPositivo))
                    {
                        IsCIVacioMod = "El valor debe ser un número positivo";
                        ciCorrectoMod = false;
                    }
                    else if (Regex.IsMatch(value, patternEspecial))
                    {
                        IsCIVacioMod = "No puede contener caracteres especiales";
                        ciCorrectoMod = false;
                    }
                    else
                    {
                        IsCIVacioMod = "";
                        ciCorrectoMod = true;
                    }
                }
            }
        }
        public string NombresMod
        {
            get => _isNombreSel;
            set
            {
                _isNombreSel = value;
                OnPropertyChanged(nameof(NombresMod));

                if (string.IsNullOrEmpty(value))
                {
                    IsNombreVacioMod = "No puede estar vacio";
                    nombreCorrectoMod = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\s]+$"; // Acepta letras y espacios en blanco
                    if (!Regex.IsMatch(value, pattern))
                    {
                        IsNombreVacioMod = "No puede contener números ni caracteres especiales";
                        nombreCorrectoMod = false;
                    }
                    else
                    {
                        IsNombreVacioMod = "";
                        nombreCorrectoMod = true;
                    }
                }

            }
        }
        public string ApPaternoMod
        {
            get => _isApPaternoSel;
            set
            {
                _isApPaternoSel = value;
                OnPropertyChanged(nameof(ApPaternoMod));

                if (string.IsNullOrWhiteSpace(value))
                {
                    IsApPaternoVacioMod = "No puede estar vacio o contener espacios";
                    apPaternoCorrectoMod = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z]+$";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        IsApPaternoVacioMod = "No puede contener números ni caracteres especiales";
                        apPaternoCorrectoMod = false;
                    }
                    else
                    {
                        IsApPaternoVacioMod = "";
                        apPaternoCorrectoMod = true;
                    }
                }
            }
        }
        public string ApMaternoMod
        {
            get => _isApMaternoSel;
            set
            {
                _isApMaternoSel = value;
                OnPropertyChanged(nameof(ApMaternoMod));

                if (string.IsNullOrWhiteSpace(value))
                {
                    IsApMaternoVacioMod = "No puede estar vacio o contener espacios";
                    apMaternoCorrectoMod = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z]+$";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        IsApMaternoVacioMod = "No puede contener números ni caracteres especiales";
                        apMaternoCorrectoMod = false;
                    }
                    else
                    {
                        IsApMaternoVacioMod = "";
                        apMaternoCorrectoMod = true;
                    }
                }
            }
        }
        public DateTime FechaNacimientoMod
        {
            get => _isFechaNacSel;
            set
            {
                _isFechaNacSel = value;
                OnPropertyChanged(nameof(FechaNacimiento));

                if (value > DateTime.Now)
                {
                    IsFechaVacioMod = "La fecha no puede superior al dia de hoy";
                    fechaNacimientoCorrectoMod = false;
                }
                else
                {
                    IsFechaVacioMod = "";
                    fechaNacimientoCorrectoMod = true;
                }
            }
        }
        public string DireccionMod
        {
            get => _isDireccionSel;
            set
            {
                _isDireccionSel = value;
                OnPropertyChanged(nameof(DireccionMod));

                if (String.IsNullOrWhiteSpace(DireccionMod))
                {
                    IsDireccionVacioMod = "No se permite este campo vacio";
                    direccionCorrectoMod = false;
                }
                else
                {
                    IsDireccionVacioMod = "";
                    direccionCorrectoMod = true;
                }
            }
        }
        public string TelefonoMod
        {
            get => _isTelefonoSel;
            set
            {
                _isTelefonoSel = value;
                OnPropertyChanged(nameof(TelefonoMod));

                if (string.IsNullOrEmpty(value))
                {
                    IsTelefonoVacioMod = "No puede estar vacio";
                    telefonoCorrectoMod = false;
                }
                else if (value == "0")
                {
                    IsTelefonoVacioMod = "El valor no puede ser cero";
                    telefonoCorrectoMod = false;
                }
                else
                {
                    string patternPositivo = @"^[1-9][0-9]*$"; // Expresión regular que acepta solo números positivos (no cero)
                    string patternNegativo = @"^-"; // Expresión regular que detecta números negativos
                    string patternEspecial = @"[^0-9]"; // Expresión regular que detecta caracteres no numéricos

                    if (Regex.IsMatch(value, patternNegativo))
                    {
                        IsTelefonoVacioMod = "No puede haber campos negativos";
                        telefonoCorrectoMod = false;
                    }
                    else if (!Regex.IsMatch(value, patternPositivo))
                    {
                        IsTelefonoVacioMod = "El valor debe ser un número positivo";
                        telefonoCorrectoMod = false;
                    }
                    else if (Regex.IsMatch(value, patternEspecial))
                    {
                        IsTelefonoVacioMod = "No puede contener caracteres especiales";
                        telefonoCorrectoMod = false;
                    }
                    else
                    {
                        IsTelefonoVacioMod = "";
                        telefonoCorrectoMod = true;
                    }
                }
            }
        }
        public string CorreoMod
        {
            get => _isCorreoSel;
            set
            {
                _isCorreoSel = value;
                OnPropertyChanged(nameof(CorreoMod));

                if (String.IsNullOrWhiteSpace(value))
                {
                    IsCorreoVacioMod = "Este campo no puede estar vacio";
                    correoCorrectoMod = false;
                }
                else
                {
                    IsCorreoVacioMod = "";
                    correoCorrectoMod = true;
                }
            }
        }
        public string CuentaMod
        {
            get => _isCuentaSel;
            set
            {
                _isCuentaSel = value;
                OnPropertyChanged(nameof(CuentaMod));

                if (String.IsNullOrEmpty(value))
                {
                    IsCuentaVacioMod = "No puede estar vacio";
                    cuentaCorrectoMod = false;
                }
                else
                {
                    IsCuentaVacioMod = "";
                    cuentaCorrectoMod = true;
                }
            }
        }
        public string ContrasenaMod
        {
            get => _isContrasenaSel;
            set
            {
                _isContrasenaSel = value;
                OnPropertyChanged(nameof(ContrasenaMod));

                if (String.IsNullOrEmpty(value))
                {
                    IsContrasenaVacioMod = "No puede estar vacio";
                    contrasenaCorrectoMod = false;
                }
                else
                {
                    IsContrasenaVacioMod = "";
                    contrasenaCorrectoMod = true;
                }
            }
        }
        public string SexoMod
        {
            get => _isSexoSel;
            set
            {
                _isSexoSel = value;
                OnPropertyChanged(nameof(SexoMod));

                if (String.IsNullOrEmpty(value))
                {
                    IsSexoVacioMod = "seleccionar";
                    sexoCorrectoMod = false;
                }
                else
                {
                    IsSexoVacioMod = "";
                    sexoCorrectoMod = true;
                }
            }
        }
        public string CarreraMod
        {
            get => _isCarreraSel;
            set
            {
                _isCarreraSel = value;
                OnPropertyChanged(nameof(CarreraMod));

                if (String.IsNullOrWhiteSpace(value))
                {
                    IsCarreraVacioMod = "El código no puede estar vacío";
                    carreraCorrectoMod = false;
                }
                else if (value.Length > 3)
                {
                    IsCarreraVacioMod = "El código no puede tener más de 3 letras";
                    carreraCorrectoMod = false;
                }
                else if (!Items.Any(item => item.codigo == value))
                {
                    IsCarreraVacioMod = "El código ingresado no existe";
                    carreraCorrectoMod = false;
                }
                else
                {
                    IsCarreraVacioMod = "";
                    carreraCorrectoMod = true;
                }
            }
        }
        public string SemestreIngresoMod
        {
            get => _isSemetreIngresoSel;
            set
            {
                _isSemetreIngresoSel = value;
                OnPropertyChanged(nameof(SemestreIngresoMod));

                if (String.IsNullOrEmpty(value))
                {
                    IsSemestreIngresoVacioMod = "no puede estar vacio";
                    semestreIngresoCorrectoMod = false;
                }
                else
                {
                    string pattern = @"^(I{1,2}/\d{4})$";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        IsSemestreIngresoVacioMod = "Ingrese en Formato II/####";
                        semestreIngresoCorrectoMod = false;
                    }
                    else
                    {
                        IsSemestreIngresoVacioMod = "";
                        semestreIngresoCorrectoMod = true;

                    }
                }
            }
        }
        public string ResultadoMod
        {
            get => _isResultadoMod;
            set
            {
                _isResultadoMod = value;
                OnPropertyChanged(nameof(ResultadoMod));
            }
        }

        private async Task Modificar(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("modificar_estudiante", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_estudiante", Int64.Parse(_idEstudianteSel));
                        cmd.Parameters.AddWithValue("@pci", Int64.Parse(CIMod));
                        cmd.Parameters.AddWithValue("@pnombres", NombresMod);
                        cmd.Parameters.AddWithValue("@pap_paterno", ApPaternoMod);
                        cmd.Parameters.AddWithValue("@pap_materno", ApMaternoMod);
                        cmd.Parameters.AddWithValue("@pfecha_nacimiento", FechaNacimientoMod);
                        cmd.Parameters.AddWithValue("@pdireccion", DireccionMod);
                        cmd.Parameters.AddWithValue("@ptelefono", Int64.Parse(TelefonoMod));
                        cmd.Parameters.AddWithValue("@pcorreo", CorreoMod);
                        cmd.Parameters.AddWithValue("@pcuenta", CuentaMod);
                        if (ContrasenaMod == _anteriorPass)
                        {
                            cmd.Parameters.AddWithValue("@pcontrasena", ContrasenaMod);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@pcontrasena", encriptar.ComputeSha256Hash(ContrasenaMod));
                        }
                        cmd.Parameters.AddWithValue("@psexo", SexoMod);
                        cmd.Parameters.AddWithValue("@pactivo", true);
                        cmd.Parameters.AddWithValue("@pc_codigo", CarreraMod);
                        cmd.Parameters.AddWithValue("@psemestre_ingreso", SemestreIngresoMod);

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;

                        int res = await cmd.ExecuteNonQueryAsync();

                        ResultadoMod = (string)cmd.Parameters["@resultado"].Value;

                        await cnx.CloseAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            //MessageBox.Show($"nombres: {Nombres} \n Apellido paterno: {ApPaterno} \n Apellido materno: {ApMaterno} \n Fechde nacimiento: {FechaNacimiento} \n Direccion: {Direccion} \n Telefono: {Telefono} \n Correo: {Correo} \n Cuenta: {Cuenta} \n Contraseña: {Contrasena} \n Sexo: {Sexo} \n Fecha de contrato: {FechaContrato}");
        }

        private bool PuedeModificar(object parameter)
        {
            return ciCorrectoMod &&
                   nombreCorrectoMod &&
                   apPaternoCorrectoMod &&
                   apMaternoCorrectoMod &&
                   fechaNacimientoCorrectoMod &&
                   direccionCorrectoMod &&
                   telefonoCorrectoMod &&
                   correoCorrectoMod &&
                   cuentaCorrectoMod &&
                   contrasenaCorrectoMod &&
                   sexoCorrectoMod &&
                   carreraCorrectoMod &&
                   semestreIngresoCorrectoMod;
        }
    }


    //REGISTRAR ESTUDIANTE
    public partial class EstudianteVM
    {
        private int _id_carrera;

        //Notify de os Binding del alerta del textbox
        private string _isCIVacio = "";
        private string _isNombreVacio = "";
        private string _isApPaternoVacio = "";
        private string _isApMaternoVacio = "";
        private string _isFechaVacio = "";
        private string _isDireccionVacio = "";
        private string _isTelefonoVacio = "";
        private string _isCorreoVacio = "";
        private string _isCuentaVacio = "";
        private string _isContrasenaVacio = "";
        private string _isSexoVacio = "";
        private string _isCarreraVacio = "";
        private string _isSemestreIngresoVacio = "";

        //control de texto del textbox correcto; para registrar
        private bool ciCorrecto;
        private bool nombreCorrecto;
        private bool apPaternoCorrecto;
        private bool apMaternoCorrecto;
        private bool fechaNacimientoCorrecto;
        private bool direccionCorrecto;
        private bool telefonoCorrecto;
        private bool correoCorrecto;
        private bool cuentaCorrecto;
        private bool contrasenaCorrecto;
        private bool sexoCorrecto;
        private bool carreraCorrecto;
        private bool semestreIngresoCorrecto;

        //Binding para el control de los textBox; para registrar
        public string IsCIVacio
        {
            get => _isCIVacio;
            set
            {
                _isCIVacio = value;
                OnPropertyChanged(nameof(IsCIVacio));
            }
        }
        public string IsNombreVacio
        {
            get => _isNombreVacio;
            set
            {
                _isNombreVacio = value;
                OnPropertyChanged(nameof(IsNombreVacio));
            }
        }
        public string IsApPaternoVacio
        {
            get => _isApPaternoVacio;
            set
            {
                _isApPaternoVacio = value;
                OnPropertyChanged(nameof(IsApPaternoVacio));
            }
        }
        public string IsApMaternoVacio
        {
            get => _isApMaternoVacio;
            set
            {
                _isApMaternoVacio = value;
                OnPropertyChanged(nameof(IsApMaternoVacio));
            }
        }
        public string IsFechaVacio
        {
            get => _isFechaVacio;
            set
            {
                _isFechaVacio = value;
                OnPropertyChanged(nameof(IsFechaVacio));
            }
        }
        public string IsDireccionVacio
        {
            get => _isDireccionVacio;
            set
            {
                _isDireccionVacio = value;
                OnPropertyChanged(nameof(IsDireccionVacio));
            }
        }
        public string IsTelefonoVacio
        {
            get => _isTelefonoVacio;
            set
            {
                _isTelefonoVacio = value;
                OnPropertyChanged(nameof(IsTelefonoVacio));
            }
        }
        public string IsCorreoVacio
        {
            get => _isCorreoVacio;
            set
            {
                _isCorreoVacio = value;
                OnPropertyChanged(nameof(IsCorreoVacio));
            }
        }
        public string IsCuentaVacio
        {
            get => _isCuentaVacio;
            set
            {
                _isCuentaVacio = value;
                OnPropertyChanged(nameof(IsCuentaVacio));
            }
        }
        public string IsContrasenaVacio
        {
            get => _isContrasenaVacio;
            set
            {
                _isContrasenaVacio = value;
                OnPropertyChanged(nameof(IsContrasenaVacio));
            }
        }
        public string IsSexoVacio
        {
            get => _isSexoVacio;
            set
            {
                _isSexoVacio = value;
                OnPropertyChanged(nameof(IsSexoVacio));
            }
        }
        public string IsCarreraVacio
        {
            get => _isCarreraVacio;
            set
            {
                _isCarreraVacio = value;
                OnPropertyChanged(nameof(IsCarreraVacio));
            }
        }
        public string IsSemestreIngresoVacio
        {
            get => _isSemestreIngresoVacio;
            set
            {
                _isSemestreIngresoVacio = value;
                OnPropertyChanged(nameof(IsSemestreIngresoVacio));
            }
        }


        // control de atributos de Bibliotecario; para registrar
        public string CI
        {
            get => _estudiante.ci;
            set
            {
                _estudiante.ci = value;
                OnPropertyChanged(nameof(CI));

                if (string.IsNullOrEmpty(value))
                {
                    IsCIVacio = "No puede estar vacio";
                    ciCorrecto = false;
                }
                else if (value == "0")
                {
                    IsCIVacio = "El valor no puede ser cero";
                    ciCorrecto = false;
                }
                else
                {
                    string patternPositivo = @"^[1-9][0-9]*$"; // Expresión regular que acepta solo números positivos (no cero)
                    string patternNegativo = @"^-"; // Expresión regular que detecta números negativos
                    string patternEspecial = @"[^0-9]"; // Expresión regular que detecta caracteres no numéricos

                    if (Regex.IsMatch(value, patternNegativo))
                    {
                        IsCIVacio = "No puede haber campos negativos";
                        ciCorrecto = false;
                    }
                    else if (!Regex.IsMatch(value, patternPositivo))
                    {
                        IsCIVacio = "El valor debe ser un número positivo";
                        ciCorrecto = false;
                    }
                    else if (Regex.IsMatch(value, patternEspecial))
                    {
                        IsCIVacio = "No puede contener caracteres especiales";
                        ciCorrecto = false;
                    }
                    else
                    {
                        IsCIVacio = "";
                        ciCorrecto = true;

                        if (!string.IsNullOrEmpty(_estudiante.nombres))
                        {
                            // Obtiene el primer nombre y lo convierte a minúsculas
                            string primerNombre = _estudiante.nombres.Split(' ')[0].ToLower();

                            Contrasena = primerNombre + _estudiante.ci;
                        }
                    }
                }
            }
        }
        public string Nombres
        {
            get => _estudiante.nombres;
            set
            {
                _estudiante.nombres = value;
                OnPropertyChanged(nameof(Nombres));

                if (string.IsNullOrEmpty(value))
                {
                    IsNombreVacio = "No puede estar vacio o contener espacios";
                    nombreCorrecto = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z\s]+$";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        IsNombreVacio = "No puede contener números ni caracteres especiales";
                        nombreCorrecto = false;
                    }
                    else
                    {
                        IsNombreVacio = "";
                        nombreCorrecto = true;

                        string primerNombre = _estudiante.nombres.Split(' ')[0].ToLower();

                        if (!string.IsNullOrEmpty(_estudiante.apPaterno))
                        {
                            string primerApellido = _estudiante.apPaterno.ToLower();
                            Cuenta = primerNombre + "." + primerApellido + "@sistemas.edu.bo";
                        }

                        Contrasena = primerNombre + _estudiante.ci;
                    }
                }


            }
        }
        public string ApPaterno
        {

            get => _estudiante.apPaterno;
            set
            {
                _estudiante.apPaterno = value;
                OnPropertyChanged(nameof(ApPaterno));

                if (string.IsNullOrWhiteSpace(value))
                {
                    IsApPaternoVacio = "No puede estar vacio o contener espacios";
                    apPaternoCorrecto = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z]+$";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        IsApPaternoVacio = "No puede contener números ni caracteres especiales";
                        apPaternoCorrecto = false;
                    }
                    else
                    {
                        IsApPaternoVacio = "";
                        apPaternoCorrecto = true;

                        if (_estudiante.nombres != null && _estudiante.apPaterno != null)
                        {
                            Cuenta = _estudiante.nombres.Split(' ')[0].ToLower() + "." + _estudiante.apPaterno.ToLower() + "@sistemas.edu.bo";
                        }
                    }
                }
            }
        }
        public string ApMaterno
        {
            get => _estudiante.apMaterno;
            set
            {
                _estudiante.apMaterno = value;
                OnPropertyChanged(nameof(ApMaterno));

                if (string.IsNullOrWhiteSpace(value))
                {
                    IsApMaternoVacio = "No puede estar vacio o contener espacios";
                    apMaternoCorrecto = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z]+$";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        IsApMaternoVacio = "No puede contener números ni caracteres especiales";
                        apMaternoCorrecto = false;
                    }
                    else
                    {
                        IsApMaternoVacio = "";
                        apMaternoCorrecto = true;
                    }
                }
            }
        }
        public DateTime FechaNacimiento
        {
            get => _estudiante.fechaNacimiento;
            set
            {
                _estudiante.fechaNacimiento = value;
                OnPropertyChanged(nameof(FechaNacimiento));

                if (value > DateTime.Now)
                {
                    IsFechaVacio = "La fecha no puede superior al dia de hoy";
                    fechaNacimientoCorrecto = false;
                }
                else
                {
                    IsFechaVacio = "";
                    fechaNacimientoCorrecto = true;
                }
            }
        }
        public string Direccion
        {
            get => _estudiante.direccion;
            set
            {
                _estudiante.direccion = value;
                OnPropertyChanged(nameof(Direccion));

                if (String.IsNullOrWhiteSpace(Direccion))
                {
                    IsDireccionVacio = "No se permite este campo vacio";
                    direccionCorrecto = false;
                }
                else
                {
                    IsDireccionVacio = "";
                    direccionCorrecto |= true;
                }
            }
        }
        public string Telefono
        {
            get => _estudiante.telefono;
            set
            {
                _estudiante.telefono = value;
                OnPropertyChanged(nameof(Telefono));

                if (string.IsNullOrEmpty(value))
                {
                    IsTelefonoVacio = "No puede estar vacio";
                    telefonoCorrecto = false;
                }
                else if (value == "0")
                {
                    IsTelefonoVacio = "El valor no puede ser cero";
                    telefonoCorrecto = false;
                }
                else
                {
                    string patternPositivo = @"^[1-9][0-9]*$"; // Expresión regular que acepta solo números positivos (no cero)
                    string patternNegativo = @"^-"; // Expresión regular que detecta números negativos
                    string patternEspecial = @"[^0-9]"; // Expresión regular que detecta caracteres no numéricos

                    if (Regex.IsMatch(value, patternNegativo))
                    {
                        IsTelefonoVacio = "No puede haber campos negativos";
                        telefonoCorrecto = false;
                    }
                    else if (!Regex.IsMatch(value, patternPositivo))
                    {
                        IsTelefonoVacio = "El valor debe ser un número positivo";
                        telefonoCorrecto = false;
                    }
                    else if (Regex.IsMatch(value, patternEspecial))
                    {
                        IsTelefonoVacio = "No puede contener caracteres especiales";
                        telefonoCorrecto = false;
                    }
                    else
                    {
                        IsTelefonoVacio = "";
                        telefonoCorrecto = true;
                    }
                }
            }
        }
        public string Correo
        {
            get => _estudiante.correo;
            set
            {
                _estudiante.correo = value;
                OnPropertyChanged(nameof(Correo));

                if (String.IsNullOrWhiteSpace(value))
                {
                    IsCorreoVacio = "Este campo no puede estar vacio";
                    correoCorrecto = false;
                }
                else
                {
                    IsCorreoVacio = "";
                    correoCorrecto = true;
                }
            }
        }
        public string Cuenta
        {
            get => _estudiante.cuenta;
            set
            {
                _estudiante.cuenta = value;
                OnPropertyChanged(nameof(Cuenta));

                if (!String.IsNullOrEmpty(value))
                {
                    IsCuentaVacio = "campo generado automaticamente";
                    cuentaCorrecto = true;
                }
                else
                {
                    IsCuentaVacio = "";
                    cuentaCorrecto = false;
                }
            }
        }
        public string Contrasena
        {
            get => _estudiante.contrasena;
            set
            {
                _estudiante.contrasena = value;
                OnPropertyChanged(nameof(Contrasena));

                if (!String.IsNullOrEmpty(value))
                {
                    IsContrasenaVacio = "campo generado automaticamente";
                    contrasenaCorrecto = true;
                }
                else
                {
                    IsContrasenaVacio = "";
                    contrasenaCorrecto = false;
                }
            }
        }
        public string Sexo
        {
            get => _estudiante.sexo;
            set
            {
                _estudiante.sexo = value;
                OnPropertyChanged(nameof(Sexo));

                if (String.IsNullOrEmpty(value))
                {
                    IsSexoVacio = "seleccionar";
                    sexoCorrecto = false;
                }
                else
                {
                    IsSexoVacio = "";
                    sexoCorrecto = true;
                }
            }
        }
        public string Carrera
        {
            get => _estudiante.carrera;
            set
            {
                _estudiante.carrera = value;
                OnPropertyChanged(nameof(Carrera));
                if (String.IsNullOrEmpty(value))
                {
                    IsCarreraVacio = "seleccionar";
                    carreraCorrecto = false;
                }
                else
                {
                    IsCarreraVacio = "";
                    carreraCorrecto = true;
                }
            }
        }
        public string SemestreIngreso
        {
            get => _estudiante.ingreso;
            set
            {
                _estudiante.ingreso = value;
                OnPropertyChanged(nameof(SemestreIngreso));
                if (String.IsNullOrEmpty(value))
                {
                    IsSemestreIngresoVacio = "no puede estar vacio";
                    semestreIngresoCorrecto = false;
                }
                else
                {
                    string pattern = @"^(I{1,2}/\d{4})$";
                    if (!Regex.IsMatch(value, pattern))
                    {
                        IsSemestreIngresoVacio = "Ingrese en Formato II/####";
                        semestreIngresoCorrecto = false;
                    }
                    else
                    {

                        IsSemestreIngresoVacio = "";
                        semestreIngresoCorrecto = true;

                    }
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

        private async Task Registrar(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("registrar_estudiante", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pci", Int64.Parse(CI));
                        cmd.Parameters.AddWithValue("@pnombres", Nombres);
                        cmd.Parameters.AddWithValue("@pap_paterno", ApPaterno);
                        cmd.Parameters.AddWithValue("@pap_materno", ApMaterno);
                        cmd.Parameters.AddWithValue("@pfecha_nacimiento", FechaNacimiento);
                        cmd.Parameters.AddWithValue("@pdireccion", Direccion);
                        cmd.Parameters.AddWithValue("@ptelefono", Int64.Parse(Telefono));
                        cmd.Parameters.AddWithValue("@pcorreo", Correo);
                        cmd.Parameters.AddWithValue("@pcuenta", Cuenta);
                        cmd.Parameters.AddWithValue("@pcontrasena", encriptar.ComputeSha256Hash(Contrasena));
                        cmd.Parameters.AddWithValue("@psexo", Sexo);
                        cmd.Parameters.AddWithValue("@psemestre_ingreso", SemestreIngreso);
                        cmd.Parameters.AddWithValue("@pid_carrera", _id_carrera);

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

        private bool PuedeRegistrar(object parameter)
        {
            return ciCorrecto &&
                   nombreCorrecto &&
                   apPaternoCorrecto &&
                   apMaternoCorrecto &&
                   fechaNacimientoCorrecto &&
                   direccionCorrecto &&
                   telefonoCorrecto &&
                   correoCorrecto &&
                   cuentaCorrecto &&
                   contrasenaCorrecto &&
                   sexoCorrecto &&
                   carreraCorrecto &&
                   semestreIngresoCorrecto;
        }

    }


    //ELIMINAR ESTUDIANTE
    public partial class EstudianteVM
    {
        private bool elementoElimCorrecto;
        private string _resultadoEliminacion;

        public string ElementoSelEliminarE
        {
            get => _idEstudianteSel;
            set
            {
                _idEstudianteSel = value;
                OnPropertyChanged(nameof(ElementoSelEliminarE));

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
        public string ResultadoEliminacionE
        {
            get => _resultadoEliminacion;
            set
            {
                _resultadoEliminacion = value;
                OnPropertyChanged(nameof(ResultadoEliminacionE));
            }
        }


        private async Task Eliminar(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("eliminar_estudiante", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_estudiante", Int64.Parse(ElementoSelEliminarE));

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        ResultadoEliminacionE = (string)cmd.Parameters["@resultado"].Value;

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
