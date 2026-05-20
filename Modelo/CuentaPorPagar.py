from enum import Enum
class Estado(Enum):
    PENDIENTE = 'PENDIENTE'
    PARCIAL = 'PARCIAL'
    PAGADO = 'PAGADO'

class CuentaPorPagar:
    def __init__(self):
        self._id = 0
        self._proveedor = None
        self._ordenId = 0
        self._montoTotal = 0.0
        self._pagado = 0.0
        self._saldo = 0.0
        self._vencimiento = ''
        self._estado = None

    def getId(self):
        return self._id

    def setId(self, id):
        self._id = id

    def getProveedor(self):
        return self._proveedor

    def setProveedor(self, proveedor):
        self._proveedor = proveedor

    def getOrdenId(self):
        return self._ordenId

    def setOrdenId(self, ordenId):
        self._ordenId = ordenId

    def getMontoTotal(self):
        return self._montoTotal

    def setMontoTotal(self, montoTotal):
        self._montoTotal = montoTotal

    def getPagado(self):
        return self._pagado

    def setPagado(self, pagado):
        self._pagado = pagado

    def getSaldo(self):
        return self._saldo

    def setSaldo(self, saldo):
        self._saldo = saldo

    def getVencimiento(self):
        return self._vencimiento

    def setVencimiento(self, vencimiento):
        self._vencimiento = vencimiento

    def getEstado(self):
        return self._estado

    def setEstado(self, estado):
        self._estado = estado
