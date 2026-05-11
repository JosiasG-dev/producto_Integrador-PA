package Controlador;

import java.util.Date;
import java.util.List;

import Modelo.*;
import Vista.ProveedorPanel;

public class ProveedorControlador {

	private final AppControlador app;
	private ProveedorPanel panel;

	public ProveedorControlador(AppControlador app) {
		this.app = app;
	}

	public void setPanel(ProveedorPanel panel) {
		this.panel = panel;
	}

	public void guardarProveedor(Proveedor p) {
		if (app.getProveedorDAO().buscarPorId(p.getId()) != null) {
			app.getProveedorDAO().actualizar(p);
		} else {
			app.getProveedorDAO().insertar(p);
		}
		if (panel != null)
			panel.refrescarProveedores();
	}

	public void eliminarProveedor(int id) {
		app.getProveedorDAO().eliminar(id);
		if (panel != null)
			panel.refrescarProveedores();
	}

	public int generarIdProveedor() {
		return (int) (System.currentTimeMillis() % 100000);
	}

	public void crearOrden(OrdenCompra orden) {
		app.getOrdenDAO().insertar(orden);
		if (panel != null)
			panel.refrescarOrdenes();
	}

	public void recibirOrden(int ordenId) {
		OrdenCompra orden = app.getOrdenDAO().buscarPorId(ordenId);
		if (orden == null || !orden.isPendiente())
			return;

		for (OrdenCompra.ItemOrden item : orden.getItems()) {
			Producto p = app.getProductoDAO().buscarPorId(item.getProducto().getId());
			if (p != null) {
				double nuevoStock = p.getStock() + item.getCantidadSolicitada();
				app.getProductoDAO().actualizarStock(p.getId(), nuevoStock);
			}
		}
		app.getOrdenDAO().marcarRecibida(ordenId);

		if (orden.getTipoPago() == OrdenCompra.TipoPago.CREDITO) {
			CuentaPorPagar cuenta = new CuentaPorPagar(0, orden.getProveedor(), ordenId, orden.getTotal(), 0,
					orden.getTotal(), "7 días", CuentaPorPagar.Estado.PENDIENTE);
			app.getCuentaDAO().insertar(cuenta);
			if (panel != null)
				panel.refrescarCuentas();
		}

		if (panel != null) {
			panel.refrescarOrdenes();
			app.getVentanaPrincipal().refrescarInventario();
		}
	}

	public void liquidarCuenta(int cuentaId, double monto) {
		for (CuentaPorPagar c : app.getCuentaDAO().obtenerTodas()) {
			if (c.getId() == cuentaId) {
				double nuevoPagado = c.getPagado() + monto;
				double nuevoSaldo = c.getMontoTotal() - nuevoPagado;
				String nuevoEstado = nuevoSaldo <= 0 ? "PAGADO" : "PARCIAL";
				app.getCuentaDAO().aplicarPago(cuentaId, nuevoPagado, Math.max(0, nuevoSaldo), nuevoEstado);
				app.registrarRetiro("Pago " + c.getProveedor().getNombre(), monto);
				break;
			}
		}
		if (panel != null)
			panel.refrescarCuentas();
	}

	public List<Proveedor> getProveedores() {
		return app.getProveedores();
	}

	public List<OrdenCompra> getOrdenes() {
		return app.getOrdenes();
	}

	public List<CuentaPorPagar> getCuentas() {
		return app.getCuentas();
	}

	public List<Producto> getProductos() {
		return app.getProductos();
	}
}
