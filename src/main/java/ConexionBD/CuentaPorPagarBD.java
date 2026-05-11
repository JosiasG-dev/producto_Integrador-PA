package ConexionBD;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;

import ConexionBD.Conexion;
import Modelo.CuentaPorPagar;
import Modelo.Proveedor;

public class CuentaPorPagarBD {

	public boolean insertar(CuentaPorPagar c) {
		String sql = "INSERT INTO cuentas_por_pagar (proveedor_id, orden_id, monto_total, pagado, saldo, vencimiento, estado) VALUES (?,?,?,?,?,?,?)";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql, Statement.RETURN_GENERATED_KEYS)) {
			ps.setInt(1, c.getProveedor().getId());
			ps.setInt(2, c.getOrdenId());
			ps.setDouble(3, c.getMontoTotal());
			ps.setDouble(4, c.getPagado());
			ps.setDouble(5, c.getSaldo());
			ps.setString(6, c.getVencimiento());
			ps.setString(7, c.getEstado().name());
			int rows = ps.executeUpdate();
			if (rows > 0) {
				ResultSet keys = ps.getGeneratedKeys();
				if (keys.next())
					c.setId(keys.getInt(1));
				return true;
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public boolean aplicarPago(int id, double montoPagado, double nuevoSaldo, String nuevoEstado) {
		String sql = "UPDATE cuentas_por_pagar SET pagado=?, saldo=?, estado=? WHERE id=?";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setDouble(1, montoPagado);
			ps.setDouble(2, nuevoSaldo);
			ps.setString(3, nuevoEstado);
			ps.setInt(4, id);
			return ps.executeUpdate() > 0;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public List<CuentaPorPagar> obtenerActivas() {
		List<CuentaPorPagar> lista = new ArrayList<>();
		String sql = "SELECT cpp.*, p.nombre AS prov_nombre, p.contacto, p.telefono, p.email, p.direccion "
				+ "FROM cuentas_por_pagar cpp JOIN proveedores p ON cpp.proveedor_id = p.id "
				+ "WHERE cpp.estado != 'PAGADO' ORDER BY cpp.id DESC";
		return ejecutarQuery(lista, sql);
	}

	public List<CuentaPorPagar> obtenerTodas() {
		List<CuentaPorPagar> lista = new ArrayList<>();
		String sql = "SELECT cpp.*, p.nombre AS prov_nombre, p.contacto, p.telefono, p.email, p.direccion "
				+ "FROM cuentas_por_pagar cpp JOIN proveedores p ON cpp.proveedor_id = p.id ORDER BY cpp.id DESC";
		return ejecutarQuery(lista, sql);
	}

	private List<CuentaPorPagar> ejecutarQuery(List<CuentaPorPagar> lista, String sql) {
		try (Statement st = Conexion.getConexion().createStatement(); ResultSet rs = st.executeQuery(sql)) {
			while (rs.next()) {
				Proveedor prov = new Proveedor(rs.getInt("proveedor_id"), rs.getString("prov_nombre"),
						rs.getString("contacto"), rs.getString("telefono"), rs.getString("email"),
						rs.getString("direccion"), true);
				lista.add(new CuentaPorPagar(rs.getInt("id"), prov, rs.getInt("orden_id"), rs.getDouble("monto_total"),
						rs.getDouble("pagado"), rs.getDouble("saldo"), rs.getString("vencimiento"),
						CuentaPorPagar.Estado.valueOf(rs.getString("estado"))));
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return lista;
	}
}
