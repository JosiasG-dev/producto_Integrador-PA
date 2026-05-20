using System;
using System.Collections.Generic;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.ConexionBD;
using punto_de_venta_C_.Vista;

namespace punto_de_venta_C_.Controlador
{
    public class ControladorPrincipal
    {
        private readonly UsuarioBD usuarioBD = new UsuarioBD();
        private readonly ProductoBD productoBD = new ProductoBD();
        private readonly VentaBD ventaBD = new VentaBD();
        private readonly MovimientoBD movimientoBD = new MovimientoBD();
        private readonly ProveedorBD proveedorBD = new ProveedorBD();
        private readonly OrdenCompraBD ordenBD = new OrdenCompraBD();
        private readonly CuentaPorPagarBD cuentaBD = new CuentaPorPagarBD();
        private readonly DevolucionBD devolucionBD = new DevolucionBD();
        private readonly ConfiguracionBD configBD = new ConfiguracionBD();

        private double montoCaja = 0;
        private bool cajaAbierta = false;
        private Usuario usuarioActivo;

        private ConfiguracionTienda config = new ConfiguracionTienda("CORPORATIVO POS", "Sucursal Principal - Centro", "XAXX010101000");

        private LoginVista loginVista;
        private VentanaPrincipal ventanaPrincipal;

        private VentaControlador ventaCtrl;
        private InventarioControlador inventarioCtrl;
        private CajaControlador cajaCtrl;
        private ProveedorControlador proveedorCtrl;
        private UsuarioControlador usuarioCtrl;

        public void iniciar()
        {
            ConfiguracionTienda guardada = configBD.obtener();
            if (guardada != null)
            {
                this.config = guardada;
            }

            cargarProductosIniciales();

            loginVista = new LoginVista(this);
            loginVista.mostrar();
        }

        public void onLoginExitoso(Usuario u)
        {
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

        public void onCerrarSesion()
        {
            this.usuarioActivo = null;
            this.cajaAbierta = false;
            this.montoCaja = 0;
            if (ventanaPrincipal != null)
                ventanaPrincipal.ocultar();
            loginVista.limpiar();
            loginVista.mostrar();
        }

        public Usuario autenticar(string username, string password)
        {
            return usuarioBD.autenticar(username, password);
        }

        public void registrarVenta(Venta venta)
        {
            ventaBD.insertar(venta);

            if (venta.getMetodoPago().Equals("Efectivo", StringComparison.OrdinalIgnoreCase))
            {
                this.montoCaja += venta.getTotal();
            }

            movimientoBD.insertar(
                new Movimiento(Movimiento.Tipo.VENTA, "venta #" + venta.getId() + " (" + venta.getMetodoPago() + ")",
                        venta.getTotal(), DateTime.Now, usuarioActivo.getNombre()));

            if (ventanaPrincipal != null)
            {
                ventanaPrincipal.refrescarCaja();
                ventanaPrincipal.refrescarInventario();
            }
        }

        public void registrarDevolucion(Devolucion d)
        {
            devolucionBD.insertar(d);
            Producto p = productoBD.buscarPorId(d.getProducto().getId());
            if (p != null)
                productoBD.actualizarStock(p.getId(), p.getStock() + d.getCantidad());
            movimientoBD.insertar(new Movimiento(Movimiento.Tipo.RETIRO, "Devolucion Venta #" + d.getVentaId(),
                    d.getMontoDevuelto(), DateTime.Now, usuarioActivo.getNombre()));
            if (ventanaPrincipal != null)
                ventanaPrincipal.refrescarInventario();
        }

        public void registrarVentaSimple(double monto)
        {
            this.montoCaja += monto;
            if (ventanaPrincipal != null)
            {
                ventanaPrincipal.refrescarCaja();
            }
        }

        public void registrarRetiro(string concepto, double monto)
        {
            this.montoCaja -= monto;
            movimientoBD.insertar(new Movimiento(Movimiento.Tipo.RETIRO, concepto, monto, DateTime.Now,
                    usuarioActivo.getNombre()));
            if (ventanaPrincipal != null)
            {
                ventanaPrincipal.refrescarCaja();
            }
        }

        public void abrirCaja(double fondo)
        {
            this.montoCaja = fondo;
            this.cajaAbierta = true;

            movimientoBD.insertar(new Movimiento(Movimiento.Tipo.VENTA, "apertura de caja", fondo, DateTime.Now,
                    usuarioActivo.getNombre()));

            if (ventanaPrincipal != null)
            {
                ventanaPrincipal.refrescarCaja();
            }
        }

        public void guardarConfiguracion(ConfiguracionTienda nuevaConfig)
        {
            this.config = nuevaConfig;

            configBD.actualizar(nuevaConfig);

            if (ventanaPrincipal != null)
            {
                ventanaPrincipal.actualizarTitulo();
            }
        }

        public void reiniciarSistemaCompleto()
        {
            try
            {
                string[] tablas = { "devoluciones", "cuentas_por_pagar", "ordenes_compra", "movimientos", "ventas" };

                using (MySqlConnector.MySqlConnection c = Conexion.getConexion())
                {
                    using (MySqlConnector.MySqlCommand st = c.CreateCommand())
                    {
                        st.CommandText = "SET FOREIGN_KEY_CHECKS = 0";
                        st.ExecuteNonQuery();

                        foreach (string tabla in tablas)
                        {
                            st.CommandText = "TRUNCATE TABLE " + tabla;
                            st.ExecuteNonQuery();
                        }

                        st.CommandText = "SET FOREIGN_KEY_CHECKS = 1";
                        st.ExecuteNonQuery();
                    }
                }

                this.montoCaja = 0;
                this.cajaAbierta = false;

                if (ventanaPrincipal != null)
                {
                    ventanaPrincipal.refrescarCaja();
                    this.onCerrarSesion();
                }

                if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
                    Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Sistema", "sistema reiniciado con exito", "OK");

            }
            catch (Exception e)
            {
                if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
                    Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error", "error al reiniciar: " + e.Message, "OK");
            }
        }

        public List<Usuario> getUsuarios() { return usuarioBD.obtenerTodos(); }
        public List<Producto> getProductos() { return productoBD.obtenerTodos(); }
        public List<Venta> getVentas() { return ventaBD.obtenerTodas(); }
        public List<Movimiento> getMovimientos() { return movimientoBD.obtenerDelDia(); }
        public List<Proveedor> getProveedores() { return proveedorBD.obtenerTodos(); }
        public List<OrdenCompra> getOrdenes() { return ordenBD.obtenerTodas(); }
        public List<CuentaPorPagar> getCuentas() { return cuentaBD.obtenerActivas(); }
        public List<Devolucion> getDevoluciones() { return devolucionBD.obtenerTodas(); }

        public UsuarioBD getUsuarioBD() { return usuarioBD; }
        public ProductoBD getProductoBD() { return productoBD; }
        public VentaBD getVentaBD() { return ventaBD; }
        public MovimientoBD getMovimientoBD() { return movimientoBD; }
        public ProveedorBD getProveedorBD() { return proveedorBD; }
        public OrdenCompraBD getOrdenBD() { return ordenBD; }
        public CuentaPorPagarBD getCuentaBD() { return cuentaBD; }
        public DevolucionBD getDevolucionBD() { return devolucionBD; }

        public ConfiguracionTienda getConfig() { return config; }
        public void setConfig(ConfiguracionTienda c) { this.config = c; }
        public double getMontoCaja() { return montoCaja; }
        public bool isCajaAbierta() { return cajaAbierta; }
        public Usuario getUsuarioActivo() { return usuarioActivo; }
        public VentanaPrincipal getVentanaPrincipal() { return ventanaPrincipal; }
        
        private void cargarProductosIniciales()
        {
            try
            {
                List<Producto> productosBase = DatosIniciales.getProductos();
                int insertados = 0;
                int actualizados = 0;
                foreach (Producto p in productosBase)
                {
                    Producto existente = productoBD.buscarPorId(p.getId());
                    if (existente == null)
                    {
                        productoBD.insertar(p);
                        insertados++;
                    }
                    else
                    {
                        if (existente.getNombre() != p.getNombre() || 
                            existente.getPrecio() != p.getPrecio() || 
                            existente.getCategoria() != p.getCategoria())
                        {
                            p.setStock(existente.getStock());
                            productoBD.actualizar(p);
                            actualizados++;
                        }
                    }
                }
                Console.WriteLine("Seed finalizado. Insertados: " + insertados + ", Actualizados: " + actualizados);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error en seed de productos: " + e.Message);
            }
        }
    }
}
