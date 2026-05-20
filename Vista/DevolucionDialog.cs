using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;

namespace punto_de_venta_C_.Vista
{
    public class DevolucionDialog : ContentPage
    {
        private readonly ControladorPrincipal app;
        private Entry txtVentaId;
        private Entry txtMotivo;
        private VerticalStackLayout listItems;
        private List<ItemCarrito> itemsVenta;
        private List<Entry> entryCantidades = new List<Entry>();
        private int ventaId = -1;
        private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public DevolucionDialog(ControladorPrincipal app)
        {
            this.app = app;
            Title = "Devolucion de Productos";
            BackgroundColor = Estilos.BG_BLANCO;
            construir();
        }

        private void construir()
        {
            var absoluteLayout = new AbsoluteLayout
            {
                WidthRequest = 560,
                HeightRequest = 540,
                BackgroundColor = Estilos.BG_BLANCO
            };

            var tit = new Label
            {
                Text = "DEVOLUCION DE PRODUCTOS",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(tit, new Rect(24, 16, 500, 30));
            AbsoluteLayout.SetLayoutFlags(tit, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(tit);

            var lblVenta = new Label
            {
                Text = "NUMERO DE VENTA (#):",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            AbsoluteLayout.SetLayoutBounds(lblVenta, new Rect(24, 58, 200, 14));
            AbsoluteLayout.SetLayoutFlags(lblVenta, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblVenta);

            txtVentaId = new Entry
            {
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                BackgroundColor = Estilos.BG_ZINC_100,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(txtVentaId, new Rect(24, 76, 180, 36));
            AbsoluteLayout.SetLayoutFlags(txtVentaId, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtVentaId);

            var btnBuscar = Estilos.botonPrimario("Buscar Venta");
            btnBuscar.Clicked += (s, e) => buscarVenta();
            AbsoluteLayout.SetLayoutBounds(btnBuscar, new Rect(214, 76, 130, 36));
            AbsoluteLayout.SetLayoutFlags(btnBuscar, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnBuscar);

            var lblItems = new Label
            {
                Text = "PRODUCTOS DE LA VENTA (modifica la cantidad a devolver):",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            AbsoluteLayout.SetLayoutBounds(lblItems, new Rect(24, 126, 500, 14));
            AbsoluteLayout.SetLayoutFlags(lblItems, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblItems);

            listItems = new VerticalStackLayout();
            var scroll = new ScrollView
            {
                Content = listItems,
                BackgroundColor = Estilos.BG_BLANCO
            };
            AbsoluteLayout.SetLayoutBounds(scroll, new Rect(24, 144, 510, 220));
            AbsoluteLayout.SetLayoutFlags(scroll, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(scroll);

            var lblMotivo = new Label
            {
                Text = "MOTIVO DE DEVOLUCION:",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            AbsoluteLayout.SetLayoutBounds(lblMotivo, new Rect(24, 378, 300, 14));
            AbsoluteLayout.SetLayoutFlags(lblMotivo, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblMotivo);

            txtMotivo = new Entry
            {
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                BackgroundColor = Estilos.BG_ZINC_100,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(txtMotivo, new Rect(24, 396, 510, 36));
            AbsoluteLayout.SetLayoutFlags(txtMotivo, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtMotivo);

            var btnCancelar = Estilos.botonSecundario("Cancelar");
            btnCancelar.Clicked += async (s, e) =>
            {
                tcs.SetResult(false);
                await Navigation.PopModalAsync();
            };
            AbsoluteLayout.SetLayoutBounds(btnCancelar, new Rect(300, 452, 110, 38));
            AbsoluteLayout.SetLayoutFlags(btnCancelar, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnCancelar);

            var btnRegistrar = Estilos.botonPeligro("Registrar Devolucion");
            btnRegistrar.Clicked += (s, e) => registrarDevolucion();
            AbsoluteLayout.SetLayoutBounds(btnRegistrar, new Rect(420, 452, 114, 38));
            AbsoluteLayout.SetLayoutFlags(btnRegistrar, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnRegistrar);

            Content = new Grid
            {
                Children = { absoluteLayout },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private async void buscarVenta()
        {
            string txt = txtVentaId.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(txt))
                return;

            if (!int.TryParse(txt, out ventaId))
            {
                await DisplayAlert("Error", "Ingresa un numero de venta valido", "OK");
                return;
            }

            Venta venta = app.getVentas().FirstOrDefault(v => v.getId() == ventaId);
            if (venta == null)
            {
                await DisplayAlert("Sin resultados", "No se encontro la venta #" + ventaId, "OK");
                ventaId = -1;
                return;
            }

            itemsVenta = venta.getItems();
            listItems.Children.Clear();
            entryCantidades.Clear();

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 30,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 60 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 90 },
                    new ColumnDefinition { Width = 90 },
                    new ColumnDefinition { Width = 90 }
                }
            };
            header.Add(new Label { Text = "SKU", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(5, 0) }, 0);
            header.Add(new Label { Text = "Producto", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 1);
            header.Add(new Label { Text = "Cant Vend", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 2);
            header.Add(new Label { Text = "Precio", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 3);
            header.Add(new Label { Text = "A Devolver", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 4);
            listItems.Children.Add(header);

            foreach (var item in itemsVenta)
            {
                var rowGrid = new Grid
                {
                    HeightRequest = 38,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = 60 },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 90 },
                        new ColumnDefinition { Width = 90 },
                        new ColumnDefinition { Width = 90 }
                    }
                };

                rowGrid.Add(new Label { Text = item.getProducto().getId(), VerticalOptions = LayoutOptions.Center, Margin = new Thickness(5, 0) }, 0);
                rowGrid.Add(new Label { Text = item.getProducto().getNombre(), VerticalOptions = LayoutOptions.Center }, 1);
                rowGrid.Add(new Label { Text = item.getCantidad().ToString("0.00"), VerticalOptions = LayoutOptions.Center }, 2);
                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", item.getProducto().getPrecio()), VerticalOptions = LayoutOptions.Center }, 3);

                var entryCant = new Entry
                {
                    Text = "0",
                    Keyboard = Keyboard.Numeric,
                    VerticalOptions = LayoutOptions.Center,
                    HeightRequest = 30
                };
                entryCantidades.Add(entryCant);
                rowGrid.Add(entryCant, 4);

                listItems.Children.Add(rowGrid);
            }
        }

        private async void registrarDevolucion()
        {
            if (ventaId < 0 || itemsVenta == null)
            {
                await DisplayAlert("Aviso", "Primero busca una venta", "OK");
                return;
            }

            string motivo = txtMotivo.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(motivo))
            {
                await DisplayAlert("Aviso", "Ingresa el motivo de la devolucion", "OK");
                return;
            }

            bool alguna = false;
            for (int i = 0; i < entryCantidades.Count; i++)
            {
                try
                {
                    double cantDevolver = double.Parse(entryCantidades[i].Text?.Trim() ?? "0");
                    if (cantDevolver <= 0)
                        continue;

                    ItemCarrito item = itemsVenta[i];
                    double cantVendida = item.getCantidad();
                    if (cantDevolver > cantVendida)
                    {
                        await DisplayAlert("Error", "No puedes devolver mas de lo vendido para: " + item.getProducto().getNombre(), "OK");
                        return;
                    }

                    Devolucion d = new Devolucion(0, ventaId, item.getProducto(), cantDevolver, motivo, DateTime.Now,
                            app.getUsuarioActivo().getNombre());
                    app.registrarDevolucion(d);
                    alguna = true;
                }
                catch (Exception)
                {
                }
            }

            if (alguna)
            {
                await DisplayAlert("Exito", "Devolucion registrada. El stock fue actualizado.", "OK");
                tcs.SetResult(true);
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Sin cantidades", "Ingresa al menos una cantidad mayor a 0 en 'A Devolver'", "OK");
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
