using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Biblioteca.BDConexion
{
    public class Conexion
    {
        MySqlConnection conn = new MySqlConnection();
        static string servidor = "localhost";
        static string bd = "vehicles";
        static string usuario = "root";
        static string password = "";
        static string puerto = "3307";

        string cadenaConexion = "server=" + servidor + ";"
                              + "port=" + puerto + ";"
                              + "user id=" + usuario + ";"
                              + "password=" + password + ";"
                              + "database=" + bd + ";";

        //consulta permanente optima
        public async Task<bool> comprobarConexionAsync()
        {
            try
            {
                conn.ConnectionString = cadenaConexion;
                await conn.OpenAsync();
                await conn.CloseAsync();
            }
            catch //(MySqlException ex)
            {
                return false;
            }
            return true;
        }
    }
}
