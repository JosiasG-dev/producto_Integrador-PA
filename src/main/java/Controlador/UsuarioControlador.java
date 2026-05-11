package Controlador;

import java.util.List;

import Modelo.Usuario;
import Vista.UsuariosPanel;

public class UsuarioControlador {

	private final AppControlador app;
	private UsuariosPanel panel;

	public UsuarioControlador(AppControlador app) {
		this.app = app;
	}

	public void setPanel(UsuariosPanel panel) {
		this.panel = panel;
	}

	public void guardarUsuario(Usuario u) {
		boolean existe = app.getUsuarios().stream().anyMatch(x -> x.getId() == u.getId());
		if (existe) {
			app.getUsuarioDAO().actualizar(u);
		} else {
			app.getUsuarioDAO().insertar(u);
		}
		if (panel != null)
			panel.refrescar();
	}

	public void eliminarUsuario(int id) {
		if (id == app.getUsuarioActivo().getId())
			return;
		app.getUsuarioDAO().eliminar(id);
		if (panel != null)
			panel.refrescar();
	}

	public int generarId() {
		return 0;
	}

	public List<Usuario> getUsuarios() {
		return app.getUsuarios();
	}

	public Usuario getUsuarioActivo() {
		return app.getUsuarioActivo();
	}
}
