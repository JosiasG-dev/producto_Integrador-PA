from PyQt6.QtWidgets import (QDialog, QVBoxLayout, QHBoxLayout, QLabel, QLineEdit, QPushButton, 
                             QComboBox, QTableWidget, QTableWidgetItem, QHeaderView, QMessageBox, 
                             QWidget, QScrollArea, QInputDialog, QFrame)
from PyQt6.QtCore import Qt
from PyQt6.QtGui import QFont
from Vista.Estilos import Estilos
from Modelo.OrdenCompra import OrdenCompra, TipoPago, Estado as EstadoOrden
from datetime import datetime

class NuevaOrdenDialog(QDialog):
    def __init__(self, parent, ctrl):
        super().__init__(parent)
        self.ctrl = ctrl
        self.items = []
        self.setWindowTitle("Nueva Orden de Compra")
        self.setModal(True)
        self.construir()

    def construir(self):
        self.resize(740, 640)
        layout = QVBoxLayout(self)
        layout.setContentsMargins(0, 0, 0, 0)
        layout.setSpacing(0)

        panel = QWidget()
        panel.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()};")
        p_layout = QVBoxLayout(panel)
        p_layout.setContentsMargins(0, 0, 0, 0)
        p_layout.setSpacing(0)

        header = QWidget()
        header.setStyleSheet(f"background-color: {Estilos.BG_ZINC_100.name()}; border-bottom: 1px solid {Estilos.BORDE.name()};")
        h_layout = QVBoxLayout(header)
        h_layout.setContentsMargins(24, 20, 24, 20)
        tit = QLabel("NUEVA ORDEN DE COMPRA")
        tit.setFont(Estilos.FUENTE_TITULO)
        tit.setStyleSheet("border: none;")
        h_layout.addWidget(tit)
        p_layout.addWidget(header)

        form = QWidget()
        form.setStyleSheet("border: none;")
        form_layout = QHBoxLayout(form)
        form_layout.setContentsMargins(24, 20, 24, 12)
        form_layout.setSpacing(24)

        izq = QWidget()
        izq_layout = QVBoxLayout(izq)
        izq_layout.setContentsMargins(0, 0, 0, 0)
        
        izq_layout.addWidget(self.lbl("1. Proveedor Responsable"))
        self.cmbProveedor = QComboBox()
        self.proveedores_list = self.ctrl.getProveedores()
        self.cmbProveedor.addItems([p.getNombre() for p in self.proveedores_list])
        self.cmbProveedor.setFont(Estilos.FUENTE_BOLD)
        self.cmbProveedor.setFixedHeight(42)
        izq_layout.addWidget(self.cmbProveedor)
        
        izq_layout.addSpacing(14)
        izq_layout.addWidget(self.lbl("2. Terminos de Pago"))
        self.cmbTipoPago = QComboBox()
        self.cmbTipoPago.addItems(["CONTADO", "CREDITO"])
        self.cmbTipoPago.setFont(Estilos.FUENTE_BOLD)
        self.cmbTipoPago.setFixedHeight(42)
        izq_layout.addWidget(self.cmbTipoPago)
        
        form_layout.addWidget(izq)

        der = QWidget()
        der_layout = QVBoxLayout(der)
        der_layout.setContentsMargins(0, 0, 0, 0)
        
        der_layout.addWidget(self.lbl("3. Buscar y Agregar Productos"))
        self.txtBusquedaProd = QLineEdit()
        self.txtBusquedaProd.setFont(Estilos.FUENTE_BOLD)
        self.txtBusquedaProd.setStyleSheet(f"""
            QLineEdit {{
                background-color: {Estilos.BG_ZINC_100.name()};
                border: 2px solid {Estilos.BORDE.name()};
                padding: 10px 12px;
            }}
        """)
        self.txtBusquedaProd.setFixedHeight(42)
        der_layout.addWidget(self.txtBusquedaProd)
        
        der_layout.addSpacing(8)
        btnAgregar = Estilos.botonExito("Agregar")
        btnAgregar.setFixedSize(120, 36)
        btnAgregar.clicked.connect(self.buscarYAgregar)
        der_layout.addWidget(btnAgregar)
        
        form_layout.addWidget(der)
        p_layout.addWidget(form)

        self.tablaItems = QTableWidget(0, 6)
        self.tablaItems.setHorizontalHeaderLabels(["Producto", "Unidad", "Cant.", "Costo Unit.", "Subtotal", "Quitar"])
        self.tablaItems.setFont(Estilos.FUENTE_NORMAL)
        self.tablaItems.horizontalHeader().setFont(Estilos.FUENTE_XS)
        self.tablaItems.horizontalHeader().setStyleSheet(f"background-color: {Estilos.BG_ZINC_100.name()};")
        self.tablaItems.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; border: none;")
        self.tablaItems.verticalHeader().setDefaultSectionSize(40)
        
        self.tablaItems.horizontalHeader().setSectionResizeMode(0, QHeaderView.ResizeMode.Stretch)
        self.tablaItems.setColumnWidth(1, 120)
        self.tablaItems.setColumnWidth(2, 70)
        self.tablaItems.setColumnWidth(3, 110)
        self.tablaItems.setColumnWidth(4, 110)
        self.tablaItems.setColumnWidth(5, 90)
        
        self.tablaItems.itemDoubleClicked.connect(self.onTablaDoubleClick)
        
        p_layout.addWidget(self.tablaItems)

        footer = QWidget()
        footer.setStyleSheet(f"background-color: {Estilos.BG_ZINC_100.name()}; border-top: 1px solid {Estilos.BORDE.name()};")
        footer_layout = QHBoxLayout(footer)
        footer_layout.setContentsMargins(24, 16, 24, 16)
        
        self.lblTotal = QLabel("Inversion Total:  $0.00")
        self.lblTotal.setFont(QFont("SansSerif", 26, QFont.Weight.Bold))
        self.lblTotal.setStyleSheet(f"color: {Estilos.TEXTO_PRINCIPAL.name()}; border: none;")
        footer_layout.addWidget(self.lblTotal)
        
        footer_layout.addStretch()
        
        btnCancelar = Estilos.botonSecundario("Cancelar")
        btnCancelar.setFixedSize(110, 44)
        btnCancelar.clicked.connect(self.reject)
        footer_layout.addWidget(btnCancelar)
        
        btnConfirmar = Estilos.botonPrimario("Confirmar Orden")
        btnConfirmar.setFixedSize(180, 44)
        btnConfirmar.clicked.connect(self.confirmarOrden)
        footer_layout.addWidget(btnConfirmar)
        
        p_layout.addWidget(footer)
        layout.addWidget(panel)

    def onTablaDoubleClick(self, item):
        col = item.column()
        row = item.row()
        if col == 2:
            actual = self.tablaItems.item(row, 2).text()
            entrada, ok = QInputDialog.getText(self, "Nueva cantidad", "Nueva cantidad:", text=actual)
            if ok and entrada:
                try:
                    nueva = float(entrada.strip())
                    if nueva <= 0: return
                    item_orden = self.items[row]
                    if item_orden.getProducto().esPorPieza():
                        nueva = round(nueva)
                    item_orden.setCantidadSolicitada(nueva)
                    self.actualizarTablaItems()
                except ValueError:
                    pass

    def quitar_item_accion(self, row):
        if 0 <= row < len(self.items):
            self.items.pop(row)
            self.actualizarTablaItems()

    def buscarYAgregar(self):
        q = self.txtBusquedaProd.text().strip().lower()
        if not q: return
        
        res = []
        for p in self.ctrl.getProductos():
            if q in p.getNombre().lower():
                res.append(p)
            if len(res) >= 5:
                break
                
        if not res:
            QMessageBox.information(self, "Aviso", "No se encontraron productos.")
            return
            
        nombres = [p.getNombre() for p in res]
        sel_nombre, ok = QInputDialog.getItem(self, "Resultados", "Seleccione un producto:", nombres, 0, False)
        if not ok or not sel_nombre: return
        
        sel = next(p for p in res if p.getNombre() == sel_nombre)
        
        unidad = sel.getUnidad()
        mensajeCant = f"Cantidad a pedir (Piezas):" if sel.esPorPieza() else f"Cantidad a pedir ({unidad}):"
        entradaCant, ok2 = QInputDialog.getText(self, "Cantidad", mensajeCant, text="1" if sel.esPorPieza() else "1.0")
        if not ok2 or not entradaCant: return
        
        try:
            cantidad = float(entradaCant.strip())
            if cantidad <= 0: return
            if sel.esPorPieza():
                cantidad = round(cantidad)
        except ValueError:
            QMessageBox.critical(self, "Error", "Cantidad invalida")
            return
            
        for item in self.items:
            if item.getProducto().getId() == sel.getId():
                item.setCantidadSolicitada(item.getCantidadSolicitada() + cantidad)
                self.actualizarTablaItems()
                self.txtBusquedaProd.setText("")
                return
                
        # Inner class ItemOrden import requires nested usage or fully qualified
        from Modelo.OrdenCompra import ItemOrden
        self.items.append(ItemOrden(sel, cantidad, sel.getPrecio() * 0.75))
        self.actualizarTablaItems()
        self.txtBusquedaProd.setText("")

    def actualizarTablaItems(self):
        self.tablaItems.setRowCount(0)
        total = 0.0
        for i, item in enumerate(self.items):
            self.tablaItems.insertRow(i)
            p = item.getProducto()
            cantStr = f"{item.getCantidadSolicitada():.0f}" if p.esPorPieza() else f"{item.getCantidadSolicitada():.2f}"
            
            item0 = QTableWidgetItem(p.getNombre().upper())
            item0.setFlags(item0.flags() & ~Qt.ItemFlag.ItemIsEditable)
            self.tablaItems.setItem(i, 0, item0)
            
            item1 = QTableWidgetItem(p.getUnidad())
            item1.setFlags(item1.flags() & ~Qt.ItemFlag.ItemIsEditable)
            self.tablaItems.setItem(i, 1, item1)
            
            item2 = QTableWidgetItem(cantStr)
            item2.setFlags(item2.flags() & ~Qt.ItemFlag.ItemIsEditable)
            self.tablaItems.setItem(i, 2, item2)
            
            item3 = QTableWidgetItem(f"${item.getPrecioCosto():.2f}")
            item3.setFlags(item3.flags() & ~Qt.ItemFlag.ItemIsEditable)
            self.tablaItems.setItem(i, 3, item3)
            
            item4 = QTableWidgetItem(f"${item.getSubtotal():.2f}")
            item4.setFlags(item4.flags() & ~Qt.ItemFlag.ItemIsEditable)
            self.tablaItems.setItem(i, 4, item4)
            
            btnQuitar = QPushButton("Quitar")
            btnQuitar.setStyleSheet(f"background-color: {Estilos.ROSE_CLARO.name()}; color: {Estilos.ROSE.name()}; border: none; font-size: 11px;")
            btnQuitar.clicked.connect(lambda checked, row=i: self.quitar_item_accion(row))
            self.tablaItems.setCellWidget(i, 5, btnQuitar)
            
            total += item.getSubtotal()
            
        self.lblTotal.setText(f"Inversion Total:  ${total:.2f}")

    def confirmarOrden(self):
        if not self.items:
            QMessageBox.warning(self, "Aviso", "Agregue al menos un producto.")
            return
            
        idx = self.cmbProveedor.currentIndex()
        if idx < 0: return
        prov = self.proveedores_list[idx]
        
        tipoPago = TipoPago.CREDITO if self.cmbTipoPago.currentText() == "CREDITO" else TipoPago.CONTADO
        total = sum(item.getSubtotal() for item in self.items)
        
        orden = OrdenCompra(0, prov, list(self.items), total, tipoPago, EstadoOrden.PENDIENTE, datetime.now())
        self.ctrl.crearOrden(orden)
        QMessageBox.information(self, "Exito", f"Orden {orden.getFolioCorto()} generada correctamente.")
        self.accept()

    def lbl(self, t):
        l = QLabel(t.upper())
        l.setFont(Estilos.FUENTE_XS)
        l.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        return l
