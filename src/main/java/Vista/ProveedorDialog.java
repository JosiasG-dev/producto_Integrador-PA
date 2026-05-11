package Vista;

import javax.swing.*;

import Controlador.ProveedorControlador;
import Modelo.Proveedor;

import java.awt.*;

public class ProveedorDialog extends JDialog {

	private final ProveedorControlador ctrl;
	private final Proveedor proveedor;

	private JTextField txtNombre, txtContacto, txtTelefono, txtEmail, txtDireccion;

	public ProveedorDialog(Window parent, Proveedor p, ProveedorControlador ctrl) {
		super(parent, p == null ? "Nuevo Proveedor" : "Editar Proveedor", ModalityType.APPLICATION_MODAL);
		this.ctrl = ctrl;
		this.proveedor = p;
		construir();
		if (p != null)
			cargar(p);
	}

	private void construir() {
		setSize(500, 500);
		setLocationRelativeTo(getParent());
		setResizable(false);

		JPanel panel = new JPanel();
		panel.setLayout(new BoxLayout(panel, BoxLayout.Y_AXIS));
		panel.setBackground(Estilos.BG_BLANCO);
		panel.setBorder(BorderFactory.createEmptyBorder(28, 32, 28, 32));

		JLabel tit = new JLabel(proveedor == null ? "NUEVO PROVEEDOR" : "EDITAR PROVEEDOR");
		tit.setFont(Estilos.FUENTE_TITULO);
		tit.setAlignmentX(Component.LEFT_ALIGNMENT);
		panel.add(tit);
		panel.add(Box.createVerticalStrut(20));

		panel.add(lbl("Empresa / Razón Social"));
		txtNombre = campo();
		panel.add(txtNombre);
		panel.add(Box.createVerticalStrut(10));

		panel.add(lbl("Nombre de Contacto"));
		txtContacto = campo();
		panel.add(txtContacto);
		panel.add(Box.createVerticalStrut(10));

		panel.add(lbl("Teléfono"));
		txtTelefono = campo();
		panel.add(txtTelefono);
		panel.add(Box.createVerticalStrut(10));

		panel.add(lbl("Email Corporativo"));
		txtEmail = campo();
		panel.add(txtEmail);
		panel.add(Box.createVerticalStrut(10));

		panel.add(lbl("Dirección / Matriz"));
		txtDireccion = campo();
		panel.add(txtDireccion);
		panel.add(Box.createVerticalStrut(24));

		JPanel btnPanel = new JPanel(new FlowLayout(FlowLayout.RIGHT, 10, 0));
		btnPanel.setBackground(Estilos.BG_BLANCO);
		btnPanel.setAlignmentX(Component.LEFT_ALIGNMENT);
		btnPanel.setMaximumSize(new Dimension(Integer.MAX_VALUE, 52));

		if (proveedor != null) {
			JButton btnElim = Estilos.botonPeligro("Eliminar");
			btnElim.setPreferredSize(new Dimension(110, 42));
			btnElim.addActionListener(e -> {
				ctrl.eliminarProveedor(proveedor.getId());
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

	private void cargar(Proveedor p) {
		txtNombre.setText(p.getNombre());
		txtContacto.setText(p.getContacto());
		txtTelefono.setText(p.getTelefono());
		txtEmail.setText(p.getEmail());
		txtDireccion.setText(p.getDireccion());
	}

	private void guardar() {
		String nombre = txtNombre.getText().trim().toUpperCase();
		if (nombre.isBlank()) {
			JOptionPane.showMessageDialog(this, "El nombre es obligatorio.", "Error", JOptionPane.ERROR_MESSAGE);
			return;
		}
		int id = proveedor != null ? proveedor.getId() : ctrl.generarIdProveedor();
		Proveedor p = new Proveedor(id, nombre, txtContacto.getText().trim(), txtTelefono.getText().trim(),
				txtEmail.getText().trim(), txtDireccion.getText().trim(), true);
		ctrl.guardarProveedor(p);
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
