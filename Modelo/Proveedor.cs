namespace punto_de_venta_C_.Modelo
{
    public class Proveedor
    {
        private int id;
        private string nombre;
        private string contacto;
        private string telefono;
        private string email;
        private string direccion;
        private bool activo;

        public Proveedor(int id, string nombre, string contacto, string telefono, string email, string direccion, bool activo)
        {
            this.id = id;
            this.nombre = nombre;
            this.contacto = contacto;
            this.telefono = telefono;
            this.email = email;
            this.direccion = direccion;
            this.activo = activo;
        }

        public int getId() { return id; }
        public void setId(int id) { this.id = id; }
        public string getNombre() { return nombre; }
        public void setNombre(string nombre) { this.nombre = nombre; }
        public string getContacto() { return contacto; }
        public void setContacto(string contacto) { this.contacto = contacto; }
        public string getTelefono() { return telefono; }
        public void setTelefono(string telefono) { this.telefono = telefono; }
        public string getEmail() { return email; }
        public void setEmail(string email) { this.email = email; }
        public string getDireccion() { return direccion; }
        public void setDireccion(string direccion) { this.direccion = direccion; }
        public bool isActivo() { return activo; }
        public void setActivo(bool activo) { this.activo = activo; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
