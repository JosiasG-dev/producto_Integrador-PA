using System;
using System.Collections.Generic;
using System.Linq;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Vista;
using Microsoft.Maui.Controls;

namespace punto_de_venta_C_.Controlador
{
    public class VentaControlador
    {
        private readonly ControladorPrincipal app;
        private VentaPanel panel;
        private List<ItemCarrito> carrito = new List<ItemCarrito>();
        private double descuento = 0;

        public VentaControlador(ControladorPrincipal app)
        {
            this.app = app;
        }

        public void setPanel(VentaPanel panel)
        {
            this.panel = panel;
        }

        public List<Producto> buscarProductos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return new List<Producto>();
            return app.getProductoBD().buscar(texto);
        }

        public async void agregarAlCarrito(Producto p)
        {
            foreach (ItemCarrito item in carrito)
            {
                if (item.getProducto().getId().Equals(p.getId()))
                {
                    double nuevaCant = item.getCantidad() + 1.0;
                    if (nuevaCant > p.getStock())
                    {
                        if (Application.Current?.MainPage != null)
                        {
                            await Application.Current.MainPage.DisplayAlert("Sin stock",
                                "Stock insuficiente. Disponible: " + (int)p.getStock(), "OK");
                        }
                        return;
                    }
                    item.incrementar(1.0);
                    notificarPanel();
                    return;
                }
            }
            if (p.getStock() <= 0)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Sin stock", "El producto no tiene existencia", "OK");
                }
                return;
            }
            carrito.Add(new ItemCarrito(p, 1.0));
            notificarPanel();
        }

        public void eliminarDelCarrito(string productoId)
        {
            carrito.RemoveAll(i => i.getProducto().getId().Equals(productoId));
            notificarPanel();
        }

        public async void setCantidad(string productoId, double nuevaCantidad)
        {
            foreach (ItemCarrito item in carrito)
            {
                if (item.getProducto().getId().Equals(productoId))
                {
                    nuevaCantidad = Math.Round(nuevaCantidad);
                    if (nuevaCantidad > item.getProducto().getStock())
                    {
                        if (Application.Current?.MainPage != null)
                        {
                            await Application.Current.MainPage.DisplayAlert("Sin stock",
                                "Stock insuficiente. Disponible: " + (int)item.getProducto().getStock(), "OK");
                        }
                        return;
                    }
                    item.setCantidad(nuevaCantidad);
                    break;
                }
            }
            notificarPanel();
        }

        public void setDescuento(double descuento)
        {
            this.descuento = Math.Max(0, descuento);
            notificarPanel();
        }

        public double getDescuento()
        {
            return descuento;
        }

        public double calcularSubtotal()
        {
            return carrito.Select(item => item.getSubtotal()).Sum();
        }

        public double calcularTotal()
        {
            double sub = calcularSubtotal();
            return Math.Round(Math.Max(0, sub - descuento) * 100.0) / 100.0;
        }

        public double calcularCambio(double recibido)
        {
            return Math.Max(0, recibido - calcularTotal());
        }

        public async void procesarCobro(string metodoPago, double montoRecibido)
        {
            if (carrito.Count == 0)
                return;
            double total = calcularTotal();
            if (montoRecibido < total && "Efectivo".Equals(metodoPago))
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Importe insuficiente",
                        string.Format("El importe recibido (${0:0.00}) es menor al total (${1:0.00})", montoRecibido, total), "OK");
                }
                return;
            }
            Venta venta = new Venta(0, new List<ItemCarrito>(carrito), total, descuento, metodoPago, DateTime.Now,
                    app.getUsuarioActivo().getNombre());
            app.registrarVenta(venta);
            double cambio = calcularCambio(montoRecibido);
            
            TicketDialog ticket = new TicketDialog(venta, app.getConfig(), cambio, descuento, montoRecibido);
            await ticket.mostrar();
            
            carrito.Clear();
            descuento = 0;
            if (panel != null)
                panel.limpiar();
        }

        private void notificarPanel()
        {
            if (panel != null)
                panel.refrescarCarrito(carrito, calcularTotal(), descuento);
        }

        public List<ItemCarrito> getCarrito()
        {
            return carrito;
        }
    }
}
