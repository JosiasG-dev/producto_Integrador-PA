package Modelo;

public class ConfiguracionTienda {
	private String nombre;
	private String sucursal;
	private String rfc;

	public ConfiguracionTienda(String nombre, String sucursal, String rfc) {
		this.nombre = nombre;
		this.sucursal = sucursal;
		this.rfc = rfc;
	}

	public String getNombre() {
		return nombre;
	}

	public void setNombre(String nombre) {
		this.nombre = nombre;
	}

	public String getSucursal() {
		return sucursal;
	}

	public void setSucursal(String sucursal) {
		this.sucursal = sucursal;
	}

	public String getRfc() {
		return rfc;
	}

	public void setRfc(String rfc) {
		this.rfc = rfc;
	}
}
