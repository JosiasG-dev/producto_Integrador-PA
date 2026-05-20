from PyQt6.QtWidgets import (QWidget, QVBoxLayout, QHBoxLayout, QLabel, QPushButton, 
                             QTabWidget, QTableWidget, QTableWidgetItem, QHeaderView, 
                             QInputDialog, QMessageBox, QDialog)
from PyQt6.QtCore import Qt
from PyQt6.QtGui import QFont, QColor
from Vista.Estilos import Estilos
from Modelo.OrdenCompra import Estado as EstadoOrden, TipoPago
from Modelo.CuentaPorPagar import Estado as EstadoCuenta

class ProveedorPanel(QWidget):
    def __init__(self, ctrl, usuario):
        super().__init__()
        self.ctrl = ctrl
        self.usuario = usuario
        self.construir()

    def construir(self):
        self.setStyleSheet(f"background-color: {Estilos.BG_CLARO.name()};")
        layout = QVBoxLayout(self)
        layout.setContentsMargins(24, 24, 24, 24)
        layout.setSpacing(16)

        header = QWidget()
        header.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()}; border: 1px solid {Estilos.BORDE.name()};")
        h_layout = QVBoxLayout(header)
        h_layout.setContentsMargins(24, 20, 24, 20)
        tit = QLabel("ABASTECIMIENTO  —  Proveedores y Cuentas por Pagar")
        tit.setFont(Estilos.FUENTE_TITULO)
        tit.setStyleSheet("border: none;")
        h_layout.addWidget(tit)
        layout.addWidget(header)

        self.tabs = QTabWidget()
        self.tabs.setFont(Estilos.FUENTE_BOLD)
        self.tabs.addTab(self.construirTabProveedores(), "Directorio")
        self.tabs.addTab(self.construirTabOrdenes(), "Ordenes")
        self.tabs.addTab(self.construirTabCuentas(), "Deudas")
        
        layout.addWidget(self.tabs)

    def construirTabProveedores(self):
        p = QWidget()
        p.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()};")
        layout = QVBoxLayout(p)
        layout.setContentsMargins(12, 12, 12, 12)
        layout.setSpacing(8)

        toolbar = QHBoxLayout()
        toolbar.addStretch()
        btnNuevo = Estilos.botonPrimario("+ Alta Proveedor")
        btnNuevo.setFixedSize(160, 40)
        btnNuevo.clicked.connect(lambda: self.abrirFormularioProveedor(None))
        toolbar.addWidget(btnNuevo)
        layout.addLayout(toolbar)

        self.tablaProveedores = QTableWidget(0, 6)
        self.tablaProveedores.setHorizontalHeaderLabels(["ID", "Empresa", "Contacto", "Telefono", "Email", "Direccion"])
        self.tablaProveedores.horizontalHeader().setSectionResizeMode(QHeaderView.ResizeMode.Stretch)
        self.tablaProveedores.setColumnWidth(0, 60)
        self.tablaProveedores.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
        self.tablaProveedores.setSelectionBehavior(QTableWidget.SelectionBehavior.SelectRows)
        self.tablaProveedores.itemDoubleClicked.connect(self.onProveedorDoubleClick)
        
        layout.addWidget(self.tablaProveedores)
        return p

    def onProveedorDoubleClick(self, item):
        row = item.row()
        id_prov = int(self.tablaProveedores.item(row, 0).text())
        prov = next((x for x in self.ctrl.getProveedores() if x.getId() == id_prov), None)
        if prov:
            self.abrirFormularioProveedor(prov)

    def construirTabOrdenes(self):
        p = QWidget()
        p.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()};")
        layout = QVBoxLayout(p)
        layout.setContentsMargins(12, 12, 12, 12)
        layout.setSpacing(8)

        toolbar = QHBoxLayout()
        toolbar.addStretch()
        
        btnBitacora = Estilos.botonSecundario("Bitacora de Entradas")
        btnBitacora.setFixedSize(190, 40)
        btnBitacora.clicked.connect(self.mostrarBitacoraEntradas)
        toolbar.addWidget(btnBitacora)
        
        btnNueva = Estilos.botonExito("+ Generar Pedido")
        btnNueva.setFixedSize(170, 40)
        btnNueva.clicked.connect(self.abrirNuevaOrden)
        toolbar.addWidget(btnNueva)
        
        layout.addLayout(toolbar)

        self.tablaOrdenes = QTableWidget(0, 6)
        self.tablaOrdenes.setHorizontalHeaderLabels(["Folio", "Proveedor", "Total", "Pago", "Estado", "Accion"])
        self.tablaOrdenes.horizontalHeader().setSectionResizeMode(1, QHeaderView.ResizeMode.Stretch)
        self.tablaOrdenes.setColumnWidth(0, 100)
        self.tablaOrdenes.setColumnWidth(3, 100)
        self.tablaOrdenes.setColumnWidth(4, 110)
        self.tablaOrdenes.setColumnWidth(5, 110)
        
        layout.addWidget(self.tablaOrdenes)
        return p

    def recibir_orden_accion(self, oid):
        orden = next((o for o in self.ctrl.getOrdenes() if o.getId() == oid), None)
        if orden and orden.getEstado() == EstadoOrden.RECIBIDO:
            QMessageBox.information(self.window(), "Aviso", "Esta orden ya fue recibida.")
            return
        self.ctrl.recibirOrden(oid)
        self.refrescarOrdenes()
        self.refrescarCuentas()

    def construirTabCuentas(self):
        p = QWidget()
        p.setStyleSheet(f"background-color: {Estilos.BG_BLANCO.name()};")
        layout = QVBoxLayout(p)
        layout.setContentsMargins(12, 12, 12, 12)
        layout.setSpacing(8)

        subtit = QLabel("  Saldos pendientes con acreedores")
        subtit.setFont(Estilos.FUENTE_SMALL)
        subtit.setStyleSheet(f"color: {Estilos.TEXTO_TENUE.name()}; border: none;")
        layout.addWidget(subtit)

        self.tablaCuentas = QTableWidget(0, 7)
        self.tablaCuentas.setHorizontalHeaderLabels(["Proveedor", "Orden", "Total", "Pagado", "Saldo", "Estado", "Accion"])
        self.tablaCuentas.horizontalHeader().setSectionResizeMode(0, QHeaderView.ResizeMode.Stretch)
        self.tablaCuentas.setColumnWidth(5, 100)
        self.tablaCuentas.setColumnWidth(6, 120)
        
        layout.addWidget(self.tablaCuentas)
        return p

    def liquidar_cuenta_accion(self, cid):
        cuenta = next((c for c in self.ctrl.getCuentas() if c.getId() == cid), None)
        if cuenta and cuenta.getSaldo() > 0:
            monto, ok = QInputDialog.getDouble(self.window(), "Liquidar", f"Monto a liquidar (saldo: ${cuenta.getSaldo():.2f}):", cuenta.getSaldo(), 0.01, cuenta.getSaldo(), 2)
            if ok:
                self.ctrl.liquidarCuenta(cid, monto)

    def refrescarProveedores(self):
        self.tablaProveedores.setRowCount(0)
        for i, p in enumerate(self.ctrl.getProveedores()):
            self.tablaProveedores.insertRow(i)
            self.tablaProveedores.setItem(i, 0, QTableWidgetItem(str(p.getId())))
            self.tablaProveedores.setItem(i, 1, QTableWidgetItem(p.getNombre()))
            self.tablaProveedores.setItem(i, 2, QTableWidgetItem(p.getContacto()))
            self.tablaProveedores.setItem(i, 3, QTableWidgetItem(p.getTelefono()))
            self.tablaProveedores.setItem(i, 4, QTableWidgetItem(p.getEmail()))
            self.tablaProveedores.setItem(i, 5, QTableWidgetItem(p.getDireccion()))

    def refrescarOrdenes(self):
        self.tablaOrdenes.setRowCount(0)
        for o in self.ctrl.getOrdenes():
            if o.getEstado() == EstadoOrden.PENDIENTE:
                row = self.tablaOrdenes.rowCount()
                self.tablaOrdenes.insertRow(row)
                self.tablaOrdenes.setItem(row, 0, QTableWidgetItem(o.getFolioCorto() if hasattr(o, 'getFolioCorto') else str(o.getId())))
                self.tablaOrdenes.setItem(row, 1, QTableWidgetItem(o.getProveedor().getNombre()))
                self.tablaOrdenes.setItem(row, 2, QTableWidgetItem(f"${o.getTotal():.2f}"))
                self.tablaOrdenes.setItem(row, 3, QTableWidgetItem(o.getTipoPago().name))
                self.tablaOrdenes.setItem(row, 4, QTableWidgetItem(o.getEstado().name))
                
                btnRecibir = QPushButton("Recibir")
                btnRecibir.setStyleSheet(f"background-color: {Estilos.EMERALD_CLARO.name()}; color: {Estilos.EMERALD.name()}; border: none;")
                btnRecibir.clicked.connect(lambda checked, oid=o.getId(): self.recibir_orden_accion(oid))
                self.tablaOrdenes.setCellWidget(row, 5, btnRecibir)

    def refrescarCuentas(self):
        self.tablaCuentas.setRowCount(0)
        for c in self.ctrl.getCuentas():
            row = self.tablaCuentas.rowCount()
            self.tablaCuentas.insertRow(row)
            self.tablaCuentas.setItem(row, 0, QTableWidgetItem(c.getProveedor().getNombre()))
            self.tablaCuentas.setItem(row, 1, QTableWidgetItem(c.getFolioOrden() if hasattr(c, 'getFolioOrden') else str(c.getOrdenId())))
            self.tablaCuentas.setItem(row, 2, QTableWidgetItem(f"${c.getMontoTotal():.2f}"))
            self.tablaCuentas.setItem(row, 3, QTableWidgetItem(f"${c.getPagado():.2f}"))
            self.tablaCuentas.setItem(row, 4, QTableWidgetItem(f"${c.getSaldo():.2f}"))
            self.tablaCuentas.setItem(row, 5, QTableWidgetItem(c.getEstado().name))
            
            btnLiquidar = QPushButton("Liquidar")
            btnLiquidar.setStyleSheet(f"background-color: {Estilos.INDIGO_CLARO.name()}; color: {Estilos.INDIGO.name()}; border: none;")
            btnLiquidar.clicked.connect(lambda checked, cid=c.getId(): self.liquidar_cuenta_accion(cid))
            self.tablaCuentas.setCellWidget(row, 6, btnLiquidar)

    def mostrarBitacoraEntradas(self):
        ordenes = [o for o in self.ctrl.getOrdenes() if o.getEstado() == EstadoOrden.RECIBIDO]
        dlg = QDialog(self.window())
        dlg.setWindowTitle("Bitacora de Entradas de Mercancia")
        dlg.setFixedSize(860, 520)
        layout = QVBoxLayout(dlg)
        layout.setContentsMargins(20, 20, 20, 20)
        
        tit = QLabel("BITACORA DE ENTRADAS DE MERCANCIA")
        tit.setFont(Estilos.FUENTE_TITULO)
        layout.addWidget(tit)
        
        sub = QLabel(f"{len(ordenes)} recepciones registradas")
        layout.addWidget(sub)
        
        tabla = QTableWidget(0, 6)
        tabla.setHorizontalHeaderLabels(["Folio", "Proveedor", "Fecha Recepcion", "Tipo Pago", "Total", "Productos Recibidos"])
        tabla.horizontalHeader().setSectionResizeMode(5, QHeaderView.ResizeMode.Stretch)
        tabla.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
        
        for o in ordenes:
            row = tabla.rowCount()
            tabla.insertRow(row)
            tabla.setItem(row, 0, QTableWidgetItem(f"#{o.getId()}"))
            tabla.setItem(row, 1, QTableWidgetItem(o.getProveedor().getNombre()))
            fecha_str = o.getFecha().strftime("%d/%m/%Y %H:%M") if o.getFecha() else ""
            tabla.setItem(row, 2, QTableWidgetItem(fecha_str))
            tabla.setItem(row, 3, QTableWidgetItem(o.getTipoPago().name))
            tabla.setItem(row, 4, QTableWidgetItem(f"${o.getTotal():.2f}"))
            prods = ", ".join([f"{i.getProducto().getNombre()} x{int(i.getCantidadSolicitada())}" for i in o.getItems()])
            tabla.setItem(row, 5, QTableWidgetItem(prods))
            
        if not ordenes:
            tabla.insertRow(0)
            tabla.setItem(0, 1, QTableWidgetItem("Sin recepciones registradas"))
            
        layout.addWidget(tabla)
        btnCerrar = QPushButton("Cerrar")
        btnCerrar.clicked.connect(dlg.accept)
        layout.addWidget(btnCerrar, alignment=Qt.AlignmentFlag.AlignCenter)
        dlg.exec()

    def abrirFormularioProveedor(self, p):
        from Vista.ProveedorDialog import ProveedorDialog
        dlg = ProveedorDialog(self.window(), p, self.ctrl)
        dlg.exec()
        self.refrescarProveedores()

    def abrirNuevaOrden(self):
        from Vista.NuevaOrdenDialog import NuevaOrdenDialog
        dlg = NuevaOrdenDialog(self.window(), self.ctrl)
        dlg.exec()
        self.refrescarOrdenes()
