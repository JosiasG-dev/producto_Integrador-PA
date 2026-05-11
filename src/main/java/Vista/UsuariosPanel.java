package Vista;

import javax.swing.*;
import javax.swing.table.DefaultTableModel;

import Controlador.UsuarioControlador;
import Modelo.Usuario;

import java.awt.*;

public class UsuariosPanel extends JPanel {

	private final UsuarioControlador ctrl;
	private DefaultTableModel modeloTabla;

	public UsuariosPanel(UsuarioControlador ctrl) {
		this.ctrl = ctrl;
		setLayout(new BorderLayout(0, 16));
		setBackground(Estilos.BG_CLARO);
		setBorder(BorderFactory.createEmptyBorder(24, 24, 24, 24));
		construir();
	}

	private void construir() {
		JPanel header = new JPanel(new BorderLayout());
		header.setBackground(Estilos.BG_BLANCO);
		header.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE),
				BorderFactory.createEmptyBorder(20, 24, 20, 24)));
		JLabel tit = new JLabel("RRHH  —  Equipo de Operación");
		tit.setFont(Estilos.FUENTE_TITULO);

		JButton btnNuevo = Estilos.botonPrimario("+ Alta Personal");
		btnNuevo.setPreferredSize(new Dimension(150, 42));
		btnNuevo.addActionListener(e -> abrirFormulario(null));

		header.add(tit, BorderLayout.WEST);
		header.add(btnNuevo, BorderLayout.EAST);

		String[] cols = { "ID", "Nombre", "Rol", "Usuario Red", "Edad", "Sexo" };
		modeloTabla = new DefaultTableModel(cols, 0) {
			@Override
			public boolean isCellEditable(int r, int c) {
				return false;
			}
		};
		JTable tabla = new JTable(modeloTabla);
		tabla.setFont(Estilos.FUENTE_NORMAL);
		tabla.setRowHeight(44);
		tabla.setShowGrid(false);
		tabla.getTableHeader().setFont(Estilos.FUENTE_XS);
		tabla.getTableHeader().setBackground(Estilos.BG_ZINC_100);
		tabla.getColumnModel().getColumn(0).setMaxWidth(60);
		tabla.getColumnModel().getColumn(2).setMaxWidth(160);
		tabla.getColumnModel().getColumn(4).setMaxWidth(70);
		tabla.getColumnModel().getColumn(5).setMaxWidth(100);

		tabla.addMouseListener(new java.awt.event.MouseAdapter() {
			@Override
			public void mouseClicked(java.awt.event.MouseEvent e) {
				if (e.getClickCount() == 2) {
					int row = tabla.getSelectedRow();
					if (row >= 0) {
						int id = (int) modeloTabla.getValueAt(row, 0);
						Usuario u = ctrl.getUsuarios().stream().filter(x -> x.getId() == id).findFirst().orElse(null);
						if (u != null)
							abrirFormulario(u);
					}
				}
			}
		});

		JScrollPane scroll = new JScrollPane(tabla);
		scroll.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		scroll.getViewport().setBackground(Estilos.BG_BLANCO);

		add(header, BorderLayout.NORTH);
		add(scroll, BorderLayout.CENTER);

		refrescar();
	}

	public void refrescar() {
		if (modeloTabla == null)
			return;
		modeloTabla.setRowCount(0);
		for (Usuario u : ctrl.getUsuarios()) {
			modeloTabla.addRow(
					new Object[] { u.getId(), u.getNombre(), u.getRol(), u.getUsername(), u.getEdad(), u.getSexo() });
		}
	}

	private void abrirFormulario(Usuario u) {
		UsuarioDialog dlg = new UsuarioDialog(SwingUtilities.getWindowAncestor(this), u, ctrl);
		dlg.setVisible(true);
		refrescar();
	}
}
