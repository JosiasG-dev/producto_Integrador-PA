package ConexionBD;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;

import ConexionBD.Conexion;
import Modelo.Usuario;

public class UsuarioBD {

	public List<Usuario> obtenerTodos() {
		List<Usuario> lista = new ArrayList<>();
		String sql = "SELECT * FROM usuarios ORDER BY id";
		try (Statement st = Conexion.getConexion().createStatement(); ResultSet rs = st.executeQuery(sql)) {
			while (rs.next()) {
				lista.add(mapear(rs));
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return lista;
	}

	public Usuario autenticar(String username, String password) {
		String sql = "SELECT * FROM usuarios WHERE username = ? AND password = ?";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setString(1, username);
			ps.setString(2, password);
			ResultSet rs = ps.executeQuery();
			if (rs.next())
				return mapear(rs);
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return null;
	}

	public boolean insertar(Usuario u) {
		String sql = "INSERT INTO usuarios (username, password, rol, nombre, edad, sexo) VALUES (?,?,?,?,?,?)";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql, Statement.RETURN_GENERATED_KEYS)) {
			ps.setString(1, u.getUsername());
			ps.setString(2, u.getPassword());
			ps.setString(3, u.getRol());
			ps.setString(4, u.getNombre());
			ps.setInt(5, u.getEdad());
			ps.setString(6, u.getSexo());
			int rows = ps.executeUpdate();
			if (rows > 0) {
				ResultSet keys = ps.getGeneratedKeys();
				if (keys.next())
					u.setId(keys.getInt(1));
				return true;
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public boolean actualizar(Usuario u) {
		String sql = "UPDATE usuarios SET username=?, password=?, rol=?, nombre=?, edad=?, sexo=? WHERE id=?";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setString(1, u.getUsername());
			ps.setString(2, u.getPassword());
			ps.setString(3, u.getRol());
			ps.setString(4, u.getNombre());
			ps.setInt(5, u.getEdad());
			ps.setString(6, u.getSexo());
			ps.setInt(7, u.getId());
			return ps.executeUpdate() > 0;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public boolean eliminar(int id) {
		String sql = "DELETE FROM usuarios WHERE id = ?";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setInt(1, id);
			return ps.executeUpdate() > 0;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	private Usuario mapear(ResultSet rs) throws SQLException {
		return new Usuario(rs.getInt("id"), rs.getString("username"), rs.getString("password"), rs.getString("rol"),
				rs.getString("nombre"), rs.getInt("edad"), rs.getString("sexo"));
	}
}
