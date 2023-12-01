using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Timers;
using Biblioteca.BDConexion;
using Biblioteca.Modelos;
using MySql.Data.MySqlClient;
using Biblioteca.Vistas.Registros;

namespace Biblioteca.ModeloDeVista
{
    //CLASE PRINCIPAL
    public partial class BibliotecarioVM : INotifyPropertyChanged
    {
        private BibliotecarioM _biblibiotecario = new BibliotecarioM();
        public ICommand RegistrarCommand { get; set; }
        public ICommand ModificarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private Timer _timerBusqueda;

        private Conexion conexion = new Conexion();
        public string _resultado;

        private string _filtroB = "Nombres";
        private string _busquedaB;
        private DataTable _bibliotecarioT;
        private DataRowView _filaSeleccionadaB;
        private string _elementosSeleccionadosB;

        public string FiltroB
        {
            get => _filtroB;
            set
            {
                _filtroB = value;
                OnPropertyChanged(nameof(FiltroB));
            }
        }
        public string BusquedaB
        {
            get => _busquedaB;
            set
            {
                _busquedaB = value;
                OnPropertyChanged(nameof(BusquedaB));
                //CargarTablaBibliotecariosAsync();
                _timerBusqueda.Stop();
                _timerBusqueda.Start();
            }
        }
        public DataTable BibliotecarioT
        {
            get => _bibliotecarioT;
            set
            {
                _bibliotecarioT = value;
                OnPropertyChanged(nameof(BibliotecarioT));
            }
        }
        public DataRowView FilaSeleccionadaB
        {
            get => _filaSeleccionadaB;
            set
            {
                _filaSeleccionadaB = value;
                OnPropertyChanged(nameof(FilaSeleccionadaB));

                if (value != null)
                {
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
                    _isActivoSel = value["activo"].ToString();
                    _isIdBibliotecarioSel = value["id_bibliotecario"].ToString();
                    _isIdUsuarioSel = value["id_usuario"].ToString();
                    _isFecConSel = Convert.ToDateTime(value["fecha_contrato"]);

                    ElementosSeleccionadosB = $"Elementos seleccionados: \n" +
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
                                              $"Activo: {_isActivoSel} \n" +
                                              $"ID Bibliotecario: {_isIdBibliotecarioSel} \n" +
                                              $"ID Usuario: {_isIdUsuarioSel} \n" +
                                              $"Fecha de Contrato: {_isFecConSel}"; // +$"Elementos 0: {Elementos[0]}";
                }
            }
        }
        public string ElementosSeleccionadosB
        {
            get => _elementosSeleccionadosB;
            set
            {
                _elementosSeleccionadosB = value;
                OnPropertyChanged("ElementosSeleccionadosB");
            }
        }

        // CONSTRUCTOR
        public BibliotecarioVM()
        {
            FechaNacimiento = new DateTime(2000, 1, 1);
            FechaContrato = new DateTime(2000, 1, 1);

            RegistrarCommand = new AsyncRelayCommand(Registrar, PuedeRegistrar);
            ModificarCommand = new AsyncRelayCommand(Modificar, PuedeModificar);
            EliminarCommand = new AsyncRelayCommand(Eliminar, PuedeEliminar);

            _timerBusqueda = new System.Timers.Timer(650);
            _timerBusqueda.Elapsed += (sender, e) => Task.Run(() => CargarTablaBibliotecariosAsync());
            _timerBusqueda.AutoReset = false;

            CargarTablaBibliotecariosAsync();
        }

        private async void CargarTablaBibliotecariosAsync()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT u.ci, u.nombres, u.ap_paterno, u.ap_materno, u.fecha_nacimiento, u.direccion, u.telefono, u.correo, u.cuenta, u.constrasena, u.sexo, u.activo, b.id_bibliotecario, b.id_usuario, b.fecha_contrato\r\n" +
                                  "FROM bibliotecario b\r\n" +
                                  "JOIN usuario u ON b.id_usuario = u.id_usuario\r\n";

                if (FiltroB == "Carnet de identidad")
                {
                    consulta += "WHERE u.ci LIKE @busquedaB ";
                }
                else if (FiltroB == "Nombres")
                {
                    consulta += "WHERE u.nombres LIKE @busquedaB ";
                }
                else if (FiltroB == "Apellido paterno")
                {
                    consulta += "WHERE u.ap_paterno LIKE @busquedaB ";
                }
                else if (FiltroB == "Apellido materno")
                {
                    consulta += "WHERE u.ap_materno LIKE @busquedaB ";
                }
                else if (FiltroB == "Cuenta")
                {
                    consulta += "WHERE u.cuenta LIKE @busquedaB ";
                }

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@busquedaB", $"%{BusquedaB}%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                BibliotecarioT = dt;
                await cnx.CloseAsync();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //MODIFICAR UN BIBLIOTECARIO
    public partial class BibliotecarioVM
    {
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
        private string _isActivoSel = "";
        private string _isSexoSel = "";
        private string _isIdBibliotecarioSel = "";
        private string _isIdUsuarioSel = "";
        private DateTime _isFecConSel;

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
        private string _isActivoMod = "";
        private string _isSexoMod = "";
        private string _isIdBibliotecarioMod = "";
        private string _isIdUsuarioMod = "";
        private string _isFecConMod = "";
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
        private bool fechaContratoCorrectoMod = true;

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
        public string IsFecConVacioMod
        {
            get => _isFecConMod;
            set
            {
                _isFecConMod = value;
                OnPropertyChanged(nameof(IsFecConVacioMod));
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
        public DateTime FechaContratoMod
        {
            get => _isFecConSel;
            set
            {
                _isFecConSel = value;
                OnPropertyChanged(nameof(FechaContratoMod));

                if (value > DateTime.Now)
                {
                    IsFecConVacioMod = "La fecha no puede ser mayor a la actual";
                    fechaContratoCorrectoMod = false;
                }
                else
                {
                    IsFecConVacioMod = "";
                    fechaContratoCorrectoMod = true;
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

                    using (MySqlCommand cmd = new MySqlCommand("modificar_bibliotecario", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_bibliotecario", Int64.Parse(_isIdBibliotecarioSel));
                        cmd.Parameters.AddWithValue("@pci", Int64.Parse(CIMod));
                        cmd.Parameters.AddWithValue("@pnombres", NombresMod);
                        cmd.Parameters.AddWithValue("@pap_paterno", ApPaternoMod);
                        cmd.Parameters.AddWithValue("@pap_materno", ApMaternoMod);
                        cmd.Parameters.AddWithValue("@pfecha_nacimiento", FechaNacimientoMod);
                        cmd.Parameters.AddWithValue("@pdireccion", DireccionMod);
                        cmd.Parameters.AddWithValue("@ptelefono", Int64.Parse(TelefonoMod));
                        cmd.Parameters.AddWithValue("@pcorreo", CorreoMod);
                        cmd.Parameters.AddWithValue("@pcuenta", CuentaMod);
                        cmd.Parameters.AddWithValue("@pcontrasena", ContrasenaMod);
                        cmd.Parameters.AddWithValue("@psexo", SexoMod);
                        cmd.Parameters.AddWithValue("@pactivo", true);
                        cmd.Parameters.AddWithValue("@pfecha_contrato", FechaContratoMod);

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;

                        int res = await cmd.ExecuteNonQueryAsync();

                        if (res != 0)
                        {
                            ResultadoMod = (string)cmd.Parameters["@resultado"].Value;
                        }
                        else
                        {
                            ResultadoMod = (string)cmd.Parameters["@resultado"].Value;
                        }
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
                   fechaContratoCorrectoMod;
        }

    }


    //REGISTRAR NUEVO BIBLIOTECARIO
    public partial class BibliotecarioVM
    {
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
        private string _isFecConVacio = "";

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
        private bool fechaContratoCorrecto;

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
        public string IsFecConVacio
        {
            get => _isFecConVacio;
            set
            {
                _isFecConVacio = value;
                OnPropertyChanged(nameof(IsFecConVacio));
            }
        }

        // control de atributos de Bibliotecario; para registrar
        public string CI
        {
            get => _biblibiotecario.ci;
            set
            {
                _biblibiotecario.ci = value;
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

                        if (!string.IsNullOrEmpty(_biblibiotecario.nombres))
                        {
                            // Obtiene el primer nombre y lo convierte a minúsculas
                            string primerNombre = _biblibiotecario.nombres.Split(' ')[0].ToLower();

                            Contrasena = primerNombre + _biblibiotecario.ci;
                        }
                    }
                }
            }
        }
        public string Nombres
        {
            get => _biblibiotecario.nombres;
            set
            {
                _biblibiotecario.nombres = value;
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

                        string primerNombre = _biblibiotecario.nombres.Split(' ')[0].ToLower();

                        if (!string.IsNullOrEmpty(_biblibiotecario.apPaterno))
                        {
                            string primerApellido = _biblibiotecario.apPaterno.ToLower();
                            Cuenta = primerNombre + "." + primerApellido + "@sistemas.edu.bo";
                        }

                        Contrasena = primerNombre + _biblibiotecario.ci;
                    }
                }


            }
        }
        public string ApPaterno
        {

            get => _biblibiotecario.apPaterno;
            set
            {
                _biblibiotecario.apPaterno = value;
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

                        if (_biblibiotecario.nombres != null && _biblibiotecario.apPaterno != null)
                        {
                            Cuenta = _biblibiotecario.nombres.Split(' ')[0].ToLower() + "." + _biblibiotecario.apPaterno.ToLower() + "@sistemas.edu.bo";
                        }
                    }
                }
            }
        }
        public string ApMaterno
        {
            get => _biblibiotecario.apMaterno;
            set
            {
                _biblibiotecario.apMaterno = value;
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
            get => _biblibiotecario.fechaNacimiento;
            set
            {
                _biblibiotecario.fechaNacimiento = value;
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
            get => _biblibiotecario.direccion;
            set
            {
                _biblibiotecario.direccion = value;
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
            get => _biblibiotecario.telefono;
            set
            {
                _biblibiotecario.telefono = value;
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
            get => _biblibiotecario.correo;
            set
            {
                _biblibiotecario.correo = value;
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
            get => _biblibiotecario.cuenta;
            set
            {
                _biblibiotecario.cuenta = value;
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
            get => _biblibiotecario.contrasena;
            set
            {
                _biblibiotecario.contrasena = value;
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
            get => _biblibiotecario.sexo;
            set
            {
                _biblibiotecario.sexo = value;
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
        public DateTime FechaContrato
        {
            get => _biblibiotecario.fechaContrato;
            set
            {
                _biblibiotecario.fechaContrato = value;
                OnPropertyChanged(nameof(FechaContrato));

                if (value > DateTime.Now)
                {
                    IsFecConVacio = "La fecha no puede ser mayor a la actual";
                    fechaContratoCorrecto = false;
                }
                else
                {
                    IsFecConVacio = "";
                    fechaContratoCorrecto = true;
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

                    using (MySqlCommand cmd = new MySqlCommand("registrar_bibliotecario", cnx))
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
                        cmd.Parameters.AddWithValue("@pcontrasena", Contrasena);
                        cmd.Parameters.AddWithValue("@psexo", Sexo);
                        cmd.Parameters.AddWithValue("@pfecha_contrato", FechaContrato);

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;

                        int res = await cmd.ExecuteNonQueryAsync();

                        if (res == 0)
                        {
                            Resultado = (string)cmd.Parameters["@resultado"].Value;
                        }
                        else
                        {
                            Resultado = (string)cmd.Parameters["@resultado"].Value;
                        }
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
                   fechaContratoCorrecto;
        }
    }


    //ELIMINAR BIBLIOTECARIO
    public partial class BibliotecarioVM
    {
        private string _elementoSelEliminarB;
        private bool elementoElimCorrecto;
        private string _resultadoEliminacion;

        public string ElementoSelEliminarB
        {
            get => _isIdBibliotecarioSel;
            set
            {
                _isIdBibliotecarioSel = value;
                OnPropertyChanged(nameof(ElementoSelEliminarB));

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
        public string ResultadoEliminacion
        {
            get => _resultadoEliminacion;
            set
            {
                _resultadoEliminacion = value;
                OnPropertyChanged(nameof(ResultadoEliminacion));
            }
        }


        private async Task Eliminar(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("eliminar_bibliotecario", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_bibliotecario", Int64.Parse(ElementoSelEliminarB));

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;

                        int res = await cmd.ExecuteNonQueryAsync();

                        ResultadoEliminacion = (string)cmd.Parameters["@resultado"].Value;

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

        //private void MostrarMenuEliminacion()
        //{
        //    MenuEliminacionVisible = true;
        //}
    }
}
