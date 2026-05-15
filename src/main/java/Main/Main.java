package Main;

import javax.swing.*;

import ConexionBD.*;
import Controlador.*;
import Vista.ConexionDialog;

public class Main {
    public static void main(String[] args) {

        try {
            UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
        } catch (Exception ignored) {}

        Runtime.getRuntime().addShutdownHook(new Thread(Conexion::cerrar));

        SwingUtilities.invokeLater(() -> {
            ConexionDialog dialogo = new ConexionDialog();
            dialogo.setVisible(true);

            if (!dialogo.fueConfirmado()) {
                System.exit(0);
            }

            AppControlador controlador = new AppControlador();
            controlador.iniciar();
        });
    }
}
