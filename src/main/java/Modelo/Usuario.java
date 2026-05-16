package Modelo;

public class Usuario {
	private int id;
	private String username;
	private String password;
	private String rol;
	private String nombre;
	private int edad;
	private String sexo;

	public Usuario(int id, String username, String password, String rol, String nombre, int edad, String sexo) {
		this.id = id;
		this.username = username;
		this.password = password;
		this.rol = rol;
		this.nombre = nombre;
		this.edad = edad;
		this.sexo = sexo;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getUsername() {
		return username;
	}

	public void setUsername(String username) {
		this.username = username;
	}

	public String getPassword() {
		return password;
	}

	public void setPassword(String password) {
		this.password = password;
	}

	public String getRol() {
		return rol;
	}

	public void setRol(String rol) {
		this.rol = rol;
	}

	public String getNombre() {
		return nombre;
	}

	public void setNombre(String nombre) {
		this.nombre = nombre;
	}

	public int getEdad() {
		return edad;
	}

	public void setEdad(int edad) {
		this.edad = edad;
	}

	public String getSexo() {
		return sexo;
	}

	public void setSexo(String sexo) {
		this.sexo = sexo;
	}

	public boolean esAdmin() {
		return "ADMINISTRADOR".equals(this.rol);
	}

	@Override
	public String toString() {
		return nombre + " [" + rol + "]";
	}
}
