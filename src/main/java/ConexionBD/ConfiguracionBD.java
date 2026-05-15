package ConexionBD;

import Modelo.ConfiguracionTienda;
import java.sql.*;

public class ConfiguracionBD {

    public ConfiguracionTienda obtener() {
        String sql = "SELECT * FROM configuracion WHERE id = 1";
        try (Connection c = Conexion.getConexion(); Statement st = c.createStatement(); ResultSet rs = st.executeQuery(sql)) {
            if (rs.next()) {
                return new ConfiguracionTienda(rs.getString("nombre_tienda"), rs.getString("sucursal"), rs.getString("rfc"));
            }
        } catch (SQLException e) { e.printStackTrace(); }
        return null;
    }

    public void actualizar(ConfiguracionTienda config) {
        String sql = "UPDATE configuracion SET nombre_tienda=?, sucursal=?, rfc=? WHERE id=1";
        
        try (Connection c = Conexion.getConexion(); 
             PreparedStatement ps = c.prepareStatement(sql)) {
            
            ps.setString(1, config.getNombre());
            ps.setString(2, config.getSucursal());
            ps.setString(3, config.getRfc());
            
            int filasAfectadas = ps.executeUpdate();
            System.out.println("Filas actualizadas en BD: " + filasAfectadas);
            
        } catch (SQLException e) {
            System.err.println("Error al guardar en BD: " + e.getMessage());
            e.printStackTrace();
        }
    }
}