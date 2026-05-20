using System;
using System.Collections.Generic;
using System.Linq;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Vista;

namespace punto_de_venta_C_.Controlador
{
    public class CajaControlador
    {
        private readonly ControladorPrincipal app;
        private CajaPanel panel;

        public CajaControlador(ControladorPrincipal app)
        {
            this.app = app;
        }

        public void setPanel(CajaPanel panel)
        {
            this.panel = panel;
        }

        public void abrirCaja(double fondo)
        {
            app.abrirCaja(fondo);
            if (panel != null)
                panel.refrescar();
        }

        public void registrarRetiro(string concepto, double monto)
        {
            app.registrarRetiro(concepto, monto);
            if (panel != null)
                panel.refrescar();
        }

        public void registrarIngresoExtra(string concepto, double monto)
        {
            app.getMovimientoBD().insertar(new Movimiento(Movimiento.Tipo.VENTA, concepto, monto, DateTime.Now,
                    app.getUsuarioActivo().getNombre()));
            app.registrarVentaSimple(monto);
            if (panel != null)
                panel.refrescar();
        }

        public double getTotalVentas()
        {
            return app.getMovimientos().Where(m => m.getTipo() == Movimiento.Tipo.VENTA)
                    .Select(m => m.getMonto()).Sum();
        }

        public double getTotalEgresos()
        {
            return app.getMovimientos().Where(m => m.getTipo() == Movimiento.Tipo.RETIRO)
                    .Select(m => m.getMonto()).Sum();
        }

        public double getEfectivoEsperado()
        {
            return app.getMontoCaja();
        }

        public List<Movimiento> getMovimientos()
        {
            return app.getMovimientos();
        }

        public bool isCajaAbierta()
        {
            return app.isCajaAbierta();
        }

        public double getMontoCaja()
        {
            return app.getMontoCaja();
        }
    }
}
