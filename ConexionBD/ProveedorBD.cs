using punto_de_venta_C_.Modelo;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace punto_de_venta_C_.ConexionBD
{
    public class ProveedorBD
    {
        public List<Proveedor> obtenerTodos()
        {
            List<Proveedor> lista = new List<Proveedor>();
            string sql = "SELECT * FROM proveedores WHERE activo = 1 ORDER BY id";
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

        public Proveedor buscarPorId(int id)
        {
            string sql = "SELECT * FROM proveedores WHERE id = @id";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
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

        public bool insertar(Proveedor p)
        {
            string sql = "INSERT INTO proveedores (nombre, contacto, telefono, email, direccion, activo) VALUES (@nombre,@contacto,@telefono,@email,@direccion,@activo)";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@nombre", p.getNombre());
                    ps.Parameters.AddWithValue("@contacto", p.getContacto());
                    ps.Parameters.AddWithValue("@telefono", p.getTelefono());
                    ps.Parameters.AddWithValue("@email", p.getEmail());
                    ps.Parameters.AddWithValue("@direccion", p.getDireccion());
                    ps.Parameters.AddWithValue("@activo", p.isActivo());
                    int rows = ps.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        p.setId((int)ps.LastInsertedId);
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

        public bool actualizar(Proveedor p)
        {
            string sql = "UPDATE proveedores SET nombre=@nombre, contacto=@contacto, telefono=@telefono, email=@email, direccion=@direccion WHERE id=@id";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@nombre", p.getNombre());
                    ps.Parameters.AddWithValue("@contacto", p.getContacto());
                    ps.Parameters.AddWithValue("@telefono", p.getTelefono());
                    ps.Parameters.AddWithValue("@email", p.getEmail());
                    ps.Parameters.AddWithValue("@direccion", p.getDireccion());
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

        public bool eliminar(int id)
        {
            string sql = "UPDATE proveedores SET activo = 0 WHERE id = @id";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
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

        private Proveedor mapear(MySqlDataReader rs)
        {
            return new Proveedor(
                rs.GetInt32("id"), 
                rs.GetString("nombre"), 
                rs.GetString("contacto"),
                rs.GetString("telefono"), 
                rs.GetString("email"), 
                rs.GetString("direccion"), 
                rs.GetBoolean("activo")
            );
        }
    }
}
