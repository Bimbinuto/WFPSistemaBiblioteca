using Biblioteca.BDConexion;
using Biblioteca.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Input;
using System.Timers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Text.RegularExpressions;

namespace Biblioteca.ModeloDeVista
{
    public partial class DocenteVM : INotifyPropertyChanged
    {
        private DocenteM _docente = new DocenteM();
        public ICommand RegistrarCommand { get; set; }
        public ICommand ModificarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private System.Timers.Timer _timerBusquedaD;
        private EncriptadoUnico encriptar = new EncriptadoUnico();

        private Conexion conexion = new Conexion();
        public string _resultado;

        private string _filtroD = "Nombres";
        private string _busquedaD;
        private DataTable _docenteT;
        private ObservableCollection<CarreraM> _items;
        private CarreraM _elementoItem;
        private DataRowView _filaSeleccionadaD;
        private string _elementosSeleccionadosD;
        private string _anteriorPass;

        public DataTable DocenteT
        {
            get => _docenteT;
            set
            {
                _docenteT = value;
                OnPropertyChanged(nameof(DocenteT));
            }
        }
        public string FiltroD
        {
            get => _filtroD;
            set
            {
                _filtroD = value;
                OnPropertyChanged(nameof(FiltroD));
            }
        }
        public string BusquedaD
        {
            get => _busquedaD;
            set
            {
                _busquedaD = value;
                OnPropertyChanged(nameof(BusquedaD));
                _timerBusquedaD.Stop();
                _timerBusquedaD.Start();
            }
        }
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
        public DataRowView FilaSeleccionadaD
        {
            get => _filaSeleccionadaD;
            set
            {
                _filaSeleccionadaD = value;
                OnPropertyChanged(nameof(FilaSeleccionadaD));

                if (value != null)
                {
                    _idDocenteSel = value["id_docente"].ToString();
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
                    _isGradoAcademicoSel = value["grado_academico"].ToString();
                    _isCodigoCarreraSel = value["codigo"].ToString();

                    _anteriorPass = _isContrasenaSel;

                    ElementosSeleccionadosD = $"Elementos seleccionados: \n" +
                                              $"ID: {_idDocenteSel} \n" +
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
                                              $"Grado academico: {_isGradoAcademicoSel}\n" +
                                              $"ID carrera: {value["id_carrera"].ToString()}\n" +
                                              $"Codigo: {_isCodigoCarreraSel}\n" +
                                              $"Nombre carrera: {value["nombre"].ToString()}\n";
                }
            }
        }
        public string ElementosSeleccionadosD
        {
            get => _elementosSeleccionadosD;
            set
            {
                _elementosSeleccionadosD = value;
                OnPropertyChanged("ElementosSeleccionadosD");
            }
        }

        public DocenteVM()
        {
            FechaNacimiento = new DateTime(2000, 1, 1);

            RegistrarCommand = new AsyncRelayCommand(Registrar, PuedeRegistrar);
            ModificarCommand = new AsyncRelayCommand(Modificar, PuedeModificar);
            EliminarCommand = new AsyncRelayCommand(Eliminar, PuedeEliminar);

            _timerBusquedaD = new System.Timers.Timer(650);
            _timerBusquedaD.Elapsed += (sender, e) => Task.Run(() => CargarTablaDocentesAsync());
            _timerBusquedaD.AutoReset = false;

            CargarListaCarrera();
            CargarTablaDocentesAsync();
        }

        private async void CargarTablaDocentesAsync()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT d.id_docente, u.ci, u.nombres, u.ap_paterno, u.ap_materno, u.fecha_nacimiento, u.direccion, u.telefono, u.correo, u.cuenta, u.constrasena, u.sexo, d.grado_academico, l.id_carrera, c.codigo, c.nombre\r\n" +
                                  "FROM docente d\r\n" +
                                  "JOIN lector l ON d.id_lector = l.id_lector\r\n" +
                                  "JOIN usuario u ON l.id_usuario = u.id_usuario\r\n" +
                                  "JOIN carrera c ON l.id_carrera = c.id_carrera\r\n";

                if (FiltroD == "Carnet de identidad")
                {
                    consulta += "WHERE u.ci LIKE @busquedaD ";
                }
                else if (FiltroD == "Nombres")
                {
                    consulta += "WHERE u.nombres LIKE @busquedaD ";
                }
                else if (FiltroD == "Apellido paterno")
                {
                    consulta += "WHERE u.ap_paterno LIKE @busquedaD ";
                }
                else if (FiltroD == "Apellido materno")
                {
                    consulta += "WHERE u.ap_materno LIKE @busquedaD ";
                }
                else if (FiltroD == "Cuenta")
                {
                    consulta += "WHERE u.cuenta LIKE @busquedaD ";
                }
                else if (FiltroD == "Codigo de carrera")
                {
                    consulta += "WHERE c.codigo LIKE @busquedaD ";
                }

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@busquedaD", $"%{BusquedaD}%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                DocenteT = dt;
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

    //MODIFICAR DOCENTE
    public partial class DocenteVM
    {
        private string _idDocenteSel = "";
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
        private string _isSexoSel = "";
        //private string _isActivoSel = "";
        private string _isCodigoCarreraSel = "";
        private string _isGradoAcademicoSel = "";

        //control de los textbox del docente; para MODIFICAR
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
        private string _isSexoMod = "";
        private string _isGradAcadMod = "";

        private string _isCodCarrera = "";

        private string _isResultadoMod = "";

        //control de los textBox enlazados; para MODIFICAR
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
        private bool gradoAcademicoCorrectoMod = true;

        private bool codigoCarreraCorrectoMod = true;

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
        public string IsGradoAcademicoVacioMod
        {
            get => _isGradAcadMod;
            set
            {
                _isGradAcadMod = value;
                OnPropertyChanged(nameof(IsGradoAcademicoVacioMod));
            }
        }
        public string IsCodigoCarreraVacioMod
        {
            get => _isCodCarrera;
            set
            {
                _isCodCarrera = value;
                OnPropertyChanged(nameof(IsCodigoCarreraVacioMod));
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
        public string GradoAcademicoMod
        {
            get => _isGradoAcademicoSel;
            set
            {
                _isGradoAcademicoSel = value;
                OnPropertyChanged(nameof(GradoAcademico));

                if (String.IsNullOrEmpty(value))
                {
                    IsGradoAcademicoVacioMod = "seleccionar";
                    gradoAcademicoCorrectoMod = false;
                }
                else
                {
                    IsGradoAcademicoVacioMod = "";
                    gradoAcademicoCorrectoMod = true;
                }
            }
        }
        public string CodigoCarreraMod
        {
            get => _isCodigoCarreraSel;
            set
            {
                _isCodigoCarreraSel = value;
                OnPropertyChanged(nameof(CodigoCarreraMod));
                if (String.IsNullOrWhiteSpace(value))
                {
                    IsCodigoCarreraVacioMod = "El código no puede estar vacío";
                    codigoCarreraCorrectoMod = false;
                }
                else if (value.Length > 3)
                {
                    IsCodigoCarreraVacioMod = "El código no puede tener más de 3 letras";
                    codigoCarreraCorrectoMod = false;
                }
                //else if (!Regex.IsMatch(value, @"^[A-Z]+$"))
                //{
                //    IsCodigoCarreraVacioMod = "El código solo puede contener letras mayúsculas";
                //    codigoCarreraCorrectoMod = false;
                //}
                else if (!Items.Any(item => item.codigo == value))
                {
                    IsCodigoCarreraVacioMod = "El código ingresado no existe";
                    codigoCarreraCorrectoMod = false;
                }
                else
                {
                    IsCodigoCarreraVacioMod = "";
                    codigoCarreraCorrectoMod = true;
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

                    using (MySqlCommand cmd = new MySqlCommand("modificar_docente", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_docente", Int64.Parse(_idDocenteSel));
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
                        cmd.Parameters.AddWithValue("@pc_codigo", CodigoCarreraMod);
                        cmd.Parameters.AddWithValue("@pgrado_academico", GradoAcademicoMod);

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
                   codigoCarreraCorrectoMod &&
                   gradoAcademicoCorrectoMod;
        }

        public void probarDatos()
        {
            StringBuilder itemsString = new StringBuilder();
            foreach (var item in Items)
            {
                itemsString.AppendLine($"ID: {item.id_carrera}, Nombre: {item.nombre}, Código: {item.codigo}");
            }

            // Mostrar la cadena en un MessageBox
            MessageBox.Show(itemsString.ToString());
        }
    }

    //REGISTRAR DOCENTE NUEVO
    public partial class DocenteVM
    {
        private int _id_carrera;

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
        private string _isGradoAcademicoVacio = "";
        private string _isCarreraVacio = "";
        private string _isCodigoCarrera = "";

        //control de texto correcto; para registrar
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
        private bool gradoAcademicoCorrecto;
        private bool carreraCorrecto;

        //control de los textBox enlazados; para registrar
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
        public string IsGradoAcademicoVacio
        {
            get => _isGradoAcademicoVacio;
            set
            {
                _isGradoAcademicoVacio = value;
                OnPropertyChanged(nameof(IsGradoAcademicoVacio));
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
        public string IsCodigoCarrera
        {
            get => _isCodigoCarrera;
            set
            {
                _isCodigoCarrera = value;
                OnPropertyChanged(nameof(IsCodigoCarrera));
            }
        }

        public string CI
        {
            get => _docente.ci;
            set
            {
                _docente.ci = value;
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

                        if (!string.IsNullOrEmpty(_docente.nombres))
                        {
                            // Obtiene el primer nombre y lo convierte a minúsculas
                            string primerNombre = _docente.nombres.Split(' ')[0].ToLower();

                            Contrasena = primerNombre + _docente.ci;
                        }
                    }
                }
            }
        }
        public string Nombres
        {
            get => _docente.nombres;
            set
            {
                _docente.nombres = value;
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

                        string primerNombre = _docente.nombres.Split(' ')[0].ToLower();

                        if (!string.IsNullOrEmpty(_docente.apPaterno))
                        {
                            string primerApellido = _docente.apPaterno.ToLower();
                            Cuenta = primerNombre + "." + primerApellido + "@sistemas.edu.bo";
                        }

                        Contrasena = primerNombre + _docente.ci;
                    }
                }


            }
        }
        public string ApPaterno
        {

            get => _docente.apPaterno;
            set
            {
                _docente.apPaterno = value;
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

                        if (_docente.nombres != null && _docente.apPaterno != null)
                        {
                            Cuenta = _docente.nombres.Split(' ')[0].ToLower() + "." + _docente.apPaterno.ToLower() + "@sistemas.edu.bo";
                        }
                    }
                }
            }
        }
        public string ApMaterno
        {
            get => _docente.apMaterno;
            set
            {
                _docente.apMaterno = value;
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
            get => _docente.fechaNacimiento;
            set
            {
                _docente.fechaNacimiento = value;
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
            get => _docente.direccion;
            set
            {
                _docente.direccion = value;
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
            get => _docente.telefono;
            set
            {
                _docente.telefono = value;
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
            get => _docente.correo;
            set
            {
                _docente.correo = value;
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
            get => _docente.cuenta;
            set
            {
                _docente.cuenta = value;
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
            get => _docente.contrasena;
            set
            {
                _docente.contrasena = value;
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
            get => _docente.sexo;
            set
            {
                _docente.sexo = value;
                OnPropertyChanged(nameof(Sexo));

                if (String.IsNullOrEmpty(value))
                {
                    IsSexoVacio = "falta seleccionar";
                    sexoCorrecto = false;
                }
                else
                {
                    IsSexoVacio = "";
                    sexoCorrecto = true;
                }

            }
        }
        public string GradoAcademico
        {
            get => _docente.gradoAcademico;
            set
            {
                _docente.gradoAcademico = value;
                OnPropertyChanged(nameof(GradoAcademico));

                if (String.IsNullOrEmpty(value))
                {
                    IsGradoAcademicoVacio = "falta seleccionar";
                    gradoAcademicoCorrecto = false;
                }
                else
                {
                    IsGradoAcademicoVacio = "";
                    gradoAcademicoCorrecto = true;
                }
            }
        }
        public string Carrera
        {
            get => _docente.carrera;
            set
            {
                _docente.carrera = value;
                OnPropertyChanged(nameof(Carrera));

                if (String.IsNullOrEmpty(value))
                {
                    IsCarreraVacio = "falta seleccionar";
                    carreraCorrecto = false;
                }
                else
                {
                    IsCarreraVacio = "";
                    carreraCorrecto = true;
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

                    using (MySqlCommand cmd = new MySqlCommand("registrar_docente", cnx))
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
                        cmd.Parameters.AddWithValue("@pgrado_academico", GradoAcademico);
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
                   gradoAcademicoCorrecto &&
                   carreraCorrecto;
        }
    }

    //ELIMINAR DOCENTE
    public partial class DocenteVM
    {
        private bool elementoElimCorrecto;
        private string _resultadoEliminacion;

        public string ElementoSelEliminarD
        {
            get => _idDocenteSel;
            set
            {
                _idDocenteSel = value;
                OnPropertyChanged(nameof(ElementoSelEliminarD));

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
        public string ResultadoEliminacionD
        {
            get => _resultadoEliminacion;
            set
            {
                _resultadoEliminacion = value;
                OnPropertyChanged(nameof(ResultadoEliminacionD));
            }
        }


        private async Task Eliminar(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("eliminar_docente", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_docente", Int64.Parse(ElementoSelEliminarD));

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        ResultadoEliminacionD = (string)cmd.Parameters["@resultado"].Value;

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
