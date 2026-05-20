class Proveedor:
    def __init__(self, id=0, nombre='', contacto='', telefono='', email='', direccion='', activo=False):
        self._id = id
        self._nombre = nombre
        self._contacto = contacto
        self._telefono = telefono
        self._email = email
        self._direccion = direccion
        self._activo = activo

    def getId(self):
        return self._id

    def setId(self, id):
        self._id = id

    def getNombre(self):
        return self._nombre

    def setNombre(self, nombre):
        self._nombre = nombre

    def getContacto(self):
        return self._contacto

    def setContacto(self, contacto):
        self._contacto = contacto

    def getTelefono(self):
        return self._telefono

    def setTelefono(self, telefono):
        self._telefono = telefono

    def getEmail(self):
        return self._email

    def setEmail(self, email):
        self._email = email

    def getDireccion(self):
        return self._direccion

    def setDireccion(self, direccion):
        self._direccion = direccion

    def isActivo(self):
        return self._activo

    def setActivo(self, activo):
        self._activo = activo

    def __str__(self):
        return self._nombre