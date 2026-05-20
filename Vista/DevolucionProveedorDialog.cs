using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System;
using System.Threading.Tasks;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;

namespace punto_de_venta_C_.Vista
{
    public class DevolucionProveedorDialog : ContentPage
    {
        private readonly InventarioControlador ctrl;
        private readonly Producto producto;
        private Entry txtCantidad;
        private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public DevolucionProveedorDialog(Producto p, InventarioControlador ctrl)
        {
            this.ctrl = ctrl;
            this.producto = p;
            Title = "Devolucion a Proveedor";
            BackgroundColor = Estilos.BG_BLANCO;
            construir();
        }

        private void construir()
        {
            var absoluteLayout = new AbsoluteLayout
            {
                WidthRequest = 380,
                HeightRequest = 300,
                BackgroundColor = Estilos.BG_BLANCO
            };

            var tit = new Label
            {
                Text = "DEVOLVER PRODUCTOS",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(tit, new Rect(24, 18, 300, 30));
            AbsoluteLayout.SetLayoutFlags(tit, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(tit);

            var lblProd = new Label
            {
                Text = "Producto: " + producto.getNombre(),
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(lblProd, new Rect(24, 60, 330, 20));
            AbsoluteLayout.SetLayoutFlags(lblProd, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblProd);

            var lblStock = new Label
            {
                Text = "Stock actual: " + producto.getStock() + " " + producto.getUnidad(),
                FontSize = Estilos.FUENTE_NORMAL_SIZE,
                TextColor = Estilos.TEXTO_SECUNDARIO
            };
            AbsoluteLayout.SetLayoutBounds(lblStock, new Rect(24, 85, 330, 20));
            AbsoluteLayout.SetLayoutFlags(lblStock, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblStock);

            var lblCant = new Label
            {
                Text = "CANTIDAD A DEVOLVER:",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            AbsoluteLayout.SetLayoutBounds(lblCant, new Rect(24, 130, 300, 14));
            AbsoluteLayout.SetLayoutFlags(lblCant, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblCant);

            txtCantidad = new Entry
            {
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                BackgroundColor = Estilos.BG_ZINC_100,
                TextColor = Estilos.TEXTO_PRINCIPAL,
                Keyboard = Keyboard.Numeric
            };
            AbsoluteLayout.SetLayoutBounds(txtCantidad, new Rect(24, 148, 315, 36));
            AbsoluteLayout.SetLayoutFlags(txtCantidad, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtCantidad);

            var btnCancelar = Estilos.botonSecundario("Cancelar");
            btnCancelar.Clicked += async (s, e) =>
            {
                tcs.SetResult(false);
                await Navigation.PopModalAsync();
            };
            AbsoluteLayout.SetLayoutBounds(btnCancelar, new Rect(110, 210, 110, 38));
            AbsoluteLayout.SetLayoutFlags(btnCancelar, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnCancelar);

            var btnGuardar = Estilos.botonPeligro("Devolver");
            btnGuardar.Clicked += (s, e) => intentarDevolucion();
            AbsoluteLayout.SetLayoutBounds(btnGuardar, new Rect(230, 210, 110, 38));
            AbsoluteLayout.SetLayoutFlags(btnGuardar, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnGuardar);

            Content = new Grid
            {
                Children = { absoluteLayout },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private async void intentarDevolucion()
        {
            try
            {
                double cantidad = double.Parse(txtCantidad.Text?.Trim() ?? "0");
                if (cantidad <= 0)
                {
                    await DisplayAlert("Aviso", "Ingrese una cantidad valida mayor a 0", "OK");
                    return;
                }
                if (cantidad > producto.getStock())
                {
                    await DisplayAlert("Aviso", "No puede devolver mas del stock existente", "OK");
                    return;
                }
                producto.setStock(producto.getStock() - cantidad);
                ctrl.guardarProducto(producto);
                await DisplayAlert("Éxito", "Devolucion procesada exitosamente", "OK");
                tcs.SetResult(true);
                await Navigation.PopModalAsync();
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Numero invalido", "OK");
            }
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
