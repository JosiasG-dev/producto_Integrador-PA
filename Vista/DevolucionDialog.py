from PyQt6.QtWidgets import QDialog, QVBoxLayout, QHBoxLayout, QLabel, QLineEdit, QPushButton, QTableWidget, QTableWidgetItem, QHeaderView, QMessageBox, QWidget, QScrollArea
from PyQt6.QtCore import Qt
from Vista.Estilos import Estilos
from Modelo.Devolucion import Devolucion
from datetime import datetime

class DevolucionDialog(QDialog):
    def __init__(self, parent, app):
        super().__init__(parent)
        self.app = app
        self.itemsVenta = []
        self.ventaId = -1
        self.setWindowTitle("Devolucion de Productos")
        self.setModal(True)
        self.construir()

    def construir(self):
        self.setFixedSize(560, 540)
        panel = QWidget(self)
        panel.resize(560, 540)
        panel.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()};")

        tit = QLabel("DEVOLUCION DE PRODUCTOS", panel)
        tit.setFont(Estilos.FUENTE_TITULO)
        tit.setStyleSheet(f"color: {Estilos.TEXTO_PRINCIPAL.name()};")
        tit.setGeometry(24, 16, 500, 30)

        lblVenta = QLabel("NUMERO DE VENTA (#):", panel)
        lblVenta.setFont(Estilos.FUENTE_XS)
        lblVenta.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        lblVenta.setGeometry(24, 58, 200, 14)

        self.txtVentaId = QLineEdit(panel)
        self.txtVentaId.setFont(Estilos.FUENTE_BOLD)
        self.txtVentaId.setStyleSheet(f"""
            QLineEdit {{
                background-color: {Estilos.BG_ZINC_100.name()};
                border: 2px solid {Estilos.BORDE.name()};
                padding: 6px 10px;
            }}
        """)
        self.txtVentaId.setGeometry(24, 76, 180, 36)

        btnBuscar = Estilos.botonPrimario("Buscar Venta")
        btnBuscar.setParent(panel)
        btnBuscar.setGeometry(214, 76, 130, 36)
        btnBuscar.clicked.connect(self.buscarVenta)

        lblItems = QLabel("PRODUCTOS DE LA VENTA (doble clic en la ultima columna para devolver):", panel)
        lblItems.setFont(Estilos.FUENTE_XS)
        lblItems.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        lblItems.setGeometry(24, 126, 500, 14)

        self.tablaItems = QTableWidget(0, 5, panel)
        self.tablaItems.setHorizontalHeaderLabels(["SKU", "Producto", "Cant Vendida", "Precio", "A Devolver"])
        self.tablaItems.setFont(Estilos.FUENTE_NORMAL)
        self.tablaItems.horizontalHeader().setFont(Estilos.FUENTE_XS)
        self.tablaItems.horizontalHeader().setStyleSheet(f"background-color: {Estilos.BG_ZINC_100.name()};")
        self.tablaItems.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; border: 1px solid {Estilos.BORDE.name()};")
        self.tablaItems.verticalHeader().setDefaultSectionSize(38)
        self.tablaItems.setColumnWidth(0, 60)
        self.tablaItems.setColumnWidth(3, 90)
        self.tablaItems.setColumnWidth(4, 90)
        self.tablaItems.horizontalHeader().setSectionResizeMode(1, QHeaderView.ResizeMode.Stretch)

        for i in range(4):
            # Hacer no editables las primeras 4 columnas
            self.tablaItems.horizontalHeader().setSectionResizeMode(i, QHeaderView.ResizeMode.Interactive)

        scroll = QScrollArea(panel)
        scroll.setWidget(self.tablaItems)
        scroll.setWidgetResizable(True)
        scroll.setGeometry(24, 144, 510, 220)
        scroll.setStyleSheet(f"border: 1px solid {Estilos.BORDE.name()};")

        lblMotivo = QLabel("MOTIVO DE DEVOLUCION:", panel)
        lblMotivo.setFont(Estilos.FUENTE_XS)
        lblMotivo.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        lblMotivo.setGeometry(24, 378, 300, 14)

        self.txtMotivo = QLineEdit(panel)
        self.txtMotivo.setFont(Estilos.FUENTE_BOLD)
        self.txtMotivo.setStyleSheet(f"""
            QLineEdit {{
                background-color: {Estilos.BG_ZINC_100.name()};
                border: 2px solid {Estilos.BORDE.name()};
                padding: 6px 10px;
            }}
        """)
        self.txtMotivo.setGeometry(24, 396, 510, 36)

        btnCancelar = Estilos.botonSecundario("Cancelar")
        btnCancelar.setParent(panel)
        btnCancelar.setGeometry(300, 452, 110, 38)
        btnCancelar.clicked.connect(self.reject)

        btnRegistrar = Estilos.botonPeligro("Registrar Devolucion")
        btnRegistrar.setParent(panel)
        btnRegistrar.setGeometry(420, 452, 114, 38)
        btnRegistrar.clicked.connect(self.registrarDevolucion)

    def buscarVenta(self):
        txt = self.txtVentaId.text().strip()
        if not txt:
            return
        try:
            self.ventaId = int(txt)
        except ValueError:
            QMessageBox.critical(self, "Error", "Ingresa un numero de venta valido")
            return
            
        venta = next((v for v in self.app.getVentas() if v.getId() == self.ventaId), None)
        if not venta:
            QMessageBox.warning(self, "Sin resultados", f"No se encontro la venta #{self.ventaId}")
            self.ventaId = -1
            return
            
        self.itemsVenta = venta.getItems()
        self.tablaItems.setRowCount(0)
        for i, item in enumerate(self.itemsVenta):
            self.tablaItems.insertRow(i)
            
            item0 = QTableWidgetItem(item.getProducto().getId())
            item0.setFlags(item0.flags() & ~Qt.ItemFlag.ItemIsEditable)
            self.tablaItems.setItem(i, 0, item0)
            
            item1 = QTableWidgetItem(item.getProducto().getNombre())
            item1.setFlags(item1.flags() & ~Qt.ItemFlag.ItemIsEditable)
            self.tablaItems.setItem(i, 1, item1)
            
            item2 = QTableWidgetItem(f"{item.getCantidad():.2f}")
            item2.setFlags(item2.flags() & ~Qt.ItemFlag.ItemIsEditable)
            self.tablaItems.setItem(i, 2, item2)
            
            item3 = QTableWidgetItem(f"${item.getProducto().getPrecio():.2f}")
            item3.setFlags(item3.flags() & ~Qt.ItemFlag.ItemIsEditable)
            self.tablaItems.setItem(i, 3, item3)
            
            self.tablaItems.setItem(i, 4, QTableWidgetItem("0"))

    def registrarDevolucion(self):
        if self.ventaId < 0 or not self.itemsVenta:
            QMessageBox.warning(self, "Aviso", "Primero busca una venta")
            return
            
        motivo = self.txtMotivo.text().strip()
        if not motivo:
            QMessageBox.warning(self, "Aviso", "Ingresa el motivo de la devolucion")
            return
            
        alguna = False
        for i in range(self.tablaItems.rowCount()):
            try:
                item_widget = self.tablaItems.item(i, 4)
                if not item_widget: continue
                cantDevolver = float(item_widget.text().strip())
                if cantDevolver <= 0:
                    continue
                    
                item = self.itemsVenta[i]
                cantVendida = item.getCantidad()
                if cantDevolver > cantVendida:
                    QMessageBox.critical(self, "Error", f"No puedes devolver mas de lo vendido para: {item.getProducto().getNombre()}")
                    return
                    
                d = Devolucion(0, self.ventaId, item.getProducto(), cantDevolver, motivo, datetime.now(), self.app.getUsuarioActivo().getNombre())
                self.app.registrarDevolucion(d)
                alguna = True
            except ValueError:
                pass
                
        if alguna:
            QMessageBox.information(self, "Exito", "Devolucion registrada. El stock fue actualizado.")
            self.accept()
        else:
            QMessageBox.warning(self, "Sin cantidades", "Ingresa al menos una cantidad mayor a 0 en 'A Devolver'")
