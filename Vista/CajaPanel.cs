using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;

namespace punto_de_venta_C_.Vista
{
    public class CajaPanel : ContentView
    {
        private readonly CajaControlador ctrl;
        private readonly Usuario usuario;
        private readonly VentanaPrincipal ventana;

        private Grid contenido;
        private View panelCerrada;
        private View panelAbierta;

        private Label lblEfectivo;
        private Label lblEfectivoTotal;
        private VerticalStackLayout movsLayout;

        public CajaPanel(CajaControlador ctrl, Usuario usuario, VentanaPrincipal ventana)
        {
            this.ctrl = ctrl;
            this.usuario = usuario;
            this.ventana = ventana;
            BackgroundColor = Estilos.BG_CLARO;
            Padding = new Thickness(24);
            construir();
        }

        private void construir()
        {
            contenido = new Grid();

            panelCerrada = construirPanelCerrada();
            panelAbierta = construirPanelAbierta();

            contenido.Add(panelCerrada);
            contenido.Add(panelAbierta);

            Content = contenido;
            refrescar();
        }

        private View construirPanelCerrada()
        {
            var grid = new Grid
            {
                BackgroundColor = Estilos.BG_CLARO,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var tarjeta = new VerticalStackLayout
            {
                BackgroundColor = Estilos.BG_BLANCO,
                WidthRequest = 420,
                HeightRequest = 440,
                Padding = new Thickness(40, 44),
                Spacing = 16
            };

            var icono = new Label
            {
                Text = "💵",
                FontSize = 64,
                HorizontalOptions = LayoutOptions.Center
            };
            tarjeta.Add(icono);

            var tit = new Label
            {
                Text = "Terminal Cerrada",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };
            tarjeta.Add(tit);

            var resp = new Label
            {
                Text = "Responsable: " + usuario.getNombre(),
                FontSize = Estilos.FUENTE_SMALL_SIZE,
                TextColor = Estilos.TEXTO_SECUNDARIO,
                HorizontalOptions = LayoutOptions.Center
            };
            tarjeta.Add(resp);

            tarjeta.Add(new BoxView { HeightRequest = 12, Color = Colors.Transparent });

            var lblFondoTexto = new Label
            {
                Text = "FONDO INICIAL ($)",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
            tarjeta.Add(lblFondoTexto);

            var txtFondo = new Entry
            {
                Text = "0.00",
                FontSize = 26,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Estilos.BG_ZINC_100,
                TextColor = Estilos.TEXTO_PRINCIPAL,
                HeightRequest = 56
            };
            tarjeta.Add(txtFondo);

            tarjeta.Add(new BoxView { HeightRequest = 8, Color = Colors.Transparent });

            var btnAbrir = Estilos.botonPrimario("ABRIR TERMINAL");
            btnAbrir.FontSize = 16;
            btnAbrir.HeightRequest = 56;
            btnAbrir.Clicked += (s, e) =>
            {
                try
                {
                    double fondo = double.Parse(txtFondo.Text?.Trim() ?? "0");
                    ctrl.abrirCaja(fondo);
                }
                catch (Exception)
                {
                    ctrl.abrirCaja(0);
                }
            };
            tarjeta.Add(btnAbrir);

            grid.Children.Add(tarjeta);
            return grid;
        }

        private View construirPanelAbierta()
        {
            var layout = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = 16 },
                    new RowDefinition { Height = GridLength.Star }
                }
            };

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_BLANCO,
                Padding = new Thickness(24, 20),
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 160 }
                }
            };

            var tit = new Label
            {
                Text = "GESTIÓN DE CAJA  —  Operativa",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };
            header.Add(tit, 0, 0);

            var btnRetiro = Estilos.botonPeligro("↑ Retiro / Pago");
            btnRetiro.HeightRequest = 42;
            btnRetiro.Clicked += (s, e) => abrirDialogoRetiro();
            header.Add(btnRetiro, 1, 0);

            layout.Add(header, 0, 0);

            var centro = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = 110 },
                    new RowDefinition { Height = 16 },
                    new RowDefinition { Height = GridLength.Star }
                }
            };

            var tarjetasGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 16 },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };

            var cardFondo = tarjetaCaja("Total Ventas", "$0.00", Estilos.BG_BLANCO, Estilos.TEXTO_PRINCIPAL, out lblEfectivo);
            tarjetasGrid.Add(cardFondo, 0, 0);

            var cardEfectivo = tarjetaCaja("Efectivo en Caja", "$0.00", Estilos.INDIGO, Colors.White, out lblEfectivoTotal);
            tarjetasGrid.Add(cardEfectivo, 2, 0);

            centro.Add(tarjetasGrid, 0, 0);

            movsLayout = new VerticalStackLayout();
            var scroll = new ScrollView
            {
                Content = movsLayout,
                BackgroundColor = Estilos.BG_BLANCO
            };

            centro.Add(scroll, 0, 2);
            layout.Add(centro, 0, 2);

            return layout;
        }

        private Frame tarjetaCaja(string label, string valor, Color bg, Color fg, out Label labelRef)
        {
            var valLbl = new Label
            {
                Text = valor,
                FontSize = 34,
                FontAttributes = FontAttributes.Bold,
                TextColor = fg
            };
            labelRef = valLbl;

            var card = new Frame
            {
                BackgroundColor = bg,
                BorderColor = bg == Estilos.BG_BLANCO ? Estilos.BORDE : bg,
                CornerRadius = 8,
                Padding = new Thickness(22, 18),
                Content = new VerticalStackLayout
                {
                    Spacing = 6,
                    Children =
                    {
                        new Label
                        {
                            Text = label.ToUpper(),
                            FontSize = Estilos.FUENTE_XS_SIZE,
                            TextColor = bg == Estilos.INDIGO ? Color.FromRgb(199, 210, 254) : Estilos.TEXTO_TENUE
                        },
                        valLbl
                    }
                }
            };

            return card;
        }

        private async void abrirDialogoRetiro()
        {
            var dlg = new RetiroDialog(ctrl);
            await dlg.mostrar();
            refrescar();
        }

        public void refrescar()
        {
            if (ctrl.isCajaAbierta())
            {
                panelCerrada.IsVisible = false;
                panelAbierta.IsVisible = true;

                double totalGeneral = ctrl.getMovimientos()
                    .Where(m => m.getTipo() == Movimiento.Tipo.VENTA)
                    .Select(m => m.getMonto()).Sum();

                if (lblEfectivo != null)
                {
                    lblEfectivo.Text = string.Format("${0:0.00}", totalGeneral);
                }

                if (lblEfectivoTotal != null)
                {
                    lblEfectivoTotal.Text = string.Format("${0:0.00}", ctrl.getEfectivoEsperado());
                }

                actualizarTablaMovimientos();
            }
            else
            {
                panelCerrada.IsVisible = true;
                panelAbierta.IsVisible = false;
            }
        }

        private void actualizarTablaMovimientos()
        {
            if (movsLayout == null)
                return;

            var movs = new List<Movimiento>(ctrl.getMovimientos());
            movs.Reverse();
            movsLayout.Children.Clear();

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 30,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 110 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 120 }
                }
            };
            header.Add(new Label { Text = "Hora", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
            header.Add(new Label { Text = "Tipo", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 1);
            header.Add(new Label { Text = "Concepto", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 2);
            header.Add(new Label { Text = "Monto", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 3);
            movsLayout.Children.Add(header);

            foreach (Movimiento m in movs)
            {
                var rowGrid = new Grid
                {
                    HeightRequest = 40,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = 100 },
                        new ColumnDefinition { Width = 110 },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 120 }
                    }
                };

                rowGrid.Add(new Label { Text = m.getFecha().ToString("HH:mm:ss"), VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
                rowGrid.Add(new Label { Text = m.getTipo().ToString(), VerticalOptions = LayoutOptions.Center }, 1);
                rowGrid.Add(new Label { Text = m.getDescripcion(), VerticalOptions = LayoutOptions.Center }, 2);

                var montoLbl = new Label
                {
                    Text = string.Format(m.esIngreso() ? "+${0:0.00}" : "-${0:0.00}", m.getMonto()),
                    TextColor = m.esIngreso() ? Estilos.EMERALD : Estilos.ROSE,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center
                };
                rowGrid.Add(montoLbl, 3);

                movsLayout.Children.Add(rowGrid);
            }
        }
    }
}
