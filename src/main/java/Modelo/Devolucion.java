package Modelo;

import java.util.Date;

public class Devolucion {
	private int id;
	private int ventaId;
	private Producto producto;
	private double cantidad;
	private String motivo;
	private Date fecha;
	private String cajero;

	public Devolucion(int id, int ventaId, Producto producto, double cantidad, String motivo, Date fecha,
			String cajero) {
		this.id = id;
		this.ventaId = ventaId;
		this.producto = producto;
		this.cantidad = cantidad;
		this.motivo = motivo;
		this.fecha = fecha;
		this.cajero = cajero;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public int getVentaId() {
		return ventaId;
	}

	public Producto getProducto() {
		return producto;
	}

	public double getCantidad() {
		return cantidad;
	}

	public String getMotivo() {
		return motivo;
	}

	public Date getFecha() {
		return fecha;
	}

	public String getCajero() {
		return cajero;
	}

	public double getMontoDevuelto() {
		return Math.round(producto.getPrecio() * cantidad * 100.0) / 100.0;
	}
}
