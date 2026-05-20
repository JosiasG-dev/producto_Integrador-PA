using System;

namespace punto_de_venta_C_.Modelo
{
    public class Devolucion
    {
        private int id;
        private int ventaId;
        private Producto producto;
        private double cantidad;
        private string motivo;
        private DateTime fecha;
        private string cajero;

        public Devolucion(int id, int ventaId, Producto producto, double cantidad, string motivo, DateTime fecha, string cajero)
        {
            this.id = id;
            this.ventaId = ventaId;
            this.producto = producto;
            this.cantidad = cantidad;
            this.motivo = motivo;
            this.fecha = fecha;
            this.cajero = cajero;
        }

        public int getId() { return id; }
        public void setId(int id) { this.id = id; }
        public int getVentaId() { return ventaId; }
        public Producto getProducto() { return producto; }
        public double getCantidad() { return cantidad; }
        public string getMotivo() { return motivo; }
        public DateTime getFecha() { return fecha; }
        public string getCajero() { return cajero; }

        public double getMontoDevuelto()
        {
            return Math.Round(producto.getPrecio() * cantidad * 100.0) / 100.0;
        }
    }
}
