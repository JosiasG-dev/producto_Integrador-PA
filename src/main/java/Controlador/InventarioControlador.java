package Controlador;

import java.util.List;

import Modelo.Producto;
import Vista.InventarioPanel;

public class InventarioControlador {

	private final ControladorPrincipal app;
	private InventarioPanel panel;

	public InventarioControlador(ControladorPrincipal app) {
		this.app = app;
	}

	public ControladorPrincipal getApp() {
		return app;
	}

	public void setPanel(InventarioPanel panel) {
		this.panel = panel;
	}

	public List<Producto> filtrar(String texto, String categoria) {
		List<Producto> todos = app.getProductos();
		List<Producto> resultado = new java.util.ArrayList<>();
		for (Producto p : todos) {
			boolean matchTexto = texto == null || texto.isBlank()
					|| p.getNombre().toLowerCase().contains(texto.toLowerCase()) || p.getId().contains(texto);
			boolean matchCat = categoria == null || categoria.isBlank() || p.getCategoria().equals(categoria);
			if (matchTexto && matchCat)
				resultado.add(p);
		}
		return resultado;
	}

	public void guardarProducto(Producto p) {
		Producto existente = app.getProductoBD().buscarPorId(p.getId());
		if (existente != null) {
			app.getProductoBD().actualizar(p);
		} else {
			app.getProductoBD().insertar(p);
		}
		if (panel != null)
			panel.refrescar();
	}

	public void eliminarProducto(String id) {
		app.getProductoBD().eliminar(id);
		if (panel != null)
			panel.refrescar();
	}

	public Producto buscarPorId(String id) {
		return app.getProductoBD().buscarPorId(id);
	}

	public String generarNuevoId() {
		return app.getProductoBD().generarNuevoId();
	}
}
