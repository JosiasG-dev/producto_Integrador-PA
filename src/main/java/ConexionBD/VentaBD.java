package ConexionBD;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;
import Modelo.ItemCarrito;
import Modelo.Producto;
import Modelo.Venta;

public class VentaBD {

	public boolean insertar(Venta venta) {
		Connection conn = Conexion.getConexion();
		String sqlVenta = "INSERT INTO ventas (total, metodo_pago, fecha, cajero) VALUES (?,?,?,?)";
		String sqlItem = "INSERT INTO venta_items (venta_id, producto_id, cantidad, precio_unit) VALUES (?,?,?,?)";
		try {
			conn.setAutoCommit(false);

			PreparedStatement psVenta = conn.prepareStatement(sqlVenta, Statement.RETURN_GENERATED_KEYS);
			psVenta.setDouble(1, venta.getTotal());
			psVenta.setString(2, venta.getMetodoPago());
			psVenta.setTimestamp(3, new Timestamp(venta.getFecha().getTime()));
			psVenta.setString(4, venta.getCajero());
			psVenta.executeUpdate();

			ResultSet keys = psVenta.getGeneratedKeys();
			if (!keys.next())
				throw new SQLException("No se genero ID para la venta");
			int idVenta = keys.getInt(1);
			venta.setId(idVenta);

			PreparedStatement psItem = conn.prepareStatement(sqlItem);
			for (ItemCarrito item : venta.getItems()) {
				psItem.setInt(1, idVenta);
				psItem.setString(2, item.getProducto().getId());
				psItem.setDouble(3, item.getCantidad());
				psItem.setDouble(4, item.getProducto().getPrecio());
				psItem.addBatch();
			}
			psItem.executeBatch();

			ProductoBD productoDAO = new ProductoBD();
			for (ItemCarrito item : venta.getItems()) {
				Producto p = item.getProducto();
				double nuevoStock = Math.round((p.getStock() - item.getCantidad()) * 1000.0) / 1000.0;
				productoDAO.actualizarStock(p.getId(), nuevoStock);
				p.setStock(nuevoStock);
			}

			conn.commit();
			return true;
		} catch (SQLException e) {
			try {
				conn.rollback();
			} catch (SQLException ex) {
				ex.printStackTrace();
			}
			e.printStackTrace();
			return false;
		} finally {
			try {
				conn.setAutoCommit(true);
			} catch (SQLException e) {
				e.printStackTrace();
			}
		}
	}

	public List<Venta> obtenerTodas() {
		List<Venta> lista = new ArrayList<>();
		try (Statement st = Conexion.getConexion().createStatement();
				ResultSet rs = st.executeQuery("SELECT * FROM ventas ORDER BY fecha DESC")) {
			while (rs.next()) {
				lista.add(new Venta(rs.getInt("id"), obtenerItems(rs.getInt("id")), rs.getDouble("total"),
						rs.getString("metodo_pago"), rs.getTimestamp("fecha"), rs.getString("cajero")));
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return lista;
	}

	private List<ItemCarrito> obtenerItems(int ventaId) {
		List<ItemCarrito> items = new ArrayList<>();
		String sql = "SELECT vi.*, p.nombre, p.precio, p.categoria, p.unidad "
				+ "FROM venta_items vi JOIN productos p ON vi.producto_id = p.id WHERE vi.venta_id = ?";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setInt(1, ventaId);
			ResultSet rs = ps.executeQuery();
			while (rs.next()) {
				Producto p = new Producto(rs.getString("producto_id"), rs.getString("nombre"),
						rs.getDouble("precio_unit"), 0, rs.getString("categoria"), rs.getString("unidad"));
				items.add(new ItemCarrito(p, rs.getDouble("cantidad")));
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return items;
	}
}
