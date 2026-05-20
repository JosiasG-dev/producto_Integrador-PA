using MySqlConnector;
using System;

namespace punto_de_venta_C_.ConexionBD
{
    public class Conexion
    {
        public enum TipoMotor
        {
            MYSQL, SQLSERVER
        }

        public enum TipoAutenticacion
        {
            CREDENCIALES, WINDOWS
        }

        private static TipoMotor motorActual = TipoMotor.MYSQL;
        private static TipoAutenticacion autenticacion = TipoAutenticacion.CREDENCIALES;

        private static string host = "localhost";
        private static string puertoInstancia = "3306";
        private static string baseDatos = "corporativo_pos";
        private static string usuario = "root";
        private static string contrasena = "2306";

        private static MySqlConnection instancia = null;

        public static TipoMotor getMotor()
        {
            return motorActual;
        }

        public static void configurar(TipoMotor motor, TipoAutenticacion auth, string h, string pi, string bd, string u, string pass)
        {
            motorActual = motor;
            autenticacion = auth;
            host = h;
            puertoInstancia = pi;
            baseDatos = bd;
            usuario = u;
            contrasena = pass;
            instancia = null;
        }

        public static MySqlConnection getConexion()
        {
            try
            {
                if (instancia == null || instancia.State == System.Data.ConnectionState.Closed)
                {
                    string url = $"Server={host};Port={puertoInstancia};Database={baseDatos};Uid={usuario};Pwd={contrasena};AllowPublicKeyRetrieval=true;";
                    instancia = new MySqlConnection(url);
                    instancia.Open();
                    Console.WriteLine("Conexion con MySQL establecida");
                    asegurarColumnas();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error de conexion: " + e.Message);
            }
            return instancia;
        }

        public static void cerrar()
        {
            try
            {
                if (instancia != null && instancia.State != System.Data.ConnectionState.Closed)
                {
                    instancia.Close();
                    Console.WriteLine("Conexion cerrada");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private Conexion() { }

        private static void asegurarColumnas()
        {
            try
            {
                if (instancia != null && instancia.State != System.Data.ConnectionState.Closed)
                {
                    string sql = "ALTER TABLE ventas ADD COLUMN descuento DECIMAL(10,2) DEFAULT 0 AFTER total";
                    using (MySqlCommand st = new MySqlCommand(sql, instancia))
                    {
                        st.ExecuteNonQuery();
                    }
                    Console.WriteLine("Columna 'descuento' asegurada en la base de datos.");
                }
            }
            catch (Exception) { }
        }
    }
}
