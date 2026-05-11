package ConexionBD;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;
import Modelo.Producto;

public class ProductoBD {

	public List<Producto> obtenerTodos() {
		List<Producto> lista = new ArrayList<>();
		try (Statement st = Conexion.getConexion().createStatement();
				ResultSet rs = st.executeQuery("SELECT * FROM productos ORDER BY nombre")) {
			while (rs.next())
				lista.add(mapear(rs));
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return lista;
	}

	public Producto buscarPorId(String id) {
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement("SELECT * FROM productos WHERE id = ?")) {
			ps.setString(1, id);
			ResultSet rs = ps.executeQuery();
			if (rs.next())
				return mapear(rs);
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return null;
	}

	public List<Producto> buscar(String texto) {
		List<Producto> lista = new ArrayList<>();
		try (PreparedStatement ps = Conexion.getConexion()
				.prepareStatement("SELECT * FROM productos WHERE nombre LIKE ? OR id LIKE ? LIMIT 10")) {
			String q = "%" + texto + "%";
			ps.setString(1, q);
			ps.setString(2, q);
			ResultSet rs = ps.executeQuery();
			while (rs.next())
				lista.add(mapear(rs));
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return lista;
	}

	public boolean insertar(Producto p) {
		String sql = "INSERT INTO productos (id, nombre, precio, stock, stock_minimo, categoria, unidad) VALUES (?,?,?,?,?,?,?)";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setString(1, p.getId());
			ps.setString(2, p.getNombre());
			ps.setDouble(3, p.getPrecio());
			ps.setDouble(4, p.getStock());
			ps.setDouble(5, p.getStockMinimo());
			ps.setString(6, p.getCategoria());
			ps.setString(7, p.getUnidad());
			return ps.executeUpdate() > 0;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public boolean actualizar(Producto p) {
		String sql = "UPDATE productos SET nombre=?, precio=?, stock=?, stock_minimo=?, categoria=?, unidad=? WHERE id=?";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setString(1, p.getNombre());
			ps.setDouble(2, p.getPrecio());
			ps.setDouble(3, p.getStock());
			ps.setDouble(4, p.getStockMinimo());
			ps.setString(5, p.getCategoria());
			ps.setString(6, p.getUnidad());
			ps.setString(7, p.getId());
			return ps.executeUpdate() > 0;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public boolean actualizarStock(String id, double nuevoStock) {
		try (PreparedStatement ps = Conexion.getConexion()
				.prepareStatement("UPDATE productos SET stock=? WHERE id=?")) {
			ps.setDouble(1, nuevoStock);
			ps.setString(2, id);
			return ps.executeUpdate() > 0;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public boolean eliminar(String id) {
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement("DELETE FROM productos WHERE id=?")) {
			ps.setString(1, id);
			return ps.executeUpdate() > 0;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public String generarNuevoId() {
		try (Statement st = Conexion.getConexion().createStatement();
				ResultSet rs = st.executeQuery("SELECT MAX(CAST(id AS UNSIGNED)) AS max_id FROM productos")) {
			if (rs.next())
				return String.format("%03d", rs.getInt("max_id") + 1);
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return "001";
	}

	private Producto mapear(ResultSet rs) throws SQLException {
		Producto p = new Producto(rs.getString("id"), rs.getString("nombre"), rs.getDouble("precio"),
				rs.getDouble("stock"), rs.getString("categoria"), rs.getString("unidad"));
		try {
			p.setStockMinimo(rs.getDouble("stock_minimo"));
		} catch (SQLException ignored) {
			p.setStockMinimo(5);
		}
		return p;
	}
}
