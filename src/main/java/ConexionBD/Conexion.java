package ConexionBD;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public class Conexion {

	public enum TipoMotor {
		MYSQL, SQLSERVER
	}

	public enum TipoAutenticacion {
		CREDENCIALES, WINDOWS
	}

	private static TipoMotor motorActual = TipoMotor.MYSQL;
	private static TipoAutenticacion autenticacion = TipoAutenticacion.CREDENCIALES;

	private static String host = "localhost";
	private static String puertoInstancia = "3306";
	private static String baseDatos = "corporativo_pos";
	private static String usuario = "root";
	private static String contrasena = "2306";

	private static Connection instancia = null;

	public static TipoMotor getMotor() {
		return motorActual;
	}

	public static void configurar(TipoMotor motor, TipoAutenticacion auth, String h, String pi, String bd, String u,
			String pass) {
		motorActual = motor;
		autenticacion = auth;
		host = h;
		puertoInstancia = pi;
		baseDatos = bd;
		usuario = u;
		contrasena = pass;
		instancia = null;
	}

	public static Connection getConexion() {
		try {
			if (instancia == null || instancia.isClosed()) {
				if (motorActual == TipoMotor.MYSQL) {
					Class.forName("com.mysql.cj.jdbc.Driver");
					String url = "jdbc:mysql://" + host + ":" + puertoInstancia + "/" + baseDatos
							+ "?useSSL=false&serverTimezone=America/Mexico_City"
							+ "&useUnicode=true&characterEncoding=UTF-8&allowPublicKeyRetrieval=true";
					instancia = DriverManager.getConnection(url, usuario, contrasena);
					System.out.println("Conexion con MySQL establecida");
				} else {
					Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
					String url;
					if (autenticacion == TipoAutenticacion.WINDOWS) {
						url = "jdbc:sqlserver://" + host + ":" + puertoInstancia + ";databaseName=" + baseDatos
								+ ";integratedSecurity=true" + ";encrypt=false;trustServerCertificate=true";
						instancia = DriverManager.getConnection(url);
					} else {
						url = "jdbc:sqlserver://" + host + ":" + puertoInstancia + ";databaseName=" + baseDatos
								+ ";encrypt=false;trustServerCertificate=true";
						instancia = DriverManager.getConnection(url, usuario, contrasena);
					}
					System.out.println("Conexion con SQL Server establecida");
				}
				asegurarColumnas();
			}
		} catch (ClassNotFoundException e) {
			System.err.println("Driver no encontrado: " + e.getMessage());
			e.printStackTrace();
		} catch (SQLException e) {
			System.err.println("Error de conexion: " + e.getMessage());
			e.printStackTrace();
		}
		return instancia;
	}

	public static void cerrar() {
		try {
			if (instancia != null && !instancia.isClosed()) {
				instancia.close();
				System.out.println("Conexion cerrada");
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}

	private Conexion() {
	}

	private static void asegurarColumnas() {
		try {
			if (instancia != null && !instancia.isClosed()) {
				java.sql.Statement st = instancia.createStatement();
				String sql;
				if (motorActual == TipoMotor.MYSQL) {
					sql = "ALTER TABLE ventas ADD COLUMN descuento DECIMAL(10,2) DEFAULT 0 AFTER total";
				} else {
					sql = "ALTER TABLE ventas ADD descuento DECIMAL(10,2) DEFAULT 0";
				}
				st.execute(sql);
				st.close();
				System.out.println("Columna 'descuento' asegurada en la base de datos.");
			}
		} catch (Exception e) {
		}
	}
}
