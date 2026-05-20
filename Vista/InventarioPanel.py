from PyQt6.QtWidgets import (QWidget, QVBoxLayout, QHBoxLayout, QLabel, QLineEdit, 
                             QPushButton, QComboBox, QTableWidget, QTableWidgetItem, 
                             QHeaderView, QMessageBox, QDialog, QScrollArea)
from PyQt6.QtCore import Qt, QSize
from PyQt6.QtGui import QFont, QColor, QIcon, QPixmap
from Vista.Estilos import Estilos
import os
from Modelo.DatosIniciales import DatosIniciales
from Util.ManejoErrores import ManejoErrores
from Util.RutaBase import RutaBase

class InventarioPanel(QWidget):
    def __init__(self, ctrl, usuario):
        super().__init__()
        self.ctrl = ctrl
        self.usuarioActivo = usuario
        self.cache_imagenes = {}
        self.construir()

    def construir(self):
        self.setStyleSheet(f"background-color: {Estilos.BG_CLARO.name()};")
        layout = QVBoxLayout(self)
        layout.setContentsMargins(24, 16, 24, 16)
        layout.setSpacing(16)

        header = QWidget()
        h_layout = QHBoxLayout(header)
        h_layout.setContentsMargins(0, 0, 0, 0)
        
        titulo = QLabel("CATALOGO — Administracion de Stock")
        titulo.setFont(Estilos.FUENTE_TITULO)
        titulo.setStyleSheet(f"color: {Estilos.TEXTO_PRINCIPAL.name()}; border: none;")
        h_layout.addWidget(titulo)
        h_layout.addStretch()

        if self.usuarioActivo.esAdmin():
            btnDevolver = Estilos.botonPeligro("Devolver")
            btnDevolver.setFixedSize(90, 36)
            btnDevolver.clicked.connect(self.abrirDevolucionProveedor)
            h_layout.addWidget(btnDevolver)

            btnEntrada = Estilos.botonSecundario("Entrada")
            btnEntrada.setFixedSize(90, 36)
            btnEntrada.clicked.connect(self.abrirEntradaRapida)
            h_layout.addWidget(btnEntrada)

        btnBajoStock = Estilos.botonSecundario("Bajo Stock")
        btnBajoStock.setFixedSize(130, 36)
        btnBajoStock.clicked.connect(self.mostrarBajoStock)
        h_layout.addWidget(btnBajoStock)

        if self.usuarioActivo.esAdmin():
            btnNuevo = Estilos.botonPrimario("+ Nuevo Item")
            btnNuevo.setFixedSize(130, 36)
            btnNuevo.clicked.connect(lambda: self.abrirFormulario(None))
            h_layout.addWidget(btnNuevo)

        layout.addWidget(header)

        filtros = QWidget()
        f_layout = QHBoxLayout(filtros)
        f_layout.setContentsMargins(0, 0, 0, 0)

        lblBusq = QLabel("BUSCAR:")
        lblBusq.setFont(Estilos.FUENTE_XS)
        lblBusq.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        f_layout.addWidget(lblBusq)

        self.txtBusqueda = Estilos.campo()
        self.txtBusqueda.setFixedWidth(280)
        self.txtBusqueda.textChanged.connect(self.refrescar)
        f_layout.addWidget(self.txtBusqueda)

        f_layout.addSpacing(16)

        lblCat = QLabel("CATEGORIA:")
        lblCat.setFont(Estilos.FUENTE_XS)
        lblCat.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        f_layout.addWidget(lblCat)

        cats = ["Todas"] + list(DatosIniciales.getCategorias())
        self.cmbCategoria = QComboBox()
        self.cmbCategoria.addItems(cats)
        self.cmbCategoria.setFont(Estilos.FUENTE_BOLD)
        self.cmbCategoria.setFixedWidth(200)
        self.cmbCategoria.currentIndexChanged.connect(self.refrescar)
        f_layout.addWidget(self.cmbCategoria)
        
        f_layout.addStretch()
        layout.addWidget(filtros)

        self.tabla = QTableWidget(0, 8)
        self.tabla.setHorizontalHeaderLabels(["SKU", "Foto", "Producto", "Categoria", "Precio", "Existencia", "Stock Min", "Estado"])
        self.tabla.setFont(Estilos.FUENTE_NORMAL)
        self.tabla.horizontalHeader().setFont(Estilos.FUENTE_XS)
        self.tabla.horizontalHeader().setStyleSheet(f"background-color: {Estilos.BG_ZINC_100.name()};")
        self.tabla.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; border: 1px solid {Estilos.BORDE.name()};")
        self.tabla.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
        self.tabla.setSelectionBehavior(QTableWidget.SelectionBehavior.SelectRows)
        self.tabla.setIconSize(QSize(45, 45))
        self.tabla.verticalHeader().setDefaultSectionSize(55)

        self.tabla.horizontalHeader().setSectionResizeMode(2, QHeaderView.ResizeMode.Stretch)
        self.tabla.setColumnWidth(0, 70)
        self.tabla.setColumnWidth(1, 60)
        self.tabla.setColumnWidth(4, 100)
        self.tabla.setColumnWidth(5, 90)
        self.tabla.setColumnWidth(6, 80)
        self.tabla.setColumnWidth(7, 80)

        self.tabla.itemDoubleClicked.connect(self.onTablaDoubleClick)
        layout.addWidget(self.tabla)
        self.refrescar()

    def abrirDevolucionProveedor(self):
        row = self.tabla.currentRow()
        if row < 0:
            QMessageBox.information(self, "Aviso", "Seleccione un producto de la tabla")
            return
        prod_id = self.tabla.item(row, 0).text()
        from Vista.DevolucionProveedorDialog import DevolucionProveedorDialog
        dlg = DevolucionProveedorDialog(self.window(), self.ctrl.buscarPorId(prod_id), self.ctrl)
        dlg.exec()
        self.refrescar()

    def abrirEntradaRapida(self):
        row = self.tabla.currentRow()
        if row < 0:
            QMessageBox.information(self, "Aviso", "Seleccione un producto de la tabla")
            return
        prod_id = self.tabla.item(row, 0).text()
        from Vista.EntradaRapidaDialog import EntradaRapidaDialog
        dlg = EntradaRapidaDialog(self.window(), self.ctrl.buscarPorId(prod_id), self.ctrl)
        dlg.exec()
        self.refrescar()

    def refrescar(self):
        texto = self.txtBusqueda.text().strip()
        cat = self.cmbCategoria.currentText() if self.cmbCategoria.currentIndex() > 0 else ""
        lista = self.ctrl.filtrar(texto, cat)
        self.tabla.setRowCount(len(lista))
        
        for i, p in enumerate(lista):
            self.tabla.setItem(i, 0, QTableWidgetItem(p.getId()))
            
            foto_item = QTableWidgetItem()
            ruta = p.getImagenRuta()
            if ruta:
                if ruta not in self.cache_imagenes:
                    r = ruta if os.path.isabs(ruta) else os.path.join(RutaBase.getRaiz(), ruta)
                    if os.path.exists(r):
                        self.cache_imagenes[ruta] = QIcon(r)
                    else:
                        self.cache_imagenes[ruta] = QIcon()
                foto_item.setIcon(self.cache_imagenes[ruta])
            else:
                foto_item.setText("Sin foto")
            
            self.tabla.setItem(i, 1, foto_item)
            self.tabla.setItem(i, 2, QTableWidgetItem(p.getNombre().upper()))
            self.tabla.setItem(i, 3, QTableWidgetItem(p.getCategoria()))
            self.tabla.setItem(i, 4, QTableWidgetItem(f"${p.getPrecio():.2f}"))
            self.tabla.setItem(i, 5, QTableWidgetItem(f"{p.getStock():.1f}"))
            self.tabla.setItem(i, 6, QTableWidgetItem(f"{p.getStockMinimo():.0f}"))
            
            estado_str = "BAJO" if p.stockBajo() else "OK"
            estado_item = QTableWidgetItem(estado_str)
            if p.stockBajo():
                estado_item.setForeground(Estilos.ROSE)
            self.tabla.setItem(i, 7, estado_item)

    def mostrarBajoStock(self):
        todos = self.ctrl.filtrar("", "")
        bajos = sorted([p for p in todos if p.stockBajo()], key=lambda x: x.getStock() - x.getStockMinimo())
        
        dlg = QDialog(self.window())
        dlg.setWindowTitle("Reporte de Productos con Bajo Stock")
        dlg.setModal(True)
        dlg.setFixedSize(720, 480)
        
        layout = QVBoxLayout(dlg)
        layout.setContentsMargins(20, 20, 20, 20)
        
        tit = QLabel("PRODUCTOS CON STOCK BAJO")
        tit.setFont(Estilos.FUENTE_TITULO)
        layout.addWidget(tit)
        
        txt = "Todo en orden — ningun producto por debajo del minimo." if not bajos else f"{len(bajos)} producto(s) requieren reposicion."
        sub = QLabel(txt)
        sub.setFont(Estilos.FUENTE_NORMAL)
        sub.setStyleSheet(f"color: {Estilos.EMERALD.name() if not bajos else Estilos.ROSE.name()};")
        layout.addWidget(sub)
        
        tabla_bajos = QTableWidget(0, 6)
        tabla_bajos.setHorizontalHeaderLabels(["SKU", "Nombre", "Categoria", "Stock Actual", "Stock Minimo", "Faltante"])
        tabla_bajos.horizontalHeader().setSectionResizeMode(1, QHeaderView.ResizeMode.Stretch)
        tabla_bajos.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
        
        for p in bajos:
            faltante = max(0, p.getStockMinimo() - p.getStock())
            row = tabla_bajos.rowCount()
            tabla_bajos.insertRow(row)
            tabla_bajos.setItem(row, 0, QTableWidgetItem(p.getId()))
            tabla_bajos.setItem(row, 1, QTableWidgetItem(p.getNombre()))
            tabla_bajos.setItem(row, 2, QTableWidgetItem(p.getCategoria()))
            item_stock = QTableWidgetItem(f"{p.getStock():.1f}")
            if p.getStock() <= 0:
                item_stock.setBackground(QColor(255, 230, 230))
            tabla_bajos.setItem(row, 3, item_stock)
            tabla_bajos.setItem(row, 4, QTableWidgetItem(f"{p.getStockMinimo():.0f}"))
            tabla_bajos.setItem(row, 5, QTableWidgetItem(f"{faltante:.1f}"))
            
        if not bajos:
            tabla_bajos.insertRow(0)
            tabla_bajos.setItem(0, 0, QTableWidgetItem("-"))
            tabla_bajos.setItem(0, 1, QTableWidgetItem("Todos los productos tienen stock suficiente"))
            
        layout.addWidget(tabla_bajos)
        
        btn_layout = QHBoxLayout()
        btn_layout.addStretch()
        btnCerrar = Estilos.botonPrimario("Cerrar")
        btnCerrar.setFixedSize(120, 36)
        btnCerrar.clicked.connect(dlg.accept)
        btn_layout.addWidget(btnCerrar)
        btn_layout.addStretch()
        
        layout.addLayout(btn_layout)
        
        ManejoErrores.registrarInfo(f"Reporte bajo stock: {len(bajos)} productos")
        dlg.exec()

    def onTablaDoubleClick(self, item):
        if self.usuarioActivo.esAdmin():
            row = item.row()
            prod_id = self.tabla.item(row, 0).text()
            self.abrirFormulario(self.ctrl.buscarPorId(prod_id))

    def abrirFormulario(self, producto):
        from Vista.ProductoDialog import ProductoDialog
        dlg = ProductoDialog(self.window(), producto, self.ctrl)
        dlg.exec()
        self.refrescar()
