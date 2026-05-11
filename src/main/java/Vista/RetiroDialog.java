package Vista;

import javax.swing.*;

import Controlador.CajaControlador;

import java.awt.*;

public class RetiroDialog extends JDialog {

	private final CajaControlador ctrl;
	private JTextField txtConcepto;
	private JTextField txtMonto;

	public RetiroDialog(JFrame parent, CajaControlador ctrl) {
		super(parent, "Salida de Efectivo", true);
		this.ctrl = ctrl;
		construir();
	}

	private void construir() {
		setSize(440, 340);
		setLocationRelativeTo(getParent());
		setResizable(false);

		JPanel panel = new JPanel();
		panel.setLayout(new BoxLayout(panel, BoxLayout.Y_AXIS));
		panel.setBackground(Estilos.BG_BLANCO);
		panel.setBorder(BorderFactory.createEmptyBorder(32, 36, 32, 36));

		JLabel tit = new JLabel("SALIDA DE EFECTIVO");
		tit.setFont(Estilos.FUENTE_TITULO);
		tit.setForeground(Estilos.TEXTO_PRINCIPAL);
		tit.setAlignmentX(Component.LEFT_ALIGNMENT);
		panel.add(tit);

		JSeparator sep = new JSeparator();
		sep.setMaximumSize(new Dimension(Integer.MAX_VALUE, 14));
		sep.setForeground(Estilos.BORDE);
		panel.add(sep);
		panel.add(Box.createVerticalStrut(16));

		panel.add(lbl("Concepto"));
		txtConcepto = campo("Ej. Pago Proveedor...");
		panel.add(txtConcepto);
		panel.add(Box.createVerticalStrut(14));

		panel.add(lbl("Monto ($)"));
		txtMonto = new JTextField("0.00");
		txtMonto.setFont(new Font("SansSerif", Font.BOLD, 26));
		txtMonto.setBackground(Estilos.BG_ZINC_100);
		txtMonto.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(10, 14, 10, 14)));
		txtMonto.setMaximumSize(new Dimension(Integer.MAX_VALUE, 54));
		txtMonto.setAlignmentX(Component.LEFT_ALIGNMENT);
		panel.add(txtMonto);
		panel.add(Box.createVerticalStrut(24));

		JPanel btnPanel = new JPanel(new FlowLayout(FlowLayout.RIGHT, 10, 0));
		btnPanel.setBackground(Estilos.BG_BLANCO);
		btnPanel.setAlignmentX(Component.LEFT_ALIGNMENT);
		btnPanel.setMaximumSize(new Dimension(Integer.MAX_VALUE, 52));

		JButton btnCancelar = Estilos.botonSecundario("Cancelar");
		btnCancelar.setPreferredSize(new Dimension(120, 44));
		btnCancelar.addActionListener(e -> dispose());

		JButton btnAutorizar = Estilos.botonPeligro("AUTORIZAR");
		btnAutorizar.setPreferredSize(new Dimension(140, 44));
		btnAutorizar.addActionListener(e -> autorizar());

		btnPanel.add(btnCancelar);
		btnPanel.add(btnAutorizar);
		panel.add(btnPanel);

		setContentPane(panel);
	}

	private void autorizar() {
		String concepto = txtConcepto.getText().trim();
		if (concepto.isBlank()) {
			JOptionPane.showMessageDialog(this, "Ingrese un concepto.", "Aviso", JOptionPane.WARNING_MESSAGE);
			return;
		}
		try {
			double monto = Double.parseDouble(txtMonto.getText().trim());
			if (monto <= 0)
				throw new NumberFormatException();
			ctrl.registrarRetiro(concepto, monto);
			dispose();
		} catch (NumberFormatException ex) {
			JOptionPane.showMessageDialog(this, "Ingrese un monto válido mayor a 0.", "Error",
					JOptionPane.ERROR_MESSAGE);
		}
	}

	private JLabel lbl(String texto) {
		JLabel l = new JLabel(texto.toUpperCase());
		l.setFont(Estilos.FUENTE_XS);
		l.setForeground(Estilos.TEXTO_TENUE);
		l.setAlignmentX(Component.LEFT_ALIGNMENT);
		return l;
	}

	private JTextField campo(String placeholder) {
		JTextField tf = new JTextField();
		tf.setFont(Estilos.FUENTE_BOLD);
		tf.setBackground(Estilos.BG_ZINC_100);
		tf.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(10, 14, 10, 14)));
		tf.setMaximumSize(new Dimension(Integer.MAX_VALUE, 44));
		tf.setAlignmentX(Component.LEFT_ALIGNMENT);
		return tf;
	}
}
