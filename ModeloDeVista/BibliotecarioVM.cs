using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Text;
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

        private bool _isNombresEmpty;

        // control de atributos de Bibliotecario.
        public long? CI
        {
            get => _biblibiotecario.ci;
            set
            {
                _biblibiotecario.ci = value;
                OnPropertyChanged(nameof(CI));

                if (!string.IsNullOrEmpty(_biblibiotecario.nombres))
                {
                    // Obtiene el primer nombre y lo convierte a minúsculas
                    string primerNombre = _biblibiotecario.nombres.Split(' ')[0].ToLower();

                    Contrasena = primerNombre + _biblibiotecario.ci.ToString();
                }
            }
        }

        public string Nombres
        {
            get => _biblibiotecario.nombres;
            set
            {
                _biblibiotecario.nombres = value;
                //IsNombresEmpty = String.IsNullOrEmpty(value);
                OnPropertyChanged(nameof(Nombres));

                string primerNombre = _biblibiotecario.nombres.Split(' ')[0].ToLower();

                if (!string.IsNullOrEmpty(_biblibiotecario.apPaterno))
                {
                    string primerApellido = _biblibiotecario.apPaterno.ToLower();
                    Cuenta = primerNombre + "." + primerApellido + "@sistemas.edu.bo";
                }
                //Cuenta = _biblibiotecario.nombres + "." + _biblibiotecario.apPaterno + "@sistenas.edu.bo";
                Contrasena = primerNombre + _biblibiotecario.ci.ToString();
            }
        }

        public string ApPaterno
        {
            get => _biblibiotecario.apPaterno;
            set
            {
                _biblibiotecario.apPaterno = value;
                OnPropertyChanged(nameof(ApPaterno));

                string primerNombre = _biblibiotecario.nombres.Split(' ')[0].ToLower();
                string primerApellido = _biblibiotecario.apPaterno.ToLower();
                Cuenta = primerNombre + "." + primerApellido + "@sistemas.edu.bo";
            }
        }

        public string ApMaterno
        {
            get => _biblibiotecario.apMaterno;
            set
            {
                _biblibiotecario.apMaterno = value;
                OnPropertyChanged(nameof(ApMaterno));
            }
        }

        public DateTime FechaNacimiento
        {
            get => _biblibiotecario.fechaNacimiento;
            set
            {
                _biblibiotecario.fechaNacimiento = value;
                OnPropertyChanged(nameof(FechaNacimiento));
            }
        }

        public string Direccion
        {
            get => _biblibiotecario.direccion;
            set
            {
                _biblibiotecario.direccion = value;
                OnPropertyChanged(nameof(Direccion));
            }
        }

        public long? Telefono
        {
            get => _biblibiotecario.telefono;
            set
            {
                _biblibiotecario.telefono = value;
                OnPropertyChanged(nameof(Telefono));
            }
        }

        public string Correo
        {
            get => _biblibiotecario.correo;
            set
            {
                _biblibiotecario.correo = value;
                OnPropertyChanged(nameof(Correo));
            }
        }

        public string Cuenta
        {
            get => _biblibiotecario.cuenta;
            set
            {
                _biblibiotecario.cuenta = value;
                OnPropertyChanged(nameof(Cuenta));
            }
        }

        public string Contrasena
        {
            get => _biblibiotecario.contrasena;
            set
            {
                _biblibiotecario.contrasena = value;
                OnPropertyChanged(nameof(Contrasena));
            }
        }

        public string Sexo
        {
            get => _biblibiotecario.sexo;
            set
            {
                _biblibiotecario.sexo = value;
                OnPropertyChanged(nameof(Sexo));
            }
        }

        public DateTime FechaContrato
        {
            get => _biblibiotecario.fechaContrato;
            set
            {
                _biblibiotecario.fechaContrato = value;
                OnPropertyChanged(nameof(FechaContrato));
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

        public bool IsNombresEmpty
        {
            get => _isNombresEmpty;
            set
            {
                _isNombresEmpty = value;
                OnPropertyChanged(nameof(IsNombresEmpty));
            }
        }


        #endregion control de contenido del TexBox

        public BibliotecarioVM()
        {
            FechaNacimiento = new DateTime(2000, 1, 1);
            FechaContrato = new DateTime(2000, 1, 1);
            RegistrarCommand = new AsyncRelayCommand(Registrar, PuedeRegistrar);
        }

        private async Task Registrar(object parameter)
        {
            //string cadena = "server=localhost;database=biblioteca2;userid=root;password=@TensorFlowK3";
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("registrar_bibliotecario", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pci", CI);
                        cmd.Parameters.AddWithValue("@pnombres", Nombres);
                        cmd.Parameters.AddWithValue("@pap_paterno", ApPaterno);
                        cmd.Parameters.AddWithValue("@pap_materno", ApMaterno);
                        cmd.Parameters.AddWithValue("@pfecha_nacimiento", FechaNacimiento);
                        cmd.Parameters.AddWithValue("@pdireccion", Direccion);
                        cmd.Parameters.AddWithValue("@ptelefono", Telefono);
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
            return (CI > 0 && CI != 0 && CI.HasValue) &&
                   !String.IsNullOrEmpty(Nombres) &&
                   !String.IsNullOrEmpty(ApPaterno) &&
                   !String.IsNullOrEmpty(ApMaterno) &&
                   (FechaNacimiento != null && FechaNacimiento != default(DateTime)) &&
                   !String.IsNullOrEmpty(Direccion) &&
                   (Telefono > 0 && Telefono != 0 && Telefono.HasValue) &&
                   !String.IsNullOrEmpty(Correo) &&
                   !String.IsNullOrEmpty(Cuenta) &&
                   !String.IsNullOrEmpty(Contrasena) &&
                   !String.IsNullOrEmpty(Sexo) &&
                   (FechaContrato != null);
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
