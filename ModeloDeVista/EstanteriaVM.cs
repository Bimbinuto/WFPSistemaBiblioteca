using Biblioteca.BDConexion;
using Biblioteca.Modelos;
using Biblioteca.Vistas.Ventanas;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace Biblioteca.ModeloDeVista
{
    public partial class EstanteriaVM : INotifyPropertyChanged
    {
        private Conexion conexion = new Conexion();
        private System.Timers.Timer timer;
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand RegistrarEstanteriaCommand { get; set; }
        public ICommand ModificarEstanteriaCommand { get; set; }
        public ICommand EliminarEstanteriaCommand { get; set; }

        private DataTable _estanteriaT;
        private DataRowView _filaSeleccionada;
        private string _busqueda;
        private string _resultado;
        private string _idEstanteria;

        public DataTable EstanteriaT
        {
            get => _estanteriaT;
            set
            {
                _estanteriaT = value;
                OnPropertyChanged(nameof(EstanteriaT));
            }
        }
        public DataRowView FilaSeleccionada
        {
            get => _filaSeleccionada;
            set
            {
                _filaSeleccionada = value;
                OnPropertyChanged(nameof(FilaSeleccionada));

                _idEstanteria = value["id_estanteria"].ToString();
                CodigoUbicacionMod = value["codigo_ubicacion"].ToString();
                CapacidadMod = value["capacidad_original"].ToString();
            }
        }
        public string Busqueda
        {
            get => _busqueda;
            set
            {
                _busqueda = value;
                OnPropertyChanged(nameof(Busqueda));
                timer.Stop();
                timer.Start();
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


        public EstanteriaVM()
        {
            RegistrarEstanteriaCommand = new AsyncRelayCommand(Registrar, PuedeRegistrar);
            ModificarEstanteriaCommand = new AsyncRelayCommand(Modificar, PuedeModificar);
            EliminarEstanteriaCommand = new AsyncRelayCommand(Eliminar, PuedeEliminar);

            timer = new System.Timers.Timer(650);
            timer.Elapsed += (sender, e) => Task.Run(() => CargarTablaEstanteria());
            timer.AutoReset = false;

            CargarTablaEstanteria();
        }

        public async void CargarTablaEstanteria()
        {
            using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
            {
                await cnx.OpenAsync();

                string consulta = "SELECT id_estanteria, codigo_ubicacion, capacidad, capacidad_original\r\nFROM estanteria\r\n";

                if (!String.IsNullOrEmpty(Busqueda))
                {
                    consulta += " WHERE codigo_ubicacion LIKE @busqueda; ";
                }

                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                cmd.Parameters.AddWithValue("@busqueda", $"%{Busqueda}%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                EstanteriaT = dt;
                await cnx.CloseAsync();
            }
        }

        private async Task Registrar(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("registrar_estanteria", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pcodigo_ubicacion", CodigoUbicacion);
                        cmd.Parameters.AddWithValue("@pcapacidad", Int32.Parse(Capacidad));

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        Resultado = (string)cmd.Parameters["@resultado"].Value;

                        await cnx.CloseAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private bool PuedeRegistrar(object parameter)
        {
            return codigoUbicacionCorrecto && capacidadCorrecto;
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //REGISTRAR ESTANTERIA
    public partial class EstanteriaVM
    {
        private string _codigoUbicacion;
        private string _capacidad;
        private string _codigoUbicacionVacio;
        private string _capacidadVacio;

        private bool codigoUbicacionCorrecto;
        private bool capacidadCorrecto;

        public string CodigoUbicacionVacio
        {
            get => _codigoUbicacionVacio;
            set
            {
                _codigoUbicacionVacio = value;
                OnPropertyChanged(nameof(CodigoUbicacionVacio));
            }
        }
        public string CapacidadVacio
        {
            get => _capacidadVacio;
            set
            {
                _capacidadVacio = value;
                OnPropertyChanged(nameof(CapacidadVacio));
            }
        }
        public string CodigoUbicacion
        {
            get => _codigoUbicacion;
            set
            {
                _codigoUbicacion = value;
                OnPropertyChanged(nameof(CodigoUbicacion));

                if (String.IsNullOrWhiteSpace(value))
                {
                    CodigoUbicacionVacio = "No puede estar vacio";
                    codigoUbicacionCorrecto = false;
                }
                else
                {
                    CodigoUbicacionVacio = "";
                    codigoUbicacionCorrecto = true;
                }
            }
        }
        public string Capacidad
        {
            get => _capacidad;
            set
            {
                _capacidad = value;
                OnPropertyChanged(nameof(Capacidad));

                if (string.IsNullOrEmpty(value))
                {
                    CapacidadVacio = "No puede estar vacio";
                    capacidadCorrecto = false;
                }
                else if (value == "0")
                {
                    CapacidadVacio = "El valor no puede ser cero";
                    capacidadCorrecto = false;
                }
                else
                {
                    string patternPositivo = @"^[1-9][0-9]*$"; // Expresión regular que acepta solo números positivos (no cero)
                    string patternNegativo = @"^-"; // Expresión regular que detecta números negativos
                    string patternEspecial = @"[^0-9]"; // Expresión regular que detecta caracteres no numéricos

                    if (Regex.IsMatch(value, patternNegativo))
                    {
                        CapacidadVacio = "No puede haber campos negativos";
                        capacidadCorrecto = false;
                    }
                    else if (!Regex.IsMatch(value, patternPositivo))
                    {
                        CapacidadVacio = "El valor debe ser un número positivo";
                        capacidadCorrecto = false;
                    }
                    else if (Regex.IsMatch(value, patternEspecial))
                    {
                        CapacidadVacio = "No puede contener caracteres especiales";
                        capacidadCorrecto = false;
                    }
                    else
                    {
                        CapacidadVacio = "";
                        capacidadCorrecto = true;
                    }
                }
            }
        }
    }


    //MODIFICAR ESTANTERIA
    public partial class EstanteriaVM
    {
        private string _codigoUbicacionMod;
        private string _capacidadMod;
        private string _codigoUbicacionVacioMod;
        private string _capacidadVacioMod;
        private string _resultadoMod;

        private bool codigoUbicacionCorrectoMod;
        private bool capacidadCorrectoMod;

        public string CodigoUbicacionVacioMod
        {
            get => _codigoUbicacionVacioMod;
            set
            {
                _codigoUbicacionVacioMod = value;
                OnPropertyChanged(nameof(CodigoUbicacionVacioMod));
            }
        }
        public string CapacidadVacioMod
        {
            get => _capacidadVacioMod;
            set
            {
                _capacidadVacioMod = value;
                OnPropertyChanged(nameof(CapacidadVacioMod));
            }
        }
        public string CodigoUbicacionMod
        {
            get => _codigoUbicacionMod;
            set
            {
                _codigoUbicacionMod = value;
                OnPropertyChanged(nameof(CodigoUbicacionMod));

                if (String.IsNullOrWhiteSpace(value))
                {
                    CodigoUbicacionVacioMod = "No puede estar vacio";
                    codigoUbicacionCorrectoMod = false;
                }
                else
                {
                    CodigoUbicacionVacioMod = "";
                    codigoUbicacionCorrectoMod = true;
                }
            }
        }
        public string CapacidadMod
        {
            get => _capacidadMod;
            set
            {
                _capacidadMod = value;
                OnPropertyChanged(nameof(CapacidadMod));

                if (string.IsNullOrEmpty(value))
                {
                    CapacidadVacioMod = "No puede estar vacio";
                    capacidadCorrectoMod = false;
                }
                else if (value == "0")
                {
                    CapacidadVacioMod = "El valor no puede ser cero";
                    capacidadCorrectoMod = false;
                }
                else
                {
                    string patternPositivo = @"^[1-9][0-9]*$"; // Expresión regular que acepta solo números positivos (no cero)
                    string patternNegativo = @"^-"; // Expresión regular que detecta números negativos
                    string patternEspecial = @"[^0-9]"; // Expresión regular que detecta caracteres no numéricos

                    if (Regex.IsMatch(value, patternNegativo))
                    {
                        CapacidadVacioMod = "No puede haber campos negativos";
                        capacidadCorrectoMod = false;
                    }
                    else if (!Regex.IsMatch(value, patternPositivo))
                    {
                        CapacidadVacioMod = "El valor debe ser un número positivo";
                        capacidadCorrectoMod = false;
                    }
                    else if (Regex.IsMatch(value, patternEspecial))
                    {
                        CapacidadVacioMod = "No puede contener caracteres especiales";
                        capacidadCorrectoMod = false;
                    }
                    else
                    {
                        CapacidadVacioMod = "";
                        capacidadCorrectoMod = true;
                    }
                }
            }
        }
        public string ResultadoMod
        {
            get => _resultadoMod;
            set
            {
                _resultadoMod = value;
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

                    using (MySqlCommand cmd = new MySqlCommand("modificar_estanteria", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_estanteria", Int32.Parse(_idEstanteria));
                        cmd.Parameters.AddWithValue("@pcodigo_ubicacion", CodigoUbicacionMod);
                        cmd.Parameters.AddWithValue("@pcapacidad_original", Int32.Parse(CapacidadMod));

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        ResultadoMod = (string)cmd.Parameters["@resultado"].Value;

                        await cnx.CloseAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private bool PuedeModificar(object parameter)
        {
            return codigoUbicacionCorrectoMod && capacidadCorrectoMod;
        }
    }

    //ELIMINAR ESTATERIA
    public partial class EstanteriaVM
    {
        private async Task Eliminar(object parameter)
        {
            try
            {
                using (MySqlConnection cnx = new MySqlConnection(conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand("eliminar_estanteria", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@pid_estanteria", Int64.Parse(_idEstanteria));

                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 200);
                        cmd.Parameters["@resultado"].Direction = System.Data.ParameterDirection.Output;

                        await cmd.ExecuteNonQueryAsync();

                        ResultadoMod = (string)cmd.Parameters["@resultado"].Value;

                        await cnx.CloseAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private bool PuedeEliminar(object parameter)
        {
            return true;
        }
    }
}
