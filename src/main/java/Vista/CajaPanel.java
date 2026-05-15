package Vista;

import javax.swing.*;
import javax.swing.table.DefaultTableModel;

import Controlador.CajaControlador;
import Modelo.*;

import java.awt.*;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class CajaPanel extends JPanel {

	private final CajaControlador ctrl;
	private final Usuario usuario;
	private final VentanaPrincipal ventana;

	private JPanel contenido;
	private CardLayout cardLayout;

	private static final String CARD_CERRADA = "cerrada";
	private static final String CARD_ABIERTA = "abierta";

	private JLabel lblEfectivo;
	private JLabel lblEfectivoTotal;
	private DefaultTableModel modeloMovimientos;

	public CajaPanel(CajaControlador ctrl, Usuario usuario, VentanaPrincipal ventana) {
		this.ctrl = ctrl;
		this.usuario = usuario;
		this.ventana = ventana;
		setLayout(new BorderLayout());
		setBackground(Estilos.BG_CLARO);
		setBorder(BorderFactory.createEmptyBorder(24, 24, 24, 24));
		construir();
	}

	private void construir() {
		cardLayout = new CardLayout();
		contenido = new JPanel(cardLayout);
		contenido.setBackground(Estilos.BG_CLARO);

		contenido.add(construirPanelCerrada(), CARD_CERRADA);
		contenido.add(construirPanelAbierta(), CARD_ABIERTA);

		add(contenido, BorderLayout.CENTER);
		refrescar();
	}

	private JPanel construirPanelCerrada() {
		JPanel p = new JPanel(new GridBagLayout());
		p.setBackground(Estilos.BG_CLARO);

		JPanel tarjeta = new JPanel();
		tarjeta.setLayout(new BoxLayout(tarjeta, BoxLayout.Y_AXIS));
		tarjeta.setBackground(Estilos.BG_BLANCO);
		tarjeta.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE),
				BorderFactory.createEmptyBorder(40, 44, 40, 44)));
		tarjeta.setPreferredSize(new Dimension(420, 440));

		JLabel icono = new JLabel("💵", SwingConstants.CENTER);
		icono.setFont(new Font("SansSerif", Font.PLAIN, 64));
		icono.setAlignmentX(Component.CENTER_ALIGNMENT);
		tarjeta.add(icono);
		tarjeta.add(Box.createVerticalStrut(16));

		JLabel tit = new JLabel("Terminal Cerrada", SwingConstants.CENTER);
		tit.setFont(Estilos.FUENTE_TITULO);
		tit.setAlignmentX(Component.CENTER_ALIGNMENT);
		tarjeta.add(tit);

		JLabel resp = new JLabel("Responsable: " + usuario.getNombre(), SwingConstants.CENTER);
		resp.setFont(Estilos.FUENTE_SMALL);
		resp.setForeground(Estilos.TEXTO_SECUNDARIO);
		resp.setAlignmentX(Component.CENTER_ALIGNMENT);
		tarjeta.add(resp);
		tarjeta.add(Box.createVerticalStrut(28));

		JLabel lblFondoTexto = new JLabel("FONDO INICIAL ($)");
		lblFondoTexto.setFont(Estilos.FUENTE_XS);
		lblFondoTexto.setForeground(Estilos.TEXTO_TENUE);
		lblFondoTexto.setAlignmentX(Component.LEFT_ALIGNMENT);
		tarjeta.add(lblFondoTexto);
		tarjeta.add(Box.createVerticalStrut(6));

		JTextField txtFondo = new JTextField("0.00");
		txtFondo.setFont(new Font("SansSerif", Font.BOLD, 26));
		txtFondo.setBackground(Estilos.BG_ZINC_100);
		txtFondo.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(12, 16, 12, 16)));
		txtFondo.setMaximumSize(new Dimension(Integer.MAX_VALUE, 56));
		txtFondo.setAlignmentX(Component.LEFT_ALIGNMENT);
		tarjeta.add(txtFondo);
		tarjeta.add(Box.createVerticalStrut(24));

		JButton btnAbrir = Estilos.botonPrimario("ABRIR TERMINAL");
		btnAbrir.setFont(new Font("SansSerif", Font.BOLD, 16));
		btnAbrir.setPreferredSize(new Dimension(340, 56));
		btnAbrir.setMaximumSize(new Dimension(Integer.MAX_VALUE, 56));
		btnAbrir.setAlignmentX(Component.CENTER_ALIGNMENT);
		btnAbrir.addActionListener(e -> {
			try {
				double fondo = Double.parseDouble(txtFondo.getText().trim());
				ctrl.abrirCaja(fondo);
			} catch (Exception ex) {
				ctrl.abrirCaja(0);
			}
		});
		tarjeta.add(btnAbrir);

		p.add(tarjeta);
		return p;
	}

	private JPanel construirPanelAbierta() {
		JPanel p = new JPanel(new BorderLayout(0, 16));
		p.setBackground(Estilos.BG_CLARO);

		JPanel header = new JPanel(new BorderLayout());
		header.setBackground(Estilos.BG_BLANCO);
		header.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE),
				BorderFactory.createEmptyBorder(20, 24, 20, 24)));

		JLabel tit = new JLabel("GESTIÓN DE CAJA  —  Operativa");
		tit.setFont(Estilos.FUENTE_TITULO);

		JButton btnRetiro = Estilos.botonPeligro("↑ Retiro / Pago");
		btnRetiro.setPreferredSize(new Dimension(160, 42));
		btnRetiro.addActionListener(e -> abrirDialogoRetiro());

		header.add(tit, BorderLayout.WEST);
		header.add(btnRetiro, BorderLayout.EAST);

		JPanel tarjetas = new JPanel(new GridLayout(1, 2, 16, 0));
		tarjetas.setBackground(Estilos.BG_CLARO);
		tarjetas.setPreferredSize(new Dimension(0, 110));

		JPanel cardFondo = tarjetaCaja("Total Ventas", "$0.00", Estilos.BG_BLANCO, Estilos.TEXTO_PRINCIPAL);
		lblEfectivo = (JLabel) cardFondo.getClientProperty("valor");

		JPanel cardEfectivo = tarjetaCaja("Efectivo en Caja", "$0.00", Estilos.INDIGO, Color.WHITE);
		lblEfectivoTotal = (JLabel) cardEfectivo.getClientProperty("valor");

		tarjetas.add(cardFondo);
		tarjetas.add(cardEfectivo);

		String[] cols = { "Hora", "Tipo", "Concepto", "Monto" };
		modeloMovimientos = new DefaultTableModel(cols, 0) {
			@Override
			public boolean isCellEditable(int r, int c) {
				return false;
			}
		};
		JTable tablaMovs = new JTable(modeloMovimientos);
		tablaMovs.setFont(Estilos.FUENTE_NORMAL);
		tablaMovs.setRowHeight(40);
		tablaMovs.setShowGrid(false);
		tablaMovs.getTableHeader().setFont(Estilos.FUENTE_XS);
		tablaMovs.getTableHeader().setBackground(Estilos.BG_ZINC_100);
		tablaMovs.getColumnModel().getColumn(0).setMaxWidth(100);
		tablaMovs.getColumnModel().getColumn(1).setMaxWidth(110);
		tablaMovs.getColumnModel().getColumn(3).setMaxWidth(120);

		JScrollPane scroll = new JScrollPane(tablaMovs);
		scroll.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		scroll.getViewport().setBackground(Estilos.BG_BLANCO);

		JPanel centro = new JPanel(new BorderLayout(0, 16));
		centro.setBackground(Estilos.BG_CLARO);
		centro.add(tarjetas, BorderLayout.NORTH);
		centro.add(scroll, BorderLayout.CENTER);

		p.add(header, BorderLayout.NORTH);
		p.add(centro, BorderLayout.CENTER);

		return p;
	}

	private JPanel tarjetaCaja(String label, String valor, Color bg, Color fg) {
		JPanel card = new JPanel();
		card.setLayout(new BoxLayout(card, BoxLayout.Y_AXIS));
		card.setBackground(bg);
		card.setBorder(BorderFactory.createCompoundBorder(
				BorderFactory.createLineBorder(bg == Estilos.BG_BLANCO ? Estilos.BORDE : bg),
				BorderFactory.createEmptyBorder(18, 22, 18, 22)));

		JLabel lLbl = new JLabel(label.toUpperCase());
		lLbl.setFont(Estilos.FUENTE_XS);
		lLbl.setForeground(bg == Estilos.INDIGO ? new Color(199, 210, 254) : Estilos.TEXTO_TENUE);
		lLbl.setAlignmentX(Component.LEFT_ALIGNMENT);

		JLabel lVal = new JLabel(valor);
		lVal.setFont(new Font("SansSerif", Font.BOLD, 34));
		lVal.setForeground(fg);
		lVal.setAlignmentX(Component.LEFT_ALIGNMENT);

		card.add(lLbl);
		card.add(Box.createVerticalStrut(6));
		card.add(lVal);
		card.putClientProperty("valor", lVal);
		return card;
	}

	private void abrirDialogoRetiro() {
		RetiroDialog dlg = new RetiroDialog(ventana.getFrame(), ctrl);
		dlg.setVisible(true);
		refrescar();
	}

	public void refrescar() {
		if (ctrl.isCajaAbierta()) {
			cardLayout.show(contenido, CARD_ABIERTA);

			double totalGeneral = ctrl.getMovimientos().stream().filter(m -> m.getTipo() == Movimiento.Tipo.VENTA)
					.mapToDouble(Movimiento::getMonto).sum();

			if (lblEfectivo != null) {
				lblEfectivo.setText(String.format("$%.2f", totalGeneral));
			}

			if (lblEfectivoTotal != null) {
				lblEfectivoTotal.setText(String.format("$%.2f", ctrl.getEfectivoEsperado()));
			}

			actualizarTablaMovimientos();
		} else {
			cardLayout.show(contenido, CARD_CERRADA);
		}
	}

	private void actualizarTablaMovimientos() {
		if (modeloMovimientos == null)
			return;
		List<Movimiento> movs = new ArrayList<>(ctrl.getMovimientos());
		Collections.reverse(movs);
		modeloMovimientos.setRowCount(0);
		SimpleDateFormat sdf = new SimpleDateFormat("HH:mm:ss");
		for (Movimiento m : movs) {
			modeloMovimientos.addRow(new Object[] { sdf.format(m.getFecha()), m.getTipo().name(), m.getDescripcion(),
					String.format(m.esIngreso() ? "+$%.2f" : "-$%.2f", m.getMonto()) });
		}
	}
}