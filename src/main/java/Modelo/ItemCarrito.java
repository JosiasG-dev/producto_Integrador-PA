package Modelo;

public class ItemCarrito {
	private Producto producto;
	private double cantidad;

	public ItemCarrito(Producto producto, double cantidad) {
		this.producto = producto;
		this.cantidad = cantidad;
	}

	public Producto getProducto() {
		return producto;
	}

	public void setProducto(Producto producto) {
		this.producto = producto;
	}

	public double getCantidad() {
		return cantidad;
	}

	public void setCantidad(double cantidad) {
		this.cantidad = Math.round(cantidad * 1000.0) / 1000.0;
	}

	public double getSubtotal() {
		return Math.round(producto.getPrecio() * cantidad * 100.0) / 100.0;
	}

	public void incrementar(double delta) {
		this.cantidad = Math.round((this.cantidad + delta) * 1000.0) / 1000.0;
	}

	public void decrementar(double delta) {
		double nueva = this.cantidad - delta;
		this.cantidad = Math.max(0, Math.round(nueva * 1000.0) / 1000.0);
	}
}
