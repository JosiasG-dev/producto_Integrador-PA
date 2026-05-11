package ConexionBD;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;

import ConexionBD.Conexion;
import Modelo.Proveedor;

public class ProveedorBD {

	public List<Proveedor> obtenerTodos() {
		List<Proveedor> lista = new ArrayList<>();
		String sql = "SELECT * FROM proveedores WHERE activo = 1 ORDER BY nombre";
		try (Statement st = Conexion.getConexion().createStatement(); ResultSet rs = st.executeQuery(sql)) {
			while (rs.next())
				lista.add(mapear(rs));
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return lista;
	}

	public Proveedor buscarPorId(int id) {
		String sql = "SELECT * FROM proveedores WHERE id = ?";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setInt(1, id);
			ResultSet rs = ps.executeQuery();
			if (rs.next())
				return mapear(rs);
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return null;
	}

	public boolean insertar(Proveedor p) {
		String sql = "INSERT INTO proveedores (nombre, contacto, telefono, email, direccion, activo) VALUES (?,?,?,?,?,?)";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql, Statement.RETURN_GENERATED_KEYS)) {
			ps.setString(1, p.getNombre());
			ps.setString(2, p.getContacto());
			ps.setString(3, p.getTelefono());
			ps.setString(4, p.getEmail());
			ps.setString(5, p.getDireccion());
			ps.setBoolean(6, p.isActivo());
			int rows = ps.executeUpdate();
			if (rows > 0) {
				ResultSet keys = ps.getGeneratedKeys();
				if (keys.next())
					p.setId(keys.getInt(1));
				return true;
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public boolean actualizar(Proveedor p) {
		String sql = "UPDATE proveedores SET nombre=?, contacto=?, telefono=?, email=?, direccion=? WHERE id=?";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setString(1, p.getNombre());
			ps.setString(2, p.getContacto());
			ps.setString(3, p.getTelefono());
			ps.setString(4, p.getEmail());
			ps.setString(5, p.getDireccion());
			ps.setInt(6, p.getId());
			return ps.executeUpdate() > 0;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public boolean eliminar(int id) {
		String sql = "UPDATE proveedores SET activo = 0 WHERE id = ?";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setInt(1, id);
			return ps.executeUpdate() > 0;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	private Proveedor mapear(ResultSet rs) throws SQLException {
		return new Proveedor(rs.getInt("id"), rs.getString("nombre"), rs.getString("contacto"),
				rs.getString("telefono"), rs.getString("email"), rs.getString("direccion"), rs.getBoolean("activo"));
	}
}
