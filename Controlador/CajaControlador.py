from Modelo.Movimiento import Movimiento, Tipo as TipoMovimiento
from datetime import datetime

class CajaControlador:
    def __init__(self, app):
        self.app = app
        self.panel = None

    def setPanel(self, panel):
        self.panel = panel

    def abrirCaja(self, fondo):
        self.app.abrirCaja(fondo)
        if self.panel is not None:
            self.panel.refrescar()

    def registrarRetiro(self, concepto, monto):
        self.app.registrarRetiro(concepto, monto)
        if self.panel is not None:
            self.panel.refrescar()

    def registrarIngresoExtra(self, concepto, monto):
        m = Movimiento()
        m.setTipo(TipoMovimiento.VENTA)
        m.setDescripcion(concepto)
        m.setMonto(monto)
        m.setFecha(datetime.now())
        m.setUsuario(self.app.getUsuarioActivo().getNombre() if self.app.getUsuarioActivo() else "SISTEMA")
        self.app.getMovimientoBD().insertar(m)
        self.app.registrarVentaSimple(monto)
        if self.panel is not None:
            self.panel.refrescar()

    def getTotalVentas(self):
        return sum(m.getMonto() for m in self.app.getMovimientos() if m.getTipo() == TipoMovimiento.VENTA)

    def getTotalEgresos(self):
        return sum(m.getMonto() for m in self.app.getMovimientos() if m.getTipo() == TipoMovimiento.RETIRO)

    def getEfectivoEsperado(self):
        return self.app.getMontoCaja()

    def getMovimientos(self):
        return self.app.getMovimientos()

    def isCajaAbierta(self):
        return self.app.isCajaAbierta()

    def getMontoCaja(self):
        return self.app.getMontoCaja()
