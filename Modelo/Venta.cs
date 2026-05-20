using System;
using System.Collections.Generic;

namespace punto_de_venta_C_.Modelo
{
    public class Venta
    {
        private int id;
        private List<ItemCarrito> items;
        private double total;
        private double descuento;
        private string metodoPago;
        private DateTime fecha;
        private string cajero;

        public Venta(int id, List<ItemCarrito> items, double total, string metodoPago, DateTime fecha, string cajero)
        {
            this.id = id;
            this.items = items;
            this.total = total;
            this.descuento = 0;
            this.metodoPago = metodoPago;
            this.fecha = fecha;
            this.cajero = cajero;
        }

        public Venta(int id, List<ItemCarrito> items, double total, double descuento, string metodoPago, DateTime fecha, string cajero)
        {
            this.id = id;
            this.items = items;
            this.total = total;
            this.descuento = descuento;
            this.metodoPago = metodoPago;
            this.fecha = fecha;
            this.cajero = cajero;
        }

        public int getId() { return id; }
        public void setId(int id) { this.id = id; }
        public List<ItemCarrito> getItems() { return items; }
        public void setItems(List<ItemCarrito> items) { this.items = items; }
        public double getTotal() { return total; }
        public void setTotal(double total) { this.total = total; }
        public string getMetodoPago() { return metodoPago; }
        public void setMetodoPago(string metodoPago) { this.metodoPago = metodoPago; }
        public DateTime getFecha() { return fecha; }
        public void setFecha(DateTime fecha) { this.fecha = fecha; }
        public string getCajero() { return cajero; }
        public void setCajero(string cajero) { this.cajero = cajero; }
        public double getDescuento() { return descuento; }
        public void setDescuento(double descuento) { this.descuento = descuento; }

        public double getSubtotal()
        {
            return Math.Round((total + descuento) * 100.0) / 100.0;
        }

        public int getTotalItems()
        {
            return items.Count;
        }
    }
}
