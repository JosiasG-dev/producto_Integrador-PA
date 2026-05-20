class Producto:
    def __init__(self, id='', nombre='', precio=0.0, stock=0.0, categoria='', unidad='', imagenRuta=''):
        self._id = id
        self._nombre = nombre
        self._precio = precio
        self._stock = stock
        self._stockMinimo = 5.0
        self._categoria = categoria
        self._unidad = unidad
        self._imagenRuta = imagenRuta

    def getId(self):
        return self._id

    def setId(self, id):
        self._id = id

    def getNombre(self):
        return self._nombre

    def setNombre(self, nombre):
        self._nombre = nombre

    def getPrecio(self):
        return self._precio

    def setPrecio(self, precio):
        self._precio = precio

    def getStock(self):
        return self._stock

    def setStock(self, stock):
        self._stock = stock

    def getStockMinimo(self):
        return self._stockMinimo

    def setStockMinimo(self, stockMinimo):
        self._stockMinimo = stockMinimo

    def getCategoria(self):
        return self._categoria

    def setCategoria(self, categoria):
        self._categoria = categoria

    def getUnidad(self):
        return self._unidad

    def setUnidad(self, unidad):
        self._unidad = unidad

    def getImagenRuta(self):
        if not self._imagenRuta or not self._imagenRuta.strip():
            try:
                numericId = int(self._id)
                return f"Images/{numericId}.jpg"
            except Exception:
                return f"Images/{self._id}.jpg"
        return self._imagenRuta

    def setImagenRuta(self, imagenRuta):
        self._imagenRuta = imagenRuta

    def esPorPieza(self):
        return self._unidad == "Piezas"

    def stockBajo(self):
        return self._stock <= self._stockMinimo

    def reducirStock(self, cantidad):
        self._stock = max(0.0, self._stock - cantidad)
        self._stock = round(self._stock, 3)

    def aumentarStock(self, cantidad):
        self._stock += cantidad
        self._stock = round(self._stock, 3)

    def __str__(self):
        return f"{self._nombre} - ${self._precio:.2f} [Stock: {self._stock}]"