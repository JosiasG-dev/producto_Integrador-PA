package Vista;

import javax.swing.*;
import Modelo.*;
import java.awt.*;
import java.awt.print.*;
import java.text.SimpleDateFormat;

public class TicketDialog extends JDialog {

	private final Venta venta;
	private final ConfiguracionTienda config;
	private final double cambio;
	private final double descuento;
	private JPanel panelTicket;

	public TicketDialog(JFrame parent, Venta venta, ConfiguracionTienda config, double cambio) {
		this(parent, venta, config, cambio, 0);
	}

	public TicketDialog(JFrame parent, Venta venta, ConfiguracionTienda config, double cambio, double descuento) {
		super(parent, "Ticket de Venta", true);
		this.venta = venta;
		this.config = config;
		this.cambio = cambio;
		this.descuento = descuento;
		construir();
	}

	private void construir() {
		setSize(430, 700);
		setLocationRelativeTo(getParent());
		setResizable(false);

		JPanel contenedor = new JPanel(null);
		contenedor.setBackground(Estilos.BG_BLANCO);

		panelTicket = new JPanel(null);
		panelTicket.setBackground(Estilos.BG_BLANCO);
		contenedor.add(panelTicket);

		int y = 0;

		JLabel barra = new JLabel();
		barra.setBackground(Estilos.INDIGO);
		barra.setOpaque(true);
		barra.setBounds(0, 0, 380, 8);
		panelTicket.add(barra);
		y = 18;

		panelTicket.add(centrado(config.getNombre(), new Font("Monospaced", Font.BOLD, 15), y));
		y += 22;
		JLabel lblSuc = centrado(config.getSucursal(), Estilos.FUENTE_XS, y);
		lblSuc.setForeground(Estilos.TEXTO_SECUNDARIO);
		panelTicket.add(lblSuc);
		y += 16;
		JLabel lblRfc = centrado("RFC: " + config.getRfc(), Estilos.FUENTE_XS, y);
		lblRfc.setForeground(Estilos.TEXTO_SECUNDARIO);
		panelTicket.add(lblRfc);
		y += 16;

		panelTicket.add(sep(y));
		y += 14;

		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss");
		panelTicket.add(centrado(sdf.format(venta.getFecha()), Estilos.FUENTE_XS, y));
		y += 16;
		panelTicket.add(centrado("Cajero: " + venta.getCajero().toUpperCase(), Estilos.FUENTE_XS, y));
		y += 16;
		JLabel lblNum = centrado("Ticket #" + venta.getId(), Estilos.FUENTE_XS, y);
		lblNum.setForeground(Estilos.TEXTO_TENUE);
		panelTicket.add(lblNum);
		y += 16;

		panelTicket.add(sep(y));
		y += 14;

		for (ItemCarrito item : venta.getItems()) {
			JLabel cod = new JLabel(
					"[" + item.getProducto().getId() + "] " + item.getProducto().getNombre().toUpperCase());
			cod.setFont(Estilos.FUENTE_XS);
			cod.setBounds(14, y, 240, 16);
			panelTicket.add(cod);
			JLabel sub = new JLabel(String.format("$%.2f", item.getSubtotal()));
			sub.setFont(Estilos.FUENTE_XS);
			sub.setHorizontalAlignment(SwingConstants.RIGHT);
			sub.setBounds(256, y, 110, 16);
			panelTicket.add(sub);
			y += 16;
			JLabel det = new JLabel(
					String.format("  %.2f x $%.2f", item.getCantidad(), item.getProducto().getPrecio()));
			det.setFont(Estilos.FUENTE_XS);
			det.setForeground(Estilos.TEXTO_TENUE);
			det.setBounds(14, y, 240, 14);
			panelTicket.add(det);
			y += 18;
		}

		panelTicket.add(sep(y));
		y += 10;

		double subtotal = venta.getItems().stream().mapToDouble(ItemCarrito::getSubtotal).sum();
		double aPagar = venta.getTotal();

		fila(panelTicket, "Subtotal:", String.format("$%.2f", subtotal), y, Estilos.TEXTO_SECUNDARIO);
		y += 18;
		if (descuento > 0) {
			fila(panelTicket, "Descuento:", String.format("-$%.2f", descuento), y, Estilos.EMERALD);
			y += 18;
		}
		fila(panelTicket, "TOTAL A PAGAR:", String.format("$%.2f", aPagar), y, Estilos.TEXTO_PRINCIPAL);
		y += 18;
		fila(panelTicket, "Recibido:", String.format("$%.2f", aPagar + cambio), y, Estilos.TEXTO_SECUNDARIO);
		y += 18;
		fila(panelTicket, "Cambio:", String.format("$%.2f", cambio), y, Estilos.EMERALD);
		y += 20;

		JLabel gracias = centrado("Gracias por su compra", Estilos.FUENTE_SMALL, y);
		gracias.setForeground(Estilos.TEXTO_TENUE);
		panelTicket.add(gracias);
		y += 30;

		panelTicket.setBounds(0, 0, 380, y);

		JButton btnImprimir = new JButton("Imprimir / Cerrar");
		btnImprimir.setFont(Estilos.FUENTE_BOLD);
		btnImprimir.setBackground(Estilos.BG_OSCURO);
		btnImprimir.setForeground(Color.WHITE);
		btnImprimir.setBorderPainted(false);
		btnImprimir.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
		btnImprimir.setBounds(14, y + 4, 352, 44);
		btnImprimir.addActionListener(e -> {
			imprimir();
			dispose();
		});
		contenedor.add(btnImprimir);

		contenedor.setPreferredSize(new Dimension(380, y + 56));
		JScrollPane scroll = new JScrollPane(contenedor);
		scroll.setBorder(null);
		setContentPane(scroll);
	}

	private void imprimir() {
		PrinterJob job = PrinterJob.getPrinterJob();
		job.setPrintable((graphics, pageFormat, pageIndex) -> {
			if (pageIndex > 0)
				return Printable.NO_SUCH_PAGE;
			panelTicket.print(graphics);
			return Printable.PAGE_EXISTS;
		});
		if (job.printDialog()) {
			try {
				job.print();
			} catch (PrinterException ex) {
				ex.printStackTrace();
			}
		}
	}

	private JLabel centrado(String texto, Font fuente, int y) {
		JLabel lbl = new JLabel(texto, SwingConstants.CENTER);
		lbl.setFont(fuente);
		lbl.setBounds(0, y, 380, 18);
		return lbl;
	}

	private JSeparator sep(int y) {
		JSeparator s = new JSeparator();
		s.setForeground(Estilos.BORDE);
		s.setBounds(14, y, 352, 6);
		return s;
	}

	private void fila(JPanel p, String label, String valor, int y, Color colorValor) {
		JLabel lbl = new JLabel(label);
		lbl.setFont(Estilos.FUENTE_XS);
		lbl.setForeground(Estilos.TEXTO_SECUNDARIO);
		lbl.setBounds(14, y, 180, 16);
		p.add(lbl);
		JLabel val = new JLabel(valor);
		val.setFont(Estilos.FUENTE_XS);
		val.setForeground(colorValor);
		val.setHorizontalAlignment(SwingConstants.RIGHT);
		val.setBounds(200, y, 166, 16);
		p.add(val);
	}

	public void mostrar() {
		setVisible(true);
	}
}
