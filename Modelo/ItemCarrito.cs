using System;

namespace punto_de_venta_C_.Modelo
{
    public class ItemCarrito
    {
        private Producto producto;
        private double cantidad;

        public ItemCarrito(Producto producto, double cantidad)
        {
            this.producto = producto;
            this.cantidad = cantidad;
        }

        public Producto getProducto() { return producto; }
        public void setProducto(Producto producto) { this.producto = producto; }
        public double getCantidad() { return cantidad; }
        public void setCantidad(double cantidad)
        {
            this.cantidad = Math.Round(cantidad * 1000.0) / 1000.0;
        }

        public double getSubtotal()
        {
            return Math.Round(producto.getPrecio() * cantidad * 100.0) / 100.0;
        }

        public void incrementar(double delta)
        {
            this.cantidad = Math.Round((this.cantidad + delta) * 1000.0) / 1000.0;
        }

        public void decrementar(double delta)
        {
            double nueva = this.cantidad - delta;
            this.cantidad = Math.Max(0, Math.Round(nueva * 1000.0) / 1000.0);
        }
    }
}
