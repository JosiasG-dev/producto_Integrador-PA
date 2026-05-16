package Vista;

import javax.swing.*;

import Controlador.*;
import Modelo.ConfiguracionTienda;
import Modelo.Usuario;

import java.awt.*;

public class VentanaPrincipal extends JFrame {

	private final AppControlador app;
	private final VentaControlador ventaCtrl;
	private final InventarioControlador invCtrl;
	private final CajaControlador cajaCtrl;
	private final ProveedorControlador provCtrl;
	private final UsuarioControlador usuCtrl;

	private JFrame frame;
	private JPanel contenido;
	private CardLayout cardLayout;
	private JLabel labelNombreTienda;

	private VentaPanel ventaPanel;
	private InventarioPanel inventarioPanel;
	private CajaPanel cajaPanel;
	private ProveedorPanel proveedorPanel;
	private UsuariosPanel usuariosPanel;
	private ReportesPanel reportesPanel;
	private ConfigPanel configPanel;

	private static final String TAB_VENTA = "venta";
	private static final String TAB_INVENTARIO = "inventario";
	private static final String TAB_PROVEEDORES = "proveedores";
	private static final String TAB_CAJA = "caja";
	private static final String TAB_REPORTES = "reportes";
	private static final String TAB_USUARIOS = "usuarios";
	private static final String TAB_CONFIG = "configuracion";

	public VentanaPrincipal(AppControlador app, VentaControlador ventaCtrl, InventarioControlador invCtrl,
			CajaControlador cajaCtrl, ProveedorControlador provCtrl, UsuarioControlador usuCtrl) {
		this.app = app;
		this.ventaCtrl = ventaCtrl;
		this.invCtrl = invCtrl;
		this.cajaCtrl = cajaCtrl;
		this.provCtrl = provCtrl;
		this.usuCtrl = usuCtrl;
		construir();
	}

	private void construir() {
		frame = new JFrame("");
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.setSize(1280, 800);
		frame.setLocationRelativeTo(null);

		JPanel root = new JPanel(new BorderLayout());
		root.setBackground(Estilos.BG_CLARO);

		JPanel sidebar = construirSidebar();
		root.add(sidebar, BorderLayout.WEST);

		cardLayout = new CardLayout();
		contenido = new JPanel(cardLayout);
		contenido.setBackground(Estilos.BG_CLARO);

		ventaPanel = new VentaPanel(ventaCtrl, app.getConfig(), app.getUsuarioActivo());
		inventarioPanel = new InventarioPanel(invCtrl, app.getUsuarioActivo());
		cajaPanel = new CajaPanel(cajaCtrl, app.getUsuarioActivo(), this);
		proveedorPanel = new ProveedorPanel(provCtrl, app.getUsuarioActivo());
		usuariosPanel = new UsuariosPanel(usuCtrl);
		reportesPanel = new ReportesPanel(app);
		configPanel = new ConfigPanel(app);

		ventaCtrl.setPanel(ventaPanel);
		invCtrl.setPanel(inventarioPanel);
		cajaCtrl.setPanel(cajaPanel);
		provCtrl.setPanel(proveedorPanel);
		usuCtrl.setPanel(usuariosPanel);

		contenido.add(ventaPanel, TAB_VENTA);
		contenido.add(inventarioPanel, TAB_INVENTARIO);
		contenido.add(cajaPanel, TAB_CAJA);
		contenido.add(proveedorPanel, TAB_PROVEEDORES);
		contenido.add(usuariosPanel, TAB_USUARIOS);
		contenido.add(reportesPanel, TAB_REPORTES);
		contenido.add(configPanel, TAB_CONFIG);

		root.add(contenido, BorderLayout.CENTER);
		frame.setContentPane(root);
		actualizarTitulo();
	}

	private JPanel construirSidebar() {
		JPanel sidebar = new JPanel();
		sidebar.setLayout(new BoxLayout(sidebar, BoxLayout.Y_AXIS));
		sidebar.setBackground(Estilos.BG_OSCURO);
		sidebar.setPreferredSize(new Dimension(220, 0));
		sidebar.setBorder(BorderFactory.createEmptyBorder(0, 0, 0, 0));

		JPanel logo = new JPanel(new FlowLayout(FlowLayout.LEFT, 16, 20));
		logo.setBackground(Estilos.BG_OSCURO);
		logo.setMaximumSize(new Dimension(Integer.MAX_VALUE, 80));

		labelNombreTienda = new JLabel(" " + app.getConfig().getNombre());
		labelNombreTienda.setFont(Estilos.FUENTE_BOLD);
		labelNombreTienda.setForeground(Color.WHITE);
		logo.add(labelNombreTienda);
		sidebar.add(logo);

		JSeparator sep = new JSeparator();
		sep.setForeground(new Color(63, 63, 70));
		sep.setMaximumSize(new Dimension(Integer.MAX_VALUE, 1));
		sidebar.add(sep);
		sidebar.add(Box.createVerticalStrut(12));

		sidebar.add(itemMenu("Venta", TAB_VENTA));
		sidebar.add(itemMenu("Inventario", TAB_INVENTARIO));
		sidebar.add(itemMenu("Proveedores", TAB_PROVEEDORES));
		sidebar.add(itemMenu("Flujo de Caja", TAB_CAJA));
		sidebar.add(itemMenu("Reportes", TAB_REPORTES));

		if (app.getUsuarioActivo().esAdmin()) {
			sidebar.add(Box.createVerticalStrut(16));
			JLabel secLabel = new JLabel("CONFIGURACIÓN");
			secLabel.setFont(Estilos.FUENTE_XS);
			secLabel.setForeground(new Color(63, 63, 70));
			secLabel.setAlignmentX(Component.LEFT_ALIGNMENT);
			sidebar.add(secLabel);
			sidebar.add(itemMenu("Personal", TAB_USUARIOS));
			sidebar.add(itemMenu("Sistema", TAB_CONFIG));
		}

		sidebar.add(Box.createVerticalGlue());

		JPanel footer = new JPanel();
		footer.setLayout(new BoxLayout(footer, BoxLayout.Y_AXIS));
		footer.setBackground(Estilos.BG_MUY_OSCURO);
		footer.setBorder(BorderFactory.createEmptyBorder(14, 14, 14, 14));
		footer.setMaximumSize(new Dimension(Integer.MAX_VALUE, 120));

		Usuario u = app.getUsuarioActivo();
		JLabel nombreLbl = new JLabel(u.getNombre().charAt(0) + "  " + u.getNombre());
		nombreLbl.setFont(Estilos.FUENTE_BOLD);
		nombreLbl.setForeground(Color.WHITE);
		nombreLbl.setAlignmentX(Component.LEFT_ALIGNMENT);

		JLabel rolLbl = new JLabel(u.getRol());
		rolLbl.setFont(Estilos.FUENTE_XS);
		rolLbl.setForeground(Estilos.INDIGO);
		rolLbl.setAlignmentX(Component.LEFT_ALIGNMENT);

		JButton btnSalir = new JButton("Cerrar Sesión");
		btnSalir.setFont(Estilos.FUENTE_XS);
		btnSalir.setForeground(Estilos.ROSE);
		btnSalir.setBackground(Estilos.BG_MUY_OSCURO);
		btnSalir.setBorderPainted(false);
		btnSalir.setContentAreaFilled(false);
		btnSalir.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
		btnSalir.setAlignmentX(Component.LEFT_ALIGNMENT);
		btnSalir.addActionListener(e -> app.onCerrarSesion());

		footer.add(nombreLbl);
		footer.add(Box.createVerticalStrut(4));
		footer.add(rolLbl);
		footer.add(Box.createVerticalStrut(10));
		footer.add(btnSalir);

		sidebar.add(footer);
		return sidebar;
	}

	private JButton itemMenu(String label, String tabId) {
		JButton btn = new JButton(label);
		btn.setFont(Estilos.FUENTE_XS);
		btn.setForeground(new Color(161, 161, 170));
		btn.setBackground(Estilos.BG_OSCURO);
		btn.setBorderPainted(false);
		btn.setContentAreaFilled(true);
		btn.setHorizontalAlignment(SwingConstants.LEFT);
		btn.setBorder(BorderFactory.createEmptyBorder(12, 20, 12, 20));
		btn.setMaximumSize(new Dimension(Integer.MAX_VALUE, 46));
		btn.setCursor(Cursor.getPredefinedCursor(Cursor.HAND_CURSOR));
		btn.addActionListener(e -> cambiarTab(tabId));
		btn.addMouseListener(new java.awt.event.MouseAdapter() {
			public void mouseEntered(java.awt.event.MouseEvent e) {
				btn.setBackground(new Color(39, 39, 42));
				btn.setForeground(Color.WHITE);
			}

			public void mouseExited(java.awt.event.MouseEvent e) {
				btn.setBackground(Estilos.BG_OSCURO);
				btn.setForeground(new Color(161, 161, 170));
			}
		});
		return btn;
	}

	private void cambiarTab(String tab) {
		cardLayout.show(contenido, tab);
		switch (tab) {
		case TAB_INVENTARIO -> inventarioPanel.refrescar();
		case TAB_CAJA -> cajaPanel.refrescar();
		case TAB_PROVEEDORES -> {
			proveedorPanel.refrescarProveedores();
			proveedorPanel.refrescarOrdenes();
			proveedorPanel.refrescarCuentas();
		}
		case TAB_USUARIOS -> usuariosPanel.refrescar();
		case TAB_REPORTES -> reportesPanel.refrescar();
		}
	}

	public void actualizarTitulo() {
		ConfiguracionTienda config = app.getConfig();

		if (labelNombreTienda != null) {
			labelNombreTienda.setText("🏪 " + config.getNombre());
		}

		if (frame != null) {
			frame.setTitle(config.getNombre() + " - Sistema de Punto de Venta");
		}
	}

	public void refrescarInventario() {
		inventarioPanel.refrescar();
	}

	public void refrescarCaja() {
		if (cajaCtrl.isCajaAbierta())
			cajaPanel.refrescar();
	}

	public void mostrar() {
		frame.setVisible(true);
	}

	public void ocultar() {
		frame.setVisible(false);
	}

	public JFrame getFrame() {
		return frame;
	}
}