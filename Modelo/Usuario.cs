namespace punto_de_venta_C_.Modelo
{
    public class Usuario
    {
        private int id;
        private string username;
        private string password;
        private string rol;
        private string nombre;
        private int edad;
        private string sexo;

        public Usuario(int id, string username, string password, string rol, string nombre, int edad, string sexo)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.rol = rol;
            this.nombre = nombre;
            this.edad = edad;
            this.sexo = sexo;
        }

        public int getId() { return id; }
        public void setId(int id) { this.id = id; }
        public string getUsername() { return username; }
        public void setUsername(string username) { this.username = username; }
        public string getPassword() { return password; }
        public void setPassword(string password) { this.password = password; }
        public string getRol() { return rol; }
        public void setRol(string rol) { this.rol = rol; }
        public string getNombre() { return nombre; }
        public void setNombre(string nombre) { this.nombre = nombre; }
        public int getEdad() { return edad; }
        public void setEdad(int edad) { this.edad = edad; }
        public string getSexo() { return sexo; }
        public void setSexo(string sexo) { this.sexo = sexo; }

        public bool esAdmin() { return "ADMINISTRADOR".Equals(this.rol); }

        public override string ToString()
        {
            return nombre + " [" + rol + "]";
        }
    }
}
