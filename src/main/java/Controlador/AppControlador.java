package Controlador;

import javax.swing.*;

import ConexionBD.*;
import Modelo.*;
import Vista.*;
import java.util.Date;
import java.util.List;

public class AppControlador {

	private final UsuarioBD usuarioDAO = new UsuarioBD();
	private final ProductoBD productoDAO = new ProductoBD();
	private final VentaBD ventaDAO = new VentaBD();
	private final MovimientoBD movimientoDAO = new MovimientoBD();
	private final ProveedorBD proveedorDAO = new ProveedorBD();
	private final OrdenCompraBD ordenDAO = new OrdenCompraBD();
	private final CuentaPorPagarBD cuentaDAO = new CuentaPorPagarBD();
	private final DevolucionBD devolucionDAO = new DevolucionBD();

	private double montoCaja = 0;
	private boolean cajaAbierta = false;
	private Usuario usuarioActivo;

	private ConfiguracionTienda config = new ConfiguracionTienda("CORPORATIVO POS", "Sucursal Principal - Centro",
			"XAXX010101000");

	private LoginVista loginVista;
	private VentanaPrincipal ventanaPrincipal;

	private VentaControlador ventaCtrl;
	private InventarioControlador inventarioCtrl;
	private CajaControlador cajaCtrl;
	private ProveedorControlador proveedorCtrl;
	private UsuarioControlador usuarioCtrl;

	public void iniciar() {
		if (Conexion.getConexion() == null) {
			JOptionPane.showMessageDialog(null,
					"No se pudo conectar a MySQL.\n\nVerifica en Conexion/Conexion.java:\n"
							+ "  HOST, PORT, USER y PASSWORD\n  Que MySQL este corriendo\n  Que la base exista",
					"Error de Conexion", JOptionPane.ERROR_MESSAGE);
			System.exit(1);
		}
		loginVista = new LoginVista(this);
		loginVista.mostrar();
	}

	public void onLoginExitoso(Usuario u) {
		this.usuarioActivo = u;
		loginVista.ocultar();
		ventaCtrl = new VentaControlador(this);
		inventarioCtrl = new InventarioControlador(this);
		cajaCtrl = new CajaControlador(this);
		proveedorCtrl = new ProveedorControlador(this);
		usuarioCtrl = new UsuarioControlador(this);
		ventanaPrincipal = new VentanaPrincipal(this, ventaCtrl, inventarioCtrl, cajaCtrl, proveedorCtrl, usuarioCtrl);
		ventanaPrincipal.mostrar();
	}

	public void onCerrarSesion() {
		this.usuarioActivo = null;
		this.cajaAbierta = false;
		this.montoCaja = 0;
		if (ventanaPrincipal != null)
			ventanaPrincipal.ocultar();
		loginVista.limpiar();
		loginVista.mostrar();
	}

	public Usuario autenticar(String username, String password) {
		return usuarioDAO.autenticar(username, password);
	}

	public void registrarVenta(Venta venta) {
		ventaDAO.insertar(venta);
		movimientoDAO.insertar(new Movimiento(Movimiento.Tipo.VENTA, "Venta #" + venta.getId(), venta.getTotal(),
				new Date(), usuarioActivo.getNombre()));
		if (ventanaPrincipal != null) {
			ventanaPrincipal.refrescarInventario();
			ventanaPrincipal.refrescarCaja();
		}
	}

	public void registrarDevolucion(Devolucion d) {
		devolucionDAO.insertar(d);
		Producto p = productoDAO.buscarPorId(d.getProducto().getId());
		if (p != null)
			productoDAO.actualizarStock(p.getId(), p.getStock() + d.getCantidad());
		movimientoDAO.insertar(new Movimiento(Movimiento.Tipo.RETIRO, "Devolucion Venta #" + d.getVentaId(),
				d.getMontoDevuelto(), new Date(), usuarioActivo.getNombre()));
		if (ventanaPrincipal != null)
			ventanaPrincipal.refrescarInventario();
	}

	public void registrarRetiro(String concepto, double monto) {
		movimientoDAO.insertar(
				new Movimiento(Movimiento.Tipo.RETIRO, concepto, monto, new Date(), usuarioActivo.getNombre()));
		if (ventanaPrincipal != null)
			ventanaPrincipal.refrescarCaja();
	}

	public void abrirCaja(double fondo) {
		this.montoCaja = fondo;
		this.cajaAbierta = true;
		movimientoDAO.insertar(new Movimiento(Movimiento.Tipo.FONDO_INICIAL, "Apertura de caja", fondo, new Date(),
				usuarioActivo.getNombre()));
	}

	public List<Usuario> getUsuarios() {
		return usuarioDAO.obtenerTodos();
	}

	public List<Producto> getProductos() {
		return productoDAO.obtenerTodos();
	}

	public List<Venta> getVentas() {
		return ventaDAO.obtenerTodas();
	}

	public List<Movimiento> getMovimientos() {
		return movimientoDAO.obtenerDelDia();
	}

	public List<Proveedor> getProveedores() {
		return proveedorDAO.obtenerTodos();
	}

	public List<OrdenCompra> getOrdenes() {
		return ordenDAO.obtenerTodas();
	}

	public List<CuentaPorPagar> getCuentas() {
		return cuentaDAO.obtenerActivas();
	}

	public List<Devolucion> getDevoluciones() {
		return devolucionDAO.obtenerTodas();
	}

	public UsuarioBD getUsuarioDAO() {
		return usuarioDAO;
	}

	public ProductoBD getProductoDAO() {
		return productoDAO;
	}

	public VentaBD getVentaDAO() {
		return ventaDAO;
	}

	public MovimientoBD getMovimientoDAO() {
		return movimientoDAO;
	}

	public ProveedorBD getProveedorDAO() {
		return proveedorDAO;
	}

	public OrdenCompraBD getOrdenDAO() {
		return ordenDAO;
	}

	public CuentaPorPagarBD getCuentaDAO() {
		return cuentaDAO;
	}

	public DevolucionBD getDevolucionDAO() {
		return devolucionDAO;
	}

	public ConfiguracionTienda getConfig() {
		return config;
	}

	public void setConfig(ConfiguracionTienda c) {
		this.config = c;
	}

	public double getMontoCaja() {
		return montoCaja;
	}

	public boolean isCajaAbierta() {
		return cajaAbierta;
	}

	public Usuario getUsuarioActivo() {
		return usuarioActivo;
	}

	public VentanaPrincipal getVentanaPrincipal() {
		return ventanaPrincipal;
	}
}
