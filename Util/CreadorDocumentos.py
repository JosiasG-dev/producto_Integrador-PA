from reportlab.lib import colors
from reportlab.lib.pagesizes import letter, landscape
from reportlab.platypus import SimpleDocTemplate, Table, TableStyle, Paragraph, Spacer
from reportlab.lib.styles import getSampleStyleSheet
import tempfile

class CreadorDocumentos:
    @staticmethod
    def crear_documento_ventas(ventas, configuracion, fecha_inicio, fecha_fin):
        archivo_temporal = tempfile.NamedTemporaryFile(delete=False, suffix=".pdf", prefix="ReporteVentas_")
        ruta_archivo = archivo_temporal.name
        archivo_temporal.close()

        documento = SimpleDocTemplate(ruta_archivo, pagesize=landscape(letter))
        elementos = []
        estilos = getSampleStyleSheet()
        
        ingresos_totales = sum(v.getTotal() for v in ventas)
        numero_ventas = len(ventas)
        promedio_venta = ingresos_totales / numero_ventas if numero_ventas > 0 else 0

        elementos.append(Paragraph(configuracion.getNombre(), estilos['Title']))
        elementos.append(Paragraph(f"Sucursal: {configuracion.getSucursal()}", estilos['Normal']))
        elementos.append(Paragraph(f"RFC: {configuracion.getRfc()}", estilos['Normal']))
        elementos.append(Paragraph(f"Periodo: {fecha_inicio} al {fecha_fin}", estilos['Normal']))
        elementos.append(Spacer(1, 12))
        
        elementos.append(Paragraph(f"Ventas totales: {numero_ventas}", estilos['Normal']))
        elementos.append(Paragraph(f"Ingresos totales: ${ingresos_totales:.2f}", estilos['Normal']))
        elementos.append(Paragraph(f"Promedio por venta: ${promedio_venta:.2f}", estilos['Normal']))
        elementos.append(Spacer(1, 12))

        lista_datos = [["ID Venta", "Fecha", "Hora", "Cajero", "Metodo Pago", "Total", "SKU", "Producto", "Cantidad", "Precio Uni", "Subtotal"]]

        for v in ventas:
            fecha_str = v.getFecha().strftime("%d/%m/%Y") if v.getFecha() else ""
            hora_str = v.getFecha().strftime("%H:%M:%S") if v.getFecha() else ""
            
            if not v.getItems():
                lista_datos.append([
                    str(v.getId()), fecha_str, hora_str, v.getCajero(), v.getMetodoPago(),
                    f"${v.getTotal():.2f}", "", "", "", "", ""
                ])
            else:
                for item in v.getItems():
                    lista_datos.append([
                        str(v.getId()), fecha_str, hora_str, v.getCajero(), v.getMetodoPago(),
                        f"${v.getTotal():.2f}", str(item.getProducto().getId()), 
                        item.getProducto().getNombre(), f"{item.getCantidad():.2f}",
                        f"${item.getProducto().getPrecio():.2f}", f"${item.getSubtotal():.2f}"
                    ])

        if len(lista_datos) == 1:
            lista_datos.append(["", "", "", "", "", "", "", "Sin ventas en el periodo", "", "", ""])

        tabla = Table(lista_datos)
        estilo_tabla = TableStyle([
            ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor("#f4f4f5")),
            ('TEXTCOLOR', (0, 0), (-1, 0), colors.black),
            ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
            ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
            ('BOTTOMPADDING', (0, 0), (-1, 0), 12),
            ('BACKGROUND', (0, 1), (-1, -1), colors.white),
            ('GRID', (0, 0), (-1, -1), 1, colors.black)
        ])
        tabla.setStyle(estilo_tabla)
        elementos.append(tabla)

        documento.build(elementos)

        return ruta_archivo
