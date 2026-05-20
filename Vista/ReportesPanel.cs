using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;
using punto_de_venta_C_.Util;

namespace punto_de_venta_C_.Vista
{
    public class ReportesPanel : ContentView
    {
        private readonly ControladorPrincipal app;
        private Label lblTotal, lblNumVentas, lblPromedio;
        private VerticalStackLayout listVentas;
        private Entry txtFechaInicio, txtFechaFin, txtFiltroProducto;
        private List<Venta> ventasFiltradas = new List<Venta>();
        private Venta selectedVenta;
        private Grid selectedRow;

        public ReportesPanel(ControladorPrincipal app)
        {
            this.app = app;
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

            var tit = new Label
            {
                Text = "REPORTES DE VENTAS",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(tit, new Rect(24, 14, 400, 30));
            AbsoluteLayout.SetLayoutFlags(tit, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(tit);

            var lblDesde = new Label
            {
                Text = "DESDE (dd/MM/yyyy):",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            AbsoluteLayout.SetLayoutBounds(lblDesde, new Rect(24, 56, 160, 14));
            AbsoluteLayout.SetLayoutFlags(lblDesde, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblDesde);

            txtFechaInicio = campo(24, 74, 150, 34);
            txtFechaInicio.Text = "01/01/2024";
            absoluteLayout.Add(txtFechaInicio);

            var lblHasta = new Label
            {
                Text = "HASTA (dd/MM/yyyy):",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            AbsoluteLayout.SetLayoutBounds(lblHasta, new Rect(184, 56, 160, 14));
            AbsoluteLayout.SetLayoutFlags(lblHasta, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblHasta);

            txtFechaFin = campo(184, 74, 150, 34);
            txtFechaFin.Text = DateTime.Now.ToString("dd/MM/yyyy");
            absoluteLayout.Add(txtFechaFin);

            var lblProd = new Label
            {
                Text = "FILTRAR POR PRODUCTO (nombre o SKU):",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            AbsoluteLayout.SetLayoutBounds(lblProd, new Rect(344, 56, 280, 14));
            AbsoluteLayout.SetLayoutFlags(lblProd, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblProd);

            txtFiltroProducto = campo(344, 74, 220, 34);
            absoluteLayout.Add(txtFiltroProducto);

            var btnFiltrar = Estilos.botonPrimario("Filtrar");
            btnFiltrar.Clicked += (s, e) => refrescar();
            AbsoluteLayout.SetLayoutBounds(btnFiltrar, new Rect(574, 74, 90, 34));
            AbsoluteLayout.SetLayoutFlags(btnFiltrar, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnFiltrar);

            var btnExcel = Estilos.botonSecundario("Exportar Excel");
            btnExcel.Clicked += (s, e) => exportarExcel();
            AbsoluteLayout.SetLayoutBounds(btnExcel, new Rect(664, 74, 110, 34));
            AbsoluteLayout.SetLayoutFlags(btnExcel, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnExcel);

            var btnGenerarPDF = Estilos.botonSecundario("Generar PDF");
            btnGenerarPDF.Clicked += (s, e) => generarPDF();
            AbsoluteLayout.SetLayoutBounds(btnGenerarPDF, new Rect(784, 74, 110, 34));
            AbsoluteLayout.SetLayoutFlags(btnGenerarPDF, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnGenerarPDF);

            var btnDevolucion = Estilos.botonPeligro("Devolucion");
            btnDevolucion.Clicked += async (s, e) =>
            {
                var dlg = new DevolucionDialog(app);
                await dlg.mostrar();
            };
            AbsoluteLayout.SetLayoutBounds(btnDevolucion, new Rect(904, 74, 100, 34));
            AbsoluteLayout.SetLayoutFlags(btnDevolucion, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnDevolucion);

            var btnVerDevoluciones = Estilos.botonSecundario("Ver Devoluciones");
            btnVerDevoluciones.Clicked += (s, e) => mostrarDevoluciones();
            AbsoluteLayout.SetLayoutBounds(btnVerDevoluciones, new Rect(1014, 74, 140, 34));
            AbsoluteLayout.SetLayoutFlags(btnVerDevoluciones, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnVerDevoluciones);

            var btnReimprimir = Estilos.botonSecundario("Reimprimir Ticket");
            btnReimprimir.Clicked += (s, e) => reimprimirSeleccionado();
            AbsoluteLayout.SetLayoutBounds(btnReimprimir, new Rect(1164, 74, 140, 34));
            AbsoluteLayout.SetLayoutFlags(btnReimprimir, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnReimprimir);

            var btnPorProducto = Estilos.botonSecundario("Ventas por Producto");
            btnPorProducto.Clicked += (s, e) => mostrarVentasPorProducto();
            AbsoluteLayout.SetLayoutBounds(btnPorProducto, new Rect(1314, 74, 160, 34));
            AbsoluteLayout.SetLayoutFlags(btnPorProducto, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnPorProducto);

            lblTotal = kpiCard("Ingresos", "$0.00", Estilos.INDIGO, Colors.White, 24, 124, absoluteLayout);
            lblNumVentas = kpiCard("Ventas", "0", Estilos.BG_BLANCO, Estilos.TEXTO_PRINCIPAL, 204, 124, absoluteLayout);
            lblPromedio = kpiCard("Ticket Prom", "$0.00", Estilos.BG_BLANCO, Estilos.TEXTO_PRINCIPAL, 384, 124, absoluteLayout);

            listVentas = new VerticalStackLayout();
            var scroll = new ScrollView
            {
                Content = listVentas,
                BackgroundColor = Estilos.BG_BLANCO
            };
            AbsoluteLayout.SetLayoutBounds(scroll, new Rect(24, 242, 1200, 440));
            AbsoluteLayout.SetLayoutFlags(scroll, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(scroll);

            Content = absoluteLayout;
            refrescar();
        }

        private Label kpiCard(string label, string valor, Color bg, Color fg, int x, int y, AbsoluteLayout absoluteLayout)
        {
            var card = new AbsoluteLayout
            {
                BackgroundColor = bg,
                WidthRequest = 168,
                HeightRequest = 90
            };
            AbsoluteLayout.SetLayoutBounds(card, new Rect(x, y, 168, 90));
            AbsoluteLayout.SetLayoutFlags(card, AbsoluteLayoutFlags.None);

            var lLbl = new Label
            {
                Text = label.ToUpper(),
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = bg == Estilos.INDIGO ? Color.FromRgb(199, 210, 254) : Estilos.TEXTO_TENUE
            };
            AbsoluteLayout.SetLayoutBounds(lLbl, new Rect(12, 12, 144, 14));
            AbsoluteLayout.SetLayoutFlags(lLbl, AbsoluteLayoutFlags.None);
            card.Add(lLbl);

            var lVal = new Label
            {
                Text = valor,
                FontSize = 28,
                FontAttributes = FontAttributes.Bold,
                TextColor = fg
            };
            AbsoluteLayout.SetLayoutBounds(lVal, new Rect(12, 32, 144, 40));
            AbsoluteLayout.SetLayoutFlags(lVal, AbsoluteLayoutFlags.None);
            card.Add(lVal);

            absoluteLayout.Add(card);
            return lVal;
        }

        public void refrescar()
        {
            string filtroProd = txtFiltroProducto != null ? txtFiltroProducto.Text?.Trim() ?? "" : "";
            List<Venta> todasFecha = filtrarPorFecha(app.getVentas());

            if (!string.IsNullOrEmpty(filtroProd))
            {
                string fp = filtroProd.ToLower();
                ventasFiltradas = todasFecha.Where(v => v.getItems().Any(item =>
                    item.getProducto().getNombre().ToLower().Contains(fp) ||
                    item.getProducto().getId().ToLower().Contains(fp)))
                    .ToList();
            }
            else
            {
                ventasFiltradas = todasFecha;
            }

            double total = ventasFiltradas.Sum(v => v.getTotal());
            double prom = ventasFiltradas.Count == 0 ? 0 : total / ventasFiltradas.Count;
            lblTotal.Text = string.Format("${0:0.00}", total);
            lblNumVentas.Text = ventasFiltradas.Count.ToString();
            lblPromedio.Text = string.Format("${0:0.00}", prom);

            if (listVentas == null)
                return;

            listVentas.Children.Clear();

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 35,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 50 },
                    new ColumnDefinition { Width = 120 },
                    new ColumnDefinition { Width = 80 },
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = 90 },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };
            header.Add(new Label { Text = "#", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
            header.Add(new Label { Text = "Fecha Venta", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 1);
            header.Add(new Label { Text = "Hora", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 2);
            header.Add(new Label { Text = "Cajero", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 3);
            header.Add(new Label { Text = "Metodo", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 4);
            header.Add(new Label { Text = "Total", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 5);
            header.Add(new Label { Text = "Productos", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 6);
            listVentas.Children.Add(header);

            foreach (Venta v in ventasFiltradas)
            {
                var rowGrid = new Grid
                {
                    HeightRequest = 38,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = 50 },
                        new ColumnDefinition { Width = 120 },
                        new ColumnDefinition { Width = 80 },
                        new ColumnDefinition { Width = 150 },
                        new ColumnDefinition { Width = 90 },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = GridLength.Star }
                    }
                };

                rowGrid.Add(new Label { Text = v.getId().ToString(), VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
                rowGrid.Add(new Label { Text = v.getFecha().ToString("dd/MM/yyyy"), VerticalOptions = LayoutOptions.Center }, 1);
                rowGrid.Add(new Label { Text = v.getFecha().ToString("HH:mm:ss"), VerticalOptions = LayoutOptions.Center }, 2);
                rowGrid.Add(new Label { Text = v.getCajero(), VerticalOptions = LayoutOptions.Center }, 3);
                rowGrid.Add(new Label { Text = v.getMetodoPago(), VerticalOptions = LayoutOptions.Center }, 4);
                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", v.getTotal()), VerticalOptions = LayoutOptions.Center }, 5);

                string productos = string.Join(", ", v.getItems().Select(i => i.getProducto().getNombre() + " x" + (int)i.getCantidad()));
                rowGrid.Add(new Label { Text = productos, VerticalOptions = LayoutOptions.Center }, 6);

                var tap = new TapGestureRecognizer();
                Venta currentVenta = v;
                Grid currentRow = rowGrid;
                tap.Tapped += (s, e) =>
                {
                    if (selectedRow != null)
                        selectedRow.BackgroundColor = Colors.Transparent;
                    selectedVenta = currentVenta;
                    selectedRow = currentRow;
                    selectedRow.BackgroundColor = Estilos.BG_ZINC_200;
                };
                rowGrid.GestureRecognizers.Add(tap);

                listVentas.Children.Add(rowGrid);
            }
        }

        private async void mostrarDevoluciones()
        {
            List<Devolucion> devoluciones = app.getDevoluciones();

            var dlgPage = new ContentPage
            {
                Title = "Historial de Devoluciones",
                BackgroundColor = Estilos.BG_BLANCO
            };

            var layout = new VerticalStackLayout { Padding = 20, Spacing = 10 };

            var tit = new Label
            {
                Text = "HISTORIAL DE DEVOLUCIONES",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            layout.Add(tit);

            var grid = new VerticalStackLayout();
            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 30,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 40 },
                    new ColumnDefinition { Width = 60 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 70 },
                    new ColumnDefinition { Width = 80 },
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = 110 },
                    new ColumnDefinition { Width = 100 }
                }
            };
            header.Add(new Label { Text = "#" }, 0);
            header.Add(new Label { Text = "Venta #" }, 1);
            header.Add(new Label { Text = "Producto" }, 2);
            header.Add(new Label { Text = "Cantidad" }, 3);
            header.Add(new Label { Text = "Monto Dev" }, 4);
            header.Add(new Label { Text = "Motivo" }, 5);
            header.Add(new Label { Text = "Fecha" }, 6);
            header.Add(new Label { Text = "Cajero" }, 7);
            grid.Children.Add(header);

            foreach (Devolucion d in devoluciones)
            {
                var rowGrid = new Grid
                {
                    HeightRequest = 36,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = 40 },
                        new ColumnDefinition { Width = 60 },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 70 },
                        new ColumnDefinition { Width = 80 },
                        new ColumnDefinition { Width = 150 },
                        new ColumnDefinition { Width = 110 },
                        new ColumnDefinition { Width = 100 }
                    }
                };
                rowGrid.Add(new Label { Text = d.getId().ToString() }, 0);
                rowGrid.Add(new Label { Text = d.getVentaId().ToString() }, 1);
                rowGrid.Add(new Label { Text = d.getProducto().getNombre() }, 2);
                rowGrid.Add(new Label { Text = string.Format("{0:0.00}", d.getCantidad()) }, 3);
                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", d.getMontoDevuelto()) }, 4);
                rowGrid.Add(new Label { Text = d.getMotivo() }, 5);
                rowGrid.Add(new Label { Text = d.getFecha().ToString("dd/MM/yyyy HH:mm") }, 6);
                rowGrid.Add(new Label { Text = d.getCajero() }, 7);
                grid.Children.Add(rowGrid);
            }

            if (devoluciones.Count == 0)
            {
                grid.Children.Add(new Label { Text = "Sin devoluciones registradas", HorizontalOptions = LayoutOptions.Center });
            }

            var scroll = new ScrollView { Content = grid, HeightRequest = 360 };
            layout.Add(scroll);

            var btnCerrar = Estilos.botonPrimario("Cerrar");
            btnCerrar.Clicked += async (s, e) => await Application.Current.MainPage.Navigation.PopModalAsync();
            layout.Add(btnCerrar);

            dlgPage.Content = layout;

            if (Application.Current?.MainPage != null)
                await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(dlgPage));
        }

        private async void reimprimirSeleccionado()
        {
            if (selectedVenta == null)
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Seleccion requerida", "Selecciona una venta de la tabla para reimprimir su ticket.", "OK");
                return;
            }
            if (selectedVenta.getItems() == null || selectedVenta.getItems().Count == 0)
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Sin detalle", "Esta venta no tiene detalle de productos disponible para reimprimir.", "OK");
                return;
            }

            var dlg = new TicketDialog(selectedVenta, app.getConfig(), 0, selectedVenta.getTotal());
            ManejoErrores.registrarInfo("Reimpresion de ticket #" + selectedVenta.getId());
            await dlg.mostrar();
        }

        private List<Venta> filtrarPorFecha(List<Venta> todas)
        {
            try
            {
                DateTime inicio = DateTime.ParseExact(txtFechaInicio.Text?.Trim() ?? "", "dd/MM/yyyy", null);
                DateTime fin = DateTime.ParseExact(txtFechaFin.Text?.Trim() ?? "", "dd/MM/yyyy", null);
                DateTime finDia = new DateTime(fin.Year, fin.Month, fin.Day, 23, 59, 59);
                return todas.Where(v => v.getFecha() >= inicio && v.getFecha() <= finDia).ToList();
            }
            catch (Exception)
            {
                return todas;
            }
        }

        private async void exportarExcel()
        {
            if (Application.Current?.MainPage == null) return;

            try
            {
                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fileName = "ReporteVentas_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
                string fullPath = Path.Combine(basePath, fileName);

                using (var sw = new StreamWriter(fullPath, false, System.Text.Encoding.UTF8))
                {
                    sw.Write('\uFEFF');

                    string[] headers = new string[] {
                        "ID Venta", "Fecha Venta", "Hora", "Cajero",
                        "Metodo Pago", "Total Venta", "SKU", "Producto",
                        "Cantidad", "Precio Unitario", "Subtotal"
                    };
                    await sw.WriteLineAsync(string.Join(",", headers.Select(h => "\"" + h.Replace("\"", "\"\"") + "\"")));

                    foreach (Venta v in ventasFiltradas)
                    {
                        string fecha = v.getFecha().ToString("dd/MM/yyyy");
                        string hora = v.getFecha().ToString("HH:mm:ss");

                        if (v.getItems() == null || v.getItems().Count == 0)
                        {
                            string[] row = new string[] {
                                v.getId().ToString(), fecha, hora, v.getCajero(),
                                v.getMetodoPago(), v.getTotal().ToString("0.00"),
                                "", "", "", "", ""
                            };
                            await sw.WriteLineAsync(string.Join(",", row.Select(r => "\"" + r.Replace("\"", "\"\"") + "\"")));
                        }
                        else
                        {
                            foreach (ItemCarrito item in v.getItems())
                            {
                                string[] row = new string[] {
                                    v.getId().ToString(), fecha, hora, v.getCajero(),
                                    v.getMetodoPago(), v.getTotal().ToString("0.00"),
                                    item.getProducto().getId(), item.getProducto().getNombre(),
                                    item.getCantidad().ToString("0.00"),
                                    item.getProducto().getPrecio().ToString("0.00"),
                                    item.getSubtotal().ToString("0.00")
                                };
                                await sw.WriteLineAsync(string.Join(",", row.Select(r => "\"" + r.Replace("\"", "\"\"") + "\"")));
                            }
                        }
                    }
                }

                await Application.Current.MainPage.DisplayAlert("Exportacion exitosa", "Reporte guardado en:\n" + fullPath, "OK");
                ManejoErrores.registrarInfo("Excel exportado: " + fullPath + " (" + ventasFiltradas.Count + " ventas)");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error al exportar", "No se pudo guardar el reporte.\n" + ex.Message, "OK");
            }
        }

        private async void generarPDF()
        {
            try
            {
                string rutaPdf = CreadorDocumentos.crearDocumentoVentas(ventasFiltradas, app.getConfig(), txtFechaInicio.Text ?? "", txtFechaFin.Text ?? "");
                await Microsoft.Maui.ApplicationModel.Launcher.Default.OpenAsync(new Microsoft.Maui.ApplicationModel.OpenFileRequest("Reporte PDF", new Microsoft.Maui.Storage.ReadOnlyFile(rutaPdf)));
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo generar el reporte PDF.\n" + ex.Message, "OK");
            }
        }

        private async void mostrarVentasPorProducto()
        {
            var resumen = new Dictionary<string, double[]>();
            foreach (Venta v in ventasFiltradas)
            {
                if (v.getItems() == null) continue;
                foreach (ItemCarrito item in v.getItems())
                {
                    string clave = "[" + item.getProducto().getId() + "] " + item.getProducto().getNombre();
                    if (resumen.ContainsKey(clave))
                    {
                        resumen[clave][0] += item.getCantidad();
                        resumen[clave][1] += item.getSubtotal();
                    }
                    else
                    {
                        resumen[clave] = new double[] { item.getCantidad(), item.getSubtotal() };
                    }
                }
            }

            var lista = resumen.ToList();
            lista.Sort((a, b) => b.Value[1].CompareTo(a.Value[1]));

            var dlgPage = new ContentPage
            {
                Title = "Reporte de Ventas por Producto",
                BackgroundColor = Estilos.BG_BLANCO
            };

            var layout = new VerticalStackLayout { Padding = 20, Spacing = 10 };

            var tit = new Label
            {
                Text = "RANKING DE PRODUCTOS MAS VENDIDOS",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            layout.Add(tit);

            var sub = new Label
            {
                Text = "Periodo: " + txtFechaInicio.Text + " a " + txtFechaFin.Text + "  |  " + lista.Count + " productos distintos",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            layout.Add(sub);

            var grid = new VerticalStackLayout();
            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 30,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 40 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 140 },
                    new ColumnDefinition { Width = 140 }
                }
            };
            header.Add(new Label { Text = "#", FontAttributes = FontAttributes.Bold }, 0);
            header.Add(new Label { Text = "Producto", FontAttributes = FontAttributes.Bold }, 1);
            header.Add(new Label { Text = "Unidades", FontAttributes = FontAttributes.Bold }, 2);
            header.Add(new Label { Text = "Ingresos", FontAttributes = FontAttributes.Bold }, 3);
            grid.Children.Add(header);

            int rank = 1;
            foreach (var e in lista)
            {
                double[] vals = e.Value;
                var rowGrid = new Grid
                {
                    HeightRequest = 36,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = 40 },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 140 },
                        new ColumnDefinition { Width = 140 }
                    }
                };
                rowGrid.Add(new Label { Text = rank++.ToString() }, 0);
                rowGrid.Add(new Label { Text = e.Key }, 1);
                rowGrid.Add(new Label { Text = string.Format("{0:0}", vals[0]) }, 2);
                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", vals[1]) }, 3);
                grid.Children.Add(rowGrid);
            }

            if (lista.Count == 0)
            {
                grid.Children.Add(new Label { Text = "Sin ventas en el periodo seleccionado", HorizontalOptions = LayoutOptions.Center });
            }

            var scroll = new ScrollView { Content = grid, HeightRequest = 380 };
            layout.Add(scroll);

            var btnCerrar = Estilos.botonPrimario("Cerrar");
            btnCerrar.Clicked += async (s, e) => await Application.Current.MainPage.Navigation.PopModalAsync();
            layout.Add(btnCerrar);

            dlgPage.Content = layout;

            if (Application.Current?.MainPage != null)
                await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(dlgPage));
        }

        private Entry campo(int x, int y, int w, int h)
        {
            var tf = new Entry
            {
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                BackgroundColor = Estilos.BG_ZINC_100,
                TextColor = Estilos.TEXTO_PRINCIPAL,
                WidthRequest = w,
                HeightRequest = h
            };
            AbsoluteLayout.SetLayoutBounds(tf, new Rect(x, y, w, h));
            AbsoluteLayout.SetLayoutFlags(tf, AbsoluteLayoutFlags.None);
            return tf;
        }
    }
}
