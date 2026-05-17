package Controlador;

import Modelo.*;
import Vista.*;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class VentaControlador {

	private final AppControlador app;
	private VentaPanel panel;
	private List<ItemCarrito> carrito = new ArrayList<>();
	private double descuento = 0;

	public VentaControlador(AppControlador app) {
		this.app = app;
	}

	public void setPanel(VentaPanel panel) {
		this.panel = panel;
	}

	public List<Producto> buscarProductos(String texto) {
		if (texto == null || texto.isBlank())
			return new ArrayList<>();
		return app.getProductoBD().buscar(texto);
	}

	public void agregarAlCarrito(Producto p) {
		for (ItemCarrito item : carrito) {
			if (item.getProducto().getId().equals(p.getId())) {
				double nuevaCant = item.getCantidad() + 1.0;
				if (nuevaCant > p.getStock()) {
					javax.swing.JOptionPane.showMessageDialog(null,
							"Stock insuficiente. Disponible: " + (int) p.getStock(), "Sin stock",
							javax.swing.JOptionPane.WARNING_MESSAGE);
					return;
				}
				item.incrementar(1.0);
				notificarPanel();
				return;
			}
		}
		if (p.getStock() <= 0) {
			javax.swing.JOptionPane.showMessageDialog(null, "El producto no tiene existencia", "Sin stock",
					javax.swing.JOptionPane.WARNING_MESSAGE);
			return;
		}
		carrito.add(new ItemCarrito(p, 1.0));
		notificarPanel();
	}

	public void eliminarDelCarrito(String productoId) {
		carrito.removeIf(i -> i.getProducto().getId().equals(productoId));
		notificarPanel();
	}

	public void setCantidad(String productoId, double nuevaCantidad) {
		for (ItemCarrito item : carrito) {
			if (item.getProducto().getId().equals(productoId)) {
					nuevaCantidad = Math.round(nuevaCantidad);
				if (nuevaCantidad > item.getProducto().getStock()) {
					javax.swing.JOptionPane.showMessageDialog(null,
							"Stock insuficiente. Disponible: " + (int) item.getProducto().getStock(), "Sin stock",
							javax.swing.JOptionPane.WARNING_MESSAGE);
					return;
				}
				item.setCantidad(nuevaCantidad);
				break;
			}
		}
		notificarPanel();
	}

	public void setDescuento(double descuento) {
		this.descuento = Math.max(0, descuento);
		notificarPanel();
	}

	public double getDescuento() {
		return descuento;
	}

	public double calcularSubtotal() {
		return carrito.stream().mapToDouble(ItemCarrito::getSubtotal).sum();
	}

	public double calcularTotal() {
		double sub = calcularSubtotal();
		return Math.round(Math.max(0, sub - descuento) * 100.0) / 100.0;
	}

	public double calcularCambio(double recibido) {
		return Math.max(0, recibido - calcularTotal());
	}

	public void procesarCobro(String metodoPago, double montoRecibido) {
		if (carrito.isEmpty())
			return;
		double total = calcularTotal();
		if (montoRecibido < total && "Efectivo".equals(metodoPago)) {
			javax.swing.JOptionPane.showMessageDialog(null,
					String.format("El importe recibido ($%.2f) es menor al total ($%.2f)", montoRecibido, total),
					"Importe insuficiente", javax.swing.JOptionPane.WARNING_MESSAGE);
			return;
		}
		Venta venta = new Venta(0, new ArrayList<>(carrito), total, descuento, metodoPago, new Date(),
				app.getUsuarioActivo().getNombre());
		app.registrarVenta(venta);
		double cambio = calcularCambio(montoRecibido);
		TicketDialog ticket = new TicketDialog(app.getVentanaPrincipal().getFrame(), venta, app.getConfig(), cambio,
				descuento);
		ticket.mostrar();
		carrito.clear();
		descuento = 0;
		if (panel != null)
			panel.limpiar();
	}

	private void notificarPanel() {
		if (panel != null)
			panel.refrescarCarrito(carrito, calcularTotal(), descuento);
	}

	public List<ItemCarrito> getCarrito() {
		return carrito;
	}
}
