package Vista;

import javax.swing.*;
import javax.swing.table.*;
import Controlador.ControladorPrincipal;
import Modelo.*;
import Util.ManejoErrores;
import java.awt.*;
import java.io.*;
import java.text.SimpleDateFormat;
import org.apache.poi.ss.usermodel.Cell;
import org.apache.poi.ss.usermodel.CellStyle;
import org.apache.poi.ss.usermodel.FillPatternType;
import org.apache.poi.ss.usermodel.IndexedColors;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.ss.usermodel.Workbook;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;
import java.util.*;
import java.util.List;
import java.util.stream.Collectors;

public class ReportesPanel extends JPanel {

	private final ControladorPrincipal app;
	private JLabel lblTotal, lblNumVentas, lblPromedio;
	private DefaultTableModel modeloVentas;
	private JTable tablaVentas;
	private JTextField txtFechaInicio, txtFechaFin, txtFiltroProducto;
	private List<Venta> ventasFiltradas = new ArrayList<>();

	public ReportesPanel(ControladorPrincipal app) {
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

		txtFechaInicio = campo(24, 74, 150, 34);
		txtFechaInicio.setText("01/01/2024");
		add(txtFechaInicio);

		JLabel lblHasta = new JLabel("HASTA (dd/MM/yyyy):");
		lblHasta.setFont(Estilos.FUENTE_XS);
		lblHasta.setForeground(Estilos.TEXTO_TENUE);
		lblHasta.setBounds(184, 56, 160, 14);
		add(lblHasta);

		txtFechaFin = campo(184, 74, 150, 34);
		txtFechaFin.setText(new SimpleDateFormat("dd/MM/yyyy").format(new Date()));
		add(txtFechaFin);

		JLabel lblProd = new JLabel("FILTRAR POR PRODUCTO (nombre o SKU):");
		lblProd.setFont(Estilos.FUENTE_XS);
		lblProd.setForeground(Estilos.TEXTO_TENUE);
		lblProd.setBounds(344, 56, 280, 14);
		add(lblProd);

		txtFiltroProducto = campo(344, 74, 220, 34);
		txtFiltroProducto.setToolTipText("Deja vacio para ver todas las ventas");
		add(txtFiltroProducto);

		JButton btnFiltrar = Estilos.botonPrimario("Filtrar");
		btnFiltrar.setBounds(574, 74, 90, 34);
		btnFiltrar.addActionListener(e -> refrescar());
		add(btnFiltrar);

		JButton btnExcel = Estilos.botonSecundario("Exportar Excel");
		btnExcel.setBounds(674, 74, 120, 34);
		btnExcel.addActionListener(e -> exportarExcel());
		add(btnExcel);

		JButton btnDevolucion = Estilos.botonPeligro("Devolucion");
		btnDevolucion.setBounds(804, 74, 110, 34);
		btnDevolucion.addActionListener(e -> {
			DevolucionDialog dlg = new DevolucionDialog(
					(JFrame) SwingUtilities.getWindowAncestor(this), app);
			dlg.setVisible(true);
		});
		add(btnDevolucion);

		JButton btnVerDevoluciones = Estilos.botonSecundario("Ver Devoluciones");
		btnVerDevoluciones.setBounds(924, 74, 148, 34);
		btnVerDevoluciones.addActionListener(e -> mostrarDevoluciones());
		add(btnVerDevoluciones);

		JButton btnReimprimir = Estilos.botonSecundario("Reimprimir Ticket");
		btnReimprimir.setBounds(1082, 74, 148, 34);
		btnReimprimir.addActionListener(e -> reimprimirSeleccionado());
		add(btnReimprimir);

		JButton btnPorProducto = Estilos.botonSecundario("Ventas por Producto");
		btnPorProducto.setBounds(1240, 74, 180, 34);
		btnPorProducto.addActionListener(e -> mostrarVentasPorProducto());
		add(btnPorProducto);

		lblTotal    = kpiCard("Ingresos",    "$0.00", Estilos.INDIGO,    Color.WHITE,             24,  124);
		lblNumVentas = kpiCard("Ventas",     "0",     Estilos.BG_BLANCO, Estilos.TEXTO_PRINCIPAL, 204, 124);
		lblPromedio  = kpiCard("Ticket Prom","$0.00", Estilos.BG_BLANCO, Estilos.TEXTO_PRINCIPAL, 384, 124);

		String[] cols = { "#", "Fecha Venta", "Hora", "Cajero", "Metodo", "Total", "Productos" };
		modeloVentas = new DefaultTableModel(cols, 0) {
			@Override public boolean isCellEditable(int r, int c) { return false; }
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
		tablaVentas.getColumnModel().getColumn(6).setPreferredWidth(340);

		JScrollPane scroll = new JScrollPane(tablaVentas);
		scroll.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		scroll.getViewport().setBackground(Estilos.BG_BLANCO);
		scroll.setBounds(24, 242, 1200, 440);
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
		String filtroProd = txtFiltroProducto != null ? txtFiltroProducto.getText().trim() : "";
		List<Venta> todasFecha = filtrarPorFecha(app.getVentas());

		if (!filtroProd.isEmpty()) {
			final String fp = filtroProd.toLowerCase();
			ventasFiltradas = todasFecha.stream()
				.filter(v -> v.getItems().stream().anyMatch(item ->
					item.getProducto().getNombre().toLowerCase().contains(fp) ||
					item.getProducto().getId().toLowerCase().contains(fp)))
				.collect(Collectors.toList());
		} else {
			ventasFiltradas = todasFecha;
		}

		double total = ventasFiltradas.stream().mapToDouble(Venta::getTotal).sum();
		double prom  = ventasFiltradas.isEmpty() ? 0 : total / ventasFiltradas.size();
		lblTotal.setText(String.format("$%.2f", total));
		lblNumVentas.setText(String.valueOf(ventasFiltradas.size()));
		lblPromedio.setText(String.format("$%.2f", prom));

		modeloVentas.setRowCount(0);
		SimpleDateFormat sdfF = new SimpleDateFormat("dd/MM/yyyy");
		SimpleDateFormat sdfH = new SimpleDateFormat("HH:mm:ss");
		for (Venta v : ventasFiltradas) {
			
			String productos = v.getItems().stream()
				.map(i -> i.getProducto().getNombre() + " x" + (int) i.getCantidad())
				.collect(Collectors.joining(", "));
			modeloVentas.addRow(new Object[] {
				v.getId(),
				sdfF.format(v.getFecha()),
				sdfH.format(v.getFecha()),
				v.getCajero(),
				v.getMetodoPago(),
				String.format("$%.2f", v.getTotal()),
				productos
			});
		}
	}

	private void mostrarDevoluciones() {
		List<Devolucion> devoluciones = app.getDevoluciones();

		JDialog dlg = new JDialog((JFrame) SwingUtilities.getWindowAncestor(this),
				"Historial de Devoluciones", true);
		dlg.setSize(800, 480);
		dlg.setLocationRelativeTo(this);
		dlg.setLayout(null);

		JLabel tit = new JLabel("HISTORIAL DE DEVOLUCIONES");
		tit.setFont(Estilos.FUENTE_TITULO);
		tit.setBounds(20, 14, 500, 30);
		dlg.add(tit);

		String[] cols = { "#", "Venta #", "Producto", "Cantidad", "Monto Dev", "Motivo", "Fecha", "Cajero" };
		DefaultTableModel modelo = new DefaultTableModel(cols, 0) {
			@Override public boolean isCellEditable(int r, int c) { return false; }
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
			modelo.addRow(new Object[] {
				d.getId(), d.getVentaId(), d.getProducto().getNombre(),
				String.format("%.2f", d.getCantidad()),
				String.format("$%.2f", d.getMontoDevuelto()),
				d.getMotivo(), sdf.format(d.getFecha()), d.getCajero()
			});
		}
		if (devoluciones.isEmpty())
			modelo.addRow(new Object[] { "", "", "Sin devoluciones registradas", "", "", "", "", "" });

		JScrollPane scroll = new JScrollPane(tabla);
		scroll.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		scroll.getViewport().setBackground(Estilos.BG_BLANCO);
		scroll.setBounds(20, 54, 760, 360);
		dlg.add(scroll);

		JButton btnCerrar = Estilos.botonPrimario("Cerrar");
		btnCerrar.setBounds(340, 424, 120, 36);
		btnCerrar.addActionListener(e -> dlg.dispose());
		dlg.add(btnCerrar);

		dlg.getContentPane().setBackground(Estilos.BG_BLANCO);
		dlg.setVisible(true);
	}

	private void reimprimirSeleccionado() {
		int row = tablaVentas.getSelectedRow();
		if (row < 0) {
			ManejoErrores.advertencia(this, "Seleccion requerida",
					"Selecciona una venta de la tabla para reimprimir su ticket.");
			return;
		}
		Venta venta = ventasFiltradas.get(row);
		if (venta.getItems() == null || venta.getItems().isEmpty()) {
			ManejoErrores.advertencia(this, "Sin detalle",
					"Esta venta no tiene detalle de productos disponible para reimprimir.");
			return;
		}
		JFrame frame = (JFrame) SwingUtilities.getWindowAncestor(this);
		TicketDialog dlg = new TicketDialog(frame, venta, app.getConfig(), venta.getCambio(), venta.getEfectivoRecibido());
		ManejoErrores.registrarInfo("Reimpresion de ticket #" + venta.getId());
		dlg.mostrar();
	}

	private List<Venta> filtrarPorFecha(List<Venta> todas) {
		try {
			SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy");
			Date inicio = sdf.parse(txtFechaInicio.getText().trim());
			Date fin    = sdf.parse(txtFechaFin.getText().trim());
			Calendar cal = Calendar.getInstance();
			cal.setTime(fin);
			cal.set(Calendar.HOUR_OF_DAY, 23);
			cal.set(Calendar.MINUTE, 59);
			cal.set(Calendar.SECOND, 59);
			Date finDia = cal.getTime();
			return todas.stream()
				.filter(v -> !v.getFecha().before(inicio) && !v.getFecha().after(finDia))
				.collect(Collectors.toList());
		} catch (Exception e) {
			return todas;
		}
	}

	private void exportarExcel() {
		JFileChooser fc = new JFileChooser();
		fc.setSelectedFile(new File("ReporteVentas.xlsx"));
		if (fc.showSaveDialog(this) != JFileChooser.APPROVE_OPTION) return;
		File archivo = fc.getSelectedFile();
		SimpleDateFormat sdfF = new SimpleDateFormat("dd/MM/yyyy");
		SimpleDateFormat sdfH = new SimpleDateFormat("HH:mm:ss");
		try (Workbook libro = new XSSFWorkbook()) {
			Sheet hoja = libro.createSheet("Reporte de Ventas");
			CellStyle estiloEncabezado = libro.createCellStyle();
			org.apache.poi.ss.usermodel.Font fuenteNegrita = libro.createFont();
			fuenteNegrita.setBold(true);
			estiloEncabezado.setFont(fuenteNegrita);
			estiloEncabezado.setFillForegroundColor(IndexedColors.GREY_25_PERCENT.getIndex());
			estiloEncabezado.setFillPattern(FillPatternType.SOLID_FOREGROUND);
			String[] columnas = {
				"ID Venta", "Fecha Venta", "Hora", "Cajero",
				"Metodo Pago", "Total Venta", "SKU", "Producto",
				"Cantidad", "Precio Unitario", "Subtotal"
			};
			Row fila = hoja.createRow(0);
			for (int i = 0; i < columnas.length; i++) {
				Cell celda = fila.createCell(i);
				celda.setCellValue(columnas[i]);
				celda.setCellStyle(estiloEncabezado);
			}
			int numFila = 1;
			for (Venta v : ventasFiltradas) {
				String fecha = sdfF.format(v.getFecha());
				String hora  = sdfH.format(v.getFecha());
				if (v.getItems() == null || v.getItems().isEmpty()) {
					Row r = hoja.createRow(numFila++);
					r.createCell(0).setCellValue(v.getId());
					r.createCell(1).setCellValue(fecha);
					r.createCell(2).setCellValue(hora);
					r.createCell(3).setCellValue(v.getCajero());
					r.createCell(4).setCellValue(v.getMetodoPago());
					r.createCell(5).setCellValue(v.getTotal());
				} else {
					for (ItemCarrito item : v.getItems()) {
						Row r = hoja.createRow(numFila++);
						r.createCell(0).setCellValue(v.getId());
						r.createCell(1).setCellValue(fecha);
						r.createCell(2).setCellValue(hora);
						r.createCell(3).setCellValue(v.getCajero());
						r.createCell(4).setCellValue(v.getMetodoPago());
						r.createCell(5).setCellValue(v.getTotal());
						r.createCell(6).setCellValue(item.getProducto().getId());
						r.createCell(7).setCellValue(item.getProducto().getNombre());
						r.createCell(8).setCellValue(item.getCantidad());
						r.createCell(9).setCellValue(item.getProducto().getPrecio());
						r.createCell(10).setCellValue(item.getSubtotal());
					}
				}
			}
			for (int i = 0; i < columnas.length; i++) {
				hoja.autoSizeColumn(i);
			}
			try (FileOutputStream salida = new FileOutputStream(archivo)) {
				libro.write(salida);
			}
			ManejoErrores.info(this, "Exportacion exitosa",
					"Excel guardado en:\n" + archivo.getAbsolutePath());
			ManejoErrores.registrarInfo("Excel exportado: " + archivo.getAbsolutePath() + " (" + ventasFiltradas.size() + " ventas)");
		} catch (IOException ex) {
			ManejoErrores.error(this, "Error al exportar", "No se pudo guardar el archivo Excel.", ex);
		}
	}

	private void mostrarVentasPorProducto() {
		Map<String, double[]> resumen = new LinkedHashMap<>();
		for (Venta v : ventasFiltradas) {
			if (v.getItems() == null) continue;
			for (ItemCarrito item : v.getItems()) {
				String clave = "[" + item.getProducto().getId() + "] " + item.getProducto().getNombre();
				resumen.merge(clave,
					new double[]{item.getCantidad(), item.getSubtotal()},
					(a, b) -> new double[]{a[0] + b[0], a[1] + b[1]});
			}
		}

		List<Map.Entry<String, double[]>> lista = new ArrayList<>(resumen.entrySet());
		lista.sort((a, b) -> Double.compare(b.getValue()[1], a.getValue()[1]));

		JDialog dlg = new JDialog((JFrame) SwingUtilities.getWindowAncestor(this),
				"Reporte de Ventas por Producto", true);
		dlg.setSize(700, 520);
		dlg.setLocationRelativeTo(this);
		dlg.setLayout(null);
		dlg.getContentPane().setBackground(Estilos.BG_BLANCO);

		JLabel tit = new JLabel("RANKING DE PRODUCTOS MAS VENDIDOS");
		tit.setFont(Estilos.FUENTE_TITULO);
		tit.setForeground(Estilos.TEXTO_PRINCIPAL);
		tit.setBounds(20, 14, 660, 30);
		dlg.add(tit);

		JLabel sub = new JLabel("Periodo: " + txtFechaInicio.getText() + "  a  " + txtFechaFin.getText()
				+ "   |   " + lista.size() + " productos distintos");
		sub.setFont(Estilos.FUENTE_XS);
		sub.setForeground(Estilos.TEXTO_TENUE);
		sub.setBounds(20, 46, 660, 16);
		dlg.add(sub);

		String[] cols = { "#", "Producto", "Unidades Vendidas", "Ingresos Totales" };
		DefaultTableModel modelo = new DefaultTableModel(cols, 0) {
			@Override public boolean isCellEditable(int r, int c) { return false; }
		};
		int rank = 1;
		for (Map.Entry<String, double[]> e : lista) {
			double[] vals = e.getValue();
			modelo.addRow(new Object[]{
				rank++,
				e.getKey(),
				String.format("%.0f", vals[0]),
				String.format("$%.2f", vals[1])
			});
		}
		if (lista.isEmpty())
			modelo.addRow(new Object[]{ "-", "Sin ventas en el periodo seleccionado", "-", "-" });

		JTable tabla = new JTable(modelo);
		tabla.setFont(Estilos.FUENTE_NORMAL);
		tabla.setRowHeight(36);
		tabla.setShowGrid(false);
		tabla.setIntercellSpacing(new Dimension(0, 2));
		tabla.getTableHeader().setFont(Estilos.FUENTE_XS);
		tabla.getTableHeader().setBackground(Estilos.BG_ZINC_100);
		tabla.getColumnModel().getColumn(0).setMaxWidth(40);
		tabla.getColumnModel().getColumn(2).setMaxWidth(140);
		tabla.getColumnModel().getColumn(3).setMaxWidth(140);

		JScrollPane scroll = new JScrollPane(tabla);
		scroll.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		scroll.getViewport().setBackground(Estilos.BG_BLANCO);
		scroll.setBounds(20, 70, 660, 380);
		dlg.add(scroll);

		JButton btnCerrar = Estilos.botonPrimario("Cerrar");
		btnCerrar.setBounds(290, 460, 120, 36);
		btnCerrar.addActionListener(e -> dlg.dispose());
		dlg.add(btnCerrar);

		dlg.setVisible(true);
	}

	private JTextField campo(int x, int y, int w, int h) {
		JTextField tf = new JTextField();
		tf.setFont(Estilos.FUENTE_BOLD);
		tf.setBackground(Estilos.BG_ZINC_100);
		tf.setBorder(BorderFactory.createCompoundBorder(
			BorderFactory.createLineBorder(Estilos.BORDE, 2),
			BorderFactory.createEmptyBorder(6, 10, 6, 10)));
		tf.setBounds(x, y, w, h);
		return tf;
	}
}
