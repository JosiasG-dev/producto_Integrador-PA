package Vista;

import javax.swing.*;
import javax.swing.table.DefaultTableModel;
import Controlador.*;
import Modelo.*;
import java.awt.*;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class NuevaOrdenDialog extends JDialog {

    private final ProveedorControlador ctrl;
    private final List<OrdenCompra.ItemOrden> items = new ArrayList<>();
    private JComboBox<Proveedor> cmbProveedor;
    private JComboBox<String> cmbTipoPago;
    private JTextField txtBusquedaProd;
    private DefaultTableModel modeloItems;
    private JTable tablaItems;
    private JLabel lblTotal;

    public NuevaOrdenDialog(Window parent, ProveedorControlador ctrl) {
        super(parent, "Nueva Orden de Compra", ModalityType.APPLICATION_MODAL);
        this.ctrl = ctrl;
        construir();
    }

    private void construir() {
        setSize(740, 640);
        setLocationRelativeTo(getParent());
        setResizable(true);

        JPanel panel = new JPanel(new BorderLayout());
        panel.setBackground(Estilos.BG_BLANCO);

        JPanel header = new JPanel(new BorderLayout());
        header.setBackground(Estilos.BG_ZINC_100);
        header.setBorder(BorderFactory.createCompoundBorder(
            BorderFactory.createMatteBorder(0, 0, 1, 0, Estilos.BORDE),
            BorderFactory.createEmptyBorder(20, 24, 20, 24)));
        JLabel tit = new JLabel("NUEVA ORDEN DE COMPRA");
        tit.setFont(Estilos.FUENTE_TITULO);
        header.add(tit, BorderLayout.WEST);

        JPanel form = new JPanel(new GridLayout(1, 2, 24, 0));
        form.setBackground(Estilos.BG_BLANCO);
        form.setBorder(BorderFactory.createEmptyBorder(20, 24, 12, 24));

        JPanel izq = new JPanel();
        izq.setLayout(new BoxLayout(izq, BoxLayout.Y_AXIS));
        izq.setBackground(Estilos.BG_BLANCO);
        izq.add(lbl("1. Proveedor Responsable"));
        cmbProveedor = new JComboBox<>(ctrl.getProveedores().toArray(new Proveedor[0]));
        cmbProveedor.setFont(Estilos.FUENTE_BOLD);
        cmbProveedor.setMaximumSize(new Dimension(Integer.MAX_VALUE, 42));
        cmbProveedor.setAlignmentX(Component.LEFT_ALIGNMENT);
        izq.add(cmbProveedor);
        izq.add(Box.createVerticalStrut(14));
        izq.add(lbl("2. Terminos de Pago"));
        cmbTipoPago = new JComboBox<>(new String[]{"CONTADO", "CREDITO"});
        cmbTipoPago.setFont(Estilos.FUENTE_BOLD);
        cmbTipoPago.setMaximumSize(new Dimension(Integer.MAX_VALUE, 42));
        cmbTipoPago.setAlignmentX(Component.LEFT_ALIGNMENT);
        izq.add(cmbTipoPago);

        JPanel der = new JPanel();
        der.setLayout(new BoxLayout(der, BoxLayout.Y_AXIS));
        der.setBackground(Estilos.BG_BLANCO);
        der.add(lbl("3. Buscar y Agregar Productos"));
        txtBusquedaProd = new JTextField();
        txtBusquedaProd.setFont(Estilos.FUENTE_BOLD);
        txtBusquedaProd.setBackground(Estilos.BG_ZINC_100);
        txtBusquedaProd.setBorder(BorderFactory.createCompoundBorder(
            BorderFactory.createLineBorder(Estilos.BORDE, 2),
            BorderFactory.createEmptyBorder(10, 12, 10, 12)));
        txtBusquedaProd.setMaximumSize(new Dimension(Integer.MAX_VALUE, 42));
        txtBusquedaProd.setAlignmentX(Component.LEFT_ALIGNMENT);
        der.add(txtBusquedaProd);
        der.add(Box.createVerticalStrut(8));
        JButton btnAgregar = Estilos.botonExito("Agregar");
        btnAgregar.setAlignmentX(Component.LEFT_ALIGNMENT);
        btnAgregar.setMaximumSize(new Dimension(120, 36));
        btnAgregar.addActionListener(e -> buscarYAgregar());
        der.add(btnAgregar);

        form.add(izq);
        form.add(der);

        String[] cols = {"Producto", "Unidad", "Cant.", "Costo Unit.", "Subtotal", "Quitar"};
        modeloItems = new DefaultTableModel(cols, 0) {
            @Override public boolean isCellEditable(int r, int c) { return c == 2 || c == 5; }
        };
        tablaItems = new JTable(modeloItems);
        tablaItems.setFont(Estilos.FUENTE_NORMAL);
        tablaItems.setRowHeight(40);
        tablaItems.setShowGrid(false);
        tablaItems.getTableHeader().setFont(Estilos.FUENTE_XS);
        tablaItems.getTableHeader().setBackground(Estilos.BG_ZINC_100);
        tablaItems.getColumnModel().getColumn(1).setMaxWidth(120);
        tablaItems.getColumnModel().getColumn(2).setMaxWidth(70);
        tablaItems.getColumnModel().getColumn(3).setMaxWidth(110);
        tablaItems.getColumnModel().getColumn(4).setMaxWidth(110);
        tablaItems.getColumnModel().getColumn(5).setMaxWidth(90);

        tablaItems.addMouseListener(new java.awt.event.MouseAdapter() {
            @Override public void mouseClicked(java.awt.event.MouseEvent e) {
                int col = tablaItems.columnAtPoint(e.getPoint());
                int row = tablaItems.rowAtPoint(e.getPoint());
                if (e.getClickCount() == 2 && col == 2 && row >= 0) {
                    String actual = String.valueOf(modeloItems.getValueAt(row, 2));
                    String entrada = JOptionPane.showInputDialog(NuevaOrdenDialog.this, "Nueva cantidad:", actual);
                    if (entrada == null) return;
                    try {
                        double nueva = Double.parseDouble(entrada.trim());
                        if (nueva <= 0) return;
                        OrdenCompra.ItemOrden item = items.get(row);
                        if (item.getProducto().esPorPieza()) nueva = Math.round(nueva);
                        item.setCantidadSolicitada(nueva);
                        actualizarTablaItems();
                    } catch (NumberFormatException ignored) {}
                }
            }
        });

        tablaItems.getColumnModel().getColumn(5).setCellRenderer(new VentaPanel.BtnRenderer());
        tablaItems.getColumnModel().getColumn(5).setCellEditor(new VentaPanel.BtnEditor(idx -> {
            try {
                int i = Integer.parseInt(idx);
                if (i >= 0 && i < items.size()) {
                    items.remove(i);
                    actualizarTablaItems();
                }
            } catch (Exception ignored) {}
        }));

        JScrollPane scroll = new JScrollPane(tablaItems);
        scroll.setBorder(BorderFactory.createMatteBorder(1, 0, 1, 0, Estilos.BORDE));

        JPanel footer = new JPanel(new BorderLayout());
        footer.setBackground(Estilos.BG_ZINC_100);
        footer.setBorder(BorderFactory.createCompoundBorder(
            BorderFactory.createMatteBorder(1, 0, 0, 0, Estilos.BORDE),
            BorderFactory.createEmptyBorder(16, 24, 16, 24)));
        lblTotal = new JLabel("Inversion Total:  $0.00");
        lblTotal.setFont(new Font("SansSerif", Font.BOLD, 26));
        lblTotal.setForeground(Estilos.TEXTO_PRINCIPAL);

        JPanel btns = new JPanel(new FlowLayout(FlowLayout.RIGHT, 10, 0));
        btns.setBackground(Estilos.BG_ZINC_100);
        JButton btnCancelar = Estilos.botonSecundario("Cancelar");
        btnCancelar.setPreferredSize(new Dimension(110, 44));
        btnCancelar.addActionListener(e -> dispose());
        JButton btnConfirmar = Estilos.botonPrimario("Confirmar Orden");
        btnConfirmar.setPreferredSize(new Dimension(180, 44));
        btnConfirmar.addActionListener(e -> confirmarOrden());
        btns.add(btnCancelar);
        btns.add(btnConfirmar);
        footer.add(lblTotal, BorderLayout.WEST);
        footer.add(btns, BorderLayout.EAST);

        JPanel centro = new JPanel(new BorderLayout());
        centro.setBackground(Estilos.BG_BLANCO);
        centro.add(form, BorderLayout.NORTH);
        centro.add(scroll, BorderLayout.CENTER);
        panel.add(header, BorderLayout.NORTH);
        panel.add(centro, BorderLayout.CENTER);
        panel.add(footer, BorderLayout.SOUTH);
        setContentPane(panel);
    }

    private void buscarYAgregar() {
        String q = txtBusquedaProd.getText().trim();
        if (q.isBlank()) return;
        List<Producto> res = new ArrayList<>();
        for (Producto p : ctrl.getProductos()) {
            if (p.getNombre().toLowerCase().contains(q.toLowerCase())) res.add(p);
            if (res.size() >= 5) break;
        }
        if (res.isEmpty()) {
            JOptionPane.showMessageDialog(this, "No se encontraron productos.", "Aviso", JOptionPane.INFORMATION_MESSAGE);
            return;
        }
        Producto sel = (Producto) JOptionPane.showInputDialog(this, "Seleccione un producto:", "Resultados",
            JOptionPane.PLAIN_MESSAGE, null, res.toArray(), res.get(0));
        if (sel == null) return;

        String unidad = sel.getUnidad();
        String mensajeCant = sel.esPorPieza()
            ? "Cantidad a pedir (Piezas):"
            : "Cantidad a pedir (" + unidad + "):";
        String entradaCant = JOptionPane.showInputDialog(this, mensajeCant, sel.esPorPieza() ? "1" : "1.0");
        if (entradaCant == null) return;
        double cantidad;
        try {
            cantidad = Double.parseDouble(entradaCant.trim());
            if (cantidad <= 0) return;
            if (sel.esPorPieza()) cantidad = Math.round(cantidad);
        } catch (NumberFormatException ex) {
            JOptionPane.showMessageDialog(this, "Cantidad invalida", "Error", JOptionPane.ERROR_MESSAGE);
            return;
        }

        for (OrdenCompra.ItemOrden item : items) {
            if (item.getProducto().getId().equals(sel.getId())) {
                item.setCantidadSolicitada(item.getCantidadSolicitada() + cantidad);
                actualizarTablaItems();
                txtBusquedaProd.setText("");
                return;
            }
        }
        items.add(new OrdenCompra.ItemOrden(sel, cantidad, sel.getPrecio() * 0.75));
        actualizarTablaItems();
        txtBusquedaProd.setText("");
    }

    private void actualizarTablaItems() {
        modeloItems.setRowCount(0);
        double total = 0;
        for (int i = 0; i < items.size(); i++) {
            OrdenCompra.ItemOrden item = items.get(i);
            Producto p = item.getProducto();
            String cantStr = p.esPorPieza()
                ? String.format("%.0f", item.getCantidadSolicitada())
                : String.format("%.2f", item.getCantidadSolicitada());
            modeloItems.addRow(new Object[]{
                p.getNombre().toUpperCase(),
                p.getUnidad(),
                cantStr,
                String.format("$%.2f", item.getPrecioCosto()),
                String.format("$%.2f", item.getSubtotal()),
                String.valueOf(i)
            });
            total += item.getSubtotal();
        }
        lblTotal.setText(String.format("Inversion Total:  $%.2f", total));
    }

    private void confirmarOrden() {
        if (items.isEmpty()) {
            JOptionPane.showMessageDialog(this, "Agregue al menos un producto.", "Aviso", JOptionPane.WARNING_MESSAGE);
            return;
        }
        Proveedor prov = (Proveedor) cmbProveedor.getSelectedItem();
        OrdenCompra.TipoPago tipoPago = "CREDITO".equals(cmbTipoPago.getSelectedItem())
            ? OrdenCompra.TipoPago.CREDITO : OrdenCompra.TipoPago.CONTADO;
        double total = items.stream().mapToDouble(OrdenCompra.ItemOrden::getSubtotal).sum();
        OrdenCompra orden = new OrdenCompra(0, prov, new ArrayList<>(items), total, tipoPago,
            OrdenCompra.Estado.PENDIENTE, new Date());
        ctrl.crearOrden(orden);
        JOptionPane.showMessageDialog(this, "Orden " + orden.getFolioCorto() + " generada correctamente.",
            "Exito", JOptionPane.INFORMATION_MESSAGE);
        dispose();
    }

    private JLabel lbl(String t) {
        JLabel l = new JLabel(t.toUpperCase());
        l.setFont(Estilos.FUENTE_XS);
        l.setForeground(Estilos.TEXTO_TENUE);
        l.setAlignmentX(Component.LEFT_ALIGNMENT);
        return l;
    }
}