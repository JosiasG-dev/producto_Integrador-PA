using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System;
using System.Linq;
using System.Threading.Tasks;
using punto_de_venta_C_.Modelo;

namespace punto_de_venta_C_.Vista
{
    public class TicketDialog : ContentPage
    {
        private readonly Venta venta;
        private readonly ConfiguracionTienda config;
        private readonly double cambio;
        private readonly double descuento;
        private readonly double efectivoRecibido;
        private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public TicketDialog(Venta venta, ConfiguracionTienda config, double cambio, double efectivoRecibido)
            : this(venta, config, cambio, venta.getDescuento(), efectivoRecibido)
        {
        }

        public TicketDialog(Venta venta, ConfiguracionTienda config, double cambio, double descuento, double efectivoRecibido)
        {
            this.venta = venta;
            this.config = config;
            this.cambio = cambio;
            this.efectivoRecibido = efectivoRecibido;
            this.descuento = venta.getDescuento() > 0 ? venta.getDescuento() : descuento;
            Title = "Ticket de Venta";
            BackgroundColor = Estilos.BG_BLANCO;
            construir();
        }

        private void construir()
        {
            var absoluteLayout = new AbsoluteLayout
            {
                WidthRequest = 380,
                HeightRequest = 660,
                BackgroundColor = Estilos.BG_BLANCO
            };

            int y = 0;

            var barra = new BoxView
            {
                Color = Estilos.INDIGO,
                HeightRequest = 8
            };
            AbsoluteLayout.SetLayoutBounds(barra, new Rect(0, 0, 380, 8));
            AbsoluteLayout.SetLayoutFlags(barra, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(barra);
            y = 18;

            absoluteLayout.Add(centrado(config.getNombre(), 15, FontAttributes.Bold, y));
            y += 22;

            var lblSuc = centrado(config.getSucursal(), Estilos.FUENTE_XS_SIZE, FontAttributes.None, y);
            lblSuc.TextColor = Estilos.TEXTO_SECUNDARIO;
            absoluteLayout.Add(lblSuc);
            y += 16;

            var lblRfc = centrado("RFC: " + config.getRfc(), Estilos.FUENTE_XS_SIZE, FontAttributes.None, y);
            lblRfc.TextColor = Estilos.TEXTO_SECUNDARIO;
            absoluteLayout.Add(lblRfc);
            y += 16;

            absoluteLayout.Add(sep(y));
            y += 14;

            absoluteLayout.Add(centrado(venta.getFecha().ToString("dd/MM/yyyy HH:mm:ss"), Estilos.FUENTE_XS_SIZE, FontAttributes.None, y));
            y += 16;
            absoluteLayout.Add(centrado("Cajero: " + venta.getCajero().ToUpper(), Estilos.FUENTE_XS_SIZE, FontAttributes.None, y));
            y += 16;

            var lblNum = centrado("Ticket #" + venta.getId(), Estilos.FUENTE_XS_SIZE, FontAttributes.None, y);
            lblNum.TextColor = Estilos.TEXTO_TENUE;
            absoluteLayout.Add(lblNum);
            y += 16;

            absoluteLayout.Add(sep(y));
            y += 14;

            // Items List
            var itemsLayout = new VerticalStackLayout();
            foreach (var item in venta.getItems())
            {
                var rowGrid = new Grid
                {
                    HeightRequest = 34,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 110 }
                    }
                };

                var itemTitleLayout = new VerticalStackLayout();
                var codLbl = new Label
                {
                    Text = "[" + item.getProducto().getId() + "] " + item.getProducto().getNombre().ToUpper(),
                    FontSize = Estilos.FUENTE_XS_SIZE,
                    TextColor = Estilos.TEXTO_PRINCIPAL
                };
                var detLbl = new Label
                {
                    Text = string.Format("  {0:0.00} x ${1:0.00}", item.getCantidad(), item.getProducto().getPrecio()),
                    FontSize = Estilos.FUENTE_XS_SIZE - 2,
                    TextColor = Estilos.TEXTO_TENUE
                };
                itemTitleLayout.Add(codLbl);
                itemTitleLayout.Add(detLbl);
                rowGrid.Add(itemTitleLayout, 0, 0);

                var subLbl = new Label
                {
                    Text = string.Format("${0:0.00}", item.getSubtotal()),
                    FontSize = Estilos.FUENTE_XS_SIZE,
                    TextColor = Estilos.TEXTO_PRINCIPAL,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                };
                rowGrid.Add(subLbl, 1, 0);

                itemsLayout.Add(rowGrid);
            }

            var itemsScroll = new ScrollView
            {
                Content = itemsLayout,
                HeightRequest = 180,
                BackgroundColor = Estilos.BG_BLANCO
            };
            AbsoluteLayout.SetLayoutBounds(itemsScroll, new Rect(14, y, 352, 180));
            AbsoluteLayout.SetLayoutFlags(itemsScroll, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(itemsScroll);
            y += 190;

            absoluteLayout.Add(sep(y));
            y += 10;

            double precioTotal = venta.getItems().Sum(i => i.getSubtotal());
            double montoDescuento = descuento;
            double montoAPagar = venta.getTotal();

            fila(absoluteLayout, "Precio total de compra:", string.Format("${0:0.00}", precioTotal), y, Estilos.TEXTO_SECUNDARIO);
            y += 18;

            Color colorDesc = montoDescuento > 0 ? Estilos.EMERALD : Estilos.TEXTO_TENUE;
            fila(absoluteLayout, "Monto de descuento:", string.Format("-${0:0.00}", montoDescuento), y, colorDesc);
            y += 18;

            absoluteLayout.Add(sep(y));
            y += 10;

            filaGrande(absoluteLayout, "MONTO A PAGAR:", string.Format("${0:0.00}", montoAPagar), y, Estilos.TEXTO_PRINCIPAL);
            y += 22;

            absoluteLayout.Add(sep(y));
            y += 10;

            fila(absoluteLayout, "Efectivo recibido:", string.Format("${0:0.00}", efectivoRecibido), y, Estilos.TEXTO_SECUNDARIO);
            y += 18;
            fila(absoluteLayout, "Cambio:", string.Format("${0:0.00}", cambio), y, Estilos.EMERALD);
            y += 20;

            var gracias = centrado("Gracias por su compra", Estilos.FUENTE_SMALL_SIZE, FontAttributes.None, y);
            gracias.TextColor = Estilos.TEXTO_TENUE;
            absoluteLayout.Add(gracias);
            y += 30;

            var btnImprimir = new Button
            {
                Text = "Imprimir / Cerrar",
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Estilos.BG_OSCURO,
                TextColor = Colors.White,
                HeightRequest = 44,
                WidthRequest = 352
            };
            btnImprimir.Clicked += async (s, e) =>
            {
                await DisplayAlert("Impresión", "Ticket enviado a la cola de impresión.", "OK");
                tcs.SetResult(true);
                await Navigation.PopModalAsync();
            };
            AbsoluteLayout.SetLayoutBounds(btnImprimir, new Rect(14, y + 4, 352, 44));
            AbsoluteLayout.SetLayoutFlags(btnImprimir, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnImprimir);

            Content = new Grid
            {
                Children = { absoluteLayout },
                BackgroundColor = Estilos.BG_BLANCO,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private Label centrado(string texto, double fontSize, FontAttributes fontAttributes, int y)
        {
            var lbl = new Label
            {
                Text = texto,
                FontSize = fontSize,
                FontAttributes = fontAttributes,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(lbl, new Rect(0, y, 380, 18));
            AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
            return lbl;
        }

        private BoxView sep(int y)
        {
            var s = new BoxView
            {
                Color = Estilos.BORDE,
                HeightRequest = 1
            };
            AbsoluteLayout.SetLayoutBounds(s, new Rect(14, y, 352, 1));
            AbsoluteLayout.SetLayoutFlags(s, AbsoluteLayoutFlags.None);
            return s;
        }

        private void fila(AbsoluteLayout p, string label, string valor, int y, Color colorValor)
        {
            var lbl = new Label
            {
                Text = label,
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_SECUNDARIO
            };
            AbsoluteLayout.SetLayoutBounds(lbl, new Rect(14, y, 180, 16));
            AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
            p.Add(lbl);

            var val = new Label
            {
                Text = valor,
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = colorValor,
                HorizontalTextAlignment = TextAlignment.End
            };
            AbsoluteLayout.SetLayoutBounds(val, new Rect(200, y, 166, 16));
            AbsoluteLayout.SetLayoutFlags(val, AbsoluteLayoutFlags.None);
            p.Add(val);
        }

        private void filaGrande(AbsoluteLayout p, string label, string valor, int y, Color colorValor)
        {
            var lbl = new Label
            {
                Text = label,
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(lbl, new Rect(14, y, 180, 20));
            AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
            p.Add(lbl);

            var val = new Label
            {
                Text = valor,
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                TextColor = colorValor,
                HorizontalTextAlignment = TextAlignment.End
            };
            AbsoluteLayout.SetLayoutBounds(val, new Rect(180, y, 186, 20));
            AbsoluteLayout.SetLayoutFlags(val, AbsoluteLayoutFlags.None);
            p.Add(val);
        }

        public async Task<bool> mostrar()
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(this));
            }
            return await tcs.Task;
        }
    }
}
