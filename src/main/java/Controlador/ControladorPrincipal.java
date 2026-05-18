package Controlador;

import javax.swing.*;

import ConexionBD.*;
import Modelo.*;
import Vista.*;
import java.util.Date;
import java.util.List;

public class ControladorPrincipal {

	private final UsuarioBD usuarioBD = new UsuarioBD();
	private final ProductoBD productoBD = new ProductoBD();
	private final VentaBD ventaBD = new VentaBD();
	private final MovimientoBD movimientoBD = new MovimientoBD();
	private final ProveedorBD proveedorBD = new ProveedorBD();
	private final OrdenCompraBD ordenBD = new OrdenCompraBD();
	private final CuentaPorPagarBD cuentaBD = new CuentaPorPagarBD();
	private final DevolucionBD devolucionBD = new DevolucionBD();
	private final ConfiguracionBD configBD = new ConfiguracionBD();

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
		ConfiguracionTienda guardada = configBD.obtener();
		if (guardada != null) {
			this.config = guardada;
		}

		cargarProductosIniciales();

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
		return usuarioBD.autenticar(username, password);
	}

	public void registrarVenta(Venta venta) {
		ventaBD.insertar(venta);

		if (venta.getMetodoPago().equalsIgnoreCase("Efectivo")) {
			this.montoCaja += venta.getTotal();
		}

		movimientoBD.insertar(
				new Movimiento(Movimiento.Tipo.VENTA, "venta #" + venta.getId() + " (" + venta.getMetodoPago() + ")",
						venta.getTotal(), new java.util.Date(), usuarioActivo.getNombre()));

		if (ventanaPrincipal != null) {
			ventanaPrincipal.refrescarCaja();
			ventanaPrincipal.refrescarInventario();
		}
	}

	public void registrarDevolucion(Devolucion d) {
		devolucionBD.insertar(d);
		Producto p = productoBD.buscarPorId(d.getProducto().getId());
		if (p != null)
			productoBD.actualizarStock(p.getId(), p.getStock() + d.getCantidad());
		movimientoBD.insertar(new Movimiento(Movimiento.Tipo.RETIRO, "Devolucion Venta #" + d.getVentaId(),
				d.getMontoDevuelto(), new Date(), usuarioActivo.getNombre()));
		if (ventanaPrincipal != null)
			ventanaPrincipal.refrescarInventario();
	}

	public void registrarVentaSimple(double monto) {
		this.montoCaja += monto;
		if (ventanaPrincipal != null) {
			ventanaPrincipal.refrescarCaja();
		}
	}

	public void registrarRetiro(String concepto, double monto) {
		this.montoCaja -= monto;
		movimientoBD.insertar(new Movimiento(Movimiento.Tipo.RETIRO, concepto, monto, new java.util.Date(),
				usuarioActivo.getNombre()));
		if (ventanaPrincipal != null) {
			ventanaPrincipal.refrescarCaja();
		}
	}

	public void abrirCaja(double fondo) {
		this.montoCaja = fondo;
		this.cajaAbierta = true;

		movimientoBD.insertar(new Movimiento(Movimiento.Tipo.VENTA, "apertura de caja", fondo, new java.util.Date(),
				usuarioActivo.getNombre()));

		if (ventanaPrincipal != null) {
			ventanaPrincipal.refrescarCaja();
		}
	}

	public void guardarConfiguracion(ConfiguracionTienda nuevaConfig) {
		this.config = nuevaConfig;

		configBD.actualizar(nuevaConfig);

		if (ventanaPrincipal != null) {
			ventanaPrincipal.actualizarTitulo();
		}
	}

	public void reiniciarSistemaCompleto() {
		try {
			String[] tablas = { "devoluciones", "cuentas_por_pagar", "ordenes_compra", "movimientos", "ventas" };

			try (java.sql.Connection c = ConexionBD.Conexion.getConexion();
					java.sql.Statement st = c.createStatement()) {

				st.executeUpdate("SET FOREIGN_KEY_CHECKS = 0");

				for (String tabla : tablas) {
					st.executeUpdate("TRUNCATE TABLE " + tabla);
				}

				st.executeUpdate("SET FOREIGN_KEY_CHECKS = 1");
			}

			this.montoCaja = 0;
			this.cajaAbierta = false;

			if (ventanaPrincipal != null) {
				ventanaPrincipal.refrescarCaja();
				this.onCerrarSesion();
			}

			JOptionPane.showMessageDialog(null, "sistema reiniciado con exito");

		} catch (Exception e) {
			JOptionPane.showMessageDialog(null, "error al reiniciar: " + e.getMessage());
		}
	}

	public List<Usuario> getUsuarios() {
		return usuarioBD.obtenerTodos();
	}

	public List<Producto> getProductos() {
		return productoBD.obtenerTodos();
	}

	public List<Venta> getVentas() {
		return ventaBD.obtenerTodas();
	}

	public List<Movimiento> getMovimientos() {
		return movimientoBD.obtenerDelDia();
	}

	public List<Proveedor> getProveedores() {
		return proveedorBD.obtenerTodos();
	}

	public List<OrdenCompra> getOrdenes() {
		return ordenBD.obtenerTodas();
	}

	public List<CuentaPorPagar> getCuentas() {
		return cuentaBD.obtenerActivas();
	}

	public List<Devolucion> getDevoluciones() {
		return devolucionBD.obtenerTodas();
	}

	public UsuarioBD getUsuarioBD() {
		return usuarioBD;
	}

	public ProductoBD getProductoBD() {
		return productoBD;
	}

	public VentaBD getVentaBD() {
		return ventaBD;
	}

	public MovimientoBD getMovimientoBD() {
		return movimientoBD;
	}

	public ProveedorBD getProveedorBD() {
		return proveedorBD;
	}

	public OrdenCompraBD getOrdenBD() {
		return ordenBD;
	}

	public CuentaPorPagarBD getCuentaBD() {
		return cuentaBD;
	}

	public DevolucionBD getDevolucionBD() {
		return devolucionBD;
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
	
	private void cargarProductosIniciales() {
		try {
			List<Modelo.Producto> productosBase = Modelo.DatosIniciales.getProductos();
			int insertados = 0;
			int actualizados = 0;
			for (Modelo.Producto p : productosBase) {
				Modelo.Producto existente = productoBD.buscarPorId(p.getId());
				if (existente == null) {
					productoBD.insertar(p);
					insertados++;
				} else {
					if (!existente.getNombre().equals(p.getNombre()) || 
						existente.getPrecio() != p.getPrecio() || 
						!existente.getCategoria().equals(p.getCategoria())) {
						p.setStock(existente.getStock());
						productoBD.actualizar(p);
						actualizados++;
					}
				}
			}
			System.out.println("Seed finalizado. Insertados: " + insertados + ", Actualizados: " + actualizados);
		} catch (Exception e) {
			System.err.println("Error en seed de productos: " + e.getMessage());
		}
	}

}