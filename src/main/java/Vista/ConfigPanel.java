package Vista;

import javax.swing.*;

import Controlador.AppControlador;
import Modelo.ConfiguracionTienda;

import java.awt.*;

public class ConfigPanel extends JPanel {

	private final AppControlador app;
	private JTextField txtNombre, txtSucursal, txtRFC;

	public ConfigPanel(AppControlador app) {
		this.app = app;
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
		JLabel tit = new JLabel("⚙️  SISTEMA  —  Identidad Fiscal");
		tit.setFont(Estilos.FUENTE_TITULO);
		header.add(tit, BorderLayout.WEST);

		JPanel form = new JPanel();
		form.setLayout(new BoxLayout(form, BoxLayout.Y_AXIS));
		form.setBackground(Estilos.BG_BLANCO);
		form.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE),
				BorderFactory.createEmptyBorder(32, 36, 32, 36)));

		ConfiguracionTienda cfg = app.getConfig();

		form.add(lbl("Razón Social / Nombre de la Tienda"));
		txtNombre = campo(cfg.getNombre(), 36);
		form.add(txtNombre);
		form.add(Box.createVerticalStrut(16));

		form.add(lbl("Sucursal"));
		txtSucursal = campo(cfg.getSucursal(), 18);
		form.add(txtSucursal);
		form.add(Box.createVerticalStrut(16));

		form.add(lbl("RFC"));
		txtRFC = campo(cfg.getRfc(), 16);
		form.add(txtRFC);
		form.add(Box.createVerticalStrut(32));

		JButton btnGuardar = Estilos.botonPrimario("✓  ACTUALIZAR CONFIGURACIÓN");
		btnGuardar.setFont(new Font("SansSerif", Font.BOLD, 16));
		btnGuardar.setMaximumSize(new Dimension(Integer.MAX_VALUE, 60));
		btnGuardar.setAlignmentX(Component.LEFT_ALIGNMENT);
		btnGuardar.addActionListener(e -> guardar());
		form.add(btnGuardar);
		
		form.add(Box.createVerticalStrut(20));
		
		JButton btnReiniciar = Estilos.botonPeligro("REINICIAR SISTEMA");
		btnReiniciar.setFont(new Font("SansSerif", Font.BOLD, 16));
		btnReiniciar.setMaximumSize(new Dimension(Integer.MAX_VALUE, 60));
		btnReiniciar.setAlignmentX(Component.LEFT_ALIGNMENT);
		btnReiniciar.addActionListener(e -> {
		    int confirm1 = JOptionPane.showConfirmDialog(this, 
		        "¿estas seguro? se borraran todas las ventas y movimientos del sistema", 
		        "ADVERTENCIA 1 DE 3", JOptionPane.YES_NO_OPTION, JOptionPane.WARNING_MESSAGE);
		        
		    if (confirm1 == JOptionPane.YES_OPTION) {
		        int confirm2 = JOptionPane.showConfirmDialog(this, 
		            "esta accion no se puede deshacer; perderas todo el historial financiero, ¿continuar?", 
		            "ADVERTENCIA 2 DE 3", JOptionPane.YES_NO_OPTION, JOptionPane.ERROR_MESSAGE);
		            
		        if (confirm2 == JOptionPane.YES_OPTION) {
		            String pass = JOptionPane.showInputDialog(this, 
		                "para confirmar escribe la palabra: ELIMINAR TODO");
		                
		            if (pass != null && pass.equalsIgnoreCase("ELIMINAR TODO")) {
		                app.reiniciarSistemaCompleto();
		            } else {
		                JOptionPane.showMessageDialog(this, "reinicio cancelado, la palabra no coincide");
		            }
		        }
		    }
		});
		form.add(btnReiniciar);

		add(header, BorderLayout.NORTH);
		
		JScrollPane scroll = new JScrollPane(form);
		scroll.setBorder(null);
		add(scroll, BorderLayout.CENTER);
	}

	private void guardar() {
	    String nuevoNombre = txtNombre.getText();
	    String nuevaSucursal = txtSucursal.getText();
	    String nuevoRFC = txtRFC.getText();
	    
	    if (nuevoNombre.isEmpty() || nuevaSucursal.isEmpty() || nuevoRFC.isEmpty()) {
	        JOptionPane.showMessageDialog(this, "por favor llena todos los campos");
	        return;
	    }
	    ConfiguracionTienda nuevaConfig = new ConfiguracionTienda(nuevoNombre, nuevaSucursal, nuevoRFC);
	    app.guardarConfiguracion(nuevaConfig);
	    app.getVentanaPrincipal().actualizarTitulo();
	    
	    JOptionPane.showMessageDialog(this, "configuracion guardada exitosamente en la base de datos");
	}

	private JLabel lbl(String t) {
		JLabel l = new JLabel(t.toUpperCase());
		l.setFont(Estilos.FUENTE_XS);
		l.setForeground(Estilos.TEXTO_TENUE);
		l.setAlignmentX(Component.LEFT_ALIGNMENT);
		return l;
	}

	private JTextField campo(String valor, int cols) {
		JTextField tf = new JTextField(valor, cols);
		tf.setFont(new Font("SansSerif", Font.BOLD, 20));
		tf.setBackground(Estilos.BG_ZINC_100);
		tf.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(12, 16, 12, 16)));
		tf.setMaximumSize(new Dimension(Integer.MAX_VALUE, 56));
		tf.setAlignmentX(Component.LEFT_ALIGNMENT);
		return tf;
	}
}