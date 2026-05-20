from ConexionBD.UsuarioBD import UsuarioBD
from ConexionBD.ProductoBD import ProductoBD
from ConexionBD.VentaBD import VentaBD
from ConexionBD.MovimientoBD import MovimientoBD
from ConexionBD.ProveedorBD import ProveedorBD
from ConexionBD.OrdenCompraBD import OrdenCompraBD
from ConexionBD.CuentaPorPagarBD import CuentaPorPagarBD
from ConexionBD.DevolucionBD import DevolucionBD
from ConexionBD.ConfiguracionBD import ConfiguracionBD
from Modelo.ConfiguracionTienda import ConfiguracionTienda
from Modelo.Movimiento import Movimiento, Tipo as TipoMovimiento
from datetime import datetime
from PyQt6.QtWidgets import QMessageBox

class ControladorPrincipal:
    def __init__(self):
        self.usuarioBD = UsuarioBD()
        self.productoBD = ProductoBD()
        self.ventaBD = VentaBD()
        self.movimientoBD = MovimientoBD()
        self.proveedorBD = ProveedorBD()
        self.ordenBD = OrdenCompraBD()
        self.cuentaBD = CuentaPorPagarBD()
        self.devolucionBD = DevolucionBD()
        self.configBD = ConfiguracionBD()

        self.montoCaja = 0.0
        self.cajaAbierta = False
        self.usuarioActivo = None

        self.config = ConfiguracionTienda()
        self.config.setNombre("CORPORATIVO POS")
        self.config.setSucursal("Sucursal Principal - Centro")
        self.config.setRfc("XAXX010101000")

        self.loginVista = None
        self.ventanaPrincipal = None

        self.ventaCtrl = None
        self.inventarioCtrl = None
        self.cajaCtrl = None
        self.proveedorCtrl = None
        self.usuarioCtrl = None

    def iniciar(self):
        guardada = self.configBD.obtener()
        if guardada is not None:
            self.config = guardada

        self.cargarProductosIniciales()

        from Vista.LoginVista import LoginVista
        self.loginVista = LoginVista(self)
        self.loginVista.mostrar()

    def onLoginExitoso(self, u):
        self.usuarioActivo = u
        if self.loginVista:
            self.loginVista.ocultar()
        
        from Controlador.VentaControlador import VentaControlador
        from Controlador.InventarioControlador import InventarioControlador
        from Controlador.CajaControlador import CajaControlador
        from Controlador.ProveedorControlador import ProveedorControlador
        from Controlador.UsuarioControlador import UsuarioControlador
        from Vista.VentanaPrincipal import VentanaPrincipal
        
        self.ventaCtrl = VentaControlador(self)
        self.inventarioCtrl = InventarioControlador(self)
        self.cajaCtrl = CajaControlador(self)
        self.proveedorCtrl = ProveedorControlador(self)
        self.usuarioCtrl = UsuarioControlador(self)
        
        self.ventanaPrincipal = VentanaPrincipal(self, self.ventaCtrl, self.inventarioCtrl, self.cajaCtrl, self.proveedorCtrl, self.usuarioCtrl)
        self.ventanaPrincipal.mostrar()

    def onCerrarSesion(self):
        self.usuarioActivo = None
        self.cajaAbierta = False
        self.montoCaja = 0.0
        if self.ventanaPrincipal is not None:
            self.ventanaPrincipal.ocultar()
        if self.loginVista:
            self.loginVista.limpiar()
            self.loginVista.mostrar()

    def autenticar(self, username, password):
        return self.usuarioBD.autenticar(username, password)

    def registrarVenta(self, venta):
        self.ventaBD.insertar(venta)
        if str(venta.getMetodoPago()).lower() == "efectivo":
            self.montoCaja += venta.getTotal()

        m = Movimiento()
        m.setTipo(TipoMovimiento.VENTA)
        m.setDescripcion(f"venta #{venta.getId()} ({venta.getMetodoPago()})")
        m.setMonto(venta.getTotal())
        m.setFecha(datetime.now())
        m.setUsuario(self.usuarioActivo.getNombre() if self.usuarioActivo else "SISTEMA")
        self.movimientoBD.insertar(m)

        if self.ventanaPrincipal is not None:
            self.ventanaPrincipal.refrescarCaja()
            self.ventanaPrincipal.refrescarInventario()

    def registrarDevolucion(self, d):
        self.devolucionBD.insertar(d)
        p = self.productoBD.buscarPorId(d.getProducto().getId())
        if p is not None:
            self.productoBD.actualizarStock(p.getId(), p.getStock() + d.getCantidad())
            
        m = Movimiento()
        m.setTipo(TipoMovimiento.RETIRO)
        m.setDescripcion(f"Devolucion Venta #{d.getVentaId()}")
        m.setMonto(d.getMontoDevuelto() if hasattr(d, 'getMontoDevuelto') else 0.0) # Assuming getMontoDevuelto exists or 0
        m.setFecha(datetime.now())
        m.setUsuario(self.usuarioActivo.getNombre() if self.usuarioActivo else "SISTEMA")
        self.movimientoBD.insertar(m)
        
        if self.ventanaPrincipal is not None:
            self.ventanaPrincipal.refrescarInventario()

    def registrarVentaSimple(self, monto):
        self.montoCaja += monto
        if self.ventanaPrincipal is not None:
            self.ventanaPrincipal.refrescarCaja()

    def registrarRetiro(self, concepto, monto):
        self.montoCaja -= monto
        m = Movimiento()
        m.setTipo(TipoMovimiento.RETIRO)
        m.setDescripcion(concepto)
        m.setMonto(monto)
        m.setFecha(datetime.now())
        m.setUsuario(self.usuarioActivo.getNombre() if self.usuarioActivo else "SISTEMA")
        self.movimientoBD.insertar(m)
        
        if self.ventanaPrincipal is not None:
            self.ventanaPrincipal.refrescarCaja()

    def abrirCaja(self, fondo):
        self.montoCaja = fondo
        self.cajaAbierta = True

        m = Movimiento()
        m.setTipo(TipoMovimiento.VENTA)
        m.setDescripcion("apertura de caja")
        m.setMonto(fondo)
        m.setFecha(datetime.now())
        m.setUsuario(self.usuarioActivo.getNombre() if self.usuarioActivo else "SISTEMA")
        self.movimientoBD.insertar(m)

        if self.ventanaPrincipal is not None:
            self.ventanaPrincipal.refrescarCaja()

    def guardarConfiguracion(self, nuevaConfig):
        self.config = nuevaConfig
        self.configBD.actualizar(nuevaConfig)
        if self.ventanaPrincipal is not None:
            self.ventanaPrincipal.actualizarTitulo()

    def reiniciarSistemaCompleto(self):
        try:
            tablas = ["devoluciones", "cuentas_por_pagar", "ordenes_compra", "movimientos", "ventas"]
            from ConexionBD.Conexion import Conexion
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute("SET FOREIGN_KEY_CHECKS = 0")
                for tabla in tablas:
                    cursor.execute(f"TRUNCATE TABLE {tabla}")
                cursor.execute("SET FOREIGN_KEY_CHECKS = 1")
                conn.commit()
                cursor.close()

            self.montoCaja = 0.0
            self.cajaAbierta = False

            if self.ventanaPrincipal is not None:
                self.ventanaPrincipal.refrescarCaja()
                self.onCerrarSesion()

            QMessageBox.information(None, "Éxito", "sistema reiniciado con exito")
        except Exception as e:
            QMessageBox.critical(None, "Error", f"error al reiniciar: {e}")

    def getUsuarios(self):
        return self.usuarioBD.obtenerTodos()

    def getProductos(self):
        return self.productoBD.obtenerTodos()

    def getVentas(self):
        return self.ventaBD.obtenerTodas()

    def getMovimientos(self):
        return self.movimientoBD.obtenerDelDia()

    def getProveedores(self):
        return self.proveedorBD.obtenerTodos()

    def getOrdenes(self):
        return self.ordenBD.obtenerTodas()

    def getCuentas(self):
        return self.cuentaBD.obtenerActivas()

    def getDevoluciones(self):
        return self.devolucionBD.obtenerTodas()

    def getUsuarioBD(self):
        return self.usuarioBD

    def getProductoBD(self):
        return self.productoBD

    def getVentaBD(self):
        return self.ventaBD

    def getMovimientoBD(self):
        return self.movimientoBD

    def getProveedorBD(self):
        return self.proveedorBD

    def getOrdenBD(self):
        return self.ordenBD

    def getCuentaBD(self):
        return self.cuentaBD

    def getDevolucionBD(self):
        return self.devolucionBD

    def getConfig(self):
        return self.config

    def setConfig(self, c):
        self.config = c

    def getMontoCaja(self):
        return self.montoCaja

    def isCajaAbierta(self):
        return self.cajaAbierta

    def getUsuarioActivo(self):
        return self.usuarioActivo

    def getVentanaPrincipal(self):
        return self.ventanaPrincipal

    def cargarProductosIniciales(self):
        try:
            from Modelo.DatosIniciales import DatosIniciales
            productosBase = DatosIniciales.getProductos()
            if not productosBase: return
            insertados = 0
            actualizados = 0
            for p in productosBase:
                existente = self.productoBD.buscarPorId(p.getId())
                if existente is None:
                    self.productoBD.insertar(p)
                    insertados += 1
                else:
                    if existente.getNombre() != p.getNombre() or existente.getPrecio() != p.getPrecio() or existente.getCategoria() != p.getCategoria():
                        p.setStock(existente.getStock())
                        self.productoBD.actualizar(p)
                        actualizados += 1
            print(f"Seed finalizado. Insertados: {insertados}, Actualizados: {actualizados}")
        except Exception as e:
            print(f"Error en seed de productos: {e}")
