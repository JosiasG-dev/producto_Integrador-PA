using System.Collections.Generic;
using System.Linq;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Vista;

namespace punto_de_venta_C_.Controlador
{
    public class UsuarioControlador
    {
        private readonly ControladorPrincipal app;
        private UsuariosPanel panel;

        public UsuarioControlador(ControladorPrincipal app)
        {
            this.app = app;
        }

        public void setPanel(UsuariosPanel panel)
        {
            this.panel = panel;
        }

        public void guardarUsuario(Usuario u)
        {
            bool existe = app.getUsuarios().Any(x => x.getId() == u.getId());
            if (existe)
            {
                app.getUsuarioBD().actualizar(u);
            }
            else
            {
                app.getUsuarioBD().insertar(u);
            }
            if (panel != null)
                panel.refrescar();
        }

        public void eliminarUsuario(int id)
        {
            if (id == app.getUsuarioActivo().getId())
                return;
            app.getUsuarioBD().eliminar(id);
            if (panel != null)
                panel.refrescar();
        }

        public int generarId()
        {
            return 0;
        }

        public List<Usuario> getUsuarios()
        {
            return app.getUsuarios();
        }

        public Usuario getUsuarioActivo()
        {
            return app.getUsuarioActivo();
        }
    }
}
