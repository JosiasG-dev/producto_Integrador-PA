package Modelo;

import java.util.Date;

public class Movimiento {
	public enum Tipo {
		VENTA, RETIRO, FONDO_INICIAL
	}

	private Tipo tipo;
	private String descripcion;
	private double monto;
	private Date fecha;
	private String usuario;

	public Movimiento(Tipo tipo, String descripcion, double monto, Date fecha, String usuario) {
		this.tipo = tipo;
		this.descripcion = descripcion;
		this.monto = monto;
		this.fecha = fecha;
		this.usuario = usuario;
	}

	public Tipo getTipo() {
		return tipo;
	}

	public void setTipo(Tipo tipo) {
		this.tipo = tipo;
	}

	public String getDescripcion() {
		return descripcion;
	}

	public void setDescripcion(String descripcion) {
		this.descripcion = descripcion;
	}

	public double getMonto() {
		return monto;
	}

	public void setMonto(double monto) {
		this.monto = monto;
	}

	public Date getFecha() {
		return fecha;
	}

	public void setFecha(Date fecha) {
		this.fecha = fecha;
	}

	public String getUsuario() {
		return usuario;
	}

	public void setUsuario(String usuario) {
		this.usuario = usuario;
	}

	public boolean esIngreso() {
		return tipo == Tipo.VENTA || tipo == Tipo.FONDO_INICIAL;
	}
}
