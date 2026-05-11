package Vista;

import javax.swing.*;
import Controlador.AppControlador;
import Modelo.Usuario;
import java.awt.*;
import java.awt.event.*;

public class LoginVista {

	private final AppControlador app;
	private JFrame frame;
	private JTextField txtUsuario;
	private JPasswordField txtPassword;
	private JLabel lblError;

	public LoginVista(AppControlador app) {
		this.app = app;
		construir();
	}

	private void construir() {
		frame = new JFrame("POS - Acceso al Sistema");
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.setSize(460, 520);
		frame.setLocationRelativeTo(null);
		frame.setResizable(false);

		JPanel fondo = new JPanel(null);
		fondo.setBackground(Estilos.BG_OSCURO);

		JPanel tarjeta = new JPanel(null);
		tarjeta.setBackground(Estilos.BG_BLANCO);
		tarjeta.setBorder(BorderFactory.createLineBorder(Estilos.BORDE));
		tarjeta.setBounds(44, 30, 372, 440);
		fondo.add(tarjeta);

		JLabel icono = new JLabel("PUNTO DE VENTA", SwingConstants.CENTER);
		icono.setFont(new Font("SansSerif", Font.BOLD, 18));
		icono.setForeground(Estilos.INDIGO);
		icono.setBounds(0, 24, 372, 30);
		tarjeta.add(icono);

		JLabel titulo = new JLabel("Terminal Corporativa", SwingConstants.CENTER);
		titulo.setFont(Estilos.FUENTE_TITULO);
		titulo.setForeground(Estilos.TEXTO_PRINCIPAL);
		titulo.setBounds(0, 60, 372, 28);
		tarjeta.add(titulo);

		JLabel sub = new JLabel("SISTEMA POS", SwingConstants.CENTER);
		sub.setFont(Estilos.FUENTE_XS);
		sub.setForeground(Estilos.INDIGO);
		sub.setBounds(0, 92, 372, 16);
		tarjeta.add(sub);

		JLabel lblUsr = new JLabel("USUARIO");
		lblUsr.setFont(Estilos.FUENTE_XS);
		lblUsr.setForeground(Estilos.TEXTO_TENUE);
		lblUsr.setBounds(36, 132, 300, 14);
		tarjeta.add(lblUsr);

		txtUsuario = new JTextField();
		txtUsuario.setFont(Estilos.FUENTE_BOLD);
		txtUsuario.setBackground(Estilos.BG_ZINC_100);
		txtUsuario.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(8, 12, 8, 12)));
		txtUsuario.setBounds(36, 150, 300, 44);
		tarjeta.add(txtUsuario);

		JLabel lblPass = new JLabel("CONTRASENA");
		lblPass.setFont(Estilos.FUENTE_XS);
		lblPass.setForeground(Estilos.TEXTO_TENUE);
		lblPass.setBounds(36, 206, 300, 14);
		tarjeta.add(lblPass);

		txtPassword = new JPasswordField();
		txtPassword.setFont(Estilos.FUENTE_BOLD);
		txtPassword.setBackground(Estilos.BG_ZINC_100);
		txtPassword.setBorder(BorderFactory.createCompoundBorder(BorderFactory.createLineBorder(Estilos.BORDE, 2),
				BorderFactory.createEmptyBorder(8, 12, 8, 12)));
		txtPassword.setBounds(36, 224, 300, 44);
		tarjeta.add(txtPassword);

		lblError = new JLabel(" ", SwingConstants.CENTER);
		lblError.setFont(Estilos.FUENTE_SMALL);
		lblError.setForeground(Estilos.ROSE);
		lblError.setBounds(36, 276, 300, 16);
		tarjeta.add(lblError);

		JButton btnEntrar = Estilos.botonPrimario("ENTRAR AL SISTEMA");
		btnEntrar.setBounds(36, 302, 300, 48);
		btnEntrar.addActionListener(e -> intentarLogin());
		tarjeta.add(btnEntrar);

		txtPassword.addKeyListener(new KeyAdapter() {
			@Override
			public void keyPressed(KeyEvent e) {
				if (e.getKeyCode() == KeyEvent.VK_ENTER)
					intentarLogin();
			}
		});

		frame.setContentPane(fondo);
	}

	private void intentarLogin() {
		String user = txtUsuario.getText().trim();
		String pass = new String(txtPassword.getPassword()).trim();
		Usuario u = app.autenticar(user, pass);
		if (u != null) {
			lblError.setText(" ");
			app.onLoginExitoso(u);
		} else {
			lblError.setText("Credenciales invalidas");
		}
	}

	public void mostrar() {
		frame.setVisible(true);
	}

	public void ocultar() {
		frame.setVisible(false);
	}

	public void limpiar() {
		txtUsuario.setText("");
		txtPassword.setText("");
		lblError.setText(" ");
	}
}
