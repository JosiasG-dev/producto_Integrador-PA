package Vista;

import javax.swing.*;

import Controlador.UsuarioControlador;
import Modelo.Usuario;

import java.awt.*;

public class UsuarioDialog extends JDialog {

	private final UsuarioControlador ctrl;
	private final Usuario usuario;

	private JTextField txtNombre, txtUsername, txtPassword;
	private JComboBox<String> cmbRol, cmbSexo;
	private JSpinner spEdad;

	public UsuarioDialog(Window parent, Usuario u, UsuarioControlador ctrl) {
		super(parent, u == null ? "Alta Personal" : "Editar Personal", ModalityType.APPLICATION_MODAL);
		this.ctrl = ctrl;
		this.usuario = u;
		construir();
		if (u != null)
			cargar(u);
	}

	private void construir() {
		setSize(480, 520);
		setLocationRelativeTo(getParent());
		setResizable(false);

		JPanel panel = new JPanel();
		panel.setLayout(new BoxLayout(panel, BoxLayout.Y_AXIS));
		panel.setBackground(Estilos.BG_BLANCO);
		panel.setBorder(BorderFactory.createEmptyBorder(28, 32, 28, 32));

		JLabel tit = new JLabel(usuario == null ? "ALTA PERSONAL" : "EDITAR PERSONAL");
		tit.setFont(Estilos.FUENTE_TITULO);
		tit.setAlignmentX(Component.LEFT_ALIGNMENT);
		panel.add(tit);
		panel.add(Box.createVerticalStrut(20));

		panel.add(lbl("Nombre Completo"));
		txtNombre = campo();
		panel.add(txtNombre);
		panel.add(Box.createVerticalStrut(10));

		panel.add(lbl("Rol en el Sistema"));
		cmbRol = new JComboBox<>(new String[] { "CAJERO", "ADMINISTRADOR" });
		cmbRol.setFont(Estilos.FUENTE_BOLD);
		cmbRol.setMaximumSize(new Dimension(Integer.MAX_VALUE, 42));
		cmbRol.setAlignmentX(Component.LEFT_ALIGNMENT);
		panel.add(cmbRol);
		panel.add(Box.createVerticalStrut(10));

		panel.add(lbl("Usuario de Acceso (Red)"));
		txtUsername = campo();
		panel.add(txtUsername);
		panel.add(Box.createVerticalStrut(10));

		panel.add(lbl("Contraseña"));
		txtPassword = new JTextField();
		txtPassword.setFont(Estilos.FUENTE_BOLD);
		txtPassword.setBackground(Estilos.BG_ZINC_100);
		txtPassword.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(10, 12, 10, 12)));
		txtPassword.setMaximumSize(new Dimension(Integer.MAX_VALUE, 44));
		txtPassword.setAlignmentX(Component.LEFT_ALIGNMENT);
		panel.add(txtPassword);
		panel.add(Box.createVerticalStrut(10));

		JPanel row = new JPanel(new GridLayout(1, 2, 14, 0));
		row.setBackground(Estilos.BG_BLANCO);
		row.setAlignmentX(Component.LEFT_ALIGNMENT);
		row.setMaximumSize(new Dimension(Integer.MAX_VALUE, 90));

		JPanel leftRow = new JPanel();
		leftRow.setLayout(new BoxLayout(leftRow, BoxLayout.Y_AXIS));
		leftRow.setBackground(Estilos.BG_BLANCO);
		leftRow.add(lbl("Edad"));
		spEdad = new JSpinner(new SpinnerNumberModel(18, 16, 70, 1));
		spEdad.setFont(Estilos.FUENTE_BOLD);
		spEdad.setMaximumSize(new Dimension(Integer.MAX_VALUE, 42));
		leftRow.add(spEdad);

		JPanel rightRow = new JPanel();
		rightRow.setLayout(new BoxLayout(rightRow, BoxLayout.Y_AXIS));
		rightRow.setBackground(Estilos.BG_BLANCO);
		rightRow.add(lbl("Sexo"));
		cmbSexo = new JComboBox<>(new String[] { "Femenino", "Masculino", "Otro" });
		cmbSexo.setFont(Estilos.FUENTE_BOLD);
		cmbSexo.setMaximumSize(new Dimension(Integer.MAX_VALUE, 42));
		rightRow.add(cmbSexo);

		row.add(leftRow);
		row.add(rightRow);
		panel.add(row);
		panel.add(Box.createVerticalStrut(24));

		JPanel btnPanel = new JPanel(new FlowLayout(FlowLayout.RIGHT, 10, 0));
		btnPanel.setBackground(Estilos.BG_BLANCO);
		btnPanel.setAlignmentX(Component.LEFT_ALIGNMENT);
		btnPanel.setMaximumSize(new Dimension(Integer.MAX_VALUE, 52));

		if (usuario != null && usuario.getId() != ctrl.getUsuarioActivo().getId()) {
			JButton btnElim = Estilos.botonPeligro("Eliminar");
			btnElim.setPreferredSize(new Dimension(110, 42));
			btnElim.addActionListener(e -> {
				ctrl.eliminarUsuario(usuario.getId());
				dispose();
			});
			btnPanel.add(btnElim);
		}

		JButton btnCancelar = Estilos.botonSecundario("Cancelar");
		btnCancelar.setPreferredSize(new Dimension(110, 42));
		btnCancelar.addActionListener(e -> dispose());

		JButton btnGuardar = Estilos.botonPrimario("Guardar");
		btnGuardar.setPreferredSize(new Dimension(120, 42));
		btnGuardar.addActionListener(e -> guardar());

		btnPanel.add(btnCancelar);
		btnPanel.add(btnGuardar);
		panel.add(btnPanel);

		setContentPane(panel);
	}

	private void cargar(Usuario u) {
		txtNombre.setText(u.getNombre());
		cmbRol.setSelectedItem(u.getRol());
		txtUsername.setText(u.getUsername());
		txtPassword.setText(u.getPassword());
		spEdad.setValue(u.getEdad());
		cmbSexo.setSelectedItem(u.getSexo());
	}

	private void guardar() {
		String nombre = txtNombre.getText().trim();
		String username = txtUsername.getText().trim();
		String password = txtPassword.getText().trim();
		if (nombre.isBlank() || username.isBlank() || password.isBlank()) {
			JOptionPane.showMessageDialog(this, "Nombre, usuario y contraseña son obligatorios.", "Error",
					JOptionPane.ERROR_MESSAGE);
			return;
		}
		int id = usuario != null ? usuario.getId() : ctrl.generarId();
		Usuario u = new Usuario(id, username, password, (String) cmbRol.getSelectedItem(), nombre,
				(int) spEdad.getValue(), (String) cmbSexo.getSelectedItem());
		ctrl.guardarUsuario(u);
		dispose();
	}

	private JLabel lbl(String t) {
		JLabel l = new JLabel(t.toUpperCase());
		l.setFont(Estilos.FUENTE_XS);
		l.setForeground(Estilos.TEXTO_TENUE);
		l.setAlignmentX(Component.LEFT_ALIGNMENT);
		return l;
	}

	private JTextField campo() {
		JTextField tf = new JTextField();
		tf.setFont(Estilos.FUENTE_BOLD);
		tf.setBackground(Estilos.BG_ZINC_100);
		tf.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(10, 12, 10, 12)));
		tf.setMaximumSize(new Dimension(Integer.MAX_VALUE, 44));
		tf.setAlignmentX(Component.LEFT_ALIGNMENT);
		return tf;
	}
}
