package Util;

import javax.swing.*;
import java.io.*;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.logging.*;

public class ManejoErrores {

    private static final Logger LOGGER = Logger.getLogger("CorporativoPOS");
    private static FileHandler manejadorArchivo;
    private static final String RUTA_LOG = "corporativo_pos_errores.log";

    static {
        try {
            manejadorArchivo = new FileHandler(RUTA_LOG, true);
            manejadorArchivo.setFormatter(new SimpleFormatter() {
                private final SimpleDateFormat sdf = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss");

                @Override
                public synchronized String format(LogRecord entrada) {
                    return String.format("[%s] [%s] %s%n",
                            sdf.format(new Date(entrada.getMillis())),
                            entrada.getLevel(),
                            entrada.getMessage());
                }
            });
            LOGGER.addHandler(manejadorArchivo);
            LOGGER.setUseParentHandlers(false);
        } catch (IOException e) {
            System.err.println("No se pudo inicializar el archivo de log: " + e.getMessage());
        }
    }

    public static void error(java.awt.Component padre, String titulo, String mensaje) {
        registrar(Level.SEVERE, titulo + ": " + mensaje);
        JOptionPane.showMessageDialog(padre, mensaje, titulo, JOptionPane.ERROR_MESSAGE);
    }

    public static void error(java.awt.Component padre, String titulo, String mensaje, Exception ex) {
        registrarError(titulo + ": " + mensaje, ex);
        JOptionPane.showMessageDialog(padre, mensaje + "\n\nDetalle: " + ex.getMessage(),
                titulo, JOptionPane.ERROR_MESSAGE);
    }

    public static void advertencia(java.awt.Component padre, String titulo, String mensaje) {
        registrar(Level.WARNING, titulo + ": " + mensaje);
        JOptionPane.showMessageDialog(padre, mensaje, titulo, JOptionPane.WARNING_MESSAGE);
    }

    public static void info(java.awt.Component padre, String titulo, String mensaje) {
        JOptionPane.showMessageDialog(padre, mensaje, titulo, JOptionPane.INFORMATION_MESSAGE);
    }

    public static boolean confirmar(java.awt.Component padre, String titulo, String mensaje) {
        int resp = JOptionPane.showConfirmDialog(padre, mensaje, titulo,
                JOptionPane.YES_NO_OPTION, JOptionPane.QUESTION_MESSAGE);
        return resp == JOptionPane.YES_OPTION;
    }

    public static void registrar(Level nivel, String mensaje) {
        LOGGER.log(nivel, mensaje);
    }

    public static void registrarError(String contexto, Exception ex) {
        StringWriter sw = new StringWriter();
        ex.printStackTrace(new PrintWriter(sw));
        LOGGER.log(Level.SEVERE, contexto + "\n" + sw);
    }

    public static void registrarInfo(String evento) {
        LOGGER.log(Level.INFO, evento);
    }

    public static boolean validarRequerido(java.awt.Component padre, String valor, String nombreCampo) {
        if (valor == null || valor.trim().isEmpty()) {
            advertencia(padre, "Campo requerido", "El campo '" + nombreCampo + "' es obligatorio.");
            return false;
        }
        return true;
    }

    public static double validarNumero(java.awt.Component padre, String valor, String nombreCampo) {
        try {
            double d = Double.parseDouble(valor.trim().replace(",", "."));
            if (d < 0) {
                advertencia(padre, "Valor invalido", "'" + nombreCampo + "' debe ser un numero positivo.");
                return -1;
            }
            return d;
        } catch (NumberFormatException ex) {
            advertencia(padre, "Formato incorrecto", "'" + nombreCampo + "' debe ser un numero valido.");
            return -1;
        }
    }

    public static String getRuta() {
        return new File(RUTA_LOG).getAbsolutePath();
    }
}
