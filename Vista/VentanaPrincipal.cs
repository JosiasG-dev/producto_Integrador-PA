using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;

namespace punto_de_venta_C_.Vista
{
    public class VentanaPrincipal : ContentPage
    {
        private readonly ControladorPrincipal app;
        private readonly VentaControlador ventaCtrl;
        private readonly InventarioControlador invCtrl;
        private readonly CajaControlador cajaCtrl;
        private readonly ProveedorControlador provCtrl;
        private readonly UsuarioControlador usuCtrl;

        private Grid contenido;
        private Label labelNombreTienda;

        private VentaPanel ventaPanel;
        private InventarioPanel inventarioPanel;
        private CajaPanel cajaPanel;
        private ProveedorPanel proveedorPanel;
        private UsuariosPanel usuariosPanel;
        private ReportesPanel reportesPanel;
        private ConfigPanel configPanel;

        private const string TAB_VENTA = "venta";
        private const string TAB_INVENTARIO = "inventario";
        private const string TAB_PROVEEDORES = "proveedores";
        private const string TAB_CAJA = "caja";
        private const string TAB_REPORTES = "reportes";
        private const string TAB_USUARIOS = "usuarios";
        private const string TAB_CONFIG = "configuracion";

        public VentanaPrincipal(ControladorPrincipal app, VentaControlador ventaCtrl, InventarioControlador invCtrl,
                CajaControlador cajaCtrl, ProveedorControlador provCtrl, UsuarioControlador usuCtrl)
        {
            this.app = app;
            this.ventaCtrl = ventaCtrl;
            this.invCtrl = invCtrl;
            this.cajaCtrl = cajaCtrl;
            this.provCtrl = provCtrl;
            this.usuCtrl = usuCtrl;
            NavigationPage.SetHasNavigationBar(this, false);
            construir();
        }

        private void construir()
        {
            var mainGrid = new Grid
            {
                BackgroundColor = Estilos.BG_CLARO,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 220 },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };

            var sidebar = construirSidebar();
            mainGrid.Add(sidebar, 0, 0);

            contenido = new Grid
            {
                BackgroundColor = Estilos.BG_CLARO
            };

            ventaPanel = new VentaPanel(ventaCtrl, app.getConfig(), app.getUsuarioActivo());
            inventarioPanel = new InventarioPanel(invCtrl, app.getUsuarioActivo());
            cajaPanel = new CajaPanel(cajaCtrl, app.getUsuarioActivo(), this);
            proveedorPanel = new ProveedorPanel(provCtrl, app.getUsuarioActivo());
            usuariosPanel = new UsuariosPanel(usuCtrl);
            reportesPanel = new ReportesPanel(app);
            configPanel = new ConfigPanel(app);

            ventaCtrl.setPanel(ventaPanel);
            invCtrl.setPanel(inventarioPanel);
            cajaCtrl.setPanel(cajaPanel);
            provCtrl.setPanel(proveedorPanel);
            usuCtrl.setPanel(usuariosPanel);

            contenido.Add(ventaPanel);
            contenido.Add(inventarioPanel);
            contenido.Add(cajaPanel);
            contenido.Add(proveedorPanel);
            contenido.Add(usuariosPanel);
            contenido.Add(reportesPanel);
            contenido.Add(configPanel);

            cambiarTab(TAB_VENTA);

            mainGrid.Add(contenido, 1, 0);
            Content = mainGrid;

            actualizarTitulo();
        }

        private View construirSidebar()
        {
            var sidebarLayout = new Grid
            {
                BackgroundColor = Estilos.BG_OSCURO,
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = 80 },
                    new RowDefinition { Height = 1 },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = 120 }
                }
            };

            var logo = new HorizontalStackLayout
            {
                Spacing = 16,
                Padding = new Thickness(16, 20),
                VerticalOptions = LayoutOptions.Center
            };

            labelNombreTienda = new Label
            {
                Text = " " + app.getConfig().getNombre(),
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                VerticalOptions = LayoutOptions.Center
            };
            logo.Add(labelNombreTienda);
            sidebarLayout.Add(logo, 0, 0);

            var sep = new BoxView
            {
                Color = Color.FromRgb(63, 63, 70),
                HeightRequest = 1,
                HorizontalOptions = LayoutOptions.Fill
            };
            sidebarLayout.Add(sep, 0, 1);

            var menuStack = new VerticalStackLayout
            {
                Spacing = 4,
                Padding = new Thickness(0, 12)
            };

            menuStack.Add(itemMenu("Venta", TAB_VENTA));
            menuStack.Add(itemMenu("Inventario", TAB_INVENTARIO));
            menuStack.Add(itemMenu("Proveedores", TAB_PROVEEDORES));
            menuStack.Add(itemMenu("Flujo de Caja", TAB_CAJA));
            menuStack.Add(itemMenu("Reportes", TAB_REPORTES));

            if (app.getUsuarioActivo().esAdmin())
            {
                menuStack.Add(new BoxView { HeightRequest = 16, Color = Colors.Transparent });
                var secLabel = new Label
                {
                    Text = "CONFIGURACIÓN",
                    FontSize = Estilos.FUENTE_XS_SIZE,
                    TextColor = Color.FromRgb(63, 63, 70),
                    Margin = new Thickness(20, 0)
                };
                menuStack.Add(secLabel);
                menuStack.Add(itemMenu("Personal", TAB_USUARIOS));
                menuStack.Add(itemMenu("Sistema", TAB_CONFIG));
            }

            var scroll = new ScrollView { Content = menuStack };
            sidebarLayout.Add(scroll, 0, 2);

            var footer = new VerticalStackLayout
            {
                BackgroundColor = Estilos.BG_MUY_OSCURO,
                Padding = new Thickness(14),
                Spacing = 4
            };

            Usuario u = app.getUsuarioActivo();
            var nombreLbl = new Label
            {
                Text = u.getNombre()[0] + "  " + u.getNombre(),
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White
            };

            var rolLbl = new Label
            {
                Text = u.getRol(),
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.INDIGO
            };

            var btnSalir = new Button
            {
                Text = "Cerrar Sesión",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.ROSE,
                BackgroundColor = Colors.Transparent,
                BorderWidth = 0,
                Padding = new Thickness(0),
                HorizontalOptions = LayoutOptions.Start
            };
            btnSalir.Clicked += (s, e) => app.onCerrarSesion();

            footer.Add(nombreLbl);
            footer.Add(rolLbl);
            footer.Add(new BoxView { HeightRequest = 10, Color = Colors.Transparent });
            footer.Add(btnSalir);

            sidebarLayout.Add(footer, 0, 3);

            return sidebarLayout;
        }

        private Button itemMenu(string label, string tabId)
        {
            var btn = new Button
            {
                Text = label,
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Color.FromRgb(161, 161, 170),
                BackgroundColor = Estilos.BG_OSCURO,
                BorderWidth = 0,
                CornerRadius = 0,
                HeightRequest = 46,
                Padding = new Thickness(20, 12),
                HorizontalOptions = LayoutOptions.Fill
            };
            btn.Clicked += (s, e) => cambiarTab(tabId);
            return btn;
        }

        private void cambiarTab(string tab)
        {
            ventaPanel.IsVisible = (tab == TAB_VENTA);
            inventarioPanel.IsVisible = (tab == TAB_INVENTARIO);
            cajaPanel.IsVisible = (tab == TAB_CAJA);
            proveedorPanel.IsVisible = (tab == TAB_PROVEEDORES);
            usuariosPanel.IsVisible = (tab == TAB_USUARIOS);
            reportesPanel.IsVisible = (tab == TAB_REPORTES);
            configPanel.IsVisible = (tab == TAB_CONFIG);

            switch (tab)
            {
                case TAB_INVENTARIO: inventarioPanel.refrescar(); break;
                case TAB_CAJA: cajaPanel.refrescar(); break;
                case TAB_PROVEEDORES:
                    proveedorPanel.refrescarProveedores();
                    proveedorPanel.refrescarOrdenes();
                    proveedorPanel.refrescarCuentas();
                    break;
                case TAB_USUARIOS: usuariosPanel.refrescar(); break;
                case TAB_REPORTES: reportesPanel.refrescar(); break;
            }
        }

        public void actualizarTitulo()
        {
            ConfiguracionTienda config = app.getConfig();

            if (labelNombreTienda != null)
            {
                labelNombreTienda.Text = "🏪 " + config.getNombre();
            }

            Title = config.getNombre() + " - Sistema de Punto de Venta";
        }

        public void refrescarInventario()
        {
            inventarioPanel.refrescar();
        }

        public void refrescarCaja()
        {
            if (cajaCtrl.isCajaAbierta())
                cajaPanel.refrescar();
        }

        public void mostrar()
        {
            if (Application.Current != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new NavigationPage(this);
                });
            }
        }

        public void ocultar()
        {
        }
    }
}
