package Vista;

import javax.swing.*;
import javax.swing.table.*;
import Controlador.AppControlador;
import Modelo.*;
import java.awt.*;
import java.io.*;
import java.text.SimpleDateFormat;
import java.util.*;
import java.util.List;
import java.util.stream.Collectors;

public class ReportesPanel extends JPanel {

	private final AppControlador app;
	private JLabel lblTotal, lblNumVentas, lblPromedio;
	private DefaultTableModel modeloVentas;
	private JTable tablaVentas;
	private JTextField txtFechaInicio, txtFechaFin;
	private List<Venta> ventasFiltradas = new ArrayList<>();

	public ReportesPanel(AppControlador app) {
		this.app = app;
		setLayout(null);
		setBackground(Estilos.BG_CLARO);
		construir();
	}

	private void construir() {
		JLabel tit = new JLabel("REPORTES DE VENTAS");
		tit.setFont(Estilos.FUENTE_TITULO);
		tit.setBounds(24, 14, 400, 30);
		add(tit);

		JLabel lblDesde = new JLabel("DESDE (dd/MM/yyyy):");
		lblDesde.setFont(Estilos.FUENTE_XS);
		lblDesde.setForeground(Estilos.TEXTO_TENUE);
		lblDesde.setBounds(24, 56, 160, 14);
		add(lblDesde);

		txtFechaInicio = campo(24, 74, 155, 34);
		txtFechaInicio.setText("01/01/2024");
		add(txtFechaInicio);

		JLabel lblHasta = new JLabel("HASTA (dd/MM/yyyy):");
		lblHasta.setFont(Estilos.FUENTE_XS);
		lblHasta.setForeground(Estilos.TEXTO_TENUE);
		lblHasta.setBounds(190, 56, 160, 14);
		add(lblHasta);

		txtFechaFin = campo(190, 74, 155, 34);
		txtFechaFin.setText(new SimpleDateFormat("dd/MM/yyyy").format(new Date()));
		add(txtFechaFin);

		JButton btnFiltrar = Estilos.botonPrimario("Filtrar");
		btnFiltrar.setBounds(356, 74, 90, 34);
		btnFiltrar.addActionListener(e -> refrescar());
		add(btnFiltrar);

		JButton btnExcel = Estilos.botonSecundario("Exportar CSV");
		btnExcel.setBounds(456, 74, 120, 34);
		btnExcel.addActionListener(e -> exportarCSV());
		add(btnExcel);

		JButton btnDevolucion = Estilos.botonPeligro("Devolucion");
		btnDevolucion.setBounds(586, 74, 110, 34);
		btnDevolucion.addActionListener(e -> {
			DevolucionDialog dlg = new DevolucionDialog((JFrame) SwingUtilities.getWindowAncestor(this), app);
			dlg.setVisible(true);
		});
		add(btnDevolucion);

		JButton btnVerDevoluciones = Estilos.botonSecundario("Ver Devoluciones");
		btnVerDevoluciones.setBounds(706, 74, 148, 34);
		btnVerDevoluciones.addActionListener(e -> mostrarDevoluciones());
		add(btnVerDevoluciones);

		JButton btnReimprimir = Estilos.botonSecundario("Reimprimir Ticket");
		btnReimprimir.setBounds(864, 74, 148, 34);
		btnReimprimir.addActionListener(e -> reimprimirSeleccionado());
		add(btnReimprimir);

		lblTotal = kpiCard("Ingresos", "$0.00", Estilos.INDIGO, Color.WHITE, 24, 124);
		lblNumVentas = kpiCard("Ventas", "0", Estilos.BG_BLANCO, Estilos.TEXTO_PRINCIPAL, 204, 124);
		lblPromedio = kpiCard("Ticket Prom", "$0.00", Estilos.BG_BLANCO, Estilos.TEXTO_PRINCIPAL, 384, 124);

		String[] cols = { "#", "Fecha Venta", "Hora", "Cajero", "Metodo", "Total" };
		modeloVentas = new DefaultTableModel(cols, 0) {
			@Override
			public boolean isCellEditable(int r, int c) {
				return false;
			}
		};
		tablaVentas = new JTable(modeloVentas);
		tablaVentas.setFont(Estilos.FUENTE_NORMAL);
		tablaVentas.setRowHeight(38);
		tablaVentas.setShowGrid(false);
		tablaVentas.setIntercellSpacing(new Dimension(0, 2));
		tablaVentas.getTableHeader().setFont(Estilos.FUENTE_XS);
		tablaVentas.getTableHeader().setBackground(Estilos.BG_ZINC_100);
		tablaVentas.getColumnModel().getColumn(0).setMaxWidth(50);
		tablaVentas.getColumnModel().getColumn(2).setMaxWidth(80);
		tablaVentas.getColumnModel().getColumn(4).setMaxWidth(90);
		tablaVentas.getColumnModel().getColumn(5).setMaxWidth(100);

		JScrollPane scroll = new JScrollPane(tablaVentas);
		scroll.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		scroll.getViewport().setBackground(Estilos.BG_BLANCO);
		scroll.setBounds(24, 242, 1000, 440);
		add(scroll);

		refrescar();
	}

	private JLabel kpiCard(String label, String valor, Color bg, Color fg, int x, int y) {
		JPanel card = new JPanel(null);
		card.setBackground(bg);
		card.setBorder(BorderFactory.createLineBorder(bg == Estilos.BG_BLANCO ? Estilos.BORDE : bg));
		card.setBounds(x, y, 168, 90);
		add(card);

		JLabel lLbl = new JLabel(label.toUpperCase());
		lLbl.setFont(Estilos.FUENTE_XS);
		lLbl.setForeground(bg == Estilos.INDIGO ? new Color(199, 210, 254) : Estilos.TEXTO_TENUE);
		lLbl.setBounds(12, 12, 144, 14);
		card.add(lLbl);

		JLabel lVal = new JLabel(valor);
		lVal.setFont(new Font("SansSerif", Font.BOLD, 28));
		lVal.setForeground(fg);
		lVal.setBounds(12, 32, 144, 40);
		card.add(lVal);
		return lVal;
	}

	public void refrescar() {
		ventasFiltradas = filtrarPorFecha(app.getVentas());
		double total = ventasFiltradas.stream().mapToDouble(Venta::getTotal).sum();
		double prom = ventasFiltradas.isEmpty() ? 0 : total / ventasFiltradas.size();
		lblTotal.setText(String.format("$%.2f", total));
		lblNumVentas.setText(String.valueOf(ventasFiltradas.size()));
		lblPromedio.setText(String.format("$%.2f", prom));

		modeloVentas.setRowCount(0);
		SimpleDateFormat sdfF = new SimpleDateFormat("dd/MM/yyyy");
		SimpleDateFormat sdfH = new SimpleDateFormat("HH:mm:ss");
		for (Venta v : ventasFiltradas) {
			modeloVentas.addRow(new Object[] { v.getId(), sdfF.format(v.getFecha()), sdfH.format(v.getFecha()),
					v.getCajero(), v.getMetodoPago(), String.format("$%.2f", v.getTotal()) });
		}
	}

	private void mostrarDevoluciones() {
		List<Devolucion> devoluciones = app.getDevoluciones();

		JDialog dlg = new JDialog((JFrame) SwingUtilities.getWindowAncestor(this), "Historial de Devoluciones", true);
		dlg.setSize(780, 480);
		dlg.setLocationRelativeTo(this);
		dlg.setLayout(null);

		JLabel tit = new JLabel("HISTORIAL DE DEVOLUCIONES");
		tit.setFont(Estilos.FUENTE_TITULO);
		tit.setBounds(20, 14, 500, 30);
		dlg.add(tit);

		String[] cols = { "#", "Venta #", "Producto", "Cantidad", "Monto Dev", "Motivo", "Fecha", "Cajero" };
		DefaultTableModel modelo = new DefaultTableModel(cols, 0) {
			@Override
			public boolean isCellEditable(int r, int c) {
				return false;
			}
		};
		JTable tabla = new JTable(modelo);
		tabla.setFont(Estilos.FUENTE_NORMAL);
		tabla.setRowHeight(36);
		tabla.setShowGrid(false);
		tabla.setIntercellSpacing(new Dimension(0, 2));
		tabla.getTableHeader().setFont(Estilos.FUENTE_XS);
		tabla.getTableHeader().setBackground(Estilos.BG_ZINC_100);
		tabla.getColumnModel().getColumn(0).setMaxWidth(40);
		tabla.getColumnModel().getColumn(1).setMaxWidth(60);
		tabla.getColumnModel().getColumn(3).setMaxWidth(70);
		tabla.getColumnModel().getColumn(4).setMaxWidth(80);
		tabla.getColumnModel().getColumn(6).setMaxWidth(110);

		SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy HH:mm");
		for (Devolucion d : devoluciones) {
			modelo.addRow(new Object[] { d.getId(), d.getVentaId(), d.getProducto().getNombre(),
					String.format("%.2f", d.getCantidad()), String.format("$%.2f", d.getMontoDevuelto()), d.getMotivo(),
					sdf.format(d.getFecha()), d.getCajero() });
		}

		if (devoluciones.isEmpty()) {
			modelo.addRow(new Object[] { "", "", "Sin devoluciones registradas", "", "", "", "", "" });
		}

		JScrollPane scroll = new JScrollPane(tabla);
		scroll.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		scroll.getViewport().setBackground(Estilos.BG_BLANCO);
		scroll.setBounds(20, 54, 740, 360);
		dlg.add(scroll);

		JButton btnCerrar = Estilos.botonPrimario("Cerrar");
		btnCerrar.setBounds(330, 424, 120, 36);
		btnCerrar.addActionListener(e -> dlg.dispose());
		dlg.add(btnCerrar);

		dlg.getContentPane().setBackground(Estilos.BG_BLANCO);
		dlg.setVisible(true);
	}

	private void reimprimirSeleccionado() {
		int row = tablaVentas.getSelectedRow();
		if (row < 0) {
			JOptionPane.showMessageDialog(this, "Selecciona una venta de la tabla", "Aviso",
					JOptionPane.INFORMATION_MESSAGE);
			return;
		}
		Venta venta = ventasFiltradas.get(row);
		JFrame frame = (JFrame) SwingUtilities.getWindowAncestor(this);
		TicketDialog dlg = new TicketDialog(frame, venta, app.getConfig(), 0);
		dlg.mostrar();
	}

	private List<Venta> filtrarPorFecha(List<Venta> todas) {
		try {
			SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy");
			Date inicio = sdf.parse(txtFechaInicio.getText().trim());
			Date fin = sdf.parse(txtFechaFin.getText().trim());
			Calendar cal = Calendar.getInstance();
			cal.setTime(fin);
			cal.set(Calendar.HOUR_OF_DAY, 23);
			cal.set(Calendar.MINUTE, 59);
			cal.set(Calendar.SECOND, 59);
			Date finDia = cal.getTime();
			return todas.stream().filter(v -> !v.getFecha().before(inicio) && !v.getFecha().after(finDia))
					.collect(Collectors.toList());
		} catch (Exception e) {
			return todas;
		}
	}

	private void exportarCSV() {
		JFileChooser fc = new JFileChooser();
		fc.setSelectedFile(new File("ReporteVentas.csv"));
		if (fc.showSaveDialog(this) != JFileChooser.APPROVE_OPTION)
			return;
		File archivo = fc.getSelectedFile();
		SimpleDateFormat sdfF = new SimpleDateFormat("dd/MM/yyyy");
		SimpleDateFormat sdfH = new SimpleDateFormat("HH:mm:ss");
		try (PrintWriter pw = new PrintWriter(new OutputStreamWriter(new FileOutputStream(archivo), "UTF-8"))) {
			pw.println("ID,Fecha Venta,Hora,Cajero,Metodo Pago,Total");
			for (Venta v : ventasFiltradas) {
				pw.printf("%d,%s,%s,%s,%s,%.2f%n", v.getId(), sdfF.format(v.getFecha()), sdfH.format(v.getFecha()),
						v.getCajero(), v.getMetodoPago(), v.getTotal());
			}
			JOptionPane.showMessageDialog(this, "Exportado: " + archivo.getAbsolutePath(), "Listo",
					JOptionPane.INFORMATION_MESSAGE);
		} catch (IOException ex) {
			JOptionPane.showMessageDialog(this, "Error: " + ex.getMessage(), "Error", JOptionPane.ERROR_MESSAGE);
		}
	}

	private JTextField campo(int x, int y, int w, int h) {
		JTextField tf = new JTextField();
		tf.setFont(Estilos.FUENTE_BOLD);
		tf.setBackground(Estilos.BG_ZINC_100);
		tf.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(6, 10, 6, 10)));
		tf.setBounds(x, y, w, h);
		return tf;
	}
}
