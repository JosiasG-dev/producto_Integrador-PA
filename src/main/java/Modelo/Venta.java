package Modelo;

import java.util.Date;
import java.util.List;

public class Venta {
	private int id;
	private List<ItemCarrito> items;
	private double total;
	private String metodoPago;
	private Date fecha;
	private String cajero;

	public Venta(int id, List<ItemCarrito> items, double total, String metodoPago, Date fecha, String cajero) {
		this.id = id;
		this.items = items;
		this.total = total;
		this.metodoPago = metodoPago;
		this.fecha = fecha;
		this.cajero = cajero;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public List<ItemCarrito> getItems() {
		return items;
	}

	public void setItems(List<ItemCarrito> items) {
		this.items = items;
	}

	public double getTotal() {
		return total;
	}

	public void setTotal(double total) {
		this.total = total;
	}

	public String getMetodoPago() {
		return metodoPago;
	}

	public void setMetodoPago(String metodoPago) {
		this.metodoPago = metodoPago;
	}

	public Date getFecha() {
		return fecha;
	}

	public void setFecha(Date fecha) {
		this.fecha = fecha;
	}

	public String getCajero() {
		return cajero;
	}

	public void setCajero(String cajero) {
		this.cajero = cajero;
	}

	public int getTotalItems() {
		return items.size();
	}
}
