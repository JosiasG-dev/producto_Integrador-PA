package Vista;

import javax.swing.*;
import Controlador.InventarioControlador;
import Modelo.Producto;
import java.awt.*;

public class EntradaRapidaDialog extends JDialog {

	private final InventarioControlador ctrl;
	private final Producto producto;

	public EntradaRapidaDialog(Window parent, Producto p, InventarioControlador ctrl) {
		super(parent, "Entrada Rapida de Mercancia", ModalityType.APPLICATION_MODAL);
		this.ctrl = ctrl;
		this.producto = p;
		construir();
	}

	private void construir() {
		setSize(380, 300);
		setLocationRelativeTo(getParent());
		setResizable(false);

		JPanel panel = new JPanel(null);
		panel.setBackground(Estilos.BG_BLANCO);

		JLabel tit = new JLabel("INGRESO DIRECTO");
		tit.setFont(Estilos.FUENTE_TITULO);
		tit.setBounds(24, 18, 300, 30);
		panel.add(tit);

		JLabel lblProd = new JLabel("Producto: " + producto.getNombre());
		lblProd.setFont(Estilos.FUENTE_BOLD);
		lblProd.setBounds(24, 60, 330, 20);
		panel.add(lblProd);

		JLabel lblStock = new JLabel("Stock actual: " + producto.getStock() + " " + producto.getUnidad());
		lblStock.setFont(Estilos.FUENTE_NORMAL);
		lblStock.setForeground(Estilos.TEXTO_SECUNDARIO);
		lblStock.setBounds(24, 85, 330, 20);
		panel.add(lblStock);

		JLabel lblCant = new JLabel("CANTIDAD A INGRESAR:");
		lblCant.setFont(Estilos.FUENTE_XS);
		lblCant.setForeground(Estilos.TEXTO_TENUE);
		lblCant.setBounds(24, 130, 300, 14);
		panel.add(lblCant);

		JTextField txtCantidad = new JTextField();
		txtCantidad.setFont(Estilos.FUENTE_BOLD);
		txtCantidad.setBackground(Estilos.BG_ZINC_100);
		txtCantidad.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(6, 10, 6, 10)));
		txtCantidad.setBounds(24, 148, 315, 36);
		panel.add(txtCantidad);

		JButton btnGuardar = Estilos.botonPrimario("Ingresar");
		btnGuardar.setBounds(230, 210, 110, 38);
		btnGuardar.addActionListener(e -> {
			try {
				double cantidad = Double.parseDouble(txtCantidad.getText().trim());
				if (cantidad <= 0) {
					JOptionPane.showMessageDialog(this, "Ingrese una cantidad valida mayor a 0", "Aviso", JOptionPane.WARNING_MESSAGE);
					return;
				}
				producto.setStock(producto.getStock() + cantidad);
				ctrl.guardarProducto(producto);
				ctrl.getApp().registrarRetiro("ingreso directo " + producto.getNombre(), cantidad * producto.getPrecio());
				JOptionPane.showMessageDialog(this, "Inventario actualizado exitosamente");
				dispose();
			} catch (NumberFormatException ex) {
				JOptionPane.showMessageDialog(this, "Numero invalido", "Error", JOptionPane.ERROR_MESSAGE);
			}
		});
		panel.add(btnGuardar);

		JButton btnCancelar = Estilos.botonSecundario("Cancelar");
		btnCancelar.setBounds(110, 210, 110, 38);
		btnCancelar.addActionListener(e -> dispose());
		panel.add(btnCancelar);

		setContentPane(panel);
	}
}
