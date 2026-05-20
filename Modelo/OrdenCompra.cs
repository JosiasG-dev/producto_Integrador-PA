using System;
using System.Collections.Generic;

namespace punto_de_venta_C_.Modelo
{
    public class OrdenCompra
    {
        public enum Estado
        {
            PENDIENTE, RECIBIDO
        }

        public enum TipoPago
        {
            CONTADO, CREDITO
        }

        private int id;
        private Proveedor proveedor;
        private List<ItemOrden> items;
        private double total;
        private TipoPago tipoPago;
        private Estado estado;
        private DateTime fecha;

        public OrdenCompra(int id, Proveedor proveedor, List<ItemOrden> items, double total, TipoPago tipoPago, Estado estado, DateTime fecha)
        {
            this.id = id;
            this.proveedor = proveedor;
            this.items = items;
            this.total = total;
            this.tipoPago = tipoPago;
            this.estado = estado;
            this.fecha = fecha;
        }

        public int getId() { return id; }
        public void setId(int id) { this.id = id; }
        public Proveedor getProveedor() { return proveedor; }
        public void setProveedor(Proveedor proveedor) { this.proveedor = proveedor; }
        public List<ItemOrden> getItems() { return items; }
        public void setItems(List<ItemOrden> items) { this.items = items; }
        public double getTotal() { return total; }
        public void setTotal(double total) { this.total = total; }
        public TipoPago getTipoPago() { return tipoPago; }
        public void setTipoPago(TipoPago tipoPago) { this.tipoPago = tipoPago; }
        public Estado getEstado() { return estado; }
        public void setEstado(Estado estado) { this.estado = estado; }
        public DateTime getFecha() { return fecha; }
        public void setFecha(DateTime fecha) { this.fecha = fecha; }

        public bool isPendiente()
        {
            return estado == Estado.PENDIENTE;
        }

        public string getFolioCorto()
        {
            return "OC-" + string.Format("{0:0000}", id % 10000);
        }

        public class ItemOrden
        {
            private Producto producto;
            private double cantidadSolicitada;
            private double precioCosto;

            public ItemOrden(Producto producto, double cantidadSolicitada, double precioCosto)
            {
                this.producto = producto;
                this.cantidadSolicitada = cantidadSolicitada;
                this.precioCosto = precioCosto;
            }

            public Producto getProducto() { return producto; }
            public void setProducto(Producto producto) { this.producto = producto; }
            public double getCantidadSolicitada() { return cantidadSolicitada; }
            public void setCantidadSolicitada(double cantidadSolicitada) { this.cantidadSolicitada = cantidadSolicitada; }
            public double getPrecioCosto() { return precioCosto; }
            public void setPrecioCosto(double precioCosto) { this.precioCosto = precioCosto; }

            public double getSubtotal()
            {
                return precioCosto * cantidadSolicitada;
            }
        }
    }
}
