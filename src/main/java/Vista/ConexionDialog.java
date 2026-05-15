package Vista;

import ConexionBD.Conexion;
import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;

public class ConexionDialog extends JDialog {

    private boolean confirmado = false;

    private JComboBox<String> comboMotor;
    private JComboBox<String> comboAuth;
    private JTextField campoHost;
    private JTextField campoPuerto;
    private JTextField campoBase;
    private JTextField campoInstancia;
    private JTextField campoUsuario;
    private JPasswordField campoContrasena;
    private JLabel labelUsuario;
    private JLabel labelContrasena;
    private JLabel labelPuerto;
    private JLabel labelInstancia;

    public ConexionDialog() {
        super((Frame) null, "Configuracion de Conexion", true);
        setDefaultCloseOperation(DO_NOTHING_ON_CLOSE);
        construirUI();
        pack();
        setLocationRelativeTo(null);
        setResizable(false);
    }

    private void construirUI() {
        JPanel panel = new JPanel(new GridBagLayout());
        panel.setBorder(new EmptyBorder(20, 24, 20, 24));
        GridBagConstraints gbc = new GridBagConstraints();
        gbc.insets  = new Insets(5, 5, 5, 5);
        gbc.anchor  = GridBagConstraints.WEST;
        gbc.fill    = GridBagConstraints.HORIZONTAL;

        JLabel titulo = new JLabel("Selecciona el motor de base de datos");
        titulo.setFont(titulo.getFont().deriveFont(Font.BOLD, 14f));
        gbc.gridx = 0; gbc.gridy = 0; gbc.gridwidth = 2;
        panel.add(titulo, gbc);
        gbc.gridwidth = 1;

        comboMotor = new JComboBox<>(new String[]{"MySQL"});
        comboMotor.setEnabled(false);
        agregarFila(panel, gbc, 1, "Motor:", comboMotor);

        campoHost = new JTextField("localhost", 18);
        agregarFila(panel, gbc, 3, "Host:", campoHost);

        labelPuerto = new JLabel("Puerto:");
        campoPuerto = new JTextField("3306", 18);
        agregarFila(panel, gbc, 4, labelPuerto, campoPuerto);

        labelInstancia = new JLabel("Puerto TCP:");
        campoInstancia = new JTextField("1433", 18);
        agregarFila(panel, gbc, 5, labelInstancia, campoInstancia);

        campoBase = new JTextField("corporativo_pos", 18);
        agregarFila(panel, gbc, 6, "Base de datos:", campoBase);

        labelUsuario   = new JLabel("Usuario:");
        campoUsuario   = new JTextField("root", 18);
        agregarFila(panel, gbc, 7, labelUsuario, campoUsuario);

        labelContrasena   = new JLabel("Contrasena:");
        campoContrasena   = new JPasswordField("2306", 18);
        agregarFila(panel, gbc, 8, labelContrasena, campoContrasena);

        comboMotor.addActionListener(e -> actualizarVista());

        JButton btnConectar = new JButton("Conectar");
        JButton btnSalir    = new JButton("Salir");
        btnConectar.addActionListener(e -> intentarConexion());
        btnSalir.addActionListener(e -> System.exit(0));

        JPanel panelBotones = new JPanel(new FlowLayout(FlowLayout.RIGHT, 8, 0));
        panelBotones.add(btnSalir);
        panelBotones.add(btnConectar);

        gbc.gridx = 0; gbc.gridy = 9; gbc.gridwidth = 2;
        gbc.anchor = GridBagConstraints.EAST;
        gbc.insets = new Insets(14, 5, 0, 5);
        panel.add(panelBotones, gbc);

        add(panel);
        actualizarVista();
    }

    private void agregarFila(JPanel panel, GridBagConstraints gbc, int fila, String etiqueta, JComponent campo) {
        agregarFila(panel, gbc, fila, new JLabel(etiqueta), campo);
    }

    private void agregarFila(JPanel panel, GridBagConstraints gbc, int fila, JLabel etiqueta, JComponent campo) {
        gbc.gridx = 0; gbc.gridy = fila; gbc.gridwidth = 1;
        gbc.anchor = GridBagConstraints.WEST;
        gbc.insets = new Insets(5, 5, 5, 5);
        panel.add(etiqueta, gbc);
        gbc.gridx = 1;
        panel.add(campo, gbc);
    }

	private void actualizarVista() {
        labelInstancia.setVisible(false);
        campoInstancia.setVisible(false);
        labelPuerto.setVisible(true);
        campoPuerto.setVisible(true);
        labelUsuario.setVisible(true);
        campoUsuario.setVisible(true);
        labelContrasena.setVisible(true);
        campoContrasena.setVisible(true);
        campoPuerto.setText("3306");
        campoUsuario.setText("root");
        pack();
    }


	private void intentarConexion() {
		String host = campoHost.getText().trim();
        String base  = campoBase.getText().trim();
        String puerto = campoPuerto.getText().trim();
        String usr  = campoUsuario.getText().trim();
        String pass = new String(campoContrasena.getPassword());

        if (host.isEmpty() || base.isEmpty() || puerto.isEmpty() || usr.isEmpty()) {
            JOptionPane.showMessageDialog(this, "Completa todos los campos.", "Aviso", JOptionPane.WARNING_MESSAGE);
            return;
        }

        Conexion.configurar(
                Conexion.TipoMotor.MYSQL,
                Conexion.TipoAutenticacion.CREDENCIALES,
                host,
                puerto,
                base,
                usr,
                pass
        );

        if (Conexion.getConexion() != null) {
            confirmado = true;
            dispose();
        } else {
            JOptionPane.showMessageDialog(this,
                    "No se pudo conectar.\nRevisa los datos e intenta de nuevo.",
                    "Error de Conexion", JOptionPane.ERROR_MESSAGE);
            Conexion.cerrar();
        }
    }

    public boolean fueConfirmado() {
        return confirmado;
    }
}
