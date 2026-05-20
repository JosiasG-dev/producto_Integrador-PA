namespace punto_de_venta_C_.Modelo
{
    public class Producto
    {
        private string id;
        private string nombre;
        private double precio;
        private double stock;
        private double stockMinimo;
        private string categoria;
        private string unidad;
        private string imagenRuta;

        public Producto(string id, string nombre, double precio, double stock, string categoria, string unidad, string imagenRuta)
        {
            this.id = id;
            this.nombre = nombre;
            this.precio = precio;
            this.stock = stock;
            this.stockMinimo = 5;
            this.categoria = categoria;
            this.unidad = unidad;
            this.imagenRuta = imagenRuta;
        }

        public string getId() { return id; }
        public void setId(string id) { this.id = id; }
        public string getNombre() { return nombre; }
        public void setNombre(string nombre) { this.nombre = nombre; }
        public double getPrecio() { return precio; }
        public void setPrecio(double precio) { this.precio = precio; }
        public double getStock() { return stock; }
        public void setStock(double stock) { this.stock = stock; }
        public double getStockMinimo() { return stockMinimo; }
        public void setStockMinimo(double min) { this.stockMinimo = min; }
        public string getCategoria() { return categoria; }
        public void setCategoria(string categoria) { this.categoria = categoria; }
        public string getUnidad() { return unidad; }
        public void setUnidad(string unidad) { this.unidad = unidad; }
        public string getImagenRuta()
        {
            if (string.IsNullOrWhiteSpace(imagenRuta))
            {
                try
                {
                    int numericId = int.Parse(id);
                    return "Images/" + numericId + ".jpg";
                }
                catch
                {
                    return "Images/" + id + ".jpg";
                }
            }
            return imagenRuta;
        }
        public void setImagenRuta(string imagenRuta) { this.imagenRuta = imagenRuta; }

        public bool esPorPieza() { return "Piezas".Equals(unidad); }
        public bool stockBajo() { return stock <= stockMinimo; }
        public void reducirStock(double cantidad)
        {
            this.stock = System.Math.Max(0, this.stock - cantidad);
            this.stock = System.Math.Round(this.stock * 1000.0) / 1000.0;
        }
        public void aumentarStock(double cantidad)
        {
            this.stock += cantidad;
            this.stock = System.Math.Round(this.stock * 1000.0) / 1000.0;
        }

        public override string ToString()
        {
            return nombre + " - $" + precio.ToString("0.00") + " [Stock: " + stock + "]";
        }
    }
}
