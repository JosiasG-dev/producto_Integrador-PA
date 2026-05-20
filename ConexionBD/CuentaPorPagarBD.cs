using punto_de_venta_C_.Modelo;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace punto_de_venta_C_.ConexionBD
{
    public class CuentaPorPagarBD
    {
        public bool insertar(CuentaPorPagar c)
        {
            string sql = "INSERT INTO cuentas_por_pagar (proveedor_id, orden_id, monto_total, pagado, saldo, vencimiento, estado) VALUES (@proveedorId,@ordenId,@montoTotal,@pagado,@saldo,@vencimiento,@estado)";
            try
            {
                MySqlConnection conn = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, conn))
                {
                    ps.Parameters.AddWithValue("@proveedorId", c.getProveedor().getId());
                    ps.Parameters.AddWithValue("@ordenId", c.getOrdenId());
                    ps.Parameters.AddWithValue("@montoTotal", c.getMontoTotal());
                    ps.Parameters.AddWithValue("@pagado", c.getPagado());
                    ps.Parameters.AddWithValue("@saldo", c.getSaldo());
                    ps.Parameters.AddWithValue("@vencimiento", c.getVencimiento());
                    ps.Parameters.AddWithValue("@estado", c.getEstado().ToString());
                    int rows = ps.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        c.setId((int)ps.LastInsertedId);
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

        public bool aplicarPago(int id, double montoPagado, double nuevoSaldo, string nuevoEstado)
        {
            string sql = "UPDATE cuentas_por_pagar SET pagado=@pagado, saldo=@saldo, estado=@estado WHERE id=@id";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@pagado", montoPagado);
                    ps.Parameters.AddWithValue("@saldo", nuevoSaldo);
                    ps.Parameters.AddWithValue("@estado", nuevoEstado);
                    ps.Parameters.AddWithValue("@id", id);
                    return ps.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public List<CuentaPorPagar> obtenerActivas()
        {
            List<CuentaPorPagar> lista = new List<CuentaPorPagar>();
            string sql = "SELECT cpp.*, p.nombre AS prov_nombre, p.contacto, p.telefono, p.email, p.direccion "
                       + "FROM cuentas_por_pagar cpp JOIN proveedores p ON cpp.proveedor_id = p.id "
                       + "WHERE cpp.estado != 'PAGADO' ORDER BY cpp.id DESC";
            return ejecutarQuery(lista, sql);
        }

        public List<CuentaPorPagar> obtenerTodas()
        {
            List<CuentaPorPagar> lista = new List<CuentaPorPagar>();
            string sql = "SELECT cpp.*, p.nombre AS prov_nombre, p.contacto, p.telefono, p.email, p.direccion "
                       + "FROM cuentas_por_pagar cpp JOIN proveedores p ON cpp.proveedor_id = p.id ORDER BY cpp.id DESC";
            return ejecutarQuery(lista, sql);
        }

        private List<CuentaPorPagar> ejecutarQuery(List<CuentaPorPagar> lista, string sql)
        {
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand st = new MySqlCommand(sql, c))
                {
                    using (MySqlDataReader rs = st.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            Proveedor prov = new Proveedor(
                                rs.GetInt32("proveedor_id"), 
                                rs.GetString("prov_nombre"),
                                rs.GetString("contacto"), 
                                rs.GetString("telefono"), 
                                rs.GetString("email"),
                                rs.GetString("direccion"), 
                                true
                            );
                            
                            CuentaPorPagar.Estado estadoEnum;
                            Enum.TryParse(rs.GetString("estado"), out estadoEnum);

                            lista.Add(new CuentaPorPagar(
                                rs.GetInt32("id"), 
                                prov, 
                                rs.GetInt32("orden_id"), 
                                Convert.ToDouble(rs.GetDecimal("monto_total")),
                                Convert.ToDouble(rs.GetDecimal("pagado")), 
                                Convert.ToDouble(rs.GetDecimal("saldo")), 
                                rs.GetString("vencimiento"),
                                estadoEnum
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
