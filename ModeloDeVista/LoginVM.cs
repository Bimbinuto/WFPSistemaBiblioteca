using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Biblioteca.BDConexion;
using Biblioteca.Modelos;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls;
using Biblioteca.Vistas.Ventanas;
using Biblioteca.Ventanas;

namespace Biblioteca.ModeloDeVista
{
    public class LoginVM : INotifyPropertyChanged
    {
        public LoginM _login = new LoginM();
        public Conexion conexion = new Conexion();
        private EncriptadoUnico encriptar = new EncriptadoUnico();
        public event PropertyChangedEventHandler PropertyChanged;
        private int intentos = 3;
        public ICommand AccederCommand { get; set; }

        public string nombreUsuario;
        public string idUsuario;
        public string soloNombreUsuario;
        public string tipoUsuario;
        public string resultado = "";

        public string IDUsuario
        {
            get => idUsuario;
            set
            {
                idUsuario = value;
                OnPropertyChanged(nameof(IDUsuario));
            }
        }
        public string Usuario
        {
            get => _login.usuario;
            set
            {
                _login.usuario = value;
                OnPropertyChanged(nameof(Usuario));
            }
        }
        public string Password
        {
            get => _login.password;
            set
            {
                _login.password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string NombreUsuario
        {
            get => "Bienvenido, " + nombreUsuario;
            set
            {
                nombreUsuario = value;
                OnPropertyChanged(nameof(NombreUsuario));
            }
        }
        public string SoloNombreUsuario
        {
            get => soloNombreUsuario;
            set
            {
                soloNombreUsuario = value;
                OnPropertyChanged(nameof(SoloNombreUsuario));
            }
        }
        public string TipoUsuario
        {
            get => tipoUsuario;
            set
            {
                tipoUsuario = value;
                OnPropertyChanged(nameof(TipoUsuario));
            }
        }
        public string Resultado
        {
            get => resultado;
            set
            {
                resultado = value;
                OnPropertyChanged(nameof(Resultado));
            }
        }

        public Login login;

        public LoginVM(Login sesion)
        {
            AccederCommand = new AsyncRelayCommand(Acceder, PuedeAcceder);
            login = sesion;
        }

        private async Task Acceder(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("obtener_tipo_usuario_completo", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pcuenta", Usuario);
                        cmd.Parameters.AddWithValue("@pcontrasena", encriptar.ComputeSha256Hash(Password));

                        cmd.Parameters.Add("@pid_usuario", MySqlDbType.Int64);
                        cmd.Parameters["@pid_usuario"].Direction = System.Data.ParameterDirection.Output;

                        cmd.Parameters.Add("@ptipo_usuario", MySqlDbType.VarChar, 100);
                        cmd.Parameters["@ptipo_usuario"].Direction = System.Data.ParameterDirection.Output;

                        cmd.Parameters.Add("@pnombres", MySqlDbType.VarChar, 100);
                        cmd.Parameters["@pnombres"].Direction = System.Data.ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        var id = cmd.Parameters["@pid_usuario"].Value;
                        var nom = cmd.Parameters["@pnombres"].Value;
                        var tipo = cmd.Parameters["@ptipo_usuario"].Value;

                        IDUsuario = id.ToString();
                        NombreUsuario = nom.ToString();
                        SoloNombreUsuario = nom.ToString();
                        TipoUsuario = tipo.ToString();

                        //UsuarioGlobal uglobal = new UsuarioGlobal();
                        //uglobal.IdUsuario = IDUsuario;
                        //uglobal.NombreUsuarioG = SoloNombreUsuario;
                        //uglobal.TipoUsuarioG = TipoUsuario;

                        //UsuarioGlobal.GetInstance().IdUsuario = id.ToString();
                        //UsuarioGlobal.GetInstance().NombreUsuarioG = nom.ToString();
                        //UsuarioGlobal.GetInstance().TipoUsuarioG = tipo.ToString();

                        UsuarioGlobal.GetInstance().IdUsuario = IDUsuario;
                        UsuarioGlobal.GetInstance().NombreUsuarioG = SoloNombreUsuario;
                        UsuarioGlobal.GetInstance().TipoUsuarioG = TipoUsuario;

                        //MessageBox.Show($"ID: {id}\nombres: {nom.ToString()} \n Tipo: {tipo.ToString()} \n ID usuario: {IDUsuario} \n NombreUsuario: {NombreUsuario} \n TipoUsuario: {TipoUsuario}");

                        if (TipoUsuario == "Bibliotecario" || TipoUsuario == "Docente" || TipoUsuario == "Estudiante")
                        {
                            Resultado = "OK";

                            login.Comenzar();

                            DispatcherTimer dispatcherTimer = new DispatcherTimer();
                            DispatcherTimer timerlocal = dispatcherTimer;
                            timerlocal.Interval = TimeSpan.FromSeconds(1.7);
                            timerlocal.Tick += (s, args) =>
                            {
                                timerlocal.Stop();

                                Menu nuevo = new Menu();
                                nuevo.Show();
                                login.Close();
                            };
                            timerlocal.Start();

                        }
                        else if (TipoUsuario == "Desconocido")
                        {
                            intentos--;
                            Resultado = $"Ingrese crendeciales validas\n(quedan {intentos} intentos)";
                            if (intentos == 0)
                            {
                                login.Close();
                            }
                        }
                    }

                    await cnx.CloseAsync();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private bool PuedeAcceder(object parameter)
        {
            return !String.IsNullOrEmpty(Usuario) && !String.IsNullOrEmpty(Password);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
