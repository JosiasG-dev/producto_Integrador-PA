class Usuario:
    def __init__(self, id=0, username='', password='', rol='', nombre='', edad=0, sexo=''):
        self._id = id
        self._username = username
        self._password = password
        self._rol = rol
        self._nombre = nombre
        self._edad = edad
        self._sexo = sexo

    def getId(self):
        return self._id

    def setId(self, id):
        self._id = id

    def getUsername(self):
        return self._username

    def setUsername(self, username):
        self._username = username

    def getPassword(self):
        return self._password

    def setPassword(self, password):
        self._password = password

    def getRol(self):
        return self._rol

    def setRol(self, rol):
        self._rol = rol

    def getNombre(self):
        return self._nombre

    def setNombre(self, nombre):
        self._nombre = nombre

    def getEdad(self):
        return self._edad

    def setEdad(self, edad):
        self._edad = edad

    def getSexo(self):
        return self._sexo

    def setSexo(self, sexo):
        self._sexo = sexo

    def esAdmin(self):
        return self._rol == "ADMINISTRADOR"

    def __str__(self):
        return f"{self._nombre} [{self._rol}]"