using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Biblioteca.BDConexion;
using MySql.Data.MySqlClient;

namespace Biblioteca.ModeloDeVista
{
    public class EstadisticasVM : INotifyPropertyChanged
    {
        private Conexion _conexion = new Conexion();
        public event PropertyChangedEventHandler PropertyChanged;

        private string _varones;
        private string _mujeres;

        public string Varones
        {
            get => _varones;
            set
            {
                _varones = value;
                OnPropertyChanged(nameof(Varones));
            }
        }
        public string Mujeres
        {
            get => _mujeres;
            set
            {
                _mujeres = value;
                OnPropertyChanged(nameof(Mujeres));
            }
        }

        public async void CantidadPrestamos()
        {
            string consulta = "SELECT usuario.sexo, COUNT(*) as num_prestamos\r\nFROM prestamo\r\nJOIN transaccion ON prestamo.id_transaccion = transaccion.id_transaccion\r\nJOIN usuario ON transaccion.id_usuario = usuario.id_usuario\r\nGROUP BY usuario.sexo;\r\n";


            using (MySqlConnection cnx = new MySqlConnection(_conexion.cadenaConexion))
            {
                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                await cnx.OpenAsync();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["sexo"].ToString() == "masculino")
                    {
                        Varones = reader["num_prestamos"].ToString();
                    }
                    else if (reader["sexo"].ToString() == "femenino")
                    {
                        Mujeres = reader["num_prestamos"].ToString();
                    }
                }

                reader.Close();
                await cnx.CloseAsync();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
