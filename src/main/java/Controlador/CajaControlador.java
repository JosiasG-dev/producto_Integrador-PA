package Controlador;

import java.util.List;

import Modelo.*;
import Vista.CajaPanel;

public class CajaControlador {

	private final AppControlador app;
	private CajaPanel panel;

	public CajaControlador(AppControlador app) {
		this.app = app;
	}

	public void setPanel(CajaPanel panel) {
		this.panel = panel;
	}

	public void abrirCaja(double fondo) {
		app.abrirCaja(fondo);
		if (panel != null)
			panel.refrescar();
	}

	public void registrarRetiro(String concepto, double monto) {
		app.registrarRetiro(concepto, monto);
		if (panel != null)
			panel.refrescar();
	}
	

	public void registrarIngresoExtra(String concepto, double monto) {
	    app.getMovimientoBD().insertar(new Movimiento(Movimiento.Tipo.VENTA, concepto, monto, 
	            new java.util.Date(), app.getUsuarioActivo().getNombre()));
	    app.registrarVentaSimple(monto);
	    if (panel != null)
	        panel.refrescar();
	}
	
	public double getTotalVentas() {
		return app.getMovimientos().stream().filter(m -> m.getTipo() == Movimiento.Tipo.VENTA)
				.mapToDouble(Movimiento::getMonto).sum();
	}

	public double getTotalEgresos() {
		return app.getMovimientos().stream().filter(m -> m.getTipo() == Movimiento.Tipo.RETIRO)
				.mapToDouble(Movimiento::getMonto).sum();
	}

	public double getEfectivoEsperado() {
		return app.getMontoCaja();
	}

	public List<Movimiento> getMovimientos() {
		return app.getMovimientos();
	}

	public boolean isCajaAbierta() {
		return app.isCajaAbierta();
	}

	public double getMontoCaja() {
		return app.getMontoCaja();
	}
}
