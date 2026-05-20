class ItemCarrito:
    def __init__(self, producto=None, cantidad=0.0):
        self._producto = producto
        self._cantidad = cantidad

    def getProducto(self):
        return self._producto

    def setProducto(self, producto):
        self._producto = producto

    def getCantidad(self):
        return self._cantidad

    def setCantidad(self, cantidad):
        self._cantidad = round(cantidad, 3)

    def getSubtotal(self):
        if self._producto is None:
            return 0.0
        return round(self._producto.getPrecio() * self._cantidad, 2)

    def incrementar(self, delta):
        self._cantidad = round(self._cantidad + delta, 3)

    def decrementar(self, delta):
        nueva = self._cantidad - delta
        self._cantidad = max(0.0, round(nueva, 3))