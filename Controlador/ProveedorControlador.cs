using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Vista;
using Microsoft.Maui.Controls;

namespace punto_de_venta_C_.Controlador
{
    public class ProveedorControlador
    {
        private readonly ControladorPrincipal app;
        private ProveedorPanel panel;

        public ProveedorControlador(ControladorPrincipal app)
        {
            this.app = app;
        }

        public void setPanel(ProveedorPanel panel)
        {
            this.panel = panel;
        }

        public void guardarProveedor(Proveedor p)
        {
            if (app.getProveedorBD().buscarPorId(p.getId()) != null)
            {
                app.getProveedorBD().actualizar(p);
            }
            else
            {
                app.getProveedorBD().insertar(p);
            }
            if (panel != null)
                panel.refrescarProveedores();
        }

        public void eliminarProveedor(int id)
        {
            app.getProveedorBD().eliminar(id);
            if (panel != null)
                panel.refrescarProveedores();
        }

        public int generarIdProveedor()
        {
            return (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % 100000);
        }

        public void crearOrden(OrdenCompra orden)
        {
            app.getOrdenBD().insertar(orden);
            if (panel != null)
                panel.refrescarOrdenes();
        }

        public async void recibirOrden(int ordenId)
        {
            OrdenCompra orden = app.getOrdenBD().buscarPorId(ordenId);
            if (orden == null || !orden.isPendiente())
                return;

            foreach (OrdenCompra.ItemOrden item in orden.getItems())
            {
                Producto p = app.getProductoBD().buscarPorId(item.getProducto().getId());
                if (p != null)
                {
                    double nuevoStock = p.getStock() + item.getCantidadSolicitada();
                    app.getProductoBD().actualizarStock(p.getId(), nuevoStock);
                }
            }

            app.getOrdenBD().marcarRecibida(ordenId);

            if (orden.getTipoPago() == OrdenCompra.TipoPago.CREDITO)
            {
                CuentaPorPagar cuenta = new CuentaPorPagar(0, orden.getProveedor(), ordenId, orden.getTotal(), 0,
                        orden.getTotal(), "7 dias", CuentaPorPagar.Estado.PENDIENTE);
                app.getCuentaBD().insertar(cuenta);
            }
            else
            {
                bool confirmar = false;
                if (Application.Current?.MainPage != null)
                {
                    confirmar = await Application.Current.MainPage.DisplayAlert("confirmar pago",
                        "vas a pagar " + orden.getTotal() + " en efectivo", "Sí", "No");
                }

                if (confirmar)
                {
                    app.registrarRetiro("pago de contado orden " + orden.getId(), orden.getTotal());
                }
            }

            if (panel != null)
            {
                panel.refrescarOrdenes();
                panel.refrescarCuentas();
                app.getVentanaPrincipal().refrescarInventario();
                app.getVentanaPrincipal().refrescarCaja();
            }
        }

        public void liquidarCuenta(int cuentaId, double monto)
        {
            foreach (CuentaPorPagar c in app.getCuentaBD().obtenerTodas())
            {
                if (c.getId() == cuentaId)
                {
                    double nuevoPagado = c.getPagado() + monto;
                    double nuevoSaldo = c.getMontoTotal() - nuevoPagado;
                    string nuevoEstado = nuevoSaldo <= 0 ? "PAGADO" : "PARCIAL";

                    app.getCuentaBD().aplicarPago(cuentaId, nuevoPagado, Math.Max(0, nuevoSaldo), nuevoEstado);

                    app.registrarRetiro("pago deuda " + c.getProveedor().getNombre(), monto);

                    if (panel != null)
                    {
                        panel.refrescarCuentas();
                        panel.refrescarOrdenes();
                    }
                    break;
                }
            }
        }

        public List<Proveedor> getProveedores()
        {
            return app.getProveedores();
        }

        public List<OrdenCompra> getOrdenes()
        {
            return app.getOrdenes();
        }

        public List<CuentaPorPagar> getCuentas()
        {
            return app.getCuentas();
        }

        public List<Producto> getProductos()
        {
            return app.getProductos();
        }
    }
}
