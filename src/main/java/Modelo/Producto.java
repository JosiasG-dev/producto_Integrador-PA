package Modelo;

public class Producto {
	private String id;
	private String nombre;
	private double precio;
	private double stock;
	private double stockMinimo;
	private String categoria;
	private String unidad;
	private String imagenRuta; 
	
	public Producto(String id, String nombre, double precio, double stock, String categoria, String unidad, String imagenRuta) {
		this.id = id;
		this.nombre = nombre;
		this.precio = precio;
		this.stock = stock;
		this.stockMinimo = 5;
		this.categoria = categoria;
		this.unidad = unidad;
		this.imagenRuta = imagenRuta;
	}

	public String getId() {
		return id;
	}

	public void setId(String id) {
		this.id = id;
	}

	public String getNombre() {
		return nombre;
	}

	public void setNombre(String nombre) {
		this.nombre = nombre;
	}

	public double getPrecio() {
		return precio;
	}

	public void setPrecio(double precio) {
		this.precio = precio;
	}

	public double getStock() {
		return stock;
	}

	public void setStock(double stock) {
		this.stock = stock;
	}

	public double getStockMinimo() {
		return stockMinimo;
	}

	public void setStockMinimo(double min) {
		this.stockMinimo = min;
	}

	public String getCategoria() {
		return categoria;
	}

	public void setCategoria(String categoria) {
		this.categoria = categoria;
	}

	public String getUnidad() {
		return unidad;
	}

	public void setUnidad(String unidad) {
		this.unidad = unidad;
	}

	public String getImagenRuta() {
		return imagenRuta;
	}

	public void setImagenRuta(String imagenRuta) {
		this.imagenRuta = imagenRuta;
	}

	public boolean esPorPieza() {
		return "Piezas".equals(unidad);
	}

	public boolean stockBajo() {
		return stock <= stockMinimo;
	}

	public void reducirStock(double cantidad) {
		this.stock = Math.max(0, this.stock - cantidad);
		this.stock = Math.round(this.stock * 1000.0) / 1000.0;
	}

	public void aumentarStock(double cantidad) {
		this.stock += cantidad;
		this.stock = Math.round(this.stock * 1000.0) / 1000.0;
	}

	@Override
	public String toString() {
		return nombre + " - $" + String.format("%.2f", precio) + " [Stock: " + stock + "]";
	}
}