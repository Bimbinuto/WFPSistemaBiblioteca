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
        static string bd = "Biblioteca";//"Biblioteca";
        static string usuario = "root";
        static string password = "@TensorFlowK3";
        static string puerto = "3306";

        string cadenaConexion = "server=" + servidor + ";"
                              + "port=" + puerto + ";"
                              + "user id=" + usuario + ";"
                              + "password=" + password + ";"
                              + "database=" + bd + ";";

        public bool comprobarConexion()
        {
            try
            {
                conn.ConnectionString = cadenaConexion;
                conn.Open();
                //MessageBox.Show("True");
                //testc = true;
                //return true;
            }
            catch (MySqlException ex)
            {
                //MessageBox.Show("False"); //+ "\n" + ex.ToString()
                return false;
            }
            //finally
            //{
            //    if (conn.State == ConnectionState.Open)
            //    {
            //        conn.Close();
            //    }
            //}

            return true;
            //testc = false;
        }
    }
}
