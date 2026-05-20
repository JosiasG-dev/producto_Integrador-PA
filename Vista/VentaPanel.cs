using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;

namespace punto_de_venta_C_.Vista
{
    public class VentaPanel : ContentView
    {
        private readonly VentaControlador ctrl;
        private readonly ConfiguracionTienda config;
        private readonly Usuario cajero;

        private Entry txtBusqueda;
        private VerticalStackLayout sugerenciasPanel;
        private ScrollView sugerenciasScroll;
        private VerticalStackLayout carritoList;
        private Label lblTotal;
        private Entry txtRecibido;
        private Entry txtDescuento;
        private Label lblCambio;
        private Label lblRecibido;
        private Grid containerImagenProducto;
        private Image imgProducto;
        private Label lblPlaceholderImagen;
        private Picker cmbMetodo;
        private bool actualizando = false;

        public VentaPanel(VentaControlador ctrl, ConfiguracionTienda config, Usuario cajero)
        {
            this.ctrl = ctrl;
            this.config = config;
            this.cajero = cajero;
            ctrl.setPanel(this);
            BackgroundColor = Estilos.BG_CLARO;
            construir();
        }

        private void construir()
        {
            var absoluteLayout = new AbsoluteLayout
            {
                WidthRequest = 1000,
                HeightRequest = 700
            };

            var lblBusq = new Label
            {
                Text = "BUSCAR PRODUCTO (nombre o SKU):",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            AbsoluteLayout.SetLayoutBounds(lblBusq, new Rect(16, 14, 300, 14));
            AbsoluteLayout.SetLayoutFlags(lblBusq, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblBusq);

            txtBusqueda = new Entry
            {
                FontSize = Estilos.FUENTE_SUBTITULO_SIZE,
                BackgroundColor = Estilos.BG_BLANCO,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            txtBusqueda.TextChanged += (s, e) => actualizarSugerencias();
            AbsoluteLayout.SetLayoutBounds(txtBusqueda, new Rect(16, 32, 700, 44));
            AbsoluteLayout.SetLayoutFlags(txtBusqueda, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtBusqueda);

            sugerenciasPanel = new VerticalStackLayout
            {
                BackgroundColor = Estilos.BG_BLANCO
            };

            sugerenciasScroll = new ScrollView
            {
                Content = sugerenciasPanel,
                IsVisible = false,
                BackgroundColor = Estilos.BG_BLANCO
            };
            AbsoluteLayout.SetLayoutBounds(sugerenciasScroll, new Rect(16, 76, 700, 200));
            AbsoluteLayout.SetLayoutFlags(sugerenciasScroll, AbsoluteLayoutFlags.None);
            sugerenciasScroll.ZIndex = 10;

            var lblDetalle = new Label
            {
                Text = "DETALLE DE VENTA",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            AbsoluteLayout.SetLayoutBounds(lblDetalle, new Rect(16, 84, 200, 14));
            AbsoluteLayout.SetLayoutFlags(lblDetalle, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblDetalle);

            carritoList = new VerticalStackLayout();
            var scrollCarrito = new ScrollView
            {
                Content = carritoList,
                BackgroundColor = Estilos.BG_BLANCO
            };
            AbsoluteLayout.SetLayoutBounds(scrollCarrito, new Rect(16, 100, 700, 480));
            AbsoluteLayout.SetLayoutFlags(scrollCarrito, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(scrollCarrito);

            var panelCobro = new AbsoluteLayout
            {
                BackgroundColor = Estilos.BG_OSCURO,
                WidthRequest = 250,
                HeightRequest = 600
            };
            AbsoluteLayout.SetLayoutBounds(panelCobro, new Rect(726, 14, 250, 600));
            AbsoluteLayout.SetLayoutFlags(panelCobro, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(panelCobro);

            var lblResumen = new Label
            {
                Text = "RESUMEN",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Color.FromRgb(129, 140, 248)
            };
            AbsoluteLayout.SetLayoutBounds(lblResumen, new Rect(16, 16, 218, 14));
            AbsoluteLayout.SetLayoutFlags(lblResumen, AbsoluteLayoutFlags.None);
            panelCobro.Add(lblResumen);

            containerImagenProducto = new Grid
            {
                WidthRequest = 218,
                HeightRequest = 170,
                BackgroundColor = Color.FromRgb(30, 30, 32)
            };
            imgProducto = new Image { Aspect = Aspect.AspectFit };
            lblPlaceholderImagen = new Label
            {
                Text = "NO IMAGEN",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.FromRgb(113, 113, 122)
            };
            containerImagenProducto.Add(lblPlaceholderImagen);
            containerImagenProducto.Add(imgProducto);
            AbsoluteLayout.SetLayoutBounds(containerImagenProducto, new Rect(16, 340, 218, 170));
            AbsoluteLayout.SetLayoutFlags(containerImagenProducto, AbsoluteLayoutFlags.None);
            panelCobro.Add(containerImagenProducto);

            lblTotal = new Label
            {
                Text = "$0.00",
                FontSize = 38,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromRgb(165, 180, 252)
            };
            AbsoluteLayout.SetLayoutBounds(lblTotal, new Rect(16, 34, 218, 46));
            AbsoluteLayout.SetLayoutFlags(lblTotal, AbsoluteLayoutFlags.None);
            panelCobro.Add(lblTotal);

            var lblDescuento = new Label
            {
                Text = "DESCUENTO ($ o %)",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Color.FromRgb(113, 113, 122)
            };
            AbsoluteLayout.SetLayoutBounds(lblDescuento, new Rect(16, 90, 218, 14));
            AbsoluteLayout.SetLayoutFlags(lblDescuento, AbsoluteLayoutFlags.None);
            panelCobro.Add(lblDescuento);

            txtDescuento = new Entry
            {
                Text = "0.00",
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                BackgroundColor = Color.FromRgb(39, 39, 42),
                TextColor = Color.FromRgb(52, 211, 153)
            };
            txtDescuento.TextChanged += (s, e) => aplicarDescuento();
            AbsoluteLayout.SetLayoutBounds(txtDescuento, new Rect(16, 108, 218, 36));
            AbsoluteLayout.SetLayoutFlags(txtDescuento, AbsoluteLayoutFlags.None);
            panelCobro.Add(txtDescuento);

            var lblMetodoLbl = new Label
            {
                Text = "METODO DE PAGO",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Color.FromRgb(113, 113, 122)
            };
            AbsoluteLayout.SetLayoutBounds(lblMetodoLbl, new Rect(16, 158, 218, 14));
            AbsoluteLayout.SetLayoutFlags(lblMetodoLbl, AbsoluteLayoutFlags.None);
            panelCobro.Add(lblMetodoLbl);

            cmbMetodo = new Picker
            {
                ItemsSource = new List<string> { "Efectivo", "Tarjeta" },
                SelectedIndex = 0,
                FontSize = Estilos.FUENTE_BOLD_SIZE
            };
            cmbMetodo.SelectedIndexChanged += (s, e) => actualizarVisibilidadEfectivo();
            AbsoluteLayout.SetLayoutBounds(cmbMetodo, new Rect(16, 176, 218, 36));
            AbsoluteLayout.SetLayoutFlags(cmbMetodo, AbsoluteLayoutFlags.None);
            panelCobro.Add(cmbMetodo);

            lblRecibido = new Label
            {
                Text = "IMPORTE RECIBIDO",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Color.FromRgb(113, 113, 122)
            };
            AbsoluteLayout.SetLayoutBounds(lblRecibido, new Rect(16, 224, 218, 14));
            AbsoluteLayout.SetLayoutFlags(lblRecibido, AbsoluteLayoutFlags.None);
            panelCobro.Add(lblRecibido);

            txtRecibido = new Entry
            {
                Text = "0.00",
                FontSize = 22,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromRgb(39, 39, 42),
                TextColor = Colors.White
            };
            txtRecibido.TextChanged += (s, e) => actualizarCambio();
            AbsoluteLayout.SetLayoutBounds(txtRecibido, new Rect(16, 242, 218, 48));
            AbsoluteLayout.SetLayoutFlags(txtRecibido, AbsoluteLayoutFlags.None);
            panelCobro.Add(txtRecibido);

            lblCambio = new Label
            {
                Text = "Cambio: $0.00",
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                TextColor = Color.FromRgb(52, 211, 153)
            };
            AbsoluteLayout.SetLayoutBounds(lblCambio, new Rect(16, 300, 218, 22));
            AbsoluteLayout.SetLayoutFlags(lblCambio, AbsoluteLayoutFlags.None);
            panelCobro.Add(lblCambio);

            var btnCobrar = new Button
            {
                Text = "COBRAR",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Estilos.INDIGO,
                TextColor = Colors.White
            };
            btnCobrar.Clicked += (s, e) => procesarCobro();
            AbsoluteLayout.SetLayoutBounds(btnCobrar, new Rect(16, 536, 218, 52));
            AbsoluteLayout.SetLayoutFlags(btnCobrar, AbsoluteLayoutFlags.None);
            panelCobro.Add(btnCobrar);

            absoluteLayout.Add(sugerenciasScroll);

            Content = absoluteLayout;
        }

        private void actualizarVisibilidadEfectivo()
        {
            bool esEfectivo = "Efectivo".Equals(cmbMetodo.SelectedItem);
            lblRecibido.IsVisible = esEfectivo;
            txtRecibido.IsVisible = esEfectivo;
            lblCambio.IsVisible = esEfectivo;
            if (!esEfectivo)
                txtRecibido.Text = "0.00";
        }

        private void aplicarDescuento()
        {
            try
            {
                string val = txtDescuento.Text?.Trim() ?? "";
                if (val.EndsWith("%"))
                {
                    double porcentaje = double.Parse(val.Replace("%", "").Trim());
                    double subtotal = ctrl.calcularSubtotal();
                    ctrl.setDescuento(subtotal * (porcentaje / 100.0));
                }
                else
                {
                    ctrl.setDescuento(double.Parse(val));
                }
            }
            catch (Exception)
            {
                ctrl.setDescuento(0);
            }
        }

        private void actualizarSugerencias()
        {
            sugerenciasPanel.Children.Clear();
            string q = txtBusqueda.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(q))
            {
                sugerenciasScroll.IsVisible = false;
                return;
            }
            List<Producto> res = ctrl.buscarProductos(q);
            foreach (Producto p in res)
            {
                var btn = new Button
                {
                    Text = string.Format("[{0}]  {1}  ${2:0.00}  stock:{3}", p.getId(), p.getNombre().ToUpper(), p.getPrecio(), p.getStock()),
                    FontSize = Estilos.FUENTE_SMALL_SIZE,
                    BackgroundColor = Estilos.BG_BLANCO,
                    TextColor = Estilos.TEXTO_PRINCIPAL,
                    HorizontalOptions = LayoutOptions.Fill,
                    CornerRadius = 0
                };
                btn.Clicked += (s, e) =>
                {
                    ctrl.agregarAlCarrito(p);
                    mostrarImagen(p.getImagenRuta());
                    txtBusqueda.Text = "";
                    sugerenciasPanel.Children.Clear();
                    sugerenciasScroll.IsVisible = false;
                };
                sugerenciasPanel.Children.Add(btn);
            }
            sugerenciasScroll.IsVisible = res.Count > 0;
        }

        private void actualizarCambio()
        {
            try
            {
                double rec = double.Parse(txtRecibido.Text?.Trim() ?? "0");
                lblCambio.Text = string.Format("Cambio: ${0:0.00}", ctrl.calcularCambio(rec));
            }
            catch (Exception)
            {
            }
        }

        private void procesarCobro()
        {
            if (ctrl.getCarrito().Count == 0)
            {
                if (Application.Current?.MainPage != null)
                {
                    Application.Current.MainPage.DisplayAlert("Aviso", "El carrito esta vacio", "OK");
                }
                return;
            }
            string metodo = cmbMetodo.SelectedItem?.ToString() ?? "Efectivo";
            double recibido = 0;
            if ("Efectivo".Equals(metodo))
            {
                try
                {
                    recibido = double.Parse(txtRecibido.Text?.Trim() ?? "0");
                }
                catch (Exception)
                {
                }
            }
            else
            {
                recibido = ctrl.calcularTotal();
            }
            ctrl.procesarCobro(metodo, recibido);
        }

        public void refrescarCarrito(List<ItemCarrito> carrito, double total, double descuento)
        {
            actualizando = true;
            carritoList.Children.Clear();

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 35,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 80 },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 80 }
                }
            };
            header.Add(new Label { Text = "Producto", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
            header.Add(new Label { Text = "Precio Unit", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 1);
            header.Add(new Label { Text = "Cant", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 2);
            header.Add(new Label { Text = "Subtotal", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 3);
            header.Add(new Label { Text = "Quitar", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 4);
            carritoList.Children.Add(header);

            foreach (ItemCarrito item in carrito)
            {
                var rowGrid = new Grid
                {
                    HeightRequest = 44,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 80 },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 80 }
                    }
                };

                rowGrid.Add(new Label { Text = item.getProducto().getNombre(), VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", item.getProducto().getPrecio()), VerticalOptions = LayoutOptions.Center }, 1);

                var cantEntry = new Entry
                {
                    Text = item.getProducto().esPorPieza() ? string.Format("{0:0}", item.getCantidad()) : string.Format("{0:0.00}", item.getCantidad()),
                    VerticalOptions = LayoutOptions.Center
                };
                string currentId = item.getProducto().getId();
                cantEntry.Completed += (s, e) =>
                {
                    try
                    {
                        double val = double.Parse(cantEntry.Text);
                        if (val <= 0)
                            ctrl.eliminarDelCarrito(currentId);
                        else
                            ctrl.setCantidad(currentId, val);
                    }
                    catch (Exception)
                    {
                    }
                };
                rowGrid.Add(cantEntry, 2);

                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", item.getSubtotal()), VerticalOptions = LayoutOptions.Center }, 3);

                var btnQuitar = new Button
                {
                    Text = "Quitar",
                    FontSize = Estilos.FUENTE_XS_SIZE,
                    TextColor = Estilos.ROSE,
                    BackgroundColor = Estilos.ROSE_CLARO,
                    HeightRequest = 30,
                    Padding = new Thickness(0)
                };
                btnQuitar.Clicked += (s, e) => ctrl.eliminarDelCarrito(currentId);
                rowGrid.Add(btnQuitar, 4);

                carritoList.Children.Add(rowGrid);
            }
            actualizando = false;
            lblTotal.Text = string.Format("${0:0.00}", total);
        }

        public void limpiar()
        {
            actualizando = true;
            carritoList.Children.Clear();
            actualizando = false;
            lblTotal.Text = "$0.00";
            txtRecibido.Text = "0.00";
            txtDescuento.Text = "0.00";
            lblCambio.Text = "Cambio: $0.00";
            txtBusqueda.Text = "";
            sugerenciasScroll.IsVisible = false;
            cmbMetodo.SelectedIndex = 0;
            actualizarVisibilidadEfectivo();
            imgProducto.Source = null;
            lblPlaceholderImagen.Text = "NO IMAGEN";
            lblPlaceholderImagen.IsVisible = true;
        }

        private void mostrarImagen(string ruta)
        {
            var src = Util.RutaBase.getImagenSource(ruta);
            if (src != null)
            {
                imgProducto.Source = src;
                lblPlaceholderImagen.IsVisible = false;
            }
            else
            {
                imgProducto.Source = null;
                lblPlaceholderImagen.Text = "SIN FOTO";
                lblPlaceholderImagen.IsVisible = true;
            }
        }
    }
}
