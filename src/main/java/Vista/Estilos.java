package Vista;

import java.awt.*;

public class Estilos {

	public static final Color BG_OSCURO = new Color(24, 24, 27);
	public static final Color BG_MUY_OSCURO = new Color(9, 9, 11);
	public static final Color BG_CLARO = new Color(250, 250, 250);
	public static final Color BG_BLANCO = Color.WHITE;
	public static final Color BG_ZINC_100 = new Color(244, 244, 245);
	public static final Color BG_ZINC_200 = new Color(228, 228, 231);

	public static final Color INDIGO = new Color(79, 70, 229);
	public static final Color INDIGO_OSCURO = new Color(67, 56, 202);
	public static final Color INDIGO_CLARO = new Color(238, 242, 255);

	public static final Color EMERALD = new Color(5, 150, 105);
	public static final Color EMERALD_CLARO = new Color(236, 253, 245);

	public static final Color ROSE = new Color(225, 29, 72);
	public static final Color ROSE_CLARO = new Color(255, 241, 242);
	public static final Color ROSE_OSCURO = new Color(190, 18, 60);

	public static final Color AMBER = new Color(217, 119, 6);
	public static final Color AMBER_CLARO = new Color(255, 251, 235);

	public static final Color TEXTO_PRINCIPAL = new Color(24, 24, 27);
	public static final Color TEXTO_SECUNDARIO = new Color(113, 113, 122);
	public static final Color TEXTO_TENUE = new Color(161, 161, 170);
	public static final Color TEXTO_BLANCO = Color.WHITE;

	public static final Color BORDE = new Color(228, 228, 231);
	public static final Font FUENTE_TITULO = new Font("SansSerif", Font.BOLD, 22);
	public static final Font FUENTE_SUBTITULO = new Font("SansSerif", Font.BOLD, 14);
	public static final Font FUENTE_NORMAL = new Font("SansSerif", Font.PLAIN, 13);
	public static final Font FUENTE_BOLD = new Font("SansSerif", Font.BOLD, 13);
	public static final Font FUENTE_SMALL = new Font("SansSerif", Font.BOLD, 11);
	public static final Font FUENTE_XS = new Font("SansSerif", Font.BOLD, 10);
	public static final Font FUENTE_GRANDE = new Font("SansSerif", Font.BOLD, 28);
	public static final Font FUENTE_MONO = new Font("Monospaced", Font.BOLD, 12);

	public static final int RADIO_BORDE = 16;
	public static final int PADDING = 16;
	public static final int PADDING_GRANDE = 24;

	public static javax.swing.JButton botonPrimario(String texto) {
		javax.swing.JButton btn = new javax.swing.JButton(texto) {
			@Override
			protected void paintComponent(Graphics g) {
				Graphics2D g2 = (Graphics2D) g.create();
				g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
				g2.setColor(getBackground());
				g2.fillRoundRect(0, 0, getWidth(), getHeight(), 20, 20);
				g2.dispose();
				super.paintComponent(g);
			}
		};
		btn.setBackground(INDIGO);
		btn.setForeground(TEXTO_BLANCO);
		btn.setFont(FUENTE_XS);
		btn.setBorderPainted(false);
		btn.setContentAreaFilled(false);
		btn.setOpaque(false);
		btn.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
		return btn;
	}

	public static javax.swing.JButton botonPeligro(String texto) {
		javax.swing.JButton btn = botonPrimario(texto);
		btn.setBackground(ROSE);
		return btn;
	}

	public static javax.swing.JButton botonSecundario(String texto) {
		javax.swing.JButton btn = botonPrimario(texto);
		btn.setBackground(BG_ZINC_200);
		btn.setForeground(TEXTO_PRINCIPAL);
		return btn;
	}

	public static javax.swing.JButton botonExito(String texto) {
		javax.swing.JButton btn = botonPrimario(texto);
		btn.setBackground(EMERALD);
		return btn;
	}

	public static javax.swing.JTextField campo(String placeholder) {
		javax.swing.JTextField tf = new javax.swing.JTextField();
		tf.setFont(FUENTE_BOLD);
		tf.setBackground(BG_ZINC_100);
		tf.setBorder(
				javax.swing.BorderFactory.createCompoundBorder(javax.swing.BorderFactory.createLineBorder(BORDE, 2),
						javax.swing.BorderFactory.createEmptyBorder(10, 14, 10, 14)));
		return tf;
	}
}
