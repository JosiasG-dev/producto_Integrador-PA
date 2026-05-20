using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;
using punto_de_venta_C_.Util;

namespace punto_de_venta_C_.Vista
{
    public class InventarioPanel : ContentView
    {
        private readonly InventarioControlador ctrl;
        private readonly Usuario usuarioActivo;

        private Entry txtBusqueda;
        private Picker cmbCategoria;
        private VerticalStackLayout tablaLayout;

        public InventarioPanel(InventarioControlador ctrl, Usuario usuario)
        {
            this.ctrl = ctrl;
            this.usuarioActivo = usuario;
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

            var titulo = new Label
            {
                Text = "CATALOGO — Administracion de Stock",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(titulo, new Rect(24, 16, 600, 30));
            AbsoluteLayout.SetLayoutFlags(titulo, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(titulo);

            double currentX = 450;
            if (usuarioActivo.esAdmin())
            {
                var btnDevolver = Estilos.botonPeligro("Devolver");
                btnDevolver.Clicked += async (s, e) =>
                {
                    var selected = GetSelectedProduct();
                    if (selected == null)
                    {
                        if (Application.Current?.MainPage != null)
                            await Application.Current.MainPage.DisplayAlert("Aviso", "Seleccione un producto de la tabla", "OK");
                        return;
                    }
                    var dlg = new DevolucionProveedorDialog(selected, ctrl);
                    await dlg.mostrar();
                };
                AbsoluteLayout.SetLayoutBounds(btnDevolver, new Rect(currentX, 14, 90, 36));
                AbsoluteLayout.SetLayoutFlags(btnDevolver, AbsoluteLayoutFlags.None);
                absoluteLayout.Add(btnDevolver);
                currentX += 100;

                var btnEntrada = Estilos.botonSecundario("Entrada");
                btnEntrada.Clicked += async (s, e) =>
                {
                    var selected = GetSelectedProduct();
                    if (selected == null)
                    {
                        if (Application.Current?.MainPage != null)
                            await Application.Current.MainPage.DisplayAlert("Aviso", "Seleccione un producto de la tabla", "OK");
                        return;
                    }
                    var dlg = new EntradaRapidaDialog(selected, ctrl);
                    await dlg.mostrar();
                };
                AbsoluteLayout.SetLayoutBounds(btnEntrada, new Rect(currentX, 14, 90, 36));
                AbsoluteLayout.SetLayoutFlags(btnEntrada, AbsoluteLayoutFlags.None);
                absoluteLayout.Add(btnEntrada);
                currentX += 100;
            }

            var btnBajoStock = Estilos.botonSecundario("Bajo Stock");
            btnBajoStock.Clicked += (s, e) => mostrarBajoStock();
            AbsoluteLayout.SetLayoutBounds(btnBajoStock, new Rect(currentX, 14, 130, 36));
            AbsoluteLayout.SetLayoutFlags(btnBajoStock, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnBajoStock);
            currentX += 140;

            if (usuarioActivo.esAdmin())
            {
                var btnNuevo = Estilos.botonPrimario("+ Nuevo Item");
                btnNuevo.Clicked += (s, e) => abrirFormulario(null);
                AbsoluteLayout.SetLayoutBounds(btnNuevo, new Rect(currentX, 14, 130, 36));
                AbsoluteLayout.SetLayoutFlags(btnNuevo, AbsoluteLayoutFlags.None);
                absoluteLayout.Add(btnNuevo);
            }

            var lblBusq = new Label
            {
                Text = "BUSCAR:",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            AbsoluteLayout.SetLayoutBounds(lblBusq, new Rect(24, 62, 60, 14));
            AbsoluteLayout.SetLayoutFlags(lblBusq, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblBusq);

            txtBusqueda = new Entry
            {
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                BackgroundColor = Estilos.BG_BLANCO,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            txtBusqueda.TextChanged += (s, e) => refrescar();
            AbsoluteLayout.SetLayoutBounds(txtBusqueda, new Rect(80, 56, 280, 34));
            AbsoluteLayout.SetLayoutFlags(txtBusqueda, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtBusqueda);

            var lblCat = new Label
            {
                Text = "CATEGORIA:",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            AbsoluteLayout.SetLayoutBounds(lblCat, new Rect(376, 62, 80, 14));
            AbsoluteLayout.SetLayoutFlags(lblCat, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblCat);

            var cats = DatosIniciales.getCategorias().ToList();
            cats.Insert(0, "Todas");
            cmbCategoria = new Picker
            {
                ItemsSource = cats,
                SelectedIndex = 0,
                FontSize = Estilos.FUENTE_BOLD_SIZE
            };
            cmbCategoria.SelectedIndexChanged += (s, e) => refrescar();
            AbsoluteLayout.SetLayoutBounds(cmbCategoria, new Rect(454, 56, 200, 34));
            AbsoluteLayout.SetLayoutFlags(cmbCategoria, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(cmbCategoria);

            tablaLayout = new VerticalStackLayout();
            var scroll = new ScrollView
            {
                Content = tablaLayout,
                BackgroundColor = Estilos.BG_BLANCO
            };
            AbsoluteLayout.SetLayoutBounds(scroll, new Rect(24, 104, 904, 560));
            AbsoluteLayout.SetLayoutFlags(scroll, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(scroll);

            Content = absoluteLayout;
            refrescar();
        }

        private Producto selectedProduct;
        private Grid selectedRow;

        private Producto GetSelectedProduct()
        {
            return selectedProduct;
        }

        public void refrescar()
        {
            string texto = txtBusqueda?.Text ?? "";
            string cat = (cmbCategoria != null && cmbCategoria.SelectedIndex > 0)
                ? cmbCategoria.SelectedItem.ToString()
                : "";

            List<Producto> lista = ctrl.filtrar(texto, cat);
            tablaLayout.Children.Clear();

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 35,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 70 },
                    new ColumnDefinition { Width = 60 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 90 },
                    new ColumnDefinition { Width = 80 },
                    new ColumnDefinition { Width = 80 }
                }
            };
            header.Add(new Label { Text = "SKU", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 0);
            header.Add(new Label { Text = "Foto", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 1);
            header.Add(new Label { Text = "Producto", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 2);
            header.Add(new Label { Text = "Categoria", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 3);
            header.Add(new Label { Text = "Precio", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 4);
            header.Add(new Label { Text = "Existencia", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 5);
            header.Add(new Label { Text = "Stock Min", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 6);
            header.Add(new Label { Text = "Estado", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 7);
            tablaLayout.Children.Add(header);

            foreach (Producto p in lista)
            {
                var rowGrid = new Grid
                {
                    HeightRequest = 55,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = 70 },
                        new ColumnDefinition { Width = 60 },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 150 },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 90 },
                        new ColumnDefinition { Width = 80 },
                        new ColumnDefinition { Width = 80 }
                    }
                };

                rowGrid.Add(new Label { Text = p.getId(), VerticalOptions = LayoutOptions.Center }, 0);

                var img = new Image { HeightRequest = 45, WidthRequest = 45, Aspect = Aspect.AspectFit };
                var src = RutaBase.getImagenSource(p.getImagenRuta());
                if (src != null)
                {
                    img.Source = src;
                }
                rowGrid.Add(img, 1);

                rowGrid.Add(new Label { Text = p.getNombre().ToUpper(), VerticalOptions = LayoutOptions.Center }, 2);
                rowGrid.Add(new Label { Text = p.getCategoria(), VerticalOptions = LayoutOptions.Center }, 3);
                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", p.getPrecio()), VerticalOptions = LayoutOptions.Center }, 4);
                rowGrid.Add(new Label { Text = string.Format("{0:0.1}", p.getStock()), VerticalOptions = LayoutOptions.Center }, 5);
                rowGrid.Add(new Label { Text = string.Format("{0:0}", p.getStockMinimo()), VerticalOptions = LayoutOptions.Center }, 6);

                var estadoLbl = new Label
                {
                    Text = p.stockBajo() ? "BAJO" : "OK",
                    TextColor = p.stockBajo() ? Estilos.ROSE : Estilos.EMERALD,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center
                };
                rowGrid.Add(estadoLbl, 7);

                var tap = new TapGestureRecognizer();
                Producto currentProduct = p;
                Grid currentRow = rowGrid;
                tap.Tapped += (s, e) =>
                {
                    if (selectedRow != null)
                        selectedRow.BackgroundColor = Colors.Transparent;
                    selectedProduct = currentProduct;
                    selectedRow = currentRow;
                    selectedRow.BackgroundColor = Estilos.BG_ZINC_200;
                };
                var doubleTap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
                doubleTap.Tapped += (s, e) =>
                {
                    if (usuarioActivo.esAdmin())
                        abrirFormulario(currentProduct);
                };
                rowGrid.GestureRecognizers.Add(tap);
                rowGrid.GestureRecognizers.Add(doubleTap);

                tablaLayout.Children.Add(rowGrid);
            }
        }

        private async void mostrarBajoStock()
        {
            List<Producto> todos = ctrl.filtrar("", "");
            List<Producto> bajos = todos.Where(p => p.stockBajo())
                .OrderBy(p => p.getStock() - p.getStockMinimo())
                .ToList();

            var dlgPage = new ContentPage
            {
                Title = "Reporte de Stock Bajo",
                BackgroundColor = Estilos.BG_BLANCO
            };

            var layout = new VerticalStackLayout { Padding = 20, Spacing = 10 };

            var tit = new Label
            {
                Text = "PRODUCTOS CON STOCK BAJO",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            layout.Add(tit);

            var sub = new Label
            {
                Text = bajos.Count == 0
                    ? "Todo en orden — ningun producto por debajo del minimo."
                    : bajos.Count + " producto(s) requieren reposicion.",
                FontSize = Estilos.FUENTE_NORMAL_SIZE,
                TextColor = bajos.Count == 0 ? Estilos.EMERALD : Estilos.ROSE
            };
            layout.Add(sub);

            var grid = new VerticalStackLayout();
            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 30,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 60 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 120 },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 80 }
                }
            };
            header.Add(new Label { Text = "SKU", FontAttributes = FontAttributes.Bold }, 0);
            header.Add(new Label { Text = "Nombre", FontAttributes = FontAttributes.Bold }, 1);
            header.Add(new Label { Text = "Categoria", FontAttributes = FontAttributes.Bold }, 2);
            header.Add(new Label { Text = "Stock", FontAttributes = FontAttributes.Bold }, 3);
            header.Add(new Label { Text = "Stock Min", FontAttributes = FontAttributes.Bold }, 4);
            header.Add(new Label { Text = "Faltante", FontAttributes = FontAttributes.Bold }, 5);
            grid.Children.Add(header);

            foreach (Producto p in bajos)
            {
                double faltante = Math.Max(0, p.getStockMinimo() - p.getStock());
                var rowGrid = new Grid
                {
                    HeightRequest = 36,
                    BackgroundColor = p.getStock() <= 0 ? Color.FromRgb(255, 230, 230) : Colors.Transparent,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = 60 },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 120 },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 80 }
                    }
                };
                rowGrid.Add(new Label { Text = p.getId() }, 0);
                rowGrid.Add(new Label { Text = p.getNombre() }, 1);
                rowGrid.Add(new Label { Text = p.getCategoria() }, 2);
                rowGrid.Add(new Label { Text = string.Format("{0:0.1}", p.getStock()) }, 3);
                rowGrid.Add(new Label { Text = string.Format("{0:0}", p.getStockMinimo()) }, 4);
                rowGrid.Add(new Label { Text = string.Format("{0:0.1}", faltante) }, 5);
                grid.Children.Add(rowGrid);
            }

            if (bajos.Count == 0)
            {
                grid.Children.Add(new Label { Text = "Todos los productos tienen stock suficiente", HorizontalOptions = LayoutOptions.Center });
            }

            var scroll = new ScrollView { Content = grid, HeightRequest = 300 };
            layout.Add(scroll);

            var btnCerrar = Estilos.botonPrimario("Cerrar");
            btnCerrar.Clicked += async (s, e) => await Application.Current.MainPage.Navigation.PopModalAsync();
            layout.Add(btnCerrar);

            dlgPage.Content = layout;

            ManejoErrores.registrarInfo("Reporte bajo stock: " + bajos.Count + " productos");

            if (Application.Current?.MainPage != null)
                await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(dlgPage));
        }

        private async void abrirFormulario(Producto producto)
        {
            var dlg = new ProductoDialog(producto, ctrl);
            await dlg.mostrar();
            refrescar();
        }
    }
}
