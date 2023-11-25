using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Biblioteca.BDConexion;
using Biblioteca.Modelos;
using MySql.Data.MySqlClient;

namespace Biblioteca.ModeloDeVista
{
    public class BibliotecarioVM : INotifyPropertyChanged
    {
        private BibliotecarioM _biblibiotecario = new BibliotecarioM();
        public ICommand RegistrarCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public Conexion conexion = new Conexion();
        public string _resultado;

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

        // control de atributos de Bibliotecario.
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

                if (string.IsNullOrWhiteSpace(value))
                {
                    IsNombreVacio = "No puede estar vacio o contener espacios";
                    nombreCorrecto = false;
                }
                else
                {
                    string pattern = @"^[a-zA-Z]+$";
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

        #region control de contenido del TextBox




        #endregion control de contenido del TexBox

        public BibliotecarioVM()
        {
            FechaNacimiento = new DateTime(2000, 1, 1);
            FechaContrato = new DateTime(2000, 1, 1);
            RegistrarCommand = new AsyncRelayCommand(Registrar, PuedeRegistrar);
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



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
