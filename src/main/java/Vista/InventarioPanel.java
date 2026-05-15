package Vista;

import javax.swing.*;
import javax.swing.table.*;
import Controlador.InventarioControlador;
import Modelo.*;
import java.awt.*;
import java.util.List;
import java.io.File;

public class InventarioPanel extends JPanel {

	private final InventarioControlador ctrl;
	private final Usuario usuarioActivo;

	private JTextField txtBusqueda;
	private JComboBox<String> cmbCategoria;
	private DefaultTableModel modeloTabla;
	private JTable tabla;

	public InventarioPanel(InventarioControlador ctrl, Usuario usuario) {
		this.ctrl = ctrl;
		this.usuarioActivo = usuario;
		setLayout(null);
		setBackground(Estilos.BG_CLARO);
		construir();
	}

	private void construir() {
		JLabel titulo = new JLabel("CATALOGO — Administracion de Stock");
		titulo.setFont(Estilos.FUENTE_TITULO);
		titulo.setForeground(Estilos.TEXTO_PRINCIPAL);
		titulo.setBounds(24, 16, 600, 30);
		add(titulo);

		JButton btnBajoStock = Estilos.botonSecundario("Ver Bajo Stock");
		btnBajoStock.setBounds(640, 14, 148, 36);
		btnBajoStock.addActionListener(e -> mostrarBajoStock());
		add(btnBajoStock);

		if (usuarioActivo.esAdmin()) {
			JButton btnNuevo = Estilos.botonPrimario("+ Nuevo Item");
			btnNuevo.setBounds(798, 14, 130, 36);
			btnNuevo.addActionListener(e -> abrirFormulario(null));
			add(btnNuevo);
		}

		JLabel lblBusq = new JLabel("BUSCAR:");
		lblBusq.setFont(Estilos.FUENTE_XS);
		lblBusq.setForeground(Estilos.TEXTO_TENUE);
		lblBusq.setBounds(24, 62, 60, 14);
		add(lblBusq);

		txtBusqueda = new JTextField();
		txtBusqueda.setFont(Estilos.FUENTE_BOLD);
		txtBusqueda.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(6, 10, 6, 10)));
		txtBusqueda.setBackground(Estilos.BG_BLANCO);
		txtBusqueda.setBounds(80, 56, 280, 34);
		txtBusqueda.getDocument().addDocumentListener(new javax.swing.event.DocumentListener() {
			public void insertUpdate(javax.swing.event.DocumentEvent e) { refrescar(); }
			public void removeUpdate(javax.swing.event.DocumentEvent e) { refrescar(); }
			public void changedUpdate(javax.swing.event.DocumentEvent e) { refrescar(); }
		});
		add(txtBusqueda);

		JLabel lblCat = new JLabel("CATEGORIA:");
		lblCat.setFont(Estilos.FUENTE_XS);
		lblCat.setForeground(Estilos.TEXTO_TENUE);
		lblCat.setBounds(376, 62, 80, 14);
		add(lblCat);

		String[] cats = DatosIniciales.getCategorias();
		String[] catConTodas = new String[cats.length + 1];
		catConTodas[0] = "Todas";
		System.arraycopy(cats, 0, catConTodas, 1, cats.length);
		cmbCategoria = new JComboBox<>(catConTodas);
		cmbCategoria.setFont(Estilos.FUENTE_BOLD);
		cmbCategoria.setBounds(454, 56, 200, 34);
		cmbCategoria.addActionListener(e -> refrescar());
		add(cmbCategoria);

		String[] cols = { "SKU", "Foto", "Producto", "Categoria", "Precio", "Existencia", "Stock Min", "Estado" };
		modeloTabla = new DefaultTableModel(cols, 0) {
			@Override
			public boolean isCellEditable(int r, int c) { return false; }
		};

		tabla = new JTable(modeloTabla);
		tabla.setFont(Estilos.FUENTE_NORMAL);
		tabla.setRowHeight(55); 
		
		tabla.getColumnModel().getColumn(1).setPreferredWidth(60);
		tabla.getColumnModel().getColumn(1).setCellRenderer(new ImagenRenderer());
		
		tabla.setShowGrid(false);
		tabla.setIntercellSpacing(new Dimension(0, 2));
		tabla.getTableHeader().setFont(Estilos.FUENTE_XS);
		tabla.getTableHeader().setBackground(Estilos.BG_ZINC_100);
		
		tabla.getColumnModel().getColumn(0).setMaxWidth(70);
		tabla.getColumnModel().getColumn(4).setMaxWidth(100);
		tabla.getColumnModel().getColumn(5).setMaxWidth(90);
		tabla.getColumnModel().getColumn(6).setMaxWidth(80);
		tabla.getColumnModel().getColumn(7).setMaxWidth(80);

		tabla.addMouseListener(new java.awt.event.MouseAdapter() {
			@Override
			public void mouseClicked(java.awt.event.MouseEvent e) {
				if (e.getClickCount() == 2 && usuarioActivo.esAdmin()) {
					int row = tabla.getSelectedRow();
					if (row >= 0) {
						String id = (String) modeloTabla.getValueAt(row, 0);
						abrirFormulario(ctrl.buscarPorId(id));
					}
				}
			}
		});

		JScrollPane scroll = new JScrollPane(tabla);
		scroll.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		scroll.getViewport().setBackground(Estilos.BG_BLANCO);
		scroll.setBounds(24, 104, 904, 560);
		add(scroll);

		refrescar();
	}

	public void refrescar() {
		String texto = txtBusqueda != null ? txtBusqueda.getText() : "";
		String cat = (cmbCategoria != null && cmbCategoria.getSelectedIndex() > 0)
				? (String) cmbCategoria.getSelectedItem() : "";
		
		List<Producto> lista = ctrl.filtrar(texto, cat);
		modeloTabla.setRowCount(0);

		for (Producto p : lista) {
			modeloTabla.addRow(new Object[] { 
				p.getId(),          
				p.getImagenRuta(),      
				p.getNombre().toUpperCase(), 
				p.getCategoria(),
				String.format("$%.2f", p.getPrecio()), 
				String.format("%.1f", p.getStock()),
				String.format("%.0f", p.getStockMinimo()), 
				p.stockBajo() ? "BAJO" : "OK" 
			});
		}
	}

	private void mostrarBajoStock() {
		List<Producto> todos = ctrl.filtrar("", "");
		StringBuilder sb = new StringBuilder("Reporte de Stock Bajo:\n\n");
		long cnt = 0;
		for (Producto p : todos) {
			if (p.stockBajo()) {
				sb.append(String.format("• %-20s (Cant: %.1f / Min: %.0f)\n", p.getNombre(), p.getStock(), p.getStockMinimo()));
				cnt++;
			}
		}
		if (cnt == 0) sb.append("Todo en orden, no hay stock bajo.");
		JOptionPane.showMessageDialog(this, sb.toString(), "Aviso de Inventario", JOptionPane.WARNING_MESSAGE);
	}

	private void abrirFormulario(Producto producto) {
		ProductoDialog dlg = new ProductoDialog(SwingUtilities.getWindowAncestor(this), producto, ctrl);
		dlg.setVisible(true);
		refrescar();
	}
	
	public static class ImagenRenderer extends DefaultTableCellRenderer {
		@Override
		public Component getTableCellRendererComponent(JTable table, Object value, boolean isSelected, boolean hasFocus, int row, int column) {
			JLabel lbl = new JLabel();
			lbl.setHorizontalAlignment(SwingConstants.CENTER);
			
			if (value != null && !value.toString().isEmpty()) {
				String ruta = value.toString();
				File archivo = new File(ruta);
				
				if (archivo.exists()) {
					try {
						ImageIcon icono = new ImageIcon(archivo.getAbsolutePath());
						Image img = icono.getImage().getScaledInstance(45, 45, Image.SCALE_SMOOTH);
						lbl.setIcon(new ImageIcon(img));
						lbl.setText("");
					} catch (Exception e) {
						lbl.setText("Error");
					}
				} else {
					lbl.setText("No encontrada");
				}
			} else {
				lbl.setText("Sin foto");
			}

			if (isSelected) {
				lbl.setBackground(table.getSelectionBackground());
				lbl.setOpaque(true);
			}
			return lbl;
		}
	}
}