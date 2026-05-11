package Main;

import javax.swing.*;

import ConexionBD.*;
import Controlador.*;

public class Main {
    public static void main(String[] args) {

        try {
            UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
        } catch (Exception ignored) {}

        Runtime.getRuntime().addShutdownHook(new Thread(Conexion::cerrar));

        SwingUtilities.invokeLater(() -> {
            AppControlador controlador = new AppControlador();
            controlador.iniciar();
        });
    }
}
