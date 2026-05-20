class InventarioControlador:
    def __init__(self, app):
        self.app = app
        self.panel = None

    def getApp(self):
        return self.app

    def setPanel(self, panel):
        self.panel = panel

    def filtrar(self, texto, categoria):
        todos = self.app.getProductos()
        resultado = []
        for p in todos:
            matchTexto = not texto or not texto.strip() or texto.lower() in p.getNombre().lower() or texto in str(p.getId())
            matchCat = not categoria or not categoria.strip() or p.getCategoria() == categoria
            if matchTexto and matchCat:
                resultado.append(p)
        return resultado

    def guardarProducto(self, p):
        existente = self.app.getProductoBD().buscarPorId(p.getId())
        if existente is not None:
            self.app.getProductoBD().actualizar(p)
        else:
            self.app.getProductoBD().insertar(p)
        if self.panel is not None:
            self.panel.refrescar()

    def eliminarProducto(self, id_prod):
        self.app.getProductoBD().eliminar(id_prod)
        if self.panel is not None:
            self.panel.refrescar()

    def buscarPorId(self, id_prod):
        return self.app.getProductoBD().buscarPorId(id_prod)

    def generarNuevoId(self):
        return self.app.getProductoBD().generarNuevoId()
