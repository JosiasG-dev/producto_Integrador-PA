package Vista;

import javax.swing.*;
import Controlador.InventarioControlador;
import Modelo.*;
import java.awt.*;

public class ProductoDialog extends JDialog {

	private final InventarioControlador ctrl;
	private Producto producto;

	private JTextField txtId, txtNombre, txtPrecio, txtStock, txtStockMin;
	private JComboBox<String> cmbCategoria, cmbUnidad;

	public ProductoDialog(Window parent, Producto p, InventarioControlador ctrl) {
		super(parent, p == null ? "Nuevo Producto" : "Editar Producto", ModalityType.APPLICATION_MODAL);
		this.ctrl = ctrl;
		this.producto = p;
		construir();
		if (p != null)
			cargar(p);
	}

	private void construir() {
		setSize(480, 500);
		setLocationRelativeTo(getParent());
		setResizable(false);

		JPanel panel = new JPanel(null);
		panel.setBackground(Estilos.BG_BLANCO);

		JLabel tit = new JLabel(producto == null ? "NUEVO PRODUCTO" : "EDITAR PRODUCTO");
		tit.setFont(Estilos.FUENTE_TITULO);
		tit.setBounds(28, 18, 420, 30);
		panel.add(tit);

		int x = 28, aw = 424, fh = 36;

		panel.add(lbl("SKU / Codigo", x, 60));
		txtId = campo(x, 76, aw, fh);
		txtId.setText(producto == null ? ctrl.generarNuevoId() : "");
		txtId.setEnabled(producto == null);
		panel.add(txtId);

		panel.add(lbl("Nombre del Producto", x, 124));
		txtNombre = campo(x, 140, aw, fh);
		panel.add(txtNombre);

		panel.add(lbl("Precio Publico ($)", x, 188));
		txtPrecio = campo(x, 204, 200, fh);
		txtPrecio.setText("0.00");
		panel.add(txtPrecio);

		panel.add(lbl("Existencia Actual", x + 210, 188));
		txtStock = campo(x + 210, 204, 214, fh);
		txtStock.setText("0");
		panel.add(txtStock);

		panel.add(lbl("Stock Minimo (limite de alerta)", x, 252));
		txtStockMin = campo(x, 268, 200, fh);
		txtStockMin.setText("5");
		panel.add(txtStockMin);

		panel.add(lbl("Categoria", x, 316));
		cmbCategoria = new JComboBox<>(DatosIniciales.getCategorias());
		cmbCategoria.setFont(Estilos.FUENTE_BOLD);
		cmbCategoria.setBounds(x, 332, 200, fh);
		panel.add(cmbCategoria);

		panel.add(lbl("Unidad de Medida", x + 210, 316));
		cmbUnidad = new JComboBox<>(DatosIniciales.getUnidades());
		cmbUnidad.setFont(Estilos.FUENTE_BOLD);
		cmbUnidad.setBounds(x + 210, 332, 214, fh);
		panel.add(cmbUnidad);

		if (producto != null) {
			JButton btnEliminar = Estilos.botonPeligro("Eliminar");
			btnEliminar.setBounds(28, 410, 100, 38);
			btnEliminar.addActionListener(e -> {
				int ok = JOptionPane.showConfirmDialog(this, "Eliminar producto " + producto.getNombre() + "?",
						"Confirmar", JOptionPane.YES_NO_OPTION);
				if (ok == JOptionPane.YES_OPTION) {
					ctrl.eliminarProducto(producto.getId());
					dispose();
				}
			});
			panel.add(btnEliminar);
		}

		JButton btnCancelar = Estilos.botonSecundario("Cancelar");
		btnCancelar.setBounds(240, 410, 100, 38);
		btnCancelar.addActionListener(e -> dispose());
		panel.add(btnCancelar);

		JButton btnGuardar = Estilos.botonPrimario("Guardar");
		btnGuardar.setBounds(350, 410, 102, 38);
		btnGuardar.addActionListener(e -> guardar());
		panel.add(btnGuardar);

		setContentPane(panel);
	}

	private void cargar(Producto p) {
		txtId.setText(p.getId());
		txtNombre.setText(p.getNombre());
		txtPrecio.setText(String.format("%.2f", p.getPrecio()));
		txtStock.setText(String.format("%.1f", p.getStock()));
		txtStockMin.setText(String.valueOf((int) p.getStockMinimo()));
		cmbCategoria.setSelectedItem(p.getCategoria());
		cmbUnidad.setSelectedItem(p.getUnidad());
	}

	private void guardar() {
		try {
			String id = txtId.getText().trim();
			String nom = txtNombre.getText().trim();
			double precio = Double.parseDouble(txtPrecio.getText().trim());
			double stock = Double.parseDouble(txtStock.getText().trim());
			double stockMin = Double.parseDouble(txtStockMin.getText().trim());
			String cat = (String) cmbCategoria.getSelectedItem();
			String unidad = (String) cmbUnidad.getSelectedItem();

			if (id.isBlank() || nom.isBlank()) {
				JOptionPane.showMessageDialog(this, "SKU y Nombre son obligatorios", "Error",
						JOptionPane.ERROR_MESSAGE);
				return;
			}
			if (precio < 0 || stock < 0 || stockMin < 0) {
				JOptionPane.showMessageDialog(this, "Los valores numericos no pueden ser negativos", "Error",
						JOptionPane.ERROR_MESSAGE);
				return;
			}

			Producto p = new Producto(id, nom, precio, stock, cat, unidad);
			p.setStockMinimo(stockMin);
			ctrl.guardarProducto(p);
			dispose();
		} catch (NumberFormatException ex) {
			JOptionPane.showMessageDialog(this, "Precio, Stock y Stock Minimo deben ser numeros validos", "Error",
					JOptionPane.ERROR_MESSAGE);
		}
	}

	private JLabel lbl(String texto, int x, int y) {
		JLabel l = new JLabel(texto.toUpperCase());
		l.setFont(Estilos.FUENTE_XS);
		l.setForeground(Estilos.TEXTO_TENUE);
		l.setBounds(x, y, 300, 14);
		return l;
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
