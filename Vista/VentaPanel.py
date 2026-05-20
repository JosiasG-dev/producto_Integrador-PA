from PyQt6.QtWidgets import (QWidget, QVBoxLayout, QHBoxLayout, QLabel, QLineEdit, 
                             QPushButton, QTableWidget, QTableWidgetItem, QHeaderView, 
                             QComboBox, QMessageBox, QScrollArea, QFrame)
from PyQt6.QtCore import Qt, QSize
from PyQt6.QtGui import QFont, QColor, QPixmap
from Vista.Estilos import Estilos
import os

class VentaPanel(QWidget):
    def __init__(self, ctrl, config, cajero):
        super().__init__()
        self.ctrl = ctrl
        self.config = config
        self.cajero = cajero
        self.ctrl.setPanel(self)
        self.actualizando = False
        self.construir()

    def construir(self):
        self.setStyleSheet(f"background-color: {Estilos.BG_CLARO.name()};")
        layout = QHBoxLayout(self)
        layout.setContentsMargins(16, 14, 16, 14)

        # Contenido Izquierdo
        panelIzq = QWidget()
        layoutIzq = QVBoxLayout(panelIzq)
        layoutIzq.setContentsMargins(0, 0, 0, 0)

        lblBusq = QLabel("BUSCAR PRODUCTO (nombre o SKU):")
        lblBusq.setFont(Estilos.FUENTE_XS)
        lblBusq.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        layoutIzq.addWidget(lblBusq)

        self.txtBusqueda = QLineEdit()
        self.txtBusqueda.setFont(Estilos.FUENTE_SUBTITULO)
        self.txtBusqueda.setStyleSheet(f"""
            QLineEdit {{
                background-color: {Estilos.BG_BLANCO.name()};
                color: {Estilos.TEXTO_PRINCIPAL.name()};
                border: 2px solid {Estilos.BORDE.name()};
                padding: 8px 12px;
            }}
        """)
        self.txtBusqueda.textChanged.connect(self.actualizarSugerencias)
        layoutIzq.addWidget(self.txtBusqueda)

        self.sugerenciasArea = QScrollArea()
        self.sugerenciasArea.setWidgetResizable(True)
        self.sugerenciasArea.setMaximumHeight(150)
        self.sugerenciasArea.setVisible(False)
        self.sugerenciasWidget = QWidget()
        self.sugerenciasLayout = QVBoxLayout(self.sugerenciasWidget)
        self.sugerenciasLayout.setContentsMargins(0, 0, 0, 0)
        self.sugerenciasLayout.setSpacing(0)
        self.sugerenciasArea.setWidget(self.sugerenciasWidget)
        layoutIzq.addWidget(self.sugerenciasArea)

        lblDetalle = QLabel("DETALLE DE VENTA")
        lblDetalle.setFont(Estilos.FUENTE_XS)
        lblDetalle.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        layoutIzq.addWidget(lblDetalle)

        self.tablaCarrito = QTableWidget(0, 6)
        self.tablaCarrito.setHorizontalHeaderLabels(["ID", "Producto", "Precio Unit", "Cant", "Subtotal", "Quitar"])
        self.tablaCarrito.setFont(Estilos.FUENTE_NORMAL)
        self.tablaCarrito.horizontalHeader().setFont(Estilos.FUENTE_XS)
        self.tablaCarrito.horizontalHeader().setStyleSheet(f"background-color: {Estilos.BG_ZINC_100.name()}; color: {Estilos.TEXTO_PRINCIPAL.name()};")
        self.tablaCarrito.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; color: {Estilos.TEXTO_PRINCIPAL.name()}; border: 1px solid {Estilos.BORDE.name()};")
        self.tablaCarrito.setColumnHidden(0, True)
        self.tablaCarrito.horizontalHeader().setSectionResizeMode(1, QHeaderView.ResizeMode.Stretch)
        self.tablaCarrito.setColumnWidth(2, 120)
        self.tablaCarrito.setColumnWidth(3, 80)
        self.tablaCarrito.setColumnWidth(4, 110)
        self.tablaCarrito.setColumnWidth(5, 100)
        self.tablaCarrito.cellChanged.connect(self.onCeldaCambiada)
        
        layoutIzq.addWidget(self.tablaCarrito)
        layout.addWidget(panelIzq, stretch=3)

        # Panel Cobro
        panelCobro = QWidget()
        panelCobro.setStyleSheet(f"background-color: {Estilos.BG_OSCURO.name()}; border: 1px solid #3F3F46;")
        panelCobro.setFixedWidth(250)
        layoutCobro = QVBoxLayout(panelCobro)
        layoutCobro.setContentsMargins(16, 16, 16, 16)

        lblResumen = QLabel("RESUMEN")
        lblResumen.setFont(Estilos.FUENTE_XS)
        lblResumen.setStyleSheet("color: #818CF8; border: none;")
        layoutCobro.addWidget(lblResumen)

        self.lblTotal = QLabel("$0.00")
        self.lblTotal.setFont(QFont("SansSerif", 38, QFont.Weight.Bold))
        self.lblTotal.setStyleSheet("color: #A5B4FC; border: none;")
        layoutCobro.addWidget(self.lblTotal)

        lblDescuento = QLabel("DESCUENTO ($ o %)")
        lblDescuento.setFont(Estilos.FUENTE_XS)
        lblDescuento.setStyleSheet("color: #71717A; border: none;")
        layoutCobro.addWidget(lblDescuento)

        self.txtDescuento = QLineEdit("0.00")
        self.txtDescuento.setFont(Estilos.FUENTE_BOLD)
        self.txtDescuento.setStyleSheet("""
            QLineEdit {
                background-color: #27272A;
                color: #34D399;
                border: 2px solid #3F3F46;
                padding: 6px 10px;
            }
        """)
        self.txtDescuento.textChanged.connect(self.aplicarDescuento)
        layoutCobro.addWidget(self.txtDescuento)

        lblMetodo = QLabel("METODO DE PAGO")
        lblMetodo.setFont(Estilos.FUENTE_XS)
        lblMetodo.setStyleSheet("color: #71717A; border: none;")
        layoutCobro.addWidget(lblMetodo)

        self.cmbMetodo = QComboBox()
        self.cmbMetodo.addItems(["Efectivo", "Tarjeta"])
        self.cmbMetodo.setFont(Estilos.FUENTE_BOLD)
        self.cmbMetodo.currentIndexChanged.connect(self.actualizarVisibilidadEfectivo)
        layoutCobro.addWidget(self.cmbMetodo)

        self.lblRecibido = QLabel("IMPORTE RECIBIDO")
        self.lblRecibido.setFont(Estilos.FUENTE_XS)
        self.lblRecibido.setStyleSheet("color: #71717A; border: none;")
        layoutCobro.addWidget(self.lblRecibido)

        self.txtRecibido = QLineEdit("0.00")
        self.txtRecibido.setFont(QFont("SansSerif", 22, QFont.Weight.Bold))
        self.txtRecibido.setStyleSheet("""
            QLineEdit {
                background-color: #27272A;
                color: white;
                border: 2px solid #3F3F46;
                padding: 8px 12px;
            }
        """)
        self.txtRecibido.textChanged.connect(self.actualizarCambio)
        layoutCobro.addWidget(self.txtRecibido)

        self.lblCambio = QLabel("Cambio: $0.00")
        self.lblCambio.setFont(Estilos.FUENTE_BOLD)
        self.lblCambio.setStyleSheet("color: #34D399; border: none;")
        layoutCobro.addWidget(self.lblCambio)

        layoutCobro.addStretch()

        self.lblImagenProducto = QLabel("NO IMAGEN")
        self.lblImagenProducto.setAlignment(Qt.AlignmentFlag.AlignCenter)
        self.lblImagenProducto.setFont(Estilos.FUENTE_BOLD)
        self.lblImagenProducto.setStyleSheet("color: #3F3F46; border: 1px solid #3F3F46;")
        self.lblImagenProducto.setFixedSize(218, 170)
        layoutCobro.addWidget(self.lblImagenProducto)

        layoutCobro.addSpacing(16)

        btnCobrar = QPushButton("COBRAR")
        btnCobrar.setFont(QFont("SansSerif", 16, QFont.Weight.Bold))
        btnCobrar.setCursor(Qt.CursorShape.PointingHandCursor)
        btnCobrar.setStyleSheet(f"""
            QPushButton {{
                background-color: {Estilos.INDIGO.name()};
                color: white;
                border: none;
                padding: 14px;
            }}
        """)
        btnCobrar.clicked.connect(self.procesarCobro)
        layoutCobro.addWidget(btnCobrar)

        layout.addWidget(panelCobro)

    def actualizarVisibilidadEfectivo(self):
        esEfectivo = self.cmbMetodo.currentText() == "Efectivo"
        self.lblRecibido.setVisible(esEfectivo)
        self.txtRecibido.setVisible(esEfectivo)
        self.lblCambio.setVisible(esEfectivo)
        if not esEfectivo:
            self.txtRecibido.setText("0.00")

    def aplicarDescuento(self):
        val = self.txtDescuento.text().strip()
        try:
            if val.endswith('%'):
                porcentaje = float(val.replace('%', '').strip())
                subtotal = self.ctrl.calcularSubtotal()
                self.ctrl.setDescuento(subtotal * (porcentaje / 100.0))
            else:
                self.ctrl.setDescuento(float(val) if val else 0.0)
        except ValueError:
            self.ctrl.setDescuento(0.0)

    def actualizarSugerencias(self):
        for i in reversed(range(self.sugerenciasLayout.count())): 
            widget = self.sugerenciasLayout.itemAt(i).widget()
            if widget is not None:
                widget.setParent(None)

        q = self.txtBusqueda.text().strip()
        if not q:
            self.sugerenciasArea.setVisible(False)
            return

        res = self.ctrl.buscarProductos(q)
        for p in res:
            btn = QPushButton(f"[{p.getId()}]  {p.getNombre().upper():<22}  ${p.getPrecio():.2f}  stock:{p.getStock():.0f}")
            btn.setFont(Estilos.FUENTE_SMALL)
            btn.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; color: {Estilos.TEXTO_PRINCIPAL.name()}; text-align: left; padding: 5px; border: none;")
            btn.setCursor(Qt.CursorShape.PointingHandCursor)
            
            def add_to_cart(checked, prod=p):
                self.ctrl.agregarAlCarrito(prod)
                self.mostrarImagen(prod.getImagenRuta())
                self.txtBusqueda.setText("")
                self.sugerenciasArea.setVisible(False)
            
            btn.clicked.connect(add_to_cart)
            self.sugerenciasLayout.addWidget(btn)

        self.sugerenciasArea.setVisible(len(res) > 0)

    def actualizarCambio(self):
        try:
            rec = float(self.txtRecibido.text().strip())
            self.lblCambio.setText(f"Cambio: ${self.ctrl.calcularCambio(rec):.2f}")
        except ValueError:
            pass

    def procesarCobro(self):
        if not self.ctrl.getCarrito():
            QMessageBox.information(self, "Aviso", "El carrito esta vacio")
            return
        
        metodo = self.cmbMetodo.currentText()
        recibido = 0.0
        if metodo == "Efectivo":
            try:
                recibido = float(self.txtRecibido.text().strip())
            except ValueError:
                pass
        else:
            recibido = self.ctrl.calcularTotal()
            
        self.ctrl.procesarCobro(metodo, recibido)

    def refrescarCarrito(self, carrito, total, descuento):
        self.actualizando = True
        self.tablaCarrito.setRowCount(len(carrito))
        for i, item in enumerate(carrito):
            self.tablaCarrito.setItem(i, 0, QTableWidgetItem(item.getProducto().getId()))
            self.tablaCarrito.setItem(i, 1, QTableWidgetItem(item.getProducto().getNombre()))
            
            item2 = QTableWidgetItem(f"${item.getProducto().getPrecio():.2f}")
            item2.setFlags(item2.flags() & ~Qt.ItemFlag.ItemIsEditable)
            self.tablaCarrito.setItem(i, 2, item2)
            
            cantStr = f"{item.getCantidad():.0f}" if item.getProducto().esPorPieza() else f"{item.getCantidad():.2f}"
            self.tablaCarrito.setItem(i, 3, QTableWidgetItem(cantStr))
            
            item4 = QTableWidgetItem(f"${item.getSubtotal():.2f}")
            item4.setFlags(item4.flags() & ~Qt.ItemFlag.ItemIsEditable)
            self.tablaCarrito.setItem(i, 4, item4)
            
            btnQuitar = QPushButton("Quitar")
            btnQuitar.setFont(Estilos.FUENTE_XS)
            btnQuitar.setStyleSheet(f"background-color: {Estilos.ROSE_CLARO.name()}; color: {Estilos.ROSE.name()}; border: none; margin: 2px;")
            btnQuitar.clicked.connect(lambda checked, prod_id=item.getProducto().getId(): self.ctrl.eliminarDelCarrito(prod_id))
            self.tablaCarrito.setCellWidget(i, 5, btnQuitar)
            
        self.actualizando = False
        self.lblTotal.setText(f"${total:.2f}")

    def onCeldaCambiada(self, row, col):
        if self.actualizando or col != 3:
            return
        try:
            prod_id = self.tablaCarrito.item(row, 0).text()
            val = float(self.tablaCarrito.item(row, 3).text().strip())
            if val <= 0:
                self.ctrl.eliminarDelCarrito(prod_id)
            else:
                self.ctrl.setCantidad(prod_id, val)
        except (ValueError, AttributeError):
            pass

    def limpiar(self):
        self.actualizando = True
        self.tablaCarrito.setRowCount(0)
        self.actualizando = False
        self.lblTotal.setText("$0.00")
        self.txtRecibido.setText("0.00")
        self.txtDescuento.setText("0.00")
        self.lblCambio.setText("Cambio: $0.00")
        self.txtBusqueda.setText("")
        self.sugerenciasArea.setVisible(False)
        self.cmbMetodo.setCurrentIndex(0)
        self.actualizarVisibilidadEfectivo()
        self.lblImagenProducto.setPixmap(QPixmap())
        self.lblImagenProducto.setText("")

    def mostrarImagen(self, ruta):
        if not ruta:
            self.lblImagenProducto.setPixmap(QPixmap())
            self.lblImagenProducto.setText("SIN FOTO")
            return
            
        from Util.RutaBase import RutaBase
        if not os.path.isabs(ruta):
            ruta = os.path.join(RutaBase.getRaiz(), ruta)
            
        if os.path.exists(ruta):
            pixmap = QPixmap(ruta)
            if not pixmap.isNull():
                self.lblImagenProducto.setPixmap(pixmap.scaled(218, 170, Qt.AspectRatioMode.KeepAspectRatio, Qt.TransformationMode.SmoothTransformation))
                self.lblImagenProducto.setText("")
            else:
                self.lblImagenProducto.setText("SIN FOTO")
        else:
            self.lblImagenProducto.setText("SIN FOTO")
