class UsuarioControlador:
    def __init__(self, app):
        self.app = app
        self.panel = None

    def setPanel(self, panel):
        self.panel = panel

    def guardarUsuario(self, u):
        existe = any(x.getId() == u.getId() for x in self.app.getUsuarios())
        if existe:
            self.app.getUsuarioBD().actualizar(u)
        else:
            self.app.getUsuarioBD().insertar(u)
        
        if self.panel is not None:
            self.panel.refrescar()

    def eliminarUsuario(self, id_usr):
        if self.app.getUsuarioActivo() and id_usr == self.app.getUsuarioActivo().getId():
            return
        self.app.getUsuarioBD().eliminar(id_usr)
        if self.panel is not None:
            self.panel.refrescar()

    def generarId(self):
        return 0

    def getUsuarios(self):
        return self.app.getUsuarios()

    def getUsuarioActivo(self):
        return self.app.getUsuarioActivo()
