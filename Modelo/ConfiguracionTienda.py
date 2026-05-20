class ConfiguracionTienda:
    def __init__(self):
        self._nombre = ''
        self._sucursal = ''
        self._rfc = ''

    def getNombre(self):
        return self._nombre

    def setNombre(self, nombre):
        self._nombre = nombre

    def getSucursal(self):
        return self._sucursal

    def setSucursal(self, sucursal):
        self._sucursal = sucursal

    def getRfc(self):
        return self._rfc

    def setRfc(self, rfc):
        self._rfc = rfc
