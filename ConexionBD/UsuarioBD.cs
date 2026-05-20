using punto_de_venta_C_.Modelo;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace punto_de_venta_C_.ConexionBD
{
    public class UsuarioBD
    {
        public List<Usuario> obtenerTodos()
        {
            List<Usuario> lista = new List<Usuario>();
            string sql = "SELECT * FROM usuarios ORDER BY id";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand st = new MySqlCommand(sql, c))
                {
                    using (MySqlDataReader rs = st.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            lista.Add(mapear(rs));
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

        public Usuario autenticar(string username, string password)
        {
            string sql = "SELECT * FROM usuarios WHERE username = @username AND password = @password";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@username", username);
                    ps.Parameters.AddWithValue("@password", password);
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

        public bool insertar(Usuario u)
        {
            string sql = "INSERT INTO usuarios (username, password, rol, nombre, edad, sexo) VALUES (@username,@password,@rol,@nombre,@edad,@sexo)";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@username", u.getUsername());
                    ps.Parameters.AddWithValue("@password", u.getPassword());
                    ps.Parameters.AddWithValue("@rol", u.getRol());
                    ps.Parameters.AddWithValue("@nombre", u.getNombre());
                    ps.Parameters.AddWithValue("@edad", u.getEdad());
                    ps.Parameters.AddWithValue("@sexo", u.getSexo());
                    int rows = ps.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        u.setId((int)ps.LastInsertedId);
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

        public bool actualizar(Usuario u)
        {
            string sql = "UPDATE usuarios SET username=@username, password=@password, rol=@rol, nombre=@nombre, edad=@edad, sexo=@sexo WHERE id=@id";
            try
            {
                MySqlConnection c = Conexion.getConexion();
                using (MySqlCommand ps = new MySqlCommand(sql, c))
                {
                    ps.Parameters.AddWithValue("@username", u.getUsername());
                    ps.Parameters.AddWithValue("@password", u.getPassword());
                    ps.Parameters.AddWithValue("@rol", u.getRol());
                    ps.Parameters.AddWithValue("@nombre", u.getNombre());
                    ps.Parameters.AddWithValue("@edad", u.getEdad());
                    ps.Parameters.AddWithValue("@sexo", u.getSexo());
                    ps.Parameters.AddWithValue("@id", u.getId());
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
            string sql = "DELETE FROM usuarios WHERE id = @id";
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

        private Usuario mapear(MySqlDataReader rs)
        {
            return new Usuario(rs.GetInt32("id"), rs.GetString("username"), rs.GetString("password"), rs.GetString("rol"),
                rs.GetString("nombre"), rs.GetInt32("edad"), rs.GetString("sexo"));
        }
    }
}
