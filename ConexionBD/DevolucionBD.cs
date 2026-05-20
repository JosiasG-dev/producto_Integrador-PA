using punto_de_venta_C_.Modelo;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace punto_de_venta_C_.ConexionBD
{
    public class DevolucionBD
    {
        public bool insertar(Devolucion d)
        {
            string sql = "INSERT INTO devoluciones (venta_id, producto_id, cantidad, motivo, fecha, cajero) VALUES (@ventaId,@productoId,@cantidad,@motivo,@fecha,@cajero)";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@ventaId", d.getVentaId());
                    ps.Parameters.AddWithValue("@productoId", d.getProducto().getId());
                    ps.Parameters.AddWithValue("@cantidad", d.getCantidad());
                    ps.Parameters.AddWithValue("@motivo", d.getMotivo());
                    ps.Parameters.AddWithValue("@fecha", d.getFecha());
                    ps.Parameters.AddWithValue("@cajero", d.getCajero());
                    int rows = ps.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        d.setId((int)ps.LastInsertedId);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public List<Devolucion> obtenerPorVenta(int ventaId)
        {
            List<Devolucion> lista = new List<Devolucion>();
            string sql = "SELECT d.*, p.nombre, p.precio, p.categoria, p.unidad, p.imagen_ruta "
                       + "FROM devoluciones d JOIN productos p ON d.producto_id = p.id WHERE d.venta_id = @ventaId";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@ventaId", ventaId);
                    using (MySqlDataReader rs = ps.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            Producto prod = new Producto(
                                rs.GetString("producto_id"), 
                                rs.GetString("nombre"),
                                Convert.ToDouble(rs.GetDecimal("precio")), 
                                0, 
                                rs.GetString("categoria"), 
                                rs.GetString("unidad"),
                                rs.IsDBNull(rs.GetOrdinal("imagen_ruta")) ? null : rs.GetString("imagen_ruta")
                            );

                            lista.Add(new Devolucion(
                                rs.GetInt32("id"), 
                                ventaId, 
                                prod, 
                                Convert.ToDouble(rs.GetDecimal("cantidad")),
                                rs.GetString("motivo"), 
                                rs.GetDateTime("fecha"), 
                                rs.GetString("cajero")
                            ));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return lista;
        }

        public List<Devolucion> obtenerTodas()
        {
            List<Devolucion> lista = new List<Devolucion>();
            string sql = "SELECT d.*, p.nombre, p.precio, p.categoria, p.unidad, p.imagen_ruta "
                       + "FROM devoluciones d JOIN productos p ON d.producto_id = p.id ORDER BY d.fecha DESC";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand st = new MySqlCommand(sql, c))
                {
                    using (MySqlDataReader rs = st.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            Producto prod = new Producto(
                                rs.GetString("producto_id"), 
                                rs.GetString("nombre"),
                                Convert.ToDouble(rs.GetDecimal("precio")), 
                                0, 
                                rs.GetString("categoria"), 
                                rs.GetString("unidad"),
                                rs.IsDBNull(rs.GetOrdinal("imagen_ruta")) ? null : rs.GetString("imagen_ruta")
                            );

                            lista.Add(new Devolucion(
                                rs.GetInt32("id"), 
                                rs.GetInt32("venta_id"), 
                                prod, 
                                Convert.ToDouble(rs.GetDecimal("cantidad")),
                                rs.GetString("motivo"), 
                                rs.GetDateTime("fecha"), 
                                rs.GetString("cajero")
                            ));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return lista;
        }
    }
}
