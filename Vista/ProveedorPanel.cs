using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;

namespace punto_de_venta_C_.Vista
{
    public class ProveedorPanel : ContentView
    {
        private readonly ProveedorControlador ctrl;
        private readonly Usuario usuario;

        private Grid contentGrid;

        private Button btnTabDirectorio;
        private Button btnTabOrdenes;
        private Button btnTabCuentas;

        private View viewDirectorio;
        private View viewOrdenes;
        private View viewCuentas;

        private VerticalStackLayout listProveedores;
        private VerticalStackLayout listOrdenes;
        private VerticalStackLayout listCuentas;

        public ProveedorPanel(ProveedorControlador ctrl, Usuario usuario)
        {
            this.ctrl = ctrl;
            this.usuario = usuario;
            ctrl.setPanel(this);
            BackgroundColor = Estilos.BG_CLARO;
            Padding = new Thickness(24);
            construir();
        }

        private void construir()
        {
            var rootLayout = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = 16 },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = 8 },
                    new RowDefinition { Height = GridLength.Star }
                }
            };

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_BLANCO,
                Padding = new Thickness(24, 20),
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };
            var tit = new Label
            {
                Text = "ABASTECIMIENTO  —  Proveedores y Cuentas por Pagar",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };
            header.Add(tit, 0, 0);
            rootLayout.Add(header, 0, 0);

            var tabsGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 120 },
                    new ColumnDefinition { Width = 8 },
                    new ColumnDefinition { Width = 120 },
                    new ColumnDefinition { Width = 8 },
                    new ColumnDefinition { Width = 120 }
                }
            };

            btnTabDirectorio = Estilos.botonSecundario("Directorio");
            btnTabDirectorio.Clicked += (s, e) => SwitchTab(0);
            tabsGrid.Add(btnTabDirectorio, 0, 0);

            btnTabOrdenes = Estilos.botonSecundario("Ordenes");
            btnTabOrdenes.Clicked += (s, e) => SwitchTab(1);
            tabsGrid.Add(btnTabOrdenes, 2, 0);

            btnTabCuentas = Estilos.botonSecundario("Deudas");
            btnTabCuentas.Clicked += (s, e) => SwitchTab(2);
            tabsGrid.Add(btnTabCuentas, 4, 0);

            rootLayout.Add(tabsGrid, 0, 2);

            contentGrid = new Grid();

            viewDirectorio = construirTabProveedores();
            viewOrdenes = construirTabOrdenes();
            viewCuentas = construirTabCuentas();

            contentGrid.Add(viewDirectorio);
            contentGrid.Add(viewOrdenes);
            contentGrid.Add(viewCuentas);

            rootLayout.Add(contentGrid, 0, 4);

            Content = rootLayout;

            SwitchTab(0);
            refrescarProveedores();
            refrescarOrdenes();
            refrescarCuentas();
        }

        private void SwitchTab(int index)
        {
            viewDirectorio.IsVisible = index == 0;
            viewOrdenes.IsVisible = index == 1;
            viewCuentas.IsVisible = index == 2;

            btnTabDirectorio.BackgroundColor = index == 0 ? Estilos.INDIGO : Estilos.BG_BLANCO;
            btnTabDirectorio.TextColor = index == 0 ? Colors.White : Estilos.TEXTO_PRINCIPAL;

            btnTabOrdenes.BackgroundColor = index == 1 ? Estilos.INDIGO : Estilos.BG_BLANCO;
            btnTabOrdenes.TextColor = index == 1 ? Colors.White : Estilos.TEXTO_PRINCIPAL;

            btnTabCuentas.BackgroundColor = index == 2 ? Estilos.INDIGO : Estilos.BG_BLANCO;
            btnTabCuentas.TextColor = index == 2 ? Colors.White : Estilos.TEXTO_PRINCIPAL;
        }

        private View construirTabProveedores()
        {
            var p = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                },
                BackgroundColor = Estilos.BG_BLANCO,
                Padding = new Thickness(12)
            };

            var toolbar = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 160 }
                }
            };
            var btnNuevo = Estilos.botonPrimario("+ Alta Proveedor");
            btnNuevo.HeightRequest = 40;
            btnNuevo.Clicked += (s, e) => abrirFormularioProveedor(null);
            toolbar.Add(btnNuevo, 1, 0);
            p.Add(toolbar, 0, 0);

            listProveedores = new VerticalStackLayout();
            var scroll = new ScrollView { Content = listProveedores };
            p.Add(scroll, 0, 1);

            return p;
        }

        private View construirTabOrdenes()
        {
            var p = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                },
                BackgroundColor = Estilos.BG_BLANCO,
                Padding = new Thickness(12)
            };

            var toolbar = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 190 },
                    new ColumnDefinition { Width = 8 },
                    new ColumnDefinition { Width = 170 }
                }
            };
            var btnBitacora = Estilos.botonSecundario("Bitacora de Entradas");
            btnBitacora.HeightRequest = 40;
            btnBitacora.Clicked += (s, e) => mostrarBitacoraEntradas();
            toolbar.Add(btnBitacora, 1, 0);

            var btnNueva = Estilos.botonExito("+ Generar Pedido");
            btnNueva.HeightRequest = 40;
            btnNueva.Clicked += (s, e) => abrirNuevaOrden();
            toolbar.Add(btnNueva, 3, 0);

            p.Add(toolbar, 0, 0);

            listOrdenes = new VerticalStackLayout();
            var scroll = new ScrollView { Content = listOrdenes };
            p.Add(scroll, 0, 1);

            return p;
        }

        private View construirTabCuentas()
        {
            var p = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                },
                BackgroundColor = Estilos.BG_BLANCO,
                Padding = new Thickness(12)
            };

            var subtit = new Label
            {
                Text = "  Saldos pendientes con acreedores",
                FontSize = Estilos.FUENTE_SMALL_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            p.Add(subtit, 0, 0);

            listCuentas = new VerticalStackLayout();
            var scroll = new ScrollView { Content = listCuentas };
            p.Add(scroll, 0, 1);

            return p;
        }

        public void refrescarProveedores()
        {
            if (listProveedores == null)
                return;

            listProveedores.Children.Clear();

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 35,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 60 },
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };
            header.Add(new Label { Text = "ID", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
            header.Add(new Label { Text = "Empresa", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 1);
            header.Add(new Label { Text = "Contacto", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 2);
            header.Add(new Label { Text = "Telefono", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 3);
            header.Add(new Label { Text = "Email", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 4);
            header.Add(new Label { Text = "Direccion", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 5);
            listProveedores.Children.Add(header);

            foreach (Proveedor p in ctrl.getProveedores())
            {
                var rowGrid = new Grid
                {
                    HeightRequest = 40,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = 60 },
                        new ColumnDefinition { Width = 150 },
                        new ColumnDefinition { Width = 150 },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 150 },
                        new ColumnDefinition { Width = GridLength.Star }
                    }
                };

                rowGrid.Add(new Label { Text = p.getId().ToString(), VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
                rowGrid.Add(new Label { Text = p.getNombre(), VerticalOptions = LayoutOptions.Center }, 1);
                rowGrid.Add(new Label { Text = p.getContacto(), VerticalOptions = LayoutOptions.Center }, 2);
                rowGrid.Add(new Label { Text = p.getTelefono(), VerticalOptions = LayoutOptions.Center }, 3);
                rowGrid.Add(new Label { Text = p.getEmail(), VerticalOptions = LayoutOptions.Center }, 4);
                rowGrid.Add(new Label { Text = p.getDireccion(), VerticalOptions = LayoutOptions.Center }, 5);

                var tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
                Proveedor currentProv = p;
                tap.Tapped += (s, e) => abrirFormularioProveedor(currentProv);
                rowGrid.GestureRecognizers.Add(tap);

                listProveedores.Children.Add(rowGrid);
            }
        }

        private async void mostrarBitacoraEntradas()
        {
            List<OrdenCompra> ordenes = ctrl.getOrdenes()
                .Where(o => o.getEstado() == OrdenCompra.Estado.RECIBIDO)
                .ToList();

            var dlgPage = new ContentPage
            {
                Title = "Bitacora de Entradas",
                BackgroundColor = Estilos.BG_BLANCO
            };

            var layout = new VerticalStackLayout { Padding = 20, Spacing = 10 };

            var tit = new Label
            {
                Text = "BITACORA DE ENTRADAS DE MERCANCIA",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            layout.Add(tit);

            var sub = new Label
            {
                Text = ordenes.Count + " recepciones registradas",
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
                    new ColumnDefinition { Width = 60 },
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = 140 },
                    new ColumnDefinition { Width = 90 },
                    new ColumnDefinition { Width = 90 },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };
            header.Add(new Label { Text = "Folio", FontAttributes = FontAttributes.Bold }, 0);
            header.Add(new Label { Text = "Proveedor", FontAttributes = FontAttributes.Bold }, 1);
            header.Add(new Label { Text = "Fecha Recepcion", FontAttributes = FontAttributes.Bold }, 2);
            header.Add(new Label { Text = "Tipo Pago", FontAttributes = FontAttributes.Bold }, 3);
            header.Add(new Label { Text = "Total", FontAttributes = FontAttributes.Bold }, 4);
            header.Add(new Label { Text = "Productos Recibidos", FontAttributes = FontAttributes.Bold }, 5);
            grid.Children.Add(header);

            foreach (OrdenCompra o in ordenes)
            {
                string prods = string.Join(", ", o.getItems().Select(i => i.getProducto().getNombre() + " x" + (int)i.getCantidadSolicitada()));
                var rowGrid = new Grid
                {
                    HeightRequest = 38,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = 60 },
                        new ColumnDefinition { Width = 150 },
                        new ColumnDefinition { Width = 140 },
                        new ColumnDefinition { Width = 90 },
                        new ColumnDefinition { Width = 90 },
                        new ColumnDefinition { Width = GridLength.Star }
                    }
                };
                rowGrid.Add(new Label { Text = "#" + o.getId() }, 0);
                rowGrid.Add(new Label { Text = o.getProveedor().getNombre() }, 1);
                rowGrid.Add(new Label { Text = o.getFecha().ToString("dd/MM/yyyy HH:mm") }, 2);
                rowGrid.Add(new Label { Text = o.getTipoPago().ToString() }, 3);
                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", o.getTotal()) }, 4);
                rowGrid.Add(new Label { Text = prods }, 5);
                grid.Children.Add(rowGrid);
            }

            if (ordenes.Count == 0)
            {
                grid.Children.Add(new Label { Text = "Sin recepciones registradas", HorizontalOptions = LayoutOptions.Center });
            }

            var scroll = new ScrollView { Content = grid, HeightRequest = 350 };
            layout.Add(scroll);

            var btnCerrar = Estilos.botonPrimario("Cerrar");
            btnCerrar.Clicked += async (s, e) => await Application.Current.MainPage.Navigation.PopModalAsync();
            layout.Add(btnCerrar);

            dlgPage.Content = layout;

            if (Application.Current?.MainPage != null)
                await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(dlgPage));
        }

        public void refrescarOrdenes()
        {
            if (listOrdenes == null)
                return;

            listOrdenes.Children.Clear();

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 35,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 110 },
                    new ColumnDefinition { Width = 110 }
                }
            };
            header.Add(new Label { Text = "Folio", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
            header.Add(new Label { Text = "Proveedor", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 1);
            header.Add(new Label { Text = "Total", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 2);
            header.Add(new Label { Text = "Pago", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 3);
            header.Add(new Label { Text = "Estado", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 4);
            header.Add(new Label { Text = "Accion", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 5);
            listOrdenes.Children.Add(header);

            foreach (OrdenCompra o in ctrl.getOrdenes())
            {
                if (o.getEstado() == OrdenCompra.Estado.PENDIENTE)
                {
                    var rowGrid = new Grid
                    {
                        HeightRequest = 42,
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition { Width = 100 },
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = 100 },
                            new ColumnDefinition { Width = 100 },
                            new ColumnDefinition { Width = 110 },
                            new ColumnDefinition { Width = 110 }
                        }
                    };

                    rowGrid.Add(new Label { Text = o.getFolioCorto(), VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
                    rowGrid.Add(new Label { Text = o.getProveedor().getNombre(), VerticalOptions = LayoutOptions.Center }, 1);
                    rowGrid.Add(new Label { Text = string.Format("${0:0.00}", o.getTotal()), VerticalOptions = LayoutOptions.Center }, 2);
                    rowGrid.Add(new Label { Text = o.getTipoPago().ToString(), VerticalOptions = LayoutOptions.Center }, 3);
                    rowGrid.Add(new Label { Text = o.getEstado().ToString(), VerticalOptions = LayoutOptions.Center }, 4);

                    var btnRecibir = new Button
                    {
                        Text = "Recibir",
                        FontSize = Estilos.FUENTE_XS_SIZE,
                        TextColor = Estilos.EMERALD,
                        BackgroundColor = Estilos.EMERALD_CLARO,
                        HeightRequest = 30,
                        Padding = new Thickness(0)
                    };
                    int currentOrderId = o.getId();
                    btnRecibir.Clicked += (s, e) =>
                    {
                        try
                        {
                            ctrl.recibirOrden(currentOrderId);
                            refrescarOrdenes();
                            refrescarCuentas();
                        }
                        catch (Exception)
                        {
                        }
                    };
                    rowGrid.Add(btnRecibir, 5);

                    listOrdenes.Children.Add(rowGrid);
                }
            }
        }

        public void refrescarCuentas()
        {
            if (listCuentas == null)
                return;

            listCuentas.Children.Clear();

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 35,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 120 }
                }
            };
            header.Add(new Label { Text = "Proveedor", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
            header.Add(new Label { Text = "Orden", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 1);
            header.Add(new Label { Text = "Total", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 2);
            header.Add(new Label { Text = "Pagado", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 3);
            header.Add(new Label { Text = "Saldo", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 4);
            header.Add(new Label { Text = "Estado", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 5);
            header.Add(new Label { Text = "Accion", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 6);
            listCuentas.Children.Add(header);

            foreach (CuentaPorPagar c in ctrl.getCuentas())
            {
                var rowGrid = new Grid
                {
                    HeightRequest = 42,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 120 }
                    }
                };

                rowGrid.Add(new Label { Text = c.getProveedor().getNombre(), VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
                rowGrid.Add(new Label { Text = c.getFolioOrden(), VerticalOptions = LayoutOptions.Center }, 1);
                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", c.getMontoTotal()), VerticalOptions = LayoutOptions.Center }, 2);
                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", c.getPagado()), VerticalOptions = LayoutOptions.Center }, 3);
                rowGrid.Add(new Label { Text = string.Format("${0:0.00}", c.getSaldo()), VerticalOptions = LayoutOptions.Center }, 4);
                rowGrid.Add(new Label { Text = c.getEstado().ToString(), VerticalOptions = LayoutOptions.Center }, 5);

                var btnLiquidar = new Button
                {
                    Text = "Liquidar",
                    FontSize = Estilos.FUENTE_XS_SIZE,
                    TextColor = Estilos.INDIGO,
                    BackgroundColor = Estilos.INDIGO_CLARO,
                    HeightRequest = 30,
                    Padding = new Thickness(0)
                };
                int currentCuentaId = c.getId();
                double currentSaldo = c.getSaldo();
                btnLiquidar.Clicked += async (s, e) =>
                {
                    if (currentSaldo > 0 && Application.Current?.MainPage != null)
                    {
                        string input = await Application.Current.MainPage.DisplayPromptAsync(
                            "Liquidar Cuenta",
                            string.Format("Monto a liquidar (saldo: ${0:0.00}):", currentSaldo),
                            "Aceptar",
                            "Cancelar",
                            null,
                            -1,
                            Keyboard.Numeric,
                            string.Format("{0:0.00}", currentSaldo)
                        );
                        if (!string.IsNullOrEmpty(input))
                        {
                            try
                            {
                                double monto = double.Parse(input.Trim());
                                ctrl.liquidarCuenta(currentCuentaId, monto);
                                refrescarCuentas();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                };
                rowGrid.Add(btnLiquidar, 6);

                listCuentas.Children.Add(rowGrid);
            }
        }

        private async void abrirFormularioProveedor(Proveedor p)
        {
            var dlg = new ProveedorDialog(p, ctrl);
            await dlg.mostrar();
            refrescarProveedores();
        }

        private async void abrirNuevaOrden()
        {
            var dlg = new NuevaOrdenDialog(ctrl);
            await dlg.mostrar();
            refrescarOrdenes();
        }
    }
}
