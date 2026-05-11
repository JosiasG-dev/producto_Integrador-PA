package Modelo;

import java.util.ArrayList;
import java.util.List;

public class DatosIniciales {

	public static List<Producto> getProductos() {
		List<Producto> lista = new ArrayList<>();
		lista.add(new Producto("001", "Coca-Cola 600ml", 18.00, 50, "Bebidas y Liquidos", "Litros / mL"));
		lista.add(new Producto("002", "Sabritas Original 45g", 14.00, 40, "Botanas y Dulces", "Kilogramos / g"));
		lista.add(new Producto("003", "Arroz Verde Valle 1kg", 25.00, 60, "Despensa Basica", "Kilogramos / g"));
		lista.add(new Producto("004", "Leche Lala 1L", 22.00, 30, "Lacteos y Huevo", "Litros / mL"));
		lista.add(
				new Producto("005", "Atun Herdez 140g", 19.00, 30, "Alimentos Preparados/Enlatados", "Kilogramos / g"));
		return lista;
	}

	public static List<Usuario> getUsuarios() {
		List<Usuario> lista = new ArrayList<>();
		lista.add(new Usuario(1, "admin", "admin123", "ADMINISTRADOR", "Director General", 45, "Masculino"));
		lista.add(new Usuario(2, "cajero1", "caja123", "CAJERO", "Juan Jose", 28, "Masculino"));
		return lista;
	}

	public static List<Proveedor> getProveedores() {
		List<Proveedor> lista = new ArrayList<>();
		lista.add(new Proveedor(1, "GRUPO BIMBO", "Juan Perez", "8112345678", "bimbo@proveedor.mx", "Monterrey, NL",
				true));
		lista.add(new Proveedor(2, "COCA COLA FEMSA", "Ana Torres", "8187654321", "femsa@proveedor.mx", "Guadalupe, NL",
				true));
		lista.add(new Proveedor(3, "BARCEL S.A.", "Carlos Ruiz", "8199991111", "barcel@proveedor.mx", "CDMX", true));
		lista.add(new Proveedor(4, "LALA S.A. DE C.V.", "Maria Lopez", "8133334444", "lala@proveedor.mx",
				"Torreon, Coah", true));
		return lista;
	}

	public static String[] getCategorias() {
		return new String[] { "Despensa Basica", "Lacteos y Huevo", "Bebidas y Liquidos", "Botanas y Dulces",
				"Frutas y Verduras", "Carnes y Salchichoneria", "Cuidado del Hogar", "Higiene y Cuidado Personal",
				"Alimentos Preparados/Enlatados" };
	}

	public static String[] getUnidades() {
		return new String[] { "Piezas", "Kilogramos / g", "Litros / mL" };
	}
}
