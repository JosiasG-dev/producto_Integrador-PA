using System;
using System.Collections.Generic;
using System.Linq;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Vista;

namespace punto_de_venta_C_.Controlador
{
    public class InventarioControlador
    {
        private readonly ControladorPrincipal app;
        private InventarioPanel panel;

        public InventarioControlador(ControladorPrincipal app)
        {
            this.app = app;
        }

        public ControladorPrincipal getApp()
        {
            return app;
        }

        public void setPanel(InventarioPanel panel)
        {
            this.panel = panel;
        }

        public List<Producto> filtrar(string texto, string categoria)
        {
            List<Producto> todos = app.getProductos();
            List<Producto> resultado = new List<Producto>();
            foreach (Producto p in todos)
            {
                bool matchTexto = string.IsNullOrEmpty(texto)
                        || p.getNombre().ToLower().Contains(texto.ToLower()) || p.getId().Contains(texto);
                bool matchCat = string.IsNullOrEmpty(categoria) || p.getCategoria().Equals(categoria);
                if (matchTexto && matchCat)
                    resultado.Add(p);
            }
            return resultado;
        }

        public void guardarProducto(Producto p)
        {
            Producto existente = app.getProductoBD().buscarPorId(p.getId());
            if (existente != null)
            {
                app.getProductoBD().actualizar(p);
            }
            else
            {
                app.getProductoBD().insertar(p);
            }
            if (panel != null)
                panel.refrescar();
        }

        public void eliminarProducto(string id)
        {
            app.getProductoBD().eliminar(id);
            if (panel != null)
                panel.refrescar();
        }

        public Producto buscarPorId(string id)
        {
            return app.getProductoBD().buscarPorId(id);
        }

        public string generarNuevoId()
        {
            return app.getProductoBD().generarNuevoId();
        }
    }
}
