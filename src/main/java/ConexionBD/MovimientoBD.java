package ConexionBD;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;
import Modelo.Movimiento;

public class MovimientoBD {

	public boolean insertar(Movimiento m) {
		String sql = "INSERT INTO movimientos (tipo, descripcion, monto, fecha, usuario) VALUES (?,?,?,?,?)";
		try (PreparedStatement ps = Conexion.getConexion().prepareStatement(sql)) {
			ps.setString(1, m.getTipo().name());
			ps.setString(2, m.getDescripcion());
			ps.setDouble(3, m.getMonto());
			ps.setTimestamp(4, new Timestamp(m.getFecha().getTime()));
			ps.setString(5, m.getUsuario());
			return ps.executeUpdate() > 0;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return false;
	}

	public List<Movimiento> obtenerDelDia() {
		List<Movimiento> lista = new ArrayList<>();
		String sql;
		if (Conexion.getMotor() == Conexion.TipoMotor.MYSQL) {
			sql = "SELECT * FROM movimientos WHERE DATE(fecha) = CURDATE() ORDER BY fecha DESC";
		} else {
			sql = "SELECT * FROM movimientos WHERE CAST(fecha AS DATE) = CAST(GETDATE() AS DATE) ORDER BY fecha DESC";
		}
		try (Statement st = Conexion.getConexion().createStatement(); ResultSet rs = st.executeQuery(sql)) {
			while (rs.next())
				lista.add(mapear(rs));
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return lista;
	}

	public List<Movimiento> obtenerTodos() {
		List<Movimiento> lista = new ArrayList<>();
		String sql = "SELECT * FROM movimientos ORDER BY fecha DESC";
		try (Statement st = Conexion.getConexion().createStatement(); ResultSet rs = st.executeQuery(sql)) {
			while (rs.next())
				lista.add(mapear(rs));
		} catch (SQLException e) {
			e.printStackTrace();
		}
		return lista;
	}

	private Movimiento mapear(ResultSet rs) throws SQLException {
		return new Movimiento(Movimiento.Tipo.valueOf(rs.getString("tipo")), rs.getString("descripcion"),
				rs.getDouble("monto"), rs.getTimestamp("fecha"), rs.getString("usuario"));
	}
}
