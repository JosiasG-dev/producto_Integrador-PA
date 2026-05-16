package Vista;

import javax.swing.*;
import javax.swing.table.DefaultTableModel;
import Controlador.*;
import Modelo.*;
import java.awt.*;
import java.util.List;

public class ProveedorPanel extends JPanel {

	private final ProveedorControlador ctrl;
	private final Usuario usuario;
	private JTabbedPane tabs;
	private DefaultTableModel modeloProveedores;
	private DefaultTableModel modeloOrdenes;
	private DefaultTableModel modeloCuentas;

	public ProveedorPanel(ProveedorControlador ctrl, Usuario usuario) {
		this.ctrl = ctrl;
		this.usuario = usuario;
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
		JLabel tit = new JLabel("ABASTECIMIENTO  —  Proveedores y Cuentas por Pagar");
		tit.setFont(Estilos.FUENTE_TITULO);
		header.add(tit, BorderLayout.WEST);

		tabs = new JTabbedPane();
		tabs.setFont(Estilos.FUENTE_BOLD);
		tabs.addTab("Directorio", construirTabProveedores());
		tabs.addTab("Ordenes", construirTabOrdenes());
		tabs.addTab("Deudas", construirTabCuentas());

		add(header, BorderLayout.NORTH);
		add(tabs, BorderLayout.CENTER);
	}

	private JPanel construirTabProveedores() {
		JPanel p = new JPanel(new BorderLayout(0, 8));
		p.setBackground(Estilos.BG_BLANCO);
		p.setBorder(BorderFactory.createEmptyBorder(12, 12, 12, 12));

		JPanel toolbar = new JPanel(new FlowLayout(FlowLayout.RIGHT));
		toolbar.setBackground(Estilos.BG_BLANCO);
		JButton btnNuevo = Estilos.botonPrimario("+ Alta Proveedor");
		btnNuevo.setPreferredSize(new Dimension(160, 40));
		btnNuevo.addActionListener(e -> abrirFormularioProveedor(null));
		toolbar.add(btnNuevo);

		String[] cols = { "ID", "Empresa", "Contacto", "Telefono", "Email", "Direccion" };
		modeloProveedores = new DefaultTableModel(cols, 0) {
			@Override
			public boolean isCellEditable(int r, int c) {
				return false;
			}
		};
		JTable tabla = new JTable(modeloProveedores);
		tabla.setFont(Estilos.FUENTE_NORMAL);
		tabla.setRowHeight(40);
		tabla.setShowGrid(false);
		tabla.getTableHeader().setFont(Estilos.FUENTE_XS);
		tabla.getTableHeader().setBackground(Estilos.BG_ZINC_100);
		tabla.getColumnModel().getColumn(0).setMaxWidth(60);

		tabla.addMouseListener(new java.awt.event.MouseAdapter() {
			@Override
			public void mouseClicked(java.awt.event.MouseEvent e) {
				if (e.getClickCount() == 2) {
					int row = tabla.getSelectedRow();
					if (row >= 0) {
						int id = (int) modeloProveedores.getValueAt(row, 0);
						Proveedor prov = ctrl.getProveedores().stream().filter(x -> x.getId() == id).findFirst()
								.orElse(null);
						if (prov != null)
							abrirFormularioProveedor(prov);
					}
				}
			}
		});

		JScrollPane scroll = new JScrollPane(tabla);
		scroll.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		p.add(toolbar, BorderLayout.NORTH);
		p.add(scroll, BorderLayout.CENTER);
		return p;
	}

	private JPanel construirTabOrdenes() {
		JPanel p = new JPanel(new BorderLayout(0, 8));
		p.setBackground(Estilos.BG_BLANCO);
		p.setBorder(BorderFactory.createEmptyBorder(12, 12, 12, 12));

		JPanel toolbar = new JPanel(new FlowLayout(FlowLayout.RIGHT));
		toolbar.setBackground(Estilos.BG_BLANCO);
		JButton btnNueva = Estilos.botonExito("+ Generar Pedido");
		btnNueva.setPreferredSize(new Dimension(170, 40));
		btnNueva.addActionListener(e -> abrirNuevaOrden());
		toolbar.add(btnNueva);

		String[] cols = { "Folio", "Proveedor", "Total", "Pago", "Estado", "Accion" };
		modeloOrdenes = new DefaultTableModel(cols, 0) {
			@Override
			public boolean isCellEditable(int r, int c) {
				return c == 5;
			}
		};
		JTable tablaOrdenes = new JTable(modeloOrdenes);
		tablaOrdenes.setFont(Estilos.FUENTE_NORMAL);
		tablaOrdenes.setRowHeight(42);
		tablaOrdenes.setShowGrid(false);
		tablaOrdenes.getTableHeader().setFont(Estilos.FUENTE_XS);
		tablaOrdenes.getTableHeader().setBackground(Estilos.BG_ZINC_100);
		tablaOrdenes.getColumnModel().getColumn(0).setMaxWidth(100);
		tablaOrdenes.getColumnModel().getColumn(3).setMaxWidth(100);
		tablaOrdenes.getColumnModel().getColumn(4).setMaxWidth(110);
		tablaOrdenes.getColumnModel().getColumn(5).setMaxWidth(110);

		tablaOrdenes.getColumnModel().getColumn(5)
				.setCellRenderer(new VentaPanel.BtnRenderer("Recibir", Estilos.EMERALD_CLARO, Estilos.EMERALD));
		tablaOrdenes.getColumnModel().getColumn(5).setCellEditor(new VentaPanel.BtnEditor(id -> {
			try {
				int oid = Integer.parseInt(id);
				OrdenCompra orden = ctrl.getOrdenes().stream().filter(o -> o.getId() == oid).findFirst().orElse(null);
				if (orden != null && orden.getEstado() == OrdenCompra.Estado.RECIBIDO) {
					JOptionPane.showMessageDialog(SwingUtilities.getWindowAncestor(this), "Esta orden ya fue recibida.",
							"Aviso", JOptionPane.INFORMATION_MESSAGE);
					return;
				}
				ctrl.recibirOrden(oid);
				refrescarOrdenes();
				refrescarCuentas();
			} catch (Exception ignored) {
			}
		}));

		JScrollPane scroll = new JScrollPane(tablaOrdenes);
		scroll.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		p.add(toolbar, BorderLayout.NORTH);
		p.add(scroll, BorderLayout.CENTER);
		return p;
	}

	private JPanel construirTabCuentas() {
		JPanel p = new JPanel(new BorderLayout(0, 8));
		p.setBackground(Estilos.BG_BLANCO);
		p.setBorder(BorderFactory.createEmptyBorder(12, 12, 12, 12));

		JLabel subtit = new JLabel("  Saldos pendientes con acreedores");
		subtit.setFont(Estilos.FUENTE_SMALL);
		subtit.setForeground(Estilos.TEXTO_TENUE);

		String[] cols = { "Proveedor", "Orden", "Total", "Pagado", "Saldo", "Estado", "Accion" };
		modeloCuentas = new DefaultTableModel(cols, 0) {
			@Override
			public boolean isCellEditable(int r, int c) {
				return c == 6;
			}
		};
		JTable tablaCuentas = new JTable(modeloCuentas);
		tablaCuentas.setFont(Estilos.FUENTE_NORMAL);
		tablaCuentas.setRowHeight(42);
		tablaCuentas.setShowGrid(false);
		tablaCuentas.getTableHeader().setFont(Estilos.FUENTE_XS);
		tablaCuentas.getTableHeader().setBackground(Estilos.BG_ZINC_100);
		tablaCuentas.getColumnModel().getColumn(5).setMaxWidth(100);
		tablaCuentas.getColumnModel().getColumn(6).setMaxWidth(120);

		tablaCuentas.getColumnModel().getColumn(6)
				.setCellRenderer(new VentaPanel.BtnRenderer("Liquidar", Estilos.INDIGO_CLARO, Estilos.INDIGO));
		tablaCuentas.getColumnModel().getColumn(6).setCellEditor(new VentaPanel.BtnEditor(id -> {
			try {
				int cid = Integer.parseInt(id);
				CuentaPorPagar cuenta = ctrl.getCuentas().stream().filter(c -> c.getId() == cid).findFirst()
						.orElse(null);
				if (cuenta != null && cuenta.getSaldo() > 0) {
					String input = JOptionPane.showInputDialog(SwingUtilities.getWindowAncestor(this),
							"Monto a liquidar (saldo: $" + String.format("%.2f", cuenta.getSaldo()) + "):",
							String.format("%.2f", cuenta.getSaldo()));
					if (input != null) {
						double monto = Double.parseDouble(input.trim());
						ctrl.liquidarCuenta(cid, monto);
					}
				}
			} catch (Exception ignored) {
			}
		}));

		JScrollPane scroll = new JScrollPane(tablaCuentas);
		scroll.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		p.add(subtit, BorderLayout.NORTH);
		p.add(scroll, BorderLayout.CENTER);
		return p;
	}

	public void refrescarProveedores() {
		if (modeloProveedores == null)
			return;
		modeloProveedores.setRowCount(0);
		for (Proveedor p : ctrl.getProveedores())
			modeloProveedores.addRow(new Object[] { p.getId(), p.getNombre(), p.getContacto(), p.getTelefono(),
					p.getEmail(), p.getDireccion() });
	}

	public void refrescarOrdenes() {
		if (modeloOrdenes == null)
			return;
		modeloOrdenes.setRowCount(0);
		for (OrdenCompra o : ctrl.getOrdenes()) {
			if (o.getEstado() == OrdenCompra.Estado.PENDIENTE) {
				modeloOrdenes.addRow(new Object[] { o.getFolioCorto(), o.getProveedor().getNombre(),
						String.format("$%.2f", o.getTotal()), o.getTipoPago().name(), o.getEstado().name(),
						String.valueOf(o.getId()) });
			}
		}
	}

	public void refrescarCuentas() {
		if (modeloCuentas == null)
			return;
		modeloCuentas.setRowCount(0);
		for (CuentaPorPagar c : ctrl.getCuentas())
			modeloCuentas.addRow(new Object[] { c.getProveedor().getNombre(), c.getFolioOrden(),
					String.format("$%.2f", c.getMontoTotal()), String.format("$%.2f", c.getPagado()),
					String.format("$%.2f", c.getSaldo()), c.getEstado().name(), String.valueOf(c.getId()) });
	}

	private void abrirFormularioProveedor(Proveedor p) {
		ProveedorDialog dlg = new ProveedorDialog(SwingUtilities.getWindowAncestor(this), p, ctrl);
		dlg.setVisible(true);
		refrescarProveedores();
	}

	private void abrirNuevaOrden() {
		NuevaOrdenDialog dlg = new NuevaOrdenDialog(SwingUtilities.getWindowAncestor(this), ctrl);
		dlg.setVisible(true);
		refrescarOrdenes();
	}
}
