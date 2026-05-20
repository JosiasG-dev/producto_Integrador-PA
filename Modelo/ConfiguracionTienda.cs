namespace punto_de_venta_C_.Modelo
{
    public class ConfiguracionTienda
    {
        private string nombre;
        private string sucursal;
        private string rfc;

        public ConfiguracionTienda(string nombre, string sucursal, string rfc)
        {
            this.nombre = nombre;
            this.sucursal = sucursal;
            this.rfc = rfc;
        }

        public string getNombre() { return nombre; }
        public void setNombre(string nombre) { this.nombre = nombre; }
        public string getSucursal() { return sucursal; }
        public void setSucursal(string sucursal) { this.sucursal = sucursal; }
        public string getRfc() { return rfc; }
        public void setRfc(string rfc) { this.rfc = rfc; }
    }
}
