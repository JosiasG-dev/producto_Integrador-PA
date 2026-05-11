package Vista;

import javax.swing.*;
import javax.swing.table.*;
import Controlador.AppControlador;
import Modelo.*;
import java.awt.*;
import java.util.Date;
import java.util.List;

public class DevolucionDialog extends JDialog {

	private final AppControlador app;
	private JTextField txtVentaId;
	private DefaultTableModel modeloItems;
	private JTable tablaItems;
	private JTextField txtMotivo;
	private List<ItemCarrito> itemsVenta;
	private int ventaId = -1;

	public DevolucionDialog(JFrame parent, AppControlador app) {
		super(parent, "Devolucion de Productos", true);
		this.app = app;
		construir();
	}

	private void construir() {
		setSize(560, 540);
		setLocationRelativeTo(getParent());
		setResizable(false);

		JPanel panel = new JPanel(null);
		panel.setBackground(Estilos.BG_BLANCO);

		JLabel tit = new JLabel("DEVOLUCION DE PRODUCTOS");
		tit.setFont(Estilos.FUENTE_TITULO);
		tit.setForeground(Estilos.TEXTO_PRINCIPAL);
		tit.setBounds(24, 16, 500, 30);
		panel.add(tit);

		JLabel lblVenta = new JLabel("NUMERO DE VENTA (#):");
		lblVenta.setFont(Estilos.FUENTE_XS);
		lblVenta.setForeground(Estilos.TEXTO_TENUE);
		lblVenta.setBounds(24, 58, 200, 14);
		panel.add(lblVenta);

		txtVentaId = new JTextField();
		txtVentaId.setFont(Estilos.FUENTE_BOLD);
		txtVentaId.setBackground(Estilos.BG_ZINC_100);
		txtVentaId.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(6, 10, 6, 10)));
		txtVentaId.setBounds(24, 76, 180, 36);
		panel.add(txtVentaId);

		JButton btnBuscar = Estilos.botonPrimario("Buscar Venta");
		btnBuscar.setBounds(214, 76, 130, 36);
		btnBuscar.addActionListener(e -> buscarVenta());
		panel.add(btnBuscar);

		JLabel lblItems = new JLabel("PRODUCTOS DE LA VENTA (doble clic para devolver):");
		lblItems.setFont(Estilos.FUENTE_XS);
		lblItems.setForeground(Estilos.TEXTO_TENUE);
		lblItems.setBounds(24, 126, 500, 14);
		panel.add(lblItems);

		String[] cols = { "SKU", "Producto", "Cant Vendida", "Precio", "A Devolver" };
		modeloItems = new DefaultTableModel(cols, 0) {
			@Override
			public boolean isCellEditable(int r, int c) {
				return c == 4;
			}
		};
		tablaItems = new JTable(modeloItems);
		tablaItems.setFont(Estilos.FUENTE_NORMAL);
		tablaItems.setRowHeight(38);
		tablaItems.setShowGrid(false);
		tablaItems.getTableHeader().setFont(Estilos.FUENTE_XS);
		tablaItems.getTableHeader().setBackground(Estilos.BG_ZINC_100);
		tablaItems.getColumnModel().getColumn(0).setMaxWidth(60);
		tablaItems.getColumnModel().getColumn(3).setMaxWidth(90);
		tablaItems.getColumnModel().getColumn(4).setMaxWidth(90);

		JScrollPane scroll = new JScrollPane(tablaItems);
		scroll.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		scroll.getViewport().setBackground(Estilos.BG_BLANCO);
		scroll.setBounds(24, 144, 510, 220);
		panel.add(scroll);

		JLabel lblMotivo = new JLabel("MOTIVO DE DEVOLUCION:");
		lblMotivo.setFont(Estilos.FUENTE_XS);
		lblMotivo.setForeground(Estilos.TEXTO_TENUE);
		lblMotivo.setBounds(24, 378, 300, 14);
		panel.add(lblMotivo);

		txtMotivo = new JTextField();
		txtMotivo.setFont(Estilos.FUENTE_BOLD);
		txtMotivo.setBackground(Estilos.BG_ZINC_100);
		txtMotivo.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(6, 10, 6, 10)));
		txtMotivo.setBounds(24, 396, 510, 36);
		panel.add(txtMotivo);

		JButton btnCancelar = Estilos.botonSecundario("Cancelar");
		btnCancelar.setBounds(300, 452, 110, 38);
		btnCancelar.addActionListener(e -> dispose());
		panel.add(btnCancelar);

		JButton btnRegistrar = Estilos.botonPeligro("Registrar Devolucion");
		btnRegistrar.setBounds(420, 452, 114, 38);
		btnRegistrar.addActionListener(e -> registrarDevolucion());
		panel.add(btnRegistrar);

		setContentPane(panel);
	}

	private void buscarVenta() {
		String txt = txtVentaId.getText().trim();
		if (txt.isBlank())
			return;
		try {
			ventaId = Integer.parseInt(txt);
		} catch (NumberFormatException ex) {
			JOptionPane.showMessageDialog(this, "Ingresa un numero de venta valido", "Error",
					JOptionPane.ERROR_MESSAGE);
			return;
		}
		List<Venta> ventas = app.getVentas();
		Venta venta = ventas.stream().filter(v -> v.getId() == ventaId).findFirst().orElse(null);
		if (venta == null) {
			JOptionPane.showMessageDialog(this, "No se encontro la venta #" + ventaId, "Sin resultados",
					JOptionPane.WARNING_MESSAGE);
			ventaId = -1;
			return;
		}
		itemsVenta = venta.getItems();
		modeloItems.setRowCount(0);
		for (ItemCarrito item : itemsVenta) {
			modeloItems.addRow(new Object[] { item.getProducto().getId(), item.getProducto().getNombre(),
					String.format("%.2f", item.getCantidad()), String.format("$%.2f", item.getProducto().getPrecio()),
					"0" });
		}
	}

	private void registrarDevolucion() {
		if (ventaId < 0 || itemsVenta == null) {
			JOptionPane.showMessageDialog(this, "Primero busca una venta", "Aviso", JOptionPane.WARNING_MESSAGE);
			return;
		}
		String motivo = txtMotivo.getText().trim();
		if (motivo.isBlank()) {
			JOptionPane.showMessageDialog(this, "Ingresa el motivo de la devolucion", "Aviso",
					JOptionPane.WARNING_MESSAGE);
			return;
		}
		boolean alguna = false;
		for (int i = 0; i < modeloItems.getRowCount(); i++) {
			try {
				double cantDevolver = Double.parseDouble(modeloItems.getValueAt(i, 4).toString().trim());
				if (cantDevolver <= 0)
					continue;
				ItemCarrito item = itemsVenta.get(i);
				double cantVendida = item.getCantidad();
				if (cantDevolver > cantVendida) {
					JOptionPane.showMessageDialog(this,
							"No puedes devolver mas de lo vendido para: " + item.getProducto().getNombre(), "Error",
							JOptionPane.ERROR_MESSAGE);
					return;
				}
				Devolucion d = new Devolucion(0, ventaId, item.getProducto(), cantDevolver, motivo, new Date(),
						app.getUsuarioActivo().getNombre());
				app.registrarDevolucion(d);
				alguna = true;
			} catch (NumberFormatException ignored) {
			}
		}
		if (alguna) {
			JOptionPane.showMessageDialog(this, "Devolucion registrada. El stock fue actualizado.", "Exito",
					JOptionPane.INFORMATION_MESSAGE);
			dispose();
		} else {
			JOptionPane.showMessageDialog(this, "Ingresa al menos una cantidad mayor a 0 en 'A Devolver'",
					"Sin cantidades", JOptionPane.WARNING_MESSAGE);
		}
	}
}
