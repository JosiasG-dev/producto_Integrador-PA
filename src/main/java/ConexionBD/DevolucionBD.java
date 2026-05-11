package ConexionBD;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;
import Modelo.Devolucion;
import Modelo.Producto;

public class DevolucionBD {

	public boolean insertar(Devolucion d) {
		String sql = "INSERT INTO devoluciones (venta_id, producto_id, cantidad, motivo, fecha, cajero) VALUES (?,?,?,?,?,?)";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql, Statement.RETURN_GENERATED_KEYS)) {
			ps.setInt(1, d.getVentaId());
			ps.setString(2, d.getProducto().getId());
			ps.setDouble(3, d.getCantidad());
			ps.setString(4, d.getMotivo());
			ps.setTimestamp(5, new Timestamp(d.getFecha().getTime()));
			ps.setString(6, d.getCajero());
			ps.executeUpdate();
			ResultSet keys = ps.getGeneratedKeys();
			if (keys.next())
				d.setId(keys.getInt(1));
			return true;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public List<Devolucion> obtenerPorVenta(int ventaId) {
		List<Devolucion> lista = new ArrayList<>();
		String sql = "SELECT d.*, p.nombre, p.precio, p.categoria, p.unidad "
				+ "FROM devoluciones d JOIN productos p ON d.producto_id = p.id WHERE d.venta_id = ?";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setInt(1, ventaId);
			ResultSet rs = ps.executeQuery();
			while (rs.next()) {
				Producto prod = new Producto(rs.getString("producto_id"), rs.getString("nombre"),
						rs.getDouble("precio"), 0, rs.getString("categoria"), rs.getString("unidad"));
				lista.add(new Devolucion(rs.getInt("id"), ventaId, prod, rs.getDouble("cantidad"),
						rs.getString("motivo"), rs.getTimestamp("fecha"), rs.getString("cajero")));
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return lista;
	}

	public List<Devolucion> obtenerTodas() {
		List<Devolucion> lista = new ArrayList<>();
		String sql = "SELECT d.*, p.nombre, p.precio, p.categoria, p.unidad "
				+ "FROM devoluciones d JOIN productos p ON d.producto_id = p.id ORDER BY d.fecha DESC";
		try (Statement st = Conexion.getConexion().createStatement(); ResultSet rs = st.executeQuery(sql)) {
			while (rs.next()) {
				Producto prod = new Producto(rs.getString("producto_id"), rs.getString("nombre"),
						rs.getDouble("precio"), 0, rs.getString("categoria"), rs.getString("unidad"));
				lista.add(new Devolucion(rs.getInt("id"), rs.getInt("venta_id"), prod, rs.getDouble("cantidad"),
						rs.getString("motivo"), rs.getTimestamp("fecha"), rs.getString("cajero")));
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return lista;
	}
}
