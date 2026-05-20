using punto_de_venta_C_.Modelo;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace punto_de_venta_C_.ConexionBD
{
    public class MovimientoBD
    {
        public bool insertar(Movimiento m)
        {
            string sql = "INSERT INTO movimientos (tipo, descripcion, monto, fecha, usuario) VALUES (@tipo,@descripcion,@monto,@fecha,@usuario)";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@tipo", m.getTipo().ToString());
                    ps.Parameters.AddWithValue("@descripcion", m.getDescripcion());
                    ps.Parameters.AddWithValue("@monto", m.getMonto());
                    ps.Parameters.AddWithValue("@fecha", m.getFecha());
                    ps.Parameters.AddWithValue("@usuario", m.getUsuario());
                    return ps.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public List<Movimiento> obtenerDelDia()
        {
            List<Movimiento> lista = new List<Movimiento>();
            string sql = "SELECT * FROM movimientos WHERE DATE(fecha) = CURDATE() ORDER BY fecha DESC";
            
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand st = new MySqlCommand(sql, c))
                {
                    using (MySqlDataReader rs = st.ExecuteReader())
                    {
                        while (rs.Read())
                            lista.Add(mapear(rs));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return lista;
        }

        public List<Movimiento> obtenerTodos()
        {
            List<Movimiento> lista = new List<Movimiento>();
            string sql = "SELECT * FROM movimientos ORDER BY fecha DESC";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand st = new MySqlCommand(sql, c))
                {
                    using (MySqlDataReader rs = st.ExecuteReader())
                    {
                        while (rs.Read())
                            lista.Add(mapear(rs));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return lista;
        }

        private Movimiento mapear(MySqlDataReader rs)
        {
            Movimiento.Tipo tipoEnum;
            Enum.TryParse(rs.GetString("tipo"), out tipoEnum);
            return new Movimiento(
                tipoEnum, 
                rs.GetString("descripcion"),
                Convert.ToDouble(rs.GetDecimal("monto")), 
                rs.GetDateTime("fecha"), 
                rs.GetString("usuario")
            );
        }
    }
}
