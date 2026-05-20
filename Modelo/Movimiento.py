from enum import Enum

class Tipo(Enum):
    VENTA = 'VENTA'
    RETIRO = 'RETIRO'
    FONDO_INICIAL = 'FONDO_INICIAL'

class Movimiento:
    def __init__(self, tipo=None, descripcion='', monto=0.0, fecha=None, usuario=''):
        self._tipo = tipo
        self._descripcion = descripcion
        self._monto = monto
        self._fecha = fecha
        self._usuario = usuario

    def getTipo(self):
        return self._tipo

    def setTipo(self, tipo):
        self._tipo = tipo

    def getDescripcion(self):
        return self._descripcion

    def setDescripcion(self, descripcion):
        self._descripcion = descripcion

    def getMonto(self):
        return self._monto

    def setMonto(self, monto):
        self._monto = monto

    def getFecha(self):
        return self._fecha

    def setFecha(self, fecha):
        self._fecha = fecha

    def getUsuario(self):
        return self._usuario

    def setUsuario(self, usuario):
        self._usuario = usuario

    def esIngreso(self):
        # We need to support enum checks. Sometimes in Python, self._tipo is the enum member or its value.
        # It's safest to check both.
        tipo_val = self._tipo
        if hasattr(tipo_val, 'name'):
            tipo_val = tipo_val.name
        
        return tipo_val == 'VENTA' or tipo_val == 'FONDO_INICIAL' or tipo_val == Tipo.VENTA or tipo_val == Tipo.FONDO_INICIAL
