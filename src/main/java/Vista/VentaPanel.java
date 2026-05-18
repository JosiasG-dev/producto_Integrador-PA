package Vista;

import Controlador.VentaControlador;
import Modelo.*;
import javax.swing.*;
import javax.swing.event.*;
import javax.swing.table.*;
import java.awt.*;
import java.util.List;

public class VentaPanel extends JPanel {

	private final VentaControlador ctrl;
	private final ConfiguracionTienda config;
	private final Usuario cajero;

	private JTextField txtBusqueda;
	private JPanel sugerenciasPanel;
	private DefaultTableModel modeloCarrito;
	private JTable tablaCarrito;
	private JLabel lblTotal;
	private JTextField txtRecibido;
	private JTextField txtDescuento;
	private JLabel lblCambio;
	private JLabel lblRecibido;
	private JLabel lblImagenProducto;
	private JComboBox<String> cmbMetodo;
	private boolean actualizando = false;

	public VentaPanel(VentaControlador ctrl, ConfiguracionTienda config, Usuario cajero) {
		this.ctrl = ctrl;
		this.config = config;
		this.cajero = cajero;
		ctrl.setPanel(this);
		setLayout(null);
		setBackground(Estilos.BG_CLARO);
		construir();
	}

	private void construir() {
		JLabel lblBusq = new JLabel("BUSCAR PRODUCTO (nombre o SKU):");
		lblBusq.setFont(Estilos.FUENTE_XS);
		lblBusq.setForeground(Estilos.TEXTO_TENUE);
		lblBusq.setBounds(16, 14, 300, 14);
		add(lblBusq);

		txtBusqueda = new JTextField();
		txtBusqueda.setFont(Estilos.FUENTE_SUBTITULO);
		txtBusqueda.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(8, 12, 8, 12)));
		txtBusqueda.setBackground(Estilos.BG_BLANCO);
		txtBusqueda.setBounds(16, 32, 700, 44);
		txtBusqueda.getDocument().addDocumentListener(new DocumentListener() {
			public void insertUpdate(DocumentEvent e) {
				actualizarSugerencias();
			}

			public void removeUpdate(DocumentEvent e) {
				actualizarSugerencias();
			}

			public void changedUpdate(DocumentEvent e) {
				actualizarSugerencias();
			}
		});
		add(txtBusqueda);

		sugerenciasPanel = new JPanel();
		sugerenciasPanel.setLayout(new BoxLayout(sugerenciasPanel, BoxLayout.Y_AXIS));
		sugerenciasPanel.setBackground(Estilos.BG_BLANCO);
		sugerenciasPanel.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		sugerenciasPanel.setVisible(false);
		sugerenciasPanel.setBounds(16, 76, 700, 0);
		add(sugerenciasPanel);

		JLabel lblDetalle = new JLabel("DETALLE DE VENTA");
		lblDetalle.setFont(Estilos.FUENTE_XS);
		lblDetalle.setForeground(Estilos.TEXTO_TENUE);
		lblDetalle.setBounds(16, 84, 200, 14);
		add(lblDetalle);

		String[] cols = { "ID", "Producto", "Precio Unit", "Cant", "Subtotal", "Quitar" };
		modeloCarrito = new DefaultTableModel(cols, 0) {
			@Override
			public boolean isCellEditable(int row, int col) {
				return col == 3 || col == 5;
			}
		};
		tablaCarrito = new JTable(modeloCarrito);
		tablaCarrito.setFont(Estilos.FUENTE_NORMAL);
		tablaCarrito.setRowHeight(44);
		tablaCarrito.setShowGrid(false);
		tablaCarrito.setIntercellSpacing(new Dimension(0, 2));
		tablaCarrito.getTableHeader().setFont(Estilos.FUENTE_XS);
		tablaCarrito.getTableHeader().setBackground(Estilos.BG_ZINC_100);
		tablaCarrito.getColumnModel().getColumn(0).setMaxWidth(0);
		tablaCarrito.getColumnModel().getColumn(0).setMinWidth(0);
		tablaCarrito.getColumnModel().getColumn(0).setPreferredWidth(0);
		tablaCarrito.getColumnModel().getColumn(2).setMaxWidth(120);
		tablaCarrito.getColumnModel().getColumn(3).setMaxWidth(80);
		tablaCarrito.getColumnModel().getColumn(4).setMaxWidth(110);
		tablaCarrito.getColumnModel().getColumn(5).setMaxWidth(100);
		tablaCarrito.getColumnModel().getColumn(5).setCellRenderer(new BtnRenderer());
		tablaCarrito.getColumnModel().getColumn(5).setCellEditor(new BtnEditor(id -> ctrl.eliminarDelCarrito(id)));

		modeloCarrito.addTableModelListener(e -> {
			if (actualizando)
				return;
			if (e.getColumn() != 3)
				return;
			int row = e.getFirstRow();
			if (row < 0 || row >= modeloCarrito.getRowCount())
				return;
			try {
				String id = (String) modeloCarrito.getValueAt(row, 0);
				double val = Double.parseDouble(modeloCarrito.getValueAt(row, 3).toString().trim());
				if (val <= 0)
					ctrl.eliminarDelCarrito(id);
				else
					ctrl.setCantidad(id, val);
			} catch (NumberFormatException ignored) {
			}
		});

		JScrollPane scrollCarrito = new JScrollPane(tablaCarrito);
		scrollCarrito.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		scrollCarrito.getViewport().setBackground(Estilos.BG_BLANCO);
		scrollCarrito.setBounds(16, 100, 700, 480);
		add(scrollCarrito);

		JPanel panelCobro = new JPanel(null);
		panelCobro.setBackground(Estilos.BG_OSCURO);
		panelCobro.setBorder(BorderFactory.createLineBorder(new Color(63, 63, 70)));
		panelCobro.setBounds(726, 14, 250, 600);
		add(panelCobro);

		JLabel lblResumen = new JLabel("RESUMEN");
		lblResumen.setFont(Estilos.FUENTE_XS);
		lblResumen.setForeground(new Color(129, 140, 248));
		lblResumen.setBounds(16, 16, 218, 14);
		panelCobro.add(lblResumen);

		lblImagenProducto = new JLabel("NO IMAGEN", SwingConstants.CENTER);
		lblImagenProducto.setFont(Estilos.FUENTE_BOLD);
		lblImagenProducto.setForeground(new Color(63, 63, 70));
		lblImagenProducto.setBorder(BorderFactory.createLineBorder(new Color(63, 63, 70)));
		lblImagenProducto.setBounds(16, 290, 218, 170);
		panelCobro.add(lblImagenProducto);
		;

		lblTotal = new JLabel("$0.00");
		lblTotal.setFont(new Font("SansSerif", Font.BOLD, 38));
		lblTotal.setForeground(new Color(165, 180, 252));
		lblTotal.setBounds(16, 34, 218, 46);
		panelCobro.add(lblTotal);

		JLabel lblDescuento = new JLabel("DESCUENTO ($ o %)");
		lblDescuento.setFont(Estilos.FUENTE_XS);
		lblDescuento.setForeground(new Color(113, 113, 122));
		lblDescuento.setBounds(16, 90, 218, 14);
		panelCobro.add(lblDescuento);

		txtDescuento = new JTextField("0.00");
		txtDescuento.setFont(Estilos.FUENTE_BOLD);
		txtDescuento.setBackground(new Color(39, 39, 42));
		txtDescuento.setForeground(new Color(52, 211, 153));
		txtDescuento
				.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(new Color(63, 63, 70), 2),
						BorderFactory.createEmptyBorder(6, 10, 6, 10)));
		txtDescuento.setBounds(16, 108, 218, 36);
		txtDescuento.getDocument().addDocumentListener(new DocumentListener() {
			public void insertUpdate(DocumentEvent e) {
				aplicarDescuento();
			}

			public void removeUpdate(DocumentEvent e) {
				aplicarDescuento();
			}

			public void changedUpdate(DocumentEvent e) {
				aplicarDescuento();
			}
		});
		panelCobro.add(txtDescuento);

		JLabel lblMetodoLbl = new JLabel("METODO DE PAGO");
		lblMetodoLbl.setFont(Estilos.FUENTE_XS);
		lblMetodoLbl.setForeground(new Color(113, 113, 122));
		lblMetodoLbl.setBounds(16, 158, 218, 14);
		panelCobro.add(lblMetodoLbl);

		cmbMetodo = new JComboBox<>(new String[] { "Efectivo", "Tarjeta" });
		cmbMetodo.setFont(Estilos.FUENTE_BOLD);
		cmbMetodo.setBounds(16, 176, 218, 36);
		cmbMetodo.addActionListener(e -> actualizarVisibilidadEfectivo());
		panelCobro.add(cmbMetodo);

		lblRecibido = new JLabel("IMPORTE RECIBIDO");
		lblRecibido.setFont(Estilos.FUENTE_XS);
		lblRecibido.setForeground(new Color(113, 113, 122));
		lblRecibido.setBounds(16, 224, 218, 14);
		panelCobro.add(lblRecibido);

		txtRecibido = new JTextField("0.00");
		txtRecibido.setFont(new Font("SansSerif", Font.BOLD, 22));
		txtRecibido.setBackground(new Color(39, 39, 42));
		txtRecibido.setForeground(Color.WHITE);
		txtRecibido
				.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(new Color(63, 63, 70), 2),
						BorderFactory.createEmptyBorder(8, 12, 8, 12)));
		txtRecibido.setBounds(16, 242, 218, 48);
		txtRecibido.getDocument().addDocumentListener(new DocumentListener() {
			public void insertUpdate(DocumentEvent e) {
				actualizarCambio();
			}

			public void removeUpdate(DocumentEvent e) {
				actualizarCambio();
			}

			public void changedUpdate(DocumentEvent e) {
				actualizarCambio();
			}
		});
		panelCobro.add(txtRecibido);

		lblCambio = new JLabel("Cambio: $0.00");
		lblCambio.setFont(Estilos.FUENTE_BOLD);
		lblCambio.setForeground(new Color(52, 211, 153));
		lblCambio.setBounds(16, 300, 218, 22);
		panelCobro.add(lblCambio);

		JButton btnCobrar = new JButton("COBRAR");
		btnCobrar.setFont(new Font("SansSerif", Font.BOLD, 16));
		btnCobrar.setBackground(Estilos.INDIGO);
		btnCobrar.setForeground(Color.WHITE);
		btnCobrar.setBorderPainted(false);
		btnCobrar.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
		btnCobrar.setBounds(16, 536, 218, 52);
		btnCobrar.addActionListener(e -> procesarCobro());
		panelCobro.add(btnCobrar);
	}

	private void actualizarVisibilidadEfectivo() {
		boolean esEfectivo = "Efectivo".equals(cmbMetodo.getSelectedItem());
		lblRecibido.setVisible(esEfectivo);
		txtRecibido.setVisible(esEfectivo);
		lblCambio.setVisible(esEfectivo);
		if (!esEfectivo)
			txtRecibido.setText("0.00");
	}

	private void aplicarDescuento() {
		try {
			String val = txtDescuento.getText().trim();
			if (val.endsWith("%")) {
				double porcentaje = Double.parseDouble(val.replace("%", "").trim());
				double subtotal = ctrl.calcularSubtotal();
				ctrl.setDescuento(subtotal * (porcentaje / 100.0));
			} else {
				ctrl.setDescuento(Double.parseDouble(val));
			}
		} catch (NumberFormatException ignored) {
			ctrl.setDescuento(0);
		}
	}

	private void actualizarSugerencias() {
		sugerenciasPanel.removeAll();
		String q = txtBusqueda.getText().trim();
		if (q.isBlank()) {
			sugerenciasPanel.setVisible(false);
			return;
		}
		List<Producto> res = ctrl.buscarProductos(q);
		int altoFila = 32;
		for (Producto p : res) {
			JButton btn = new JButton(String.format("[%s]  %-22s  $%.2f  stock:%.0f", p.getId(),
					p.getNombre().toUpperCase(), p.getPrecio(), p.getStock()));
			btn.setFont(Estilos.FUENTE_SMALL);
			btn.setBackground(Estilos.BG_BLANCO);
			btn.setBorderPainted(false);
			btn.setHorizontalAlignment(SwingConstants.LEFT);
			btn.setMaximumSize(new Dimension(Integer.MAX_VALUE, altoFila));
			btn.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
			btn.addActionListener(e -> {
				ctrl.agregarAlCarrito(p);
				mostrarImagen(p.getImagenRuta());
				txtBusqueda.setText("");
				sugerenciasPanel.removeAll();
				sugerenciasPanel.setVisible(false);
			});
			sugerenciasPanel.add(btn);
		}
		int alto = res.size() * altoFila;
		sugerenciasPanel.setSize(700, alto);
		sugerenciasPanel.setVisible(!res.isEmpty());
		sugerenciasPanel.revalidate();
		sugerenciasPanel.repaint();

	}

	private void actualizarCambio() {
		try {
			double rec = Double.parseDouble(txtRecibido.getText().trim());
			lblCambio.setText(String.format("Cambio: $%.2f", ctrl.calcularCambio(rec)));
		} catch (NumberFormatException ignored) {
		}
	}

	private void procesarCobro() {
		if (ctrl.getCarrito().isEmpty()) {
			JOptionPane.showMessageDialog(this, "El carrito esta vacio", "Aviso", JOptionPane.INFORMATION_MESSAGE);
			return;
		}
		String metodo = (String) cmbMetodo.getSelectedItem();
		double recibido = 0;
		if ("Efectivo".equals(metodo)) {
			try {
				recibido = Double.parseDouble(txtRecibido.getText().trim());
			} catch (Exception ignored) {
			}
		} else {
			recibido = ctrl.calcularTotal();
		}
		ctrl.procesarCobro(metodo, recibido);
	}

	public void refrescarCarrito(List<ItemCarrito> carrito, double total, double descuento) {
		actualizando = true;
		modeloCarrito.setRowCount(0);
		for (ItemCarrito item : carrito) {
			String cantStr = item.getProducto().esPorPieza() ? String.format("%.0f", item.getCantidad())
					: String.format("%.2f", item.getCantidad());
			modeloCarrito.addRow(new Object[] { item.getProducto().getId(), item.getProducto().getNombre(),
					String.format("$%.2f", item.getProducto().getPrecio()), cantStr,
					String.format("$%.2f", item.getSubtotal()), item.getProducto().getId() });
		}
		actualizando = false;
		lblTotal.setText(String.format("$%.2f", total));
	}

	public void limpiar() {
		actualizando = true;
		modeloCarrito.setRowCount(0);
		actualizando = false;
		lblTotal.setText("$0.00");
		txtRecibido.setText("0.00");
		txtDescuento.setText("0.00");
		lblCambio.setText("Cambio: $0.00");
		txtBusqueda.setText("");
		sugerenciasPanel.setVisible(false);
		cmbMetodo.setSelectedIndex(0);
		actualizarVisibilidadEfectivo();
		lblImagenProducto.setIcon(null);
		lblImagenProducto.setText("");
	}

	public static class BtnRenderer extends JButton implements TableCellRenderer {
		public BtnRenderer() {
			this("Quitar", Estilos.ROSE_CLARO, Estilos.ROSE);
		}

		public BtnRenderer(String texto, Color bg, Color fg) {
			setText(texto);
			setFont(Estilos.FUENTE_XS);
			setForeground(fg);
			setBackground(bg);
			setBorderPainted(false);
			setOpaque(true);
		}

		@Override
		public Component getTableCellRendererComponent(JTable t, Object v, boolean s, boolean f, int r, int c) {
			return this;
		}
	}

	private void mostrarImagen(String ruta) {
		if (ruta == null || ruta.isEmpty()) {
			lblImagenProducto.setIcon(null);
			lblImagenProducto.setText("SIN FOTO");
			return;
		}
		try {
			java.io.File archivo = new java.io.File(ruta);
			if (!archivo.isAbsolute() || !archivo.exists()) {
				archivo = new java.io.File(Util.RutaBase.getRaiz(), ruta);
			}
			if (archivo.exists()) {
				java.awt.image.BufferedImage bimg = javax.imageio.ImageIO.read(archivo);
				if (bimg != null) {
					Image img = bimg.getScaledInstance(218, 170, Image.SCALE_SMOOTH);
					lblImagenProducto.setIcon(new ImageIcon(img));
					lblImagenProducto.setText("");
				} else {
					lblImagenProducto.setIcon(null);
					lblImagenProducto.setText("SIN FOTO");
				}
			} else {
				lblImagenProducto.setIcon(null);
				lblImagenProducto.setText("SIN FOTO");
			}
		} catch (Exception e) {
			lblImagenProducto.setText("ERROR CARGA");
		}
	}

	public static class BtnEditor extends DefaultCellEditor {
		private final JButton btn = new JButton("Quitar");
		private String id;
		private final java.util.function.Consumer<String> accion;

		public BtnEditor(java.util.function.Consumer<String> accion) {
			super(new JCheckBox());
			this.accion = accion;
			btn.setFont(Estilos.FUENTE_XS);
			btn.setForeground(Estilos.ROSE);
			btn.setBackground(Estilos.ROSE_CLARO);
			btn.setBorderPainted(false);
			btn.setOpaque(true);
			btn.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
			btn.addActionListener(e -> {
				fireEditingStopped();
				accion.accept(id);
			});
		}

		@Override
		public Component getTableCellEditorComponent(JTable t, Object value, boolean isSelected, int row, int col) {
			id = value != null ? value.toString() : "";
			return btn;
		}

		@Override
		public Object getCellEditorValue() {
			return id;
		}

	}
}
