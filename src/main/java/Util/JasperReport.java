package Util;

import Modelo.ConfiguracionTienda;
import Modelo.ItemCarrito;
import Modelo.Venta;
import net.sf.jasperreports.engine.*;
import net.sf.jasperreports.engine.data.JRMapCollectionDataSource;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.text.SimpleDateFormat;
import java.util.*;

public class JasperReport {

    private static final String TEMPLATE = "/reportes/reporte_ventas.jrxml";

    public static File generarReporteVentas(List<Venta> ventas,
                                            ConfiguracionTienda config,
                                            String fechaInicio,
                                            String fechaFin) throws JRException, IOException {

        InputStream jrxmlStream = JasperReport.class.getResourceAsStream(TEMPLATE);
        if (jrxmlStream == null) {
            throw new JRException("No se encontró el template: " + TEMPLATE);
        }
        net.sf.jasperreports.engine.JasperReport reporteCompilado =
                JasperCompileManager.compileReport(jrxmlStream);

        List<Map<String, ?>> filas = construirFilas(ventas);
        JRMapCollectionDataSource dataSource = new JRMapCollectionDataSource(filas);

        double totalIngresos = ventas.stream().mapToDouble(Venta::getTotal).sum();
        double ticketProm    = ventas.isEmpty() ? 0 : totalIngresos / ventas.size();

        Map<String, Object> params = new HashMap<>();
        params.put("TITULO",       config.getNombre());
        params.put("SUCURSAL",     config.getSucursal());
        params.put("RFC",          config.getRfc());
        params.put("FECHA_INICIO", fechaInicio);
        params.put("FECHA_FIN",    fechaFin);
        params.put("TOTAL_VENTAS", String.format("$%.2f", totalIngresos));
        params.put("NUM_VENTAS",   String.valueOf(ventas.size()));
        params.put("TICKET_PROM",  String.format("$%.2f", ticketProm));

        JasperPrint jasperPrint = JasperFillManager.fillReport(reporteCompilado, params, dataSource);

        File archivoPdf = File.createTempFile("ReporteVentas_", ".pdf");
        archivoPdf.deleteOnExit();
        JasperExportManager.exportReportToPdfFile(jasperPrint, archivoPdf.getAbsolutePath());

        return archivoPdf;
    }

    private static List<Map<String, ?>> construirFilas(List<Venta> ventas) {
        List<Map<String, ?>> filas = new ArrayList<>();
        SimpleDateFormat sdfF = new SimpleDateFormat("dd/MM/yyyy");
        SimpleDateFormat sdfH = new SimpleDateFormat("HH:mm:ss");

        for (Venta v : ventas) {
            String fecha = sdfF.format(v.getFecha());
            String hora  = sdfH.format(v.getFecha());

            if (v.getItems() == null || v.getItems().isEmpty()) {
                filas.add(fila(
                    String.valueOf(v.getId()), fecha, hora,
                    v.getCajero(), v.getMetodoPago(),
                    String.format("$%.2f", v.getTotal()),
                    "", "", "", "", ""
                ));
            } else {
                for (ItemCarrito item : v.getItems()) {
                    filas.add(fila(
                        String.valueOf(v.getId()), fecha, hora,
                        v.getCajero(), v.getMetodoPago(),
                        String.format("$%.2f", v.getTotal()),
                        item.getProducto().getId(),
                        item.getProducto().getNombre(),
                        String.format("%.2f", item.getCantidad()),
                        String.format("$%.2f", item.getProducto().getPrecio()),
                        String.format("$%.2f", item.getSubtotal())
                    ));
                }
            }
        }

        if (filas.isEmpty()) {
            filas.add(fila("", "", "", "", "", "", "", "Sin ventas en el período", "", "", ""));
        }

        return filas;
    }

    private static Map<String, Object> fila(String idVenta, String fecha, String hora,
                                             String cajero, String metodoPago, String total,
                                             String sku, String producto,
                                             String cantidad, String precioUni, String subtotal) {
        Map<String, Object> m = new HashMap<>();
        m.put("idVenta",    idVenta);
        m.put("fecha",      fecha);
        m.put("hora",       hora);
        m.put("cajero",     cajero);
        m.put("metodoPago", metodoPago);
        m.put("total",      total);
        m.put("sku",        sku);
        m.put("producto",   producto);
        m.put("cantidad",   cantidad);
        m.put("precioUni",  precioUni);
        m.put("subtotal",   subtotal);
        return m;
    }
}
