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
	private final double efectivoRecibido;
	private JPanel panelTicket;

	public TicketDialog(JFrame parent, Venta venta, ConfiguracionTienda config, double cambio, double efectivoRecibido) {
		this(parent, venta, config, cambio, venta.getDescuento(), efectivoRecibido);
	}

	public TicketDialog(JFrame parent, Venta venta, ConfiguracionTienda config, double cambio, double descuento, double efectivoRecibido) {
		super(parent, "Ticket de Venta", true);
		this.venta = venta;
		this.config = config;
		this.cambio = cambio;
		this.efectivoRecibido = efectivoRecibido;
		this.descuento = venta.getDescuento() > 0 ? venta.getDescuento() : descuento;
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
		panelTicket.add(centrado(sdf.format(new java.util.Date()), Estilos.FUENTE_XS, y));
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

		double precioTotal = venta.getItems().stream().mapToDouble(ItemCarrito::getSubtotal).sum();
		double montoDescuento = descuento;
		double montoAPagar = venta.getTotal();

		fila(panelTicket, "Precio total de compra:", String.format("$%.2f", precioTotal), y, Estilos.TEXTO_SECUNDARIO);
		y += 18;

		Color colorDesc = montoDescuento > 0 ? Estilos.EMERALD : Estilos.TEXTO_TENUE;
		fila(panelTicket, "Monto de descuento:", String.format("-$%.2f", montoDescuento), y, colorDesc);
		y += 18;

		panelTicket.add(sep(y));
		y += 10;

		filaGrande(panelTicket, "MONTO A PAGAR:", String.format("$%.2f", montoAPagar), y, Estilos.TEXTO_PRINCIPAL);
		y += 22;

		panelTicket.add(sep(y));
		y += 10;

		fila(panelTicket, "Efectivo recibido:", String.format("$%.2f", efectivoRecibido), y, Estilos.TEXTO_SECUNDARIO);
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

	private void filaGrande(JPanel p, String label, String valor, int y, Color colorValor) {
		JLabel lbl = new JLabel(label);
		lbl.setFont(Estilos.FUENTE_BOLD);
		lbl.setForeground(Estilos.TEXTO_PRINCIPAL);
		lbl.setBounds(14, y, 180, 20);
		p.add(lbl);
		JLabel val = new JLabel(valor);
		val.setFont(new Font("Monospaced", Font.BOLD, 15));
		val.setForeground(colorValor);
		val.setHorizontalAlignment(SwingConstants.RIGHT);
		val.setBounds(180, y, 186, 20);
		p.add(val);
	}

	public void mostrar() {
		setVisible(true);
	}
}
