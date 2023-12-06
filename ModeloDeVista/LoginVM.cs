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
        public ICommand AccederCommand { get; set; }

        public string nombreUsuario;
        public string soloNombreUsuario;
        public string tipoUsuario;
        public string resultado;
        private object sender = null;

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

        public LoginVM()
        {
            AccederCommand = new AsyncRelayCommand(Acceder, PuedeAcceder);
        }

        private async Task Acceder(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("obtener_tipo_usuario", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pcuenta", Usuario);
                        cmd.Parameters.AddWithValue("@pcontrasena", encriptar.ComputeSha256Hash(Password));

                        cmd.Parameters.Add("@ptipo_usuario", MySqlDbType.VarChar, 20);
                        cmd.Parameters["@ptipo_usuario"].Direction = System.Data.ParameterDirection.Output;

                        cmd.Parameters.Add("@pnombres", MySqlDbType.VarChar, 20);
                        cmd.Parameters["@pnombres"].Direction = System.Data.ParameterDirection.Output;

                        int res = await cmd.ExecuteNonQueryAsync();

                        var nom = cmd.Parameters["@pnombres"].Value;
                        var tipo = cmd.Parameters["@ptipo_usuario"].Value;

                        NombreUsuario = nom.ToString();
                        SoloNombreUsuario = nom.ToString();
                        TipoUsuario = tipo.ToString();

                        //UsuarioGlobal uglobal = new UsuarioGlobal();
                        //uglobal.NombreUsuarioG = SoloNombreUsuario;
                        //uglobal.TipoUsuarioG = TipoUsuario;

                        //MessageBox.Show($"nombres: {nom.ToString()} \n Tipo: {tipo.ToString()} \n NombreUsuario: {NombreUsuario} \n TipoUsuario: {TipoUsuario}");

                        if (TipoUsuario == "Bibliotecario" || TipoUsuario == "Docente" || TipoUsuario == "Estudiante")
                        {
                            Resultado = "OK";
                            Login log = new Login();
                            log.btnLogin(sender, null);
                            //log.Comenzar();
                            //log.btnEjecutarAnimacion(sender, null);

                            DispatcherTimer dispatcherTimer = new DispatcherTimer();
                            DispatcherTimer timer1 = dispatcherTimer;
                            timer1.Interval = TimeSpan.FromSeconds(1.7);
                            timer1.Tick += (s, args) =>
                            {
                                timer1.Stop();
                                //colocar despues el 'this' como parametro para el menu
                                Menu nuevo = new Menu();
                                nuevo.Show();
                            };
                            timer1.Start();
                        }
                        else if (TipoUsuario == "Desconocido")
                        {
                            Resultado = "Ingrese crendeciales validas";
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
