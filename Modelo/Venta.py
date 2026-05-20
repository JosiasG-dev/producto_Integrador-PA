class Venta:
    def __init__(self):
        self._id = 0
        self._items = []
        self._total = 0.0
        self._descuento = 0.0
        self._metodoPago = ''
        self._fecha = None
        self._cajero = ''

    def getId(self):
        return self._id

    def setId(self, id):
        self._id = id

    def getItems(self):
        return self._items

    def setItems(self, items):
        self._items = items

    def getTotal(self):
        return self._total

    def setTotal(self, total):
        self._total = total

    def getDescuento(self):
        return self._descuento

    def setDescuento(self, descuento):
        self._descuento = descuento

    def getMetodoPago(self):
        return self._metodoPago

    def setMetodoPago(self, metodoPago):
        self._metodoPago = metodoPago

    def getFecha(self):
        return self._fecha

    def setFecha(self, fecha):
        self._fecha = fecha

    def getCajero(self):
        return self._cajero

    def setCajero(self, cajero):
        self._cajero = cajero
