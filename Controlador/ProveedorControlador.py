from Modelo.CuentaPorPagar import CuentaPorPagar, Estado as EstadoCuenta
from Modelo.OrdenCompra import OrdenCompra, TipoPago
import time
from PyQt6.QtWidgets import QMessageBox

class ProveedorControlador:
    def __init__(self, app):
        self.app = app
        self.panel = None

    def setPanel(self, panel):
        self.panel = panel

    def guardarProveedor(self, p):
        if self.app.getProveedorBD().buscarPorId(p.getId()) is not None:
            self.app.getProveedorBD().actualizar(p)
        else:
            self.app.getProveedorBD().insertar(p)
        if self.panel is not None:
            self.panel.refrescarProveedores()

    def eliminarProveedor(self, id_prov):
        self.app.getProveedorBD().eliminar(id_prov)
        if self.panel is not None:
            self.panel.refrescarProveedores()

    def generarIdProveedor(self):
        return int(time.time() * 1000) % 100000

    def crearOrden(self, orden):
        self.app.getOrdenBD().insertar(orden)
        if self.panel is not None:
            self.panel.refrescarOrdenes()

    def recibirOrden(self, ordenId):
        orden = self.app.getOrdenBD().buscarPorId(ordenId)
        if orden is None or orden.getEstado().name != "PENDIENTE":
            return

        for item in orden.getItems():
            p = self.app.getProductoBD().buscarPorId(item.getProducto().getId())
            if p is not None:
                nuevoStock = p.getStock() + item.getCantidadSolicitada()
                self.app.getProductoBD().actualizarStock(p.getId(), nuevoStock)

        self.app.getOrdenBD().marcarRecibida(ordenId)

        if orden.getTipoPago() == TipoPago.CREDITO:
            cuenta = CuentaPorPagar(0, orden.getProveedor(), ordenId, orden.getTotal(), 0.0, orden.getTotal(), "7 dias", EstadoCuenta.PENDIENTE)
            self.app.getCuentaBD().insertar(cuenta)
        else:
            resp = QMessageBox.question(None, "confirmar pago", f"vas a pagar {orden.getTotal()} en efectivo", QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No)
            if resp == QMessageBox.StandardButton.Yes:
                self.app.registrarRetiro(f"pago de contado orden {orden.getId()}", orden.getTotal())

        if self.panel is not None:
            self.panel.refrescarOrdenes()
            self.panel.refrescarCuentas()
            if self.app.getVentanaPrincipal() is not None:
                self.app.getVentanaPrincipal().refrescarInventario()
                self.app.getVentanaPrincipal().refrescarCaja()

    def liquidarCuenta(self, cuentaId, monto):
        for c in self.app.getCuentaBD().obtenerTodas():
            if c.getId() == cuentaId:
                nuevoPagado = c.getPagado() + monto
                nuevoSaldo = c.getMontoTotal() - nuevoPagado
                nuevoEstado = "PAGADO" if nuevoSaldo <= 0 else "PARCIAL"
                
                self.app.getCuentaBD().aplicarPago(cuentaId, nuevoPagado, max(0.0, nuevoSaldo), nuevoEstado)
                self.app.registrarRetiro(f"pago deuda {c.getProveedor().getNombre()}", monto)
                
                if self.panel is not None:
                    self.panel.refrescarCuentas()
                    self.panel.refrescarOrdenes()
                break

    def getProveedores(self):
        return self.app.getProveedores()

    def getOrdenes(self):
        return self.app.getOrdenes()

    def getCuentas(self):
        return self.app.getCuentas()

    def getProductos(self):
        return self.app.getProductos()
