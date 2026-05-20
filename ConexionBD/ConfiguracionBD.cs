using punto_de_venta_C_.Modelo;
using MySqlConnector;
using System;

namespace punto_de_venta_C_.ConexionBD
{
    public class ConfiguracionBD
    {
        public ConfiguracionTienda obtener()
        {
            string sql = "SELECT * FROM configuracion WHERE id = 1";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand st = new MySqlCommand(sql, c))
                {
                    using (MySqlDataReader rs = st.ExecuteReader())
                    {
                        if (rs.Read())
                        {
                            return new ConfiguracionTienda(rs.GetString("nombre_tienda"), rs.GetString("sucursal"), rs.GetString("rfc"));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public void actualizar(ConfiguracionTienda config)
        {
            string sql = "UPDATE configuracion SET nombre_tienda=@nombre, sucursal=@sucursal, rfc=@rfc WHERE id=1";

            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@nombre", config.getNombre());
                    ps.Parameters.AddWithValue("@sucursal", config.getSucursal());
                    ps.Parameters.AddWithValue("@rfc", config.getRfc());

                    int filasAfectadas = ps.ExecuteNonQuery();
                    Console.WriteLine("Filas actualizadas en BD: " + filasAfectadas);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error al guardar en BD: " + e.Message);
            }
        }
    }
}
