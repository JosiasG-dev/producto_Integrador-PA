using punto_de_venta_C_.Modelo;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace punto_de_venta_C_.ConexionBD
{
    public class OrdenCompraBD
    {
        public bool insertar(OrdenCompra orden)
        {
            MySqlConnection conn = Conexion.getConexion();
            string sqlOrden = "INSERT INTO ordenes_compra (proveedor_id, total, tipo_pago, estado, fecha) VALUES (@proveedorId,@total,@tipoPago,@estado,@fecha)";
            string sqlItem = "INSERT INTO orden_items (orden_id, producto_id, cantidad_solicitada, precio_costo) VALUES (@ordenId,@productoId,@cantidadSolicitada,@precioCosto)";
            MySqlTransaction tr = null;
            try
            {
                tr = conn.BeginTransaction();

                using (MySqlCommand psOrden = new MySqlCommand(sqlOrden, conn, tr))
                {
                    psOrden.Parameters.AddWithValue("@proveedorId", orden.getProveedor().getId());
                    psOrden.Parameters.AddWithValue("@total", orden.getTotal());
                    psOrden.Parameters.AddWithValue("@tipoPago", orden.getTipoPago().ToString());
                    psOrden.Parameters.AddWithValue("@estado", orden.getEstado().ToString());
                    psOrden.Parameters.AddWithValue("@fecha", orden.getFecha());
                    psOrden.ExecuteNonQuery();

                    int idOrden = (int)psOrden.LastInsertedId;
                    if (idOrden == 0)
                        throw new Exception("No se genero ID para la orden");
                    orden.setId(idOrden);

                    using (MySqlCommand psItem = new MySqlCommand(sqlItem, conn, tr))
                    {
                        psItem.Parameters.Add("@ordenId", MySqlDbType.Int32);
                        psItem.Parameters.Add("@productoId", MySqlDbType.VarChar);
                        psItem.Parameters.Add("@cantidadSolicitada", MySqlDbType.Decimal);
                        psItem.Parameters.Add("@precioCosto", MySqlDbType.Decimal);

                        foreach (OrdenCompra.ItemOrden item in orden.getItems())
                        {
                            psItem.Parameters["@ordenId"].Value = idOrden;
                            psItem.Parameters["@productoId"].Value = item.getProducto().getId();
                            psItem.Parameters["@cantidadSolicitada"].Value = item.getCantidadSolicitada();
                            psItem.Parameters["@precioCosto"].Value = item.getPrecioCosto();
                            psItem.ExecuteNonQuery();
                        }
                    }

                    tr.Commit();
                    return true;
                }
            }
            catch (Exception e)
            {
                if (tr != null)
                {
                    try { tr.Rollback(); } catch (Exception) { }
                }
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool marcarRecibida(int id)
        {
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand("UPDATE ordenes_compra SET estado = 'RECIBIDO' WHERE id = @id", c))
                {
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

        public List<OrdenCompra> obtenerTodas()
        {
            List<OrdenCompra> lista = new List<OrdenCompra>();
            string sql = "SELECT oc.*, p.nombre AS prov_nombre, p.contacto, p.telefono, p.email, p.direccion "
                       + "FROM ordenes_compra oc JOIN proveedores p ON oc.proveedor_id = p.id ORDER BY oc.fecha DESC";
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

                            OrdenCompra.TipoPago tipoPago;
                            Enum.TryParse(rs.GetString("tipo_pago"), out tipoPago);

                            OrdenCompra.Estado estado;
                            Enum.TryParse(rs.GetString("estado"), out estado);

                            lista.Add(new OrdenCompra(
                                rs.GetInt32("id"), 
                                prov, 
                                null, 
                                Convert.ToDouble(rs.GetDecimal("total")),
                                tipoPago,
                                estado, 
                                rs.GetDateTime("fecha")
                            ));
                        }
                    }
                }

                foreach(OrdenCompra oc in lista)
                {
                    oc.setItems(obtenerItems(oc.getId()));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return lista;
        }

        public OrdenCompra buscarPorId(int id)
        {
            string sql = "SELECT oc.*, p.nombre AS prov_nombre, p.contacto, p.telefono, p.email, p.direccion "
                       + "FROM ordenes_compra oc JOIN proveedores p ON oc.proveedor_id = p.id WHERE oc.id = @id";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader rs = ps.ExecuteReader())
                    {
                        if (rs.Read())
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

                            OrdenCompra.TipoPago tipoPago;
                            Enum.TryParse(rs.GetString("tipo_pago"), out tipoPago);

                            OrdenCompra.Estado estado;
                            Enum.TryParse(rs.GetString("estado"), out estado);

                            return new OrdenCompra(
                                rs.GetInt32("id"), 
                                prov, 
                                obtenerItems(id), 
                                Convert.ToDouble(rs.GetDecimal("total")),
                                tipoPago,
                                estado, 
                                rs.GetDateTime("fecha")
                            );
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

        private List<OrdenCompra.ItemOrden> obtenerItems(int ordenId)
        {
            List<OrdenCompra.ItemOrden> items = new List<OrdenCompra.ItemOrden>();
            string sql = "SELECT oi.*, p.nombre, p.precio, p.categoria, p.unidad "
                       + "FROM orden_items oi JOIN productos p ON oi.producto_id = p.id WHERE oi.orden_id = @ordenId";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@ordenId", ordenId);
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
                                ""
                            );
                            items.Add(new OrdenCompra.ItemOrden(
                                prod, 
                                Convert.ToDouble(rs.GetDecimal("cantidad_solicitada")),
                                Convert.ToDouble(rs.GetDecimal("precio_costo"))
                            ));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return items;
        }
    }
}
