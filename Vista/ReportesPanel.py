from PyQt6.QtWidgets import (QWidget, QVBoxLayout, QHBoxLayout, QLabel, QLineEdit, QPushButton, 
                             QTableWidget, QTableWidgetItem, QHeaderView, QScrollArea, QDialog, QMessageBox, QFileDialog)
from PyQt6.QtCore import Qt, QDate
from PyQt6.QtGui import QFont, QColor
from Vista.Estilos import Estilos
from Modelo.Venta import Venta
from Modelo.Devolucion import Devolucion
from datetime import datetime
from Util.ManejoErrores import ManejoErrores
from Util.CreadorDocumentos import CreadorDocumentos
import os

class ReportesPanel(QWidget):
    def __init__(self, app):
        super().__init__()
        self.app = app
        self.ventasFiltradas = []
        self.construir()

    def construir(self):
        self.setStyleSheet(f"background-color: {Estilos.BG_CLARO.name()};")
        panel = QWidget(self)
        panel.resize(1300, 700)
        
        tit = QLabel("REPORTES DE VENTAS", panel)
        tit.setFont(Estilos.FUENTE_TITULO)
        tit.setGeometry(24, 14, 400, 30)

        self.lbl("DESDE (dd/MM/yyyy):", 24, 56, 160, 14, panel)
        self.txtFechaInicio = self.campo(24, 74, 150, 34, panel)
        self.txtFechaInicio.setText("01/01/2024")

        self.lbl("HASTA (dd/MM/yyyy):", 184, 56, 160, 14, panel)
        self.txtFechaFin = self.campo(184, 74, 150, 34, panel)
        self.txtFechaFin.setText(datetime.now().strftime("%d/%m/%Y"))

        self.lbl("FILTRAR POR PRODUCTO (nombre o SKU):", 344, 56, 280, 14, panel)
        self.txtFiltroProducto = self.campo(344, 74, 220, 34, panel)
        self.txtFiltroProducto.setToolTip("Deja vacio para ver todas las ventas")

        btnFiltrar = Estilos.botonPrimario("Filtrar")
        btnFiltrar.setParent(panel)
        btnFiltrar.setGeometry(574, 74, 90, 34)
        btnFiltrar.clicked.connect(self.refrescar)

        btnExcel = Estilos.botonSecundario("Exportar Excel")
        btnExcel.setParent(panel)
        btnExcel.setGeometry(664, 74, 110, 34)
        btnExcel.clicked.connect(self.exportarExcel)

        btnGenerarPDF = Estilos.botonSecundario("Generar PDF")
        btnGenerarPDF.setParent(panel)
        btnGenerarPDF.setGeometry(784, 74, 110, 34)
        btnGenerarPDF.clicked.connect(self.generar_pdf)

        btnDevolucion = Estilos.botonPeligro("Devolucion")
        btnDevolucion.setParent(panel)
        btnDevolucion.setGeometry(904, 74, 100, 34)
        btnDevolucion.clicked.connect(self.abrirDevolucion)

        btnVerDevoluciones = Estilos.botonSecundario("Ver Devoluciones")
        btnVerDevoluciones.setParent(panel)
        btnVerDevoluciones.setGeometry(1014, 74, 140, 34)
        btnVerDevoluciones.clicked.connect(self.mostrarDevoluciones)

        btnReimprimir = Estilos.botonSecundario("Reimprimir Ticket")
        btnReimprimir.setParent(panel)
        btnReimprimir.setGeometry(1164, 74, 140, 34)
        btnReimprimir.clicked.connect(self.reimprimirSeleccionado)

        btnPorProducto = Estilos.botonSecundario("Ventas por Producto")
        btnPorProducto.setParent(panel)
        btnPorProducto.setGeometry(1314, 74, 160, 34)
        btnPorProducto.clicked.connect(self.mostrarVentasPorProducto)

        self.lblTotal = self.kpiCard("Ingresos", "$0.00", Estilos.INDIGO, QColor("white"), 24, 124, panel)
        self.lblNumVentas = self.kpiCard("Ventas", "0", Estilos.BG_BLANCO, Estilos.TEXTO_PRINCIPAL, 204, 124, panel)
        self.lblPromedio = self.kpiCard("Ticket Prom", "$0.00", Estilos.BG_BLANCO, Estilos.TEXTO_PRINCIPAL, 384, 124, panel)

        self.tablaVentas = QTableWidget(0, 7, panel)
        self.tablaVentas.setHorizontalHeaderLabels(["#", "Fecha Venta", "Hora", "Cajero", "Metodo", "Total", "Productos"])
        self.tablaVentas.setFont(Estilos.FUENTE_NORMAL)
        self.tablaVentas.horizontalHeader().setFont(Estilos.FUENTE_XS)
        self.tablaVentas.horizontalHeader().setStyleSheet(f"background-color: {Estilos.BG_ZINC_100.name()};")
        self.tablaVentas.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; border: 1px solid {Estilos.BORDE.name()};")
        self.tablaVentas.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
        self.tablaVentas.setSelectionBehavior(QTableWidget.SelectionBehavior.SelectRows)
        self.tablaVentas.verticalHeader().setDefaultSectionSize(38)

        self.tablaVentas.setColumnWidth(0, 50)
        self.tablaVentas.setColumnWidth(2, 80)
        self.tablaVentas.setColumnWidth(4, 90)
        self.tablaVentas.setColumnWidth(5, 100)
        self.tablaVentas.horizontalHeader().setSectionResizeMode(6, QHeaderView.ResizeMode.Stretch)

        scroll = QScrollArea(panel)
        scroll.setWidget(self.tablaVentas)
        scroll.setWidgetResizable(True)
        scroll.setGeometry(24, 242, 1200, 440)
        scroll.setStyleSheet("border: none;")

        self.refrescar()

    def lbl(self, texto, x, y, w, h, parent):
        l = QLabel(texto, parent)
        l.setFont(Estilos.FUENTE_XS)
        l.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        l.setGeometry(x, y, w, h)
        return l

    def campo(self, x, y, w, h, parent):
        tf = QLineEdit(parent)
        tf.setFont(Estilos.FUENTE_BOLD)
        tf.setStyleSheet(f"""
            QLineEdit {{
                background-color: {Estilos.BG_ZINC_100.name()};
                border: 2px solid {Estilos.BORDE.name()};
                padding: 6px 10px;
            }}
        """)
        tf.setGeometry(x, y, w, h)
        return tf

    def kpiCard(self, label, valor, bg, fg, x, y, parent):
        card = QWidget(parent)
        card.setGeometry(x, y, 168, 90)
        card.setStyleSheet(f"background-color: {bg.name()}; border: 1px solid {Estilos.BORDE.name() if bg == Estilos.BG_BLANCO else bg.name()};")
        
        lLbl = QLabel(label.upper(), card)
        lLbl.setFont(Estilos.FUENTE_XS)
        lLbl.setStyleSheet(f"color: {'#C7D2FE' if bg == Estilos.INDIGO else Estilos.TEXTO_TENUE.name()}; border: none;")
        lLbl.setGeometry(12, 12, 144, 14)

        lVal = QLabel(valor, card)
        lVal.setFont(QFont("SansSerif", 28, QFont.Weight.Bold))
        lVal.setStyleSheet(f"color: {fg.name()}; border: none;")
        lVal.setGeometry(12, 32, 144, 40)
        return lVal

    def refrescar(self):
        filtroProd = self.txtFiltroProducto.text().strip().lower()
        todasFecha = self.filtrarPorFecha(self.app.getVentas())

        if filtroProd:
            self.ventasFiltradas = [v for v in todasFecha if any(filtroProd in item.getProducto().getNombre().lower() or filtroProd in item.getProducto().getId().lower() for item in v.getItems())]
        else:
            self.ventasFiltradas = todasFecha

        total = sum(v.getTotal() for v in self.ventasFiltradas)
        prom = total / len(self.ventasFiltradas) if self.ventasFiltradas else 0
        
        self.lblTotal.setText(f"${total:.2f}")
        self.lblNumVentas.setText(str(len(self.ventasFiltradas)))
        self.lblPromedio.setText(f"${prom:.2f}")

        self.tablaVentas.setRowCount(0)
        for i, v in enumerate(self.ventasFiltradas):
            self.tablaVentas.insertRow(i)
            productos = ", ".join([f"{item.getProducto().getNombre()} x{int(item.getCantidad())}" for item in v.getItems()])
            fecha_str = v.getFecha().strftime("%d/%m/%Y") if v.getFecha() else ""
            hora_str = v.getFecha().strftime("%H:%M:%S") if v.getFecha() else ""
            
            self.tablaVentas.setItem(i, 0, QTableWidgetItem(str(v.getId())))
            self.tablaVentas.setItem(i, 1, QTableWidgetItem(fecha_str))
            self.tablaVentas.setItem(i, 2, QTableWidgetItem(hora_str))
            self.tablaVentas.setItem(i, 3, QTableWidgetItem(v.getCajero()))
            self.tablaVentas.setItem(i, 4, QTableWidgetItem(v.getMetodoPago()))
            self.tablaVentas.setItem(i, 5, QTableWidgetItem(f"${v.getTotal():.2f}"))
            self.tablaVentas.setItem(i, 6, QTableWidgetItem(productos))

    def filtrarPorFecha(self, todas):
        try:
            inicio = datetime.strptime(self.txtFechaInicio.text().strip(), "%d/%m/%Y")
            fin = datetime.strptime(self.txtFechaFin.text().strip(), "%d/%m/%Y").replace(hour=23, minute=59, second=59)
            return [v for v in todas if v.getFecha() and inicio <= v.getFecha() <= fin]
        except ValueError:
            return todas

    def abrirDevolucion(self):
        from Vista.DevolucionDialog import DevolucionDialog
        dlg = DevolucionDialog(self.window(), self.app)
        dlg.exec()
        self.refrescar()

    def mostrarDevoluciones(self):
        devoluciones = self.app.getDevoluciones()
        dlg = QDialog(self.window())
        dlg.setWindowTitle("Historial de Devoluciones")
        dlg.setFixedSize(800, 480)
        layout = QVBoxLayout(dlg)
        
        tit = QLabel("HISTORIAL DE DEVOLUCIONES")
        tit.setFont(Estilos.FUENTE_TITULO)
        layout.addWidget(tit)
        
        tabla = QTableWidget(0, 8)
        tabla.setHorizontalHeaderLabels(["#", "Venta #", "Producto", "Cantidad", "Monto Dev", "Motivo", "Fecha", "Cajero"])
        tabla.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
        tabla.horizontalHeader().setSectionResizeMode(2, QHeaderView.ResizeMode.Stretch)
        
        for d in devoluciones:
            row = tabla.rowCount()
            tabla.insertRow(row)
            tabla.setItem(row, 0, QTableWidgetItem(str(d.getId())))
            tabla.setItem(row, 1, QTableWidgetItem(str(d.getVentaId())))
            tabla.setItem(row, 2, QTableWidgetItem(d.getProducto().getNombre()))
            tabla.setItem(row, 3, QTableWidgetItem(f"{d.getCantidad():.2f}"))
            tabla.setItem(row, 4, QTableWidgetItem(f"${d.getMontoDevuelto():.2f}"))
            tabla.setItem(row, 5, QTableWidgetItem(d.getMotivo()))
            fecha_str = d.getFecha().strftime("%d/%m/%Y %H:%M") if d.getFecha() else ""
            tabla.setItem(row, 6, QTableWidgetItem(fecha_str))
            tabla.setItem(row, 7, QTableWidgetItem(d.getCajero()))
            
        if not devoluciones:
            tabla.insertRow(0)
            tabla.setItem(0, 2, QTableWidgetItem("Sin devoluciones registradas"))
            
        layout.addWidget(tabla)
        btnCerrar = Estilos.botonPrimario("Cerrar")
        btnCerrar.clicked.connect(dlg.accept)
        layout.addWidget(btnCerrar)
        dlg.exec()

    def reimprimirSeleccionado(self):
        row = self.tablaVentas.currentRow()
        if row < 0:
            ManejoErrores.advertencia(self.window(), "Seleccion requerida", "Selecciona una venta de la tabla para reimprimir su ticket.")
            return
        venta = self.ventasFiltradas[row]
        if not venta.getItems():
            ManejoErrores.advertencia(self.window(), "Sin detalle", "Esta venta no tiene detalle de productos disponible para reimprimir.")
            return
        
        from Vista.TicketDialog import TicketDialog
        dlg = TicketDialog(self.window(), venta, self.app.getConfig(), 0.0, venta.getTotal())
        ManejoErrores.registrarInfo(f"Reimpresion de ticket #{venta.getId()}")
        dlg.mostrar()

    def exportarExcel(self):
        archivo, _ = QFileDialog.getSaveFileName(self, "Exportar Excel", "ReporteVentas.csv", "CSV Files (*.csv)")
        if not archivo: return
        import csv
        try:
            with open(archivo, mode='w', newline='', encoding='utf-8') as f:
                writer = csv.writer(f)
                writer.writerow(["ID Venta", "Fecha Venta", "Hora", "Cajero", "Metodo Pago", "Total Venta", "SKU", "Producto", "Cantidad", "Precio Unitario", "Subtotal"])
                for v in self.ventasFiltradas:
                    fecha = v.getFecha().strftime("%d/%m/%Y") if v.getFecha() else ""
                    hora = v.getFecha().strftime("%H:%M:%S") if v.getFecha() else ""
                    if not v.getItems():
                        writer.writerow([v.getId(), fecha, hora, v.getCajero(), v.getMetodoPago(), v.getTotal(), "", "", "", "", ""])
                    else:
                        for item in v.getItems():
                            writer.writerow([v.getId(), fecha, hora, v.getCajero(), v.getMetodoPago(), v.getTotal(), item.getProducto().getId(), item.getProducto().getNombre(), item.getCantidad(), item.getProducto().getPrecio(), item.getSubtotal()])
            ManejoErrores.info(self.window(), "Exportacion exitosa", f"CSV guardado en:\n{archivo}")
        except Exception as ex:
            ManejoErrores.error(self.window(), "Error al exportar", "No se pudo guardar el archivo.", ex)

    def generar_pdf(self):
        try:
            ruta_pdf = CreadorDocumentos.crear_documento_ventas(
                self.ventasFiltradas,
                self.app.getConfig(),
                self.txtFechaInicio.text(),
                self.txtFechaFin.text()
            )
            os.startfile(ruta_pdf)
        except Exception as ex:
            ManejoErrores.error(self.window(), "Error", "No se pudo generar el reporte PDF.", ex)

    def mostrarVentasPorProducto(self):
        resumen = {}
        for v in self.ventasFiltradas:
            if not v.getItems(): continue
            for item in v.getItems():
                clave = f"[{item.getProducto().getId()}] {item.getProducto().getNombre()}"
                if clave not in resumen:
                    resumen[clave] = [0.0, 0.0]
                resumen[clave][0] += item.getCantidad()
                resumen[clave][1] += item.getSubtotal()

        lista = sorted(resumen.items(), key=lambda x: x[1][1], reverse=True)

        dlg = QDialog(self.window())
        dlg.setWindowTitle("Reporte de Ventas por Producto")
        dlg.setFixedSize(700, 520)
        layout = QVBoxLayout(dlg)
        
        tit = QLabel("RANKING DE PRODUCTOS MAS VENDIDOS")
        tit.setFont(Estilos.FUENTE_TITULO)
        layout.addWidget(tit)
        
        sub = QLabel(f"Periodo: {self.txtFechaInicio.text()} a {self.txtFechaFin.text()} | {len(lista)} productos distintos")
        layout.addWidget(sub)
        
        tabla = QTableWidget(0, 4)
        tabla.setHorizontalHeaderLabels(["#", "Producto", "Unidades Vendidas", "Ingresos Totales"])
        tabla.horizontalHeader().setSectionResizeMode(1, QHeaderView.ResizeMode.Stretch)
        tabla.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
        
        for i, (k, v) in enumerate(lista):
            row = tabla.rowCount()
            tabla.insertRow(row)
            tabla.setItem(row, 0, QTableWidgetItem(str(i + 1)))
            tabla.setItem(row, 1, QTableWidgetItem(k))
            tabla.setItem(row, 2, QTableWidgetItem(f"{v[0]:.0f}"))
            tabla.setItem(row, 3, QTableWidgetItem(f"${v[1]:.2f}"))
            
        if not lista:
            tabla.insertRow(0)
            tabla.setItem(0, 1, QTableWidgetItem("Sin ventas en el periodo seleccionado"))
            
        layout.addWidget(tabla)
        btnCerrar = Estilos.botonPrimario("Cerrar")
        btnCerrar.clicked.connect(dlg.accept)
        layout.addWidget(btnCerrar)
        dlg.exec()
