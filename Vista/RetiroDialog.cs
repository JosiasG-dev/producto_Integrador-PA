using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System;
using System.Threading.Tasks;
using punto_de_venta_C_.Controlador;

namespace punto_de_venta_C_.Vista
{
    public class RetiroDialog : ContentPage
    {
        private readonly CajaControlador ctrl;
        private Entry txtConcepto;
        private Entry txtMonto;
        private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public RetiroDialog(CajaControlador ctrl)
        {
            this.ctrl = ctrl;
            Title = "Salida de Efectivo";
            BackgroundColor = Estilos.BG_BLANCO;
            construir();
        }

        private void construir()
        {
            var absoluteLayout = new AbsoluteLayout
            {
                WidthRequest = 440,
                HeightRequest = 340,
                BackgroundColor = Estilos.BG_BLANCO
            };

            var tit = new Label
            {
                Text = "SALIDA DE EFECTIVO",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(tit, new Rect(36, 32, 368, 30));
            AbsoluteLayout.SetLayoutFlags(tit, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(tit);

            var line = new BoxView
            {
                Color = Estilos.BORDE,
                HeightRequest = 2
            };
            AbsoluteLayout.SetLayoutBounds(line, new Rect(36, 74, 368, 2));
            AbsoluteLayout.SetLayoutFlags(line, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(line);

            // Concepto
            var lblConcepto = lbl("Concepto");
            AbsoluteLayout.SetLayoutBounds(lblConcepto, new Rect(36, 92, 368, 14));
            AbsoluteLayout.SetLayoutFlags(lblConcepto, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblConcepto);

            txtConcepto = campo();
            txtConcepto.Placeholder = "Ej. Pago Proveedor...";
            AbsoluteLayout.SetLayoutBounds(txtConcepto, new Rect(36, 110, 368, 40));
            AbsoluteLayout.SetLayoutFlags(txtConcepto, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtConcepto);

            // Monto
            var lblMonto = lbl("Monto ($)");
            AbsoluteLayout.SetLayoutBounds(lblMonto, new Rect(36, 164, 368, 14));
            AbsoluteLayout.SetLayoutFlags(lblMonto, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblMonto);

            txtMonto = new Entry
            {
                Text = "0.00",
                FontSize = 26,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Estilos.BG_ZINC_100,
                TextColor = Estilos.TEXTO_PRINCIPAL,
                Keyboard = Keyboard.Numeric
            };
            AbsoluteLayout.SetLayoutBounds(txtMonto, new Rect(36, 182, 368, 50));
            AbsoluteLayout.SetLayoutFlags(txtMonto, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtMonto);

            // Botones
            var btns = new HorizontalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.End
            };

            var btnCancelar = Estilos.botonSecundario("Cancelar");
            btnCancelar.HeightRequest = 44;
            btnCancelar.WidthRequest = 120;
            btnCancelar.Clicked += async (s, e) =>
            {
                tcs.SetResult(false);
                await Navigation.PopModalAsync();
            };
            btns.Add(btnCancelar);

            var btnAutorizar = Estilos.botonPeligro("AUTORIZAR");
            btnAutorizar.HeightRequest = 44;
            btnAutorizar.WidthRequest = 140;
            btnAutorizar.Clicked += (s, e) => autorizar();
            btns.Add(btnAutorizar);

            AbsoluteLayout.SetLayoutBounds(btns, new Rect(36, 256, 368, 52));
            AbsoluteLayout.SetLayoutFlags(btns, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btns);

            Content = new Grid
            {
                Children = { absoluteLayout },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private async void autorizar()
        {
            string concepto = txtConcepto.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(concepto))
            {
                await DisplayAlert("Aviso", "Ingrese un concepto.", "OK");
                return;
            }

            try
            {
                double monto = double.Parse(txtMonto.Text?.Trim() ?? "0");
                if (monto <= 0)
                    throw new FormatException();

                ctrl.registrarRetiro(concepto, monto);
                tcs.SetResult(true);
                await Navigation.PopModalAsync();
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Ingrese un monto válido mayor a 0.", "OK");
            }
        }

        private Label lbl(string texto)
        {
            return new Label
            {
                Text = texto.ToUpper(),
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
        }

        private Entry campo()
        {
            return new Entry
            {
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                BackgroundColor = Estilos.BG_ZINC_100,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
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
