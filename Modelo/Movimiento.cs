using System;

namespace punto_de_venta_C_.Modelo
{
    public class Movimiento
    {
        public enum Tipo
        {
            VENTA, RETIRO, FONDO_INICIAL
        }

        private Tipo tipo;
        private string descripcion;
        private double monto;
        private DateTime fecha;
        private string usuario;

        public Movimiento(Tipo tipo, string descripcion, double monto, DateTime fecha, string usuario)
        {
            this.tipo = tipo;
            this.descripcion = descripcion;
            this.monto = monto;
            this.fecha = fecha;
            this.usuario = usuario;
        }

        public Tipo getTipo() { return tipo; }
        public void setTipo(Tipo tipo) { this.tipo = tipo; }
        public string getDescripcion() { return descripcion; }
        public void setDescripcion(string descripcion) { this.descripcion = descripcion; }
        public double getMonto() { return monto; }
        public void setMonto(double monto) { this.monto = monto; }
        public DateTime getFecha() { return fecha; }
        public void setFecha(DateTime fecha) { this.fecha = fecha; }
        public string getUsuario() { return usuario; }
        public void setUsuario(string usuario) { this.usuario = usuario; }

        public bool esIngreso()
        {
            return tipo == Tipo.VENTA || tipo == Tipo.FONDO_INICIAL;
        }
    }
}
