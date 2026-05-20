using punto_de_venta_C_.Modelo;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace punto_de_venta_C_.ConexionBD
{
    public class ProductoBD
    {
        public List<Producto> obtenerTodos()
        {
            List<Producto> lista = new List<Producto>();
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand st = new MySqlCommand("SELECT * FROM productos ORDER BY CAST(id AS UNSIGNED)", c))
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

        public Producto buscarPorId(string id)
        {
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand("SELECT * FROM productos WHERE id = @id", c))
                {
                    ps.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader rs = ps.ExecuteReader())
                    {
                        if (rs.Read())
                            return mapear(rs);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public List<Producto> buscar(string texto)
        {
            List<Producto> lista = new List<Producto>();
            if (string.IsNullOrWhiteSpace(texto)) return lista;
            string txt = texto.Trim();

            bool esSoloNumero = System.Text.RegularExpressions.Regex.IsMatch(txt, @"^\d+$");
            try
            {
                MySqlConnection c = Conexion.getConexion();
                if (esSoloNumero)
                {
                    string skuFormateado = string.Format("{0:000}", int.Parse(txt));
                    using (MySqlCommand ps = new MySqlCommand("SELECT * FROM productos WHERE id = @id1 OR id = @id2 LIMIT 10", c))
                    {
                        ps.Parameters.AddWithValue("@id1", txt);
                        ps.Parameters.AddWithValue("@id2", skuFormateado);
                        using (MySqlDataReader rs = ps.ExecuteReader())
                        {
                            while (rs.Read()) lista.Add(mapear(rs));
                        }
                    }

                    if (lista.Count == 0)
                    {
                        using (MySqlCommand ps = new MySqlCommand("SELECT * FROM productos WHERE nombre LIKE @nombre LIMIT 10", c))
                        {
                            ps.Parameters.AddWithValue("@nombre", "%" + txt + "%");
                            using (MySqlDataReader rs = ps.ExecuteReader())
                            {
                                while (rs.Read()) lista.Add(mapear(rs));
                            }
                        }
                    }
                }
                else
                {
                    using (MySqlCommand ps = new MySqlCommand("SELECT * FROM productos WHERE nombre LIKE @nombre LIMIT 10", c))
                    {
                        ps.Parameters.AddWithValue("@nombre", "%" + txt + "%");
                        using (MySqlDataReader rs = ps.ExecuteReader())
                        {
                            while (rs.Read()) lista.Add(mapear(rs));
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

        public bool insertar(Producto p)
        {
            string sql = "INSERT INTO productos (id, nombre, precio, stock, stock_minimo, categoria, unidad, imagen_ruta) VALUES (@id,@nombre,@precio,@stock,@stockMinimo,@categoria,@unidad,@imagenRuta)";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@id", p.getId());
                    ps.Parameters.AddWithValue("@nombre", p.getNombre());
                    ps.Parameters.AddWithValue("@precio", p.getPrecio());
                    ps.Parameters.AddWithValue("@stock", p.getStock());
                    ps.Parameters.AddWithValue("@stockMinimo", p.getStockMinimo());
                    ps.Parameters.AddWithValue("@categoria", p.getCategoria());
                    ps.Parameters.AddWithValue("@unidad", p.getUnidad());
                    ps.Parameters.AddWithValue("@imagenRuta", p.getImagenRuta());
                    return ps.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public bool actualizar(Producto p)
        {
            string sql = "UPDATE productos SET nombre=@nombre, precio=@precio, stock=@stock, stock_minimo=@stockMinimo, categoria=@categoria, unidad=@unidad, imagen_ruta=@imagenRuta WHERE id=@id";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@nombre", p.getNombre());
                    ps.Parameters.AddWithValue("@precio", p.getPrecio());
                    ps.Parameters.AddWithValue("@stock", p.getStock());
                    ps.Parameters.AddWithValue("@stockMinimo", p.getStockMinimo());
                    ps.Parameters.AddWithValue("@categoria", p.getCategoria());
                    ps.Parameters.AddWithValue("@unidad", p.getUnidad());
                    ps.Parameters.AddWithValue("@imagenRuta", p.getImagenRuta());
                    ps.Parameters.AddWithValue("@id", p.getId());
                    return ps.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public bool actualizarStock(string id, double nuevoStock)
        {
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand("UPDATE productos SET stock=@stock WHERE id=@id", c))
                {
                    ps.Parameters.AddWithValue("@stock", nuevoStock);
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

        public bool eliminar(string id)
        {
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand("DELETE FROM productos WHERE id=@id", c))
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

        public string generarNuevoId()
        {
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand st = new MySqlCommand("SELECT MAX(CAST(id AS UNSIGNED)) AS max_id FROM productos", c))
                {
                    using (MySqlDataReader rs = st.ExecuteReader())
                    {
                        if (rs.Read() && !rs.IsDBNull(rs.GetOrdinal("max_id")))
                        {
                            return string.Format("{0:000}", rs.GetInt32("max_id") + 1);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "001";
        }

        private Producto mapear(MySqlDataReader rs)
        {
            double precio = 0;
            if (!rs.IsDBNull(rs.GetOrdinal("precio")))
            {
                precio = Convert.ToDouble(rs.GetDecimal("precio"));
            }

            Producto p = new Producto(
                rs.GetString("id"),
                rs.GetString("nombre"),
                precio,
                rs.GetDouble("stock"),
                rs.GetString("categoria"),
                rs.GetString("unidad"),
                rs.IsDBNull(rs.GetOrdinal("imagen_ruta")) ? null : rs.GetString("imagen_ruta")
            );
            p.setStockMinimo(rs.GetDouble("stock_minimo"));
            return p;
        }

        public void actualizarImagenes(List<Producto> productos)
        {
            string sql = "UPDATE productos SET imagen_ruta=@imagenRuta WHERE id=@id AND (imagen_ruta IS NULL OR imagen_ruta='')";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlTransaction tr = c.BeginTransaction())
                {
                    using (MySqlCommand ps = new MySqlCommand(sql, c, tr))
                    {
                        ps.Parameters.Add("@imagenRuta", MySqlDbType.VarChar);
                        ps.Parameters.Add("@id", MySqlDbType.VarChar);

                        foreach (Producto p in productos)
                        {
                            ps.Parameters["@imagenRuta"].Value = p.getImagenRuta();
                            ps.Parameters["@id"].Value = p.getId();
                            ps.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
