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
    public class NuevaOrdenDialog : ContentPage
    {
        private readonly ProveedorControlador ctrl;
        private readonly List<OrdenCompra.ItemOrden> items = new List<OrdenCompra.ItemOrden>();
        private Picker cmbProveedor;
        private Picker cmbTipoPago;
        private Entry txtBusquedaProd;
        private VerticalStackLayout listItems;
        private Label lblTotal;
        private List<Proveedor> proveedoresList;
        private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public NuevaOrdenDialog(ProveedorControlador ctrl)
        {
            this.ctrl = ctrl;
            Title = "Nueva Orden de Compra";
            BackgroundColor = Estilos.BG_BLANCO;
            construir();
        }

        private void construir()
        {
            var absoluteLayout = new AbsoluteLayout
            {
                WidthRequest = 740,
                HeightRequest = 640,
                BackgroundColor = Estilos.BG_BLANCO
            };

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 70,
                Padding = new Thickness(24, 15)
            };
            var tit = new Label
            {
                Text = "NUEVA ORDEN DE COMPRA",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };
            header.Add(tit);
            AbsoluteLayout.SetLayoutBounds(header, new Rect(0, 0, 740, 70));
            AbsoluteLayout.SetLayoutFlags(header, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(header);

            // Izquierda
            var lblProv = new Label { Text = "1. PROVEEDOR RESPONSABLE", FontSize = Estilos.FUENTE_XS_SIZE, TextColor = Estilos.TEXTO_TENUE };
            AbsoluteLayout.SetLayoutBounds(lblProv, new Rect(24, 90, 320, 14));
            AbsoluteLayout.SetLayoutFlags(lblProv, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblProv);

            cmbProveedor = new Picker { FontAttributes = FontAttributes.Bold };
            proveedoresList = ctrl.getProveedores();
            foreach (var p in proveedoresList)
            {
                cmbProveedor.Items.Add(p.getNombre());
            }
            if (cmbProveedor.Items.Count > 0) cmbProveedor.SelectedIndex = 0;
            AbsoluteLayout.SetLayoutBounds(cmbProveedor, new Rect(24, 110, 320, 42));
            AbsoluteLayout.SetLayoutFlags(cmbProveedor, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(cmbProveedor);

            var lblPago = new Label { Text = "2. TERMINOS DE PAGO", FontSize = Estilos.FUENTE_XS_SIZE, TextColor = Estilos.TEXTO_TENUE };
            AbsoluteLayout.SetLayoutBounds(lblPago, new Rect(24, 166, 320, 14));
            AbsoluteLayout.SetLayoutFlags(lblPago, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblPago);

            cmbTipoPago = new Picker { FontAttributes = FontAttributes.Bold };
            cmbTipoPago.Items.Add("CONTADO");
            cmbTipoPago.Items.Add("CREDITO");
            cmbTipoPago.SelectedIndex = 0;
            AbsoluteLayout.SetLayoutBounds(cmbTipoPago, new Rect(24, 186, 320, 42));
            AbsoluteLayout.SetLayoutFlags(cmbTipoPago, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(cmbTipoPago);

            // Derecha
            var lblBuscar = new Label { Text = "3. BUSCAR Y AGREGAR PRODUCTOS", FontSize = Estilos.FUENTE_XS_SIZE, TextColor = Estilos.TEXTO_TENUE };
            AbsoluteLayout.SetLayoutBounds(lblBuscar, new Rect(396, 90, 320, 14));
            AbsoluteLayout.SetLayoutFlags(lblBuscar, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblBuscar);

            txtBusquedaProd = new Entry { FontSize = Estilos.FUENTE_BOLD_SIZE, BackgroundColor = Estilos.BG_ZINC_100 };
            txtBusquedaProd.Completed += (s, e) => buscarYAgregar();
            AbsoluteLayout.SetLayoutBounds(txtBusquedaProd, new Rect(396, 110, 320, 42));
            AbsoluteLayout.SetLayoutFlags(txtBusquedaProd, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtBusquedaProd);

            var btnAgregar = Estilos.botonExito("Agregar");
            btnAgregar.Clicked += (s, e) => buscarYAgregar();
            AbsoluteLayout.SetLayoutBounds(btnAgregar, new Rect(396, 162, 120, 36));
            AbsoluteLayout.SetLayoutFlags(btnAgregar, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnAgregar);

            // Table of selected products
            listItems = new VerticalStackLayout();
            var scroll = new ScrollView
            {
                Content = listItems,
                BackgroundColor = Estilos.BG_BLANCO
            };
            AbsoluteLayout.SetLayoutBounds(scroll, new Rect(24, 250, 692, 280));
            AbsoluteLayout.SetLayoutFlags(scroll, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(scroll);

            // Footer
            var footer = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 80,
                Padding = new Thickness(24, 15),
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 300 }
                }
            };

            lblTotal = new Label
            {
                Text = "Inversion Total:  $0.00",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL,
                VerticalOptions = LayoutOptions.Center
            };
            footer.Add(lblTotal, 0, 0);

            var btns = new HorizontalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };

            var btnCancelar = Estilos.botonSecundario("Cancelar");
            btnCancelar.HeightRequest = 44;
            btnCancelar.WidthRequest = 110;
            btnCancelar.Clicked += async (s, e) =>
            {
                tcs.SetResult(false);
                await Navigation.PopModalAsync();
            };
            btns.Add(btnCancelar);

            var btnConfirmar = Estilos.botonPrimario("Confirmar Orden");
            btnConfirmar.HeightRequest = 44;
            btnConfirmar.WidthRequest = 180;
            btnConfirmar.Clicked += (s, e) => confirmarOrden();
            btns.Add(btnConfirmar);

            footer.Add(btns, 1, 0);

            AbsoluteLayout.SetLayoutBounds(footer, new Rect(0, 560, 740, 80));
            AbsoluteLayout.SetLayoutFlags(footer, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(footer);

            Content = new Grid
            {
                Children = { absoluteLayout },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            actualizarTablaItems();
        }

        private async void buscarYAgregar()
        {
            string q = txtBusquedaProd.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(q))
                return;

            List<Producto> res = new List<Producto>();
            foreach (var p in ctrl.getProductos())
            {
                if (p.getNombre().Contains(q, StringComparison.OrdinalIgnoreCase))
                    res.Add(p);
                if (res.Count >= 5)
                    break;
            }

            if (res.Count == 0)
            {
                await DisplayAlert("Aviso", "No se encontraron productos.", "OK");
                return;
            }

            string[] names = res.Select(p => p.getNombre()).ToArray();
            string choice = await DisplayActionSheet("Seleccione un producto:", "Cancelar", null, names);
            if (string.IsNullOrEmpty(choice) || choice == "Cancelar")
                return;

            Producto sel = res.First(p => p.getNombre() == choice);
            string unidad = sel.getUnidad();
            string mensajeCant = sel.esPorPieza() ? "Cantidad a pedir (Piezas):" : "Cantidad a pedir (" + unidad + "):";
            string entradaCant = await DisplayPromptAsync("Agregar Producto", mensajeCant, "Aceptar", "Cancelar", sel.esPorPieza() ? "1" : "1.0");

            if (string.IsNullOrEmpty(entradaCant))
                return;

            double cantidad;
            if (!double.TryParse(entradaCant.Trim(), out cantidad) || cantidad <= 0)
            {
                await DisplayAlert("Error", "Cantidad invalida", "OK");
                return;
            }
            if (sel.esPorPieza())
                cantidad = Math.Round(cantidad);

            foreach (var item in items)
            {
                if (item.getProducto().getId().Equals(sel.getId()))
                {
                    item.setCantidadSolicitada(item.getCantidadSolicitada() + cantidad);
                    actualizarTablaItems();
                    txtBusquedaProd.Text = "";
                    return;
                }
            }

            items.Add(new OrdenCompra.ItemOrden(sel, cantidad, sel.getPrecio() * 0.75));
            actualizarTablaItems();
            txtBusquedaProd.Text = "";
        }

        private void actualizarTablaItems()
        {
            listItems.Children.Clear();

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 30,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 80 },
                    new ColumnDefinition { Width = 110 },
                    new ColumnDefinition { Width = 110 },
                    new ColumnDefinition { Width = 90 }
                }
            };
            header.Add(new Label { Text = "Producto", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(5, 0) }, 0);
            header.Add(new Label { Text = "Unidad", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 1);
            header.Add(new Label { Text = "Cant.", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 2);
            header.Add(new Label { Text = "Costo Unit.", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 3);
            header.Add(new Label { Text = "Subtotal", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 4);
            header.Add(new Label { Text = "Quitar", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 5);
            listItems.Children.Add(header);

            double total = 0;
            for (int i = 0; i < items.Count; i++)
            {
                int index = i;
                var item = items[i];
                var p = item.getProducto();

                var rowGrid = new Grid
                {
                    HeightRequest = 40,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 80 },
                        new ColumnDefinition { Width = 110 },
                        new ColumnDefinition { Width = 110 },
                        new ColumnDefinition { Width = 90 }
                    }
                };

                rowGrid.Add(new Label { Text = p.getNombre().ToUpper(), VerticalOptions = LayoutOptions.Center, Margin = new Thickness(5, 0) }, 0);
                rowGrid.Add(new Label { Text = p.getUnidad(), VerticalOptions = LayoutOptions.Center }, 1);

                // Entry for editable quantity
                var entryCant = new Entry
                {
                    Text = p.esPorPieza() ? item.getCantidadSolicitada().ToString("0") : item.getCantidadSolicitada().ToString("0.00"),
                    Keyboard = Keyboard.Numeric,
                    VerticalOptions = LayoutOptions.Center,
                    HeightRequest = 32
                };
                entryCant.Unfocused += (s, e) =>
                {
                    if (double.TryParse(entryCant.Text, out double nv) && nv > 0)
                    {
                        if (p.esPorPieza()) nv = Math.Round(nv);
                        item.setCantidadSolicitada(nv);
                        actualizarTablaItems();
                    }
                    else
                    {
                        entryCant.Text = p.esPorPieza() ? item.getCantidadSolicitada().ToString("0") : item.getCantidadSolicitada().ToString("0.00");
                    }
                };
                rowGrid.Add(entryCant, 2);

                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", item.getPrecioCosto()), VerticalOptions = LayoutOptions.Center }, 3);
                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", item.getSubtotal()), VerticalOptions = LayoutOptions.Center }, 4);

                var btnQuitar = Estilos.botonPeligro("Quitar");
                btnQuitar.FontSize = Estilos.FUENTE_XS_SIZE;
                btnQuitar.HeightRequest = 28;
                btnQuitar.Padding = new Thickness(0);
                btnQuitar.Clicked += (s, e) =>
                {
                    items.RemoveAt(index);
                    actualizarTablaItems();
                };
                rowGrid.Add(btnQuitar, 5);

                listItems.Children.Add(rowGrid);
                total += item.getSubtotal();
            }

            lblTotal.Text = string.Format("Inversion Total:  ${0:0.00}", total);
        }

        private async void confirmarOrden()
        {
            if (items.Count == 0)
            {
                await DisplayAlert("Aviso", "Agregue al menos un producto.", "OK");
                return;
            }

            if (cmbProveedor.SelectedIndex < 0)
            {
                await DisplayAlert("Aviso", "Seleccione un proveedor.", "OK");
                return;
            }

            Proveedor prov = proveedoresList[cmbProveedor.SelectedIndex];
            OrdenCompra.TipoPago tipoPago = cmbTipoPago.SelectedIndex == 1 ? OrdenCompra.TipoPago.CREDITO : OrdenCompra.TipoPago.CONTADO;

            double total = items.Sum(item => item.getSubtotal());
            OrdenCompra orden = new OrdenCompra(0, prov, new List<OrdenCompra.ItemOrden>(items), total, tipoPago,
                    OrdenCompra.Estado.PENDIENTE, DateTime.Now);

            ctrl.crearOrden(orden);
            await DisplayAlert("Éxito", "Orden " + orden.getFolioCorto() + " generada correctamente.", "OK");
            tcs.SetResult(true);
            await Navigation.PopModalAsync();
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
