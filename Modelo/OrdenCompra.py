from enum import Enum

class Estado(Enum):
    PENDIENTE = 'PENDIENTE'
    RECIBIDO = 'RECIBIDO'

class TipoPago(Enum):
    CONTADO = 'CONTADO'
    CREDITO = 'CREDITO'

class ItemOrden:
    def __init__(self, producto=None, cantidadSolicitada=0.0, precioCosto=0.0):
        self._producto = producto
        self._cantidadSolicitada = cantidadSolicitada
        self._precioCosto = precioCosto

    def getProducto(self):
        return self._producto

    def setProducto(self, producto):
        self._producto = producto

    def getCantidadSolicitada(self):
        return self._cantidadSolicitada

    def setCantidadSolicitada(self, cantidadSolicitada):
        self._cantidadSolicitada = cantidadSolicitada

    def getPrecioCosto(self):
        return self._precioCosto

    def setPrecioCosto(self, precioCosto):
        self._precioCosto = precioCosto

class OrdenCompra:
    ItemOrden = ItemOrden

    def __init__(self, id=0, proveedor=None, items=None, total=0.0, tipoPago=None, estado=None, fecha=None):
        self._id = id
        self._proveedor = proveedor
        self._items = items if items is not None else []
        self._total = total
        self._tipoPago = tipoPago
        self._estado = estado
        self._fecha = fecha

    def getId(self):
        return self._id

    def setId(self, id):
        self._id = id

    def getProveedor(self):
        return self._proveedor

    def setProveedor(self, proveedor):
        self._proveedor = proveedor

    def getItems(self):
        return self._items

    def setItems(self, items):
        self._items = items

    def getTotal(self):
        return self._total

    def setTotal(self, total):
        self._total = total

    def getTipoPago(self):
        return self._tipoPago

    def setTipoPago(self, tipoPago):
        self._tipoPago = tipoPago

    def getEstado(self):
        return self._estado

    def setEstado(self, estado):
        self._estado = estado

    def getFecha(self):
        return self._fecha

    def setFecha(self, fecha):
        self._fecha = fecha
