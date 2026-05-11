package Vista;

import javax.swing.*;

import Controlador.AppControlador;
import Modelo.ConfiguracionTienda;

import java.awt.*;

public class ConfigPanel extends JPanel {

	private final AppControlador app;
	private JTextField txtNombre, txtSucursal, txtRfc;

	public ConfigPanel(AppControlador app) {
		this.app = app;
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
		JLabel tit = new JLabel("⚙️  SISTEMA  —  Identidad Fiscal");
		tit.setFont(Estilos.FUENTE_TITULO);
		header.add(tit, BorderLayout.WEST);

		JPanel form = new JPanel();
		form.setLayout(new BoxLayout(form, BoxLayout.Y_AXIS));
		form.setBackground(Estilos.BG_BLANCO);
		form.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE),
				BorderFactory.createEmptyBorder(32, 36, 32, 36)));

		ConfiguracionTienda cfg = app.getConfig();

		form.add(lbl("Razón Social / Nombre de la Tienda"));
		txtNombre = campo(cfg.getNombre(), 36);
		form.add(txtNombre);
		form.add(Box.createVerticalStrut(16));

		form.add(lbl("Sucursal"));
		txtSucursal = campo(cfg.getSucursal(), 18);
		form.add(txtSucursal);
		form.add(Box.createVerticalStrut(16));

		form.add(lbl("RFC"));
		txtRfc = campo(cfg.getRfc(), 16);
		form.add(txtRfc);
		form.add(Box.createVerticalStrut(32));

		JButton btnGuardar = Estilos.botonPrimario("✓  ACTUALIZAR CONFIGURACIÓN");
		btnGuardar.setFont(new Font("SansSerif", Font.BOLD, 16));
		btnGuardar.setMaximumSize(new Dimension(Integer.MAX_VALUE, 60));
		btnGuardar.setAlignmentX(Component.LEFT_ALIGNMENT);
		btnGuardar.addActionListener(e -> guardar());
		form.add(btnGuardar);

		JPanel wrap = new JPanel(new BorderLayout());
		wrap.setBackground(Estilos.BG_CLARO);
		wrap.setMaximumSize(new Dimension(640, Integer.MAX_VALUE));
		wrap.add(form, BorderLayout.CENTER);

		add(header, BorderLayout.NORTH);
		add(form, BorderLayout.CENTER);
	}

	private void guardar() {
		ConfiguracionTienda cfg = app.getConfig();
		cfg.setNombre(txtNombre.getText().trim());
		cfg.setSucursal(txtSucursal.getText().trim());
		cfg.setRfc(txtRfc.getText().trim().toUpperCase());
		JOptionPane.showMessageDialog(this, "Configuración actualizada correctamente.", "Sistema",
				JOptionPane.INFORMATION_MESSAGE);
	}

	private JLabel lbl(String t) {
		JLabel l = new JLabel(t.toUpperCase());
		l.setFont(Estilos.FUENTE_XS);
		l.setForeground(Estilos.TEXTO_TENUE);
		l.setAlignmentX(Component.LEFT_ALIGNMENT);
		return l;
	}

	private JTextField campo(String valor, int cols) {
		JTextField tf = new JTextField(valor, cols);
		tf.setFont(new Font("SansSerif", Font.BOLD, 20));
		tf.setBackground(Estilos.BG_ZINC_100);
		tf.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(12, 16, 12, 16)));
		tf.setMaximumSize(new Dimension(Integer.MAX_VALUE, 56));
		tf.setAlignmentX(Component.LEFT_ALIGNMENT);
		return tf;
	}
}
