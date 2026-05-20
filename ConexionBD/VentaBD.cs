using punto_de_venta_C_.Modelo;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace punto_de_venta_C_.ConexionBD
{
    public class VentaBD
    {
        public bool insertar(Venta venta)
        {
            MySqlConnection conn = Conexion.getConexion();
            string sqlVenta = "INSERT INTO ventas (total, descuento, metodo_pago, fecha, cajero) VALUES (@total,@descuento,@metodoPago,@fecha,@cajero)";
            string sqlItem = "INSERT INTO venta_items (venta_id, producto_id, cantidad, precio_unit) VALUES (@ventaId,@productoId,@cantidad,@precioUnit)";
            MySqlTransaction tr = null;
            try
            {
                tr = conn.BeginTransaction();

                using (MySqlCommand psVenta = new MySqlCommand(sqlVenta, conn, tr))
                {
                    psVenta.Parameters.AddWithValue("@total", venta.getTotal());
                    psVenta.Parameters.AddWithValue("@descuento", venta.getDescuento());
                    psVenta.Parameters.AddWithValue("@metodoPago", venta.getMetodoPago());
                    psVenta.Parameters.AddWithValue("@fecha", venta.getFecha());
                    psVenta.Parameters.AddWithValue("@cajero", venta.getCajero());
                    psVenta.ExecuteNonQuery();

                    int idVenta = (int)psVenta.LastInsertedId;
                    if (idVenta == 0)
                        throw new Exception("No se genero ID para la venta");
                    venta.setId(idVenta);

                    using (MySqlCommand psItem = new MySqlCommand(sqlItem, conn, tr))
                    {
                        psItem.Parameters.Add("@ventaId", MySqlDbType.Int32);
                        psItem.Parameters.Add("@productoId", MySqlDbType.VarChar);
                        psItem.Parameters.Add("@cantidad", MySqlDbType.Decimal);
                        psItem.Parameters.Add("@precioUnit", MySqlDbType.Decimal);

                        foreach (ItemCarrito item in venta.getItems())
                        {
                            psItem.Parameters["@ventaId"].Value = idVenta;
                            psItem.Parameters["@productoId"].Value = item.getProducto().getId();
                            psItem.Parameters["@cantidad"].Value = item.getCantidad();
                            psItem.Parameters["@precioUnit"].Value = item.getProducto().getPrecio();
                            psItem.ExecuteNonQuery();
                        }
                    }

                    ProductoBD productoBD = new ProductoBD();
                    foreach (ItemCarrito item in venta.getItems())
                    {
                        Producto p = item.getProducto();
                        double nuevoStock = Math.Round((p.getStock() - item.getCantidad()) * 1000.0) / 1000.0;
                        
                        using (MySqlCommand psUpdate = new MySqlCommand("UPDATE productos SET stock=@stock WHERE id=@id", conn, tr))
                        {
                            psUpdate.Parameters.AddWithValue("@stock", nuevoStock);
                            psUpdate.Parameters.AddWithValue("@id", p.getId());
                            psUpdate.ExecuteNonQuery();
                        }
                        p.setStock(nuevoStock);
                    }

                    tr.Commit();
                    return true;
                }
            }
            catch (Exception e)
            {
                if (tr != null)
                {
                    try
                    {
                        tr.Rollback();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public List<Venta> obtenerTodas()
        {
            List<Venta> lista = new List<Venta>();
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand st = new MySqlCommand("SELECT * FROM ventas ORDER BY fecha DESC", c))
                {
                    using (MySqlDataReader rs = st.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            Venta v = new Venta(
                                rs.GetInt32("id"), 
                                null, 
                                Convert.ToDouble(rs.GetDecimal("total")),
                                Convert.ToDouble(rs.GetDecimal("descuento")), 
                                rs.GetString("metodo_pago"), 
                                rs.GetDateTime("fecha"),
                                rs.GetString("cajero")
                            );
                            lista.Add(v);
                        }
                    }
                }

                foreach(Venta v in lista)
                {
                    v.setItems(obtenerItems(v.getId()));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return lista;
        }

        private List<ItemCarrito> obtenerItems(int ventaId)
        {
            List<ItemCarrito> items = new List<ItemCarrito>();
            string sql = "SELECT vi.*, p.nombre, p.precio, p.categoria, p.unidad "
                       + "FROM venta_items vi JOIN productos p ON vi.producto_id = p.id WHERE vi.venta_id = @ventaId";
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
                            Producto p = new Producto(
                                rs.GetString("producto_id"), 
                                rs.GetString("nombre"),
                                Convert.ToDouble(rs.GetDecimal("precio_unit")), 
                                0, 
                                rs.GetString("categoria"), 
                                rs.GetString("unidad"), 
                                ""
                            );
                            items.Add(new ItemCarrito(p, Convert.ToDouble(rs.GetDecimal("cantidad"))));
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
