using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace punto_de_venta_C_.Vista
{
    public static class Estilos
    {
        public static readonly Color BG_OSCURO = Color.FromRgb(24, 24, 27);
        public static readonly Color BG_MUY_OSCURO = Color.FromRgb(9, 9, 11);
        public static readonly Color BG_CLARO = Color.FromRgb(250, 250, 250);
        public static readonly Color BG_BLANCO = Colors.White;
        public static readonly Color BG_ZINC_100 = Color.FromRgb(244, 244, 245);
        public static readonly Color BG_ZINC_200 = Color.FromRgb(228, 228, 231);

        public static readonly Color INDIGO = Color.FromRgb(79, 70, 229);
        public static readonly Color INDIGO_OSCURO = Color.FromRgb(67, 56, 202);
        public static readonly Color INDIGO_CLARO = Color.FromRgb(238, 242, 255);

        public static readonly Color EMERALD = Color.FromRgb(5, 150, 105);
        public static readonly Color EMERALD_CLARO = Color.FromRgb(236, 253, 245);

        public static readonly Color ROSE = Color.FromRgb(225, 29, 72);
        public static readonly Color ROSE_CLARO = Color.FromRgb(255, 241, 242);
        public static readonly Color ROSE_OSCURO = Color.FromRgb(190, 18, 60);

        public static readonly Color AMBER = Color.FromRgb(217, 119, 6);
        public static readonly Color AMBER_CLARO = Color.FromRgb(255, 251, 235);

        public static readonly Color TEXTO_PRINCIPAL = Color.FromRgb(24, 24, 27);
        public static readonly Color TEXTO_SECUNDARIO = Color.FromRgb(113, 113, 122);
        public static readonly Color TEXTO_TENUE = Color.FromRgb(161, 161, 170);
        public static readonly Color TEXTO_BLANCO = Colors.White;

        public static readonly Color BORDE = Color.FromRgb(228, 228, 231);

        public static readonly double FUENTE_TITULO_SIZE = 22;
        public static readonly double FUENTE_SUBTITULO_SIZE = 14;
        public static readonly double FUENTE_NORMAL_SIZE = 13;
        public static readonly double FUENTE_BOLD_SIZE = 13;
        public static readonly double FUENTE_SMALL_SIZE = 11;
        public static readonly double FUENTE_XS_SIZE = 10;
        public static readonly double FUENTE_GRANDE_SIZE = 28;
        public static readonly double FUENTE_MONO_SIZE = 12;

        public static readonly int RADIO_BORDE = 16;
        public static readonly int PADDING = 16;
        public static readonly int PADDING_GRANDE = 24;

        public static Button botonPrimario(string texto)
        {
            var btn = new Button
            {
                Text = texto,
                BackgroundColor = INDIGO,
                TextColor = TEXTO_BLANCO,
                FontSize = FUENTE_BOLD_SIZE,
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 10,
                HeightRequest = 40,
                Padding = new Thickness(14, 5)
            };
            return btn;
        }

        public static Button botonPeligro(string texto)
        {
            var btn = botonPrimario(texto);
            btn.BackgroundColor = ROSE;
            return btn;
        }

        public static Button botonSecundario(string texto)
        {
            var btn = botonPrimario(texto);
            btn.BackgroundColor = BG_ZINC_200;
            btn.TextColor = TEXTO_PRINCIPAL;
            return btn;
        }

        public static Button botonExito(string texto)
        {
            var btn = botonPrimario(texto);
            btn.BackgroundColor = EMERALD;
            return btn;
        }

        public static Entry campo(string placeholder)
        {
            var tf = new Entry
            {
                Placeholder = placeholder,
                FontSize = FUENTE_BOLD_SIZE,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = BG_ZINC_100,
                TextColor = TEXTO_PRINCIPAL,
                PlaceholderColor = TEXTO_TENUE
            };
            return tf;
        }
    }
}
