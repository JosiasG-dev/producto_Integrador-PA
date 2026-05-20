from Modelo.ItemCarrito import ItemCarrito
from Modelo.Venta import Venta
from datetime import datetime
from PyQt6.QtWidgets import QMessageBox

class VentaControlador:
    def __init__(self, app):
        self.app = app
        self.panel = None
        self.carrito = []
        self.descuento = 0.0

    def setPanel(self, panel):
        self.panel = panel

    def buscarProductos(self, texto):
        if not texto or not str(texto).strip():
            return []
        return self.app.getProductoBD().buscar(texto)

    def agregarAlCarrito(self, p):
        for item in self.carrito:
            if item.getProducto().getId() == p.getId():
                nuevaCant = item.getCantidad() + 1.0
                if nuevaCant > p.getStock():
                    QMessageBox.warning(None, "Sin stock", f"Stock insuficiente. Disponible: {int(p.getStock())}")
                    return
                item.setCantidad(nuevaCant)
                self.notificarPanel()
                return
        
        if p.getStock() <= 0:
            QMessageBox.warning(None, "Sin stock", "El producto no tiene existencia")
            return
            
        item = ItemCarrito()
        item.setProducto(p)
        item.setCantidad(1.0)
        self.carrito.append(item)
        self.notificarPanel()

    def eliminarDelCarrito(self, productoId):
        self.carrito = [i for i in self.carrito if i.getProducto().getId() != productoId]
        self.notificarPanel()

    def setCantidad(self, productoId, nuevaCantidad):
        for item in self.carrito:
            if item.getProducto().getId() == productoId:
                nuevaCantidad = round(nuevaCantidad)
                if nuevaCantidad > item.getProducto().getStock():
                    QMessageBox.warning(None, "Sin stock", f"Stock insuficiente. Disponible: {int(item.getProducto().getStock())}")
                    return
                item.setCantidad(nuevaCantidad)
                break
        self.notificarPanel()

    def setDescuento(self, descuento):
        self.descuento = max(0.0, descuento)
        self.notificarPanel()

    def getDescuento(self):
        return self.descuento

    def calcularSubtotal(self):
        return sum(item.getProducto().getPrecio() * item.getCantidad() for item in self.carrito)

    def calcularTotal(self):
        sub = self.calcularSubtotal()
        return round(max(0.0, sub - self.descuento), 2)

    def calcularCambio(self, recibido):
        return max(0.0, recibido - self.calcularTotal())

    def procesarCobro(self, metodoPago, montoRecibido):
        if not self.carrito:
            return
        total = self.calcularTotal()
        if montoRecibido < total and str(metodoPago).lower() == "efectivo":
            QMessageBox.warning(None, "Importe insuficiente", f"El importe recibido (${montoRecibido:.2f}) es menor al total (${total:.2f})")
            return
            
        venta = Venta()
        venta.setId(0)
        venta.setItems(list(self.carrito))
        venta.setTotal(total)
        venta.setDescuento(self.descuento)
        venta.setMetodoPago(metodoPago)
        venta.setFecha(datetime.now())
        venta.setCajero(self.app.getUsuarioActivo().getNombre() if self.app.getUsuarioActivo() else "SISTEMA")
        
        self.app.registrarVenta(venta)
        cambio = self.calcularCambio(montoRecibido)
        
        from Vista.TicketDialog import TicketDialog
        parent_frame = self.app.getVentanaPrincipal().getFrame() if self.app.getVentanaPrincipal() and hasattr(self.app.getVentanaPrincipal(), 'getFrame') else None
        ticket = TicketDialog(parent_frame, venta, self.app.getConfig(), cambio, self.descuento, montoRecibido)
        ticket.mostrar()
        
        self.carrito.clear()
        self.descuento = 0.0
        if self.panel is not None:
            self.panel.limpiar()

    def notificarPanel(self):
        if self.panel is not None:
            self.panel.refrescarCarrito(self.carrito, self.calcularTotal(), self.descuento)

    def getCarrito(self):
        return self.carrito
