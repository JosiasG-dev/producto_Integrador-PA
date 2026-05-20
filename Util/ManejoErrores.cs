using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace punto_de_venta_C_.Util
{
    public class ManejoErrores
    {
        private static readonly string RUTA_LOG = "corporativo_pos_errores.log";

        static ManejoErrores()
        {
        }

        public static async Task error(Page padre, string titulo, string mensaje)
        {
            registrar("SEVERE", titulo + ": " + mensaje);
            if (padre != null)
                await padre.DisplayAlert(titulo, mensaje, "OK");
        }

        public static async Task error(Page padre, string titulo, string mensaje, Exception ex)
        {
            registrarError(titulo + ": " + mensaje, ex);
            if (padre != null)
                await padre.DisplayAlert(titulo, mensaje + "\n\nDetalle: " + ex.Message, "OK");
        }

        public static async Task advertencia(Page padre, string titulo, string mensaje)
        {
            registrar("WARNING", titulo + ": " + mensaje);
            if (padre != null)
                await padre.DisplayAlert(titulo, mensaje, "OK");
        }

        public static async Task info(Page padre, string titulo, string mensaje)
        {
            if (padre != null)
                await padre.DisplayAlert(titulo, mensaje, "OK");
        }

        public static async Task<bool> confirmar(Page padre, string titulo, string mensaje)
        {
            if (padre != null)
                return await padre.DisplayAlert(titulo, mensaje, "Sí", "No");
            return false;
        }

        public static void registrar(string nivel, string mensaje)
        {
            try
            {
                string log = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] [{nivel}] {mensaje}\n";
                File.AppendAllText(RUTA_LOG, log);
            }
            catch { }
        }

        public static void registrarError(string contexto, Exception ex)
        {
            registrar("SEVERE", contexto + "\n" + ex.ToString());
        }

        public static void registrarInfo(string evento)
        {
            registrar("INFO", evento);
        }

        public static async Task<bool> validarRequerido(Page padre, string valor, string nombreCampo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                await advertencia(padre, "Campo requerido", "El campo '" + nombreCampo + "' es obligatorio.");
                return false;
            }
            return true;
        }

        public static async Task<double> validarNumero(Page padre, string valor, string nombreCampo)
        {
            try
            {
                double d = double.Parse(valor.Trim().Replace(",", "."));
                if (d < 0)
                {
                    await advertencia(padre, "Valor invalido", "'" + nombreCampo + "' debe ser un numero positivo.");
                    return -1;
                }
                return d;
            }
            catch (FormatException)
            {
                await advertencia(padre, "Formato incorrecto", "'" + nombreCampo + "' debe ser un numero valido.");
                return -1;
            }
        }

        public static string getRuta()
        {
            return Path.GetFullPath(RUTA_LOG);
        }
    }
}
