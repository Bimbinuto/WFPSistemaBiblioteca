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
        static string bd = "Biblioteca";
        static string usuario = "root";
        static string password = "@TensorFlowK3";
        static string puerto = "3306";

        string cadenaConexion = "server=" + servidor + ";"
                              + "port=" + puerto + ";"
                              + "user id=" + usuario + ";"
                              + "password=" + password + ";"
                              + "database=" + bd + ";";

        //consulta permanente lenta
        public bool comprobarConexion()
        {
            try
            {
                conn.ConnectionString = cadenaConexion;
                conn.Open();
            }
            catch (MySqlException ex)
            {
                return false;
            }
            return true;
        }

        //consulta permanente optima
        public async Task<bool> comprobarConexionAsync()
        {
            try
            {
                conn.ConnectionString = cadenaConexion;
                await conn.OpenAsync();
            }
            catch (MySqlException ex)
            {
                return false;
            }
            return true;
        }
    }
}
