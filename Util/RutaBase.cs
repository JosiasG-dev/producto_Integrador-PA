using System;
using System.IO;

namespace punto_de_venta_C_.Util
{
    public class RutaBase
    {
        private static string raiz = null;

        public static string getRaiz()
        {
            if (raiz != null) return raiz;
            
            raiz = AppDomain.CurrentDomain.BaseDirectory;
            return raiz;
        }

        public static string getImages()
        {
            string carpeta = Path.Combine(getRaiz(), "Images");
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
            return carpeta;
        }

        public static string getImagen(string nombre)
        {
            return Path.Combine(getImages(), nombre);
        }

        public static Microsoft.Maui.Controls.ImageSource getImagenSource(string ruta)
        {
            if (string.IsNullOrEmpty(ruta)) return null;
            try
            {
                string fullPath = ruta;
                if (!Path.IsPathRooted(fullPath))
                    fullPath = Path.Combine(getRaiz(), fullPath);
                fullPath = Path.GetFullPath(fullPath);
                if (File.Exists(fullPath))
                {
                    byte[] bytes = File.ReadAllBytes(fullPath);
                    return Microsoft.Maui.Controls.ImageSource.FromStream(() => new MemoryStream(bytes));
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
    }
}
