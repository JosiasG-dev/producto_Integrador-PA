class Devolucion:
    def __init__(self):
        self._id = 0
        self._ventaId = 0
        self._producto = None
        self._cantidad = 0.0
        self._motivo = ''
        self._fecha = None
        self._cajero = ''

    def getId(self):
        return self._id

    def setId(self, id):
        self._id = id

    def getVentaId(self):
        return self._ventaId

    def setVentaId(self, ventaId):
        self._ventaId = ventaId

    def getProducto(self):
        return self._producto

    def setProducto(self, producto):
        self._producto = producto

    def getCantidad(self):
        return self._cantidad

    def setCantidad(self, cantidad):
        self._cantidad = cantidad

    def getMotivo(self):
        return self._motivo

    def setMotivo(self, motivo):
        self._motivo = motivo

    def getFecha(self):
        return self._fecha

    def setFecha(self, fecha):
        self._fecha = fecha

    def getCajero(self):
        return self._cajero

    def setCajero(self, cajero):
        self._cajero = cajero
