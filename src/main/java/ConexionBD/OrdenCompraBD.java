package ConexionBD;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;
import Modelo.OrdenCompra;
import Modelo.Producto;
import Modelo.Proveedor;

public class OrdenCompraBD {

	public boolean insertar(OrdenCompra orden) {
		Connection conn = Conexion.getConexion();
		String sqlOrden = "INSERT INTO ordenes_compra (proveedor_id, total, tipo_pago, estado, fecha) VALUES (?,?,?,?,?)";
		String sqlItem = "INSERT INTO orden_items (orden_id, producto_id, cantidad_solicitada, precio_costo) VALUES (?,?,?,?)";
		try {
			conn.setAutoCommit(false);

			PreparedStatement psOrden = conn.prepareStatement(sqlOrden, Statement.RETURN_GENERATED_KEYS);
			psOrden.setInt(1, orden.getProveedor().getId());
			psOrden.setDouble(2, orden.getTotal());
			psOrden.setString(3, orden.getTipoPago().name());
			psOrden.setString(4, orden.getEstado().name());
			psOrden.setTimestamp(5, new Timestamp(orden.getFecha().getTime()));
			psOrden.executeUpdate();

			ResultSet keys = psOrden.getGeneratedKeys();
			if (!keys.next())
				throw new SQLException("No se genero ID para la orden");
			int idOrden = keys.getInt(1);
			orden.setId(idOrden);

			PreparedStatement psItem = conn.prepareStatement(sqlItem);
			for (OrdenCompra.ItemOrden item : orden.getItems()) {
				psItem.setInt(1, idOrden);
				psItem.setString(2, item.getProducto().getId());
				psItem.setDouble(3, item.getCantidadSolicitada());
				psItem.setDouble(4, item.getPrecioCosto());
				psItem.addBatch();
			}
			psItem.executeBatch();

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

	public boolean marcarRecibida(int id) {
		try (PreparedStatement ps = Conexion.getConexion()
				.prepareStatement("UPDATE ordenes_compra SET estado = 'RECIBIDO' WHERE id = ?")) {
			ps.setInt(1, id);
			return ps.executeUpdate() > 0;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public List<OrdenCompra> obtenerTodas() {
		List<OrdenCompra> lista = new ArrayList<>();
		String sql = "SELECT oc.*, p.nombre AS prov_nombre, p.contacto, p.telefono, p.email, p.direccion "
				+ "FROM ordenes_compra oc JOIN proveedores p ON oc.proveedor_id = p.id ORDER BY oc.fecha DESC";
		try (Statement st = Conexion.getConexion().createStatement(); ResultSet rs = st.executeQuery(sql)) {
			while (rs.next()) {
				Proveedor prov = new Proveedor(rs.getInt("proveedor_id"), rs.getString("prov_nombre"),
						rs.getString("contacto"), rs.getString("telefono"), rs.getString("email"),
						rs.getString("direccion"), true);
				lista.add(new OrdenCompra(rs.getInt("id"), prov, obtenerItems(rs.getInt("id")), rs.getDouble("total"),
						OrdenCompra.TipoPago.valueOf(rs.getString("tipo_pago")),
						OrdenCompra.Estado.valueOf(rs.getString("estado")), rs.getTimestamp("fecha")));
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return lista;
	}

	public OrdenCompra buscarPorId(int id) {
		String sql = "SELECT oc.*, p.nombre AS prov_nombre, p.contacto, p.telefono, p.email, p.direccion "
				+ "FROM ordenes_compra oc JOIN proveedores p ON oc.proveedor_id = p.id WHERE oc.id = ?";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setInt(1, id);
			ResultSet rs = ps.executeQuery();
			if (rs.next()) {
				Proveedor prov = new Proveedor(rs.getInt("proveedor_id"), rs.getString("prov_nombre"),
						rs.getString("contacto"), rs.getString("telefono"), rs.getString("email"),
						rs.getString("direccion"), true);
				return new OrdenCompra(rs.getInt("id"), prov, obtenerItems(id), rs.getDouble("total"),
						OrdenCompra.TipoPago.valueOf(rs.getString("tipo_pago")),
						OrdenCompra.Estado.valueOf(rs.getString("estado")), rs.getTimestamp("fecha"));
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return null;
	}

	private List<OrdenCompra.ItemOrden> obtenerItems(int ordenId) {
		List<OrdenCompra.ItemOrden> items = new ArrayList<>();
		String sql = "SELECT oi.*, p.nombre, p.precio, p.categoria, p.unidad "
				+ "FROM orden_items oi JOIN productos p ON oi.producto_id = p.id WHERE oi.orden_id = ?";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setInt(1, ordenId);
			ResultSet rs = ps.executeQuery();
			while (rs.next()) {
				Producto prod = new Producto(rs.getString("producto_id"), rs.getString("nombre"),
						rs.getDouble("precio"), 0, rs.getString("categoria"), rs.getString("unidad"));
				items.add(new OrdenCompra.ItemOrden(prod, rs.getDouble("cantidad_solicitada"),
						rs.getDouble("precio_costo")));
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return items;
	}
}
