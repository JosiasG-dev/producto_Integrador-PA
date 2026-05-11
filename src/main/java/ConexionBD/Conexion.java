package ConexionBD;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;


public class Conexion {

    private static final String HOST     = "localhost";
    private static final String PORT     = "3306";
    private static final String DATABASE = "corporativo_pos";
    private static final String USER     = "root";
    private static final String PASSWORD = "2306"; 

    private static final String URL =
        "jdbc:mysql://" + HOST + ":" + PORT + "/" + DATABASE
        + "?useSSL=false"
        + "&serverTimezone=America/Mexico_City"
        + "&useUnicode=true"
        + "&characterEncoding=UTF-8"
        + "&allowPublicKeyRetrieval=true";

    private static Connection instancia = null;

    public static Connection getConexion() {
        try {
            if (instancia == null || instancia.isClosed()) {
                Class.forName("com.mysql.cj.jdbc.Driver");
                instancia = DriverManager.getConnection(URL, USER, PASSWORD);
                System.out.println("Conexion con MySQL establecida");
            }
        } catch (ClassNotFoundException e) {
            System.err.println("Driver MySQL no encontrado");
            e.printStackTrace();
        } catch (SQLException e) {
            System.err.println("Error de conexion a MySQL: " + e.getMessage());
            System.err.println("   Verifica HOST, PORT, USER y PASSWORD en Conexion.java");
            e.printStackTrace();
        }
        return instancia;
    }

    public static void cerrar() {
        try {
            if (instancia != null && !instancia.isClosed()) {
                instancia.close();
                System.out.println("Conexion con MySQL cerrada");
            }
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    private Conexion() {}
}
