using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;

namespace punto_de_venta_C_.Vista
{
    public class ConfigPanel : ContentView
    {
        private readonly ControladorPrincipal app;
        private Entry txtNombre, txtSucursal, txtRFC;

        public ConfigPanel(ControladorPrincipal app)
        {
            this.app = app;
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
                Text = "⚙️  SISTEMA  —  Identidad Fiscal",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };
            header.Add(tit, 0, 0);
            rootLayout.Add(header, 0, 0);

            var formLayout = new VerticalStackLayout
            {
                Spacing = 16,
                BackgroundColor = Estilos.BG_BLANCO,
                Padding = new Thickness(36, 32)
            };

            ConfiguracionTienda cfg = app.getConfig();

            formLayout.Add(lbl("Razón Social / Nombre de la Tienda"));
            txtNombre = campo(cfg.getNombre());
            formLayout.Add(txtNombre);

            formLayout.Add(lbl("Sucursal"));
            txtSucursal = campo(cfg.getSucursal());
            formLayout.Add(txtSucursal);

            formLayout.Add(lbl("RFC"));
            txtRFC = campo(cfg.getRfc());
            formLayout.Add(txtRFC);

            var btnGuardar = Estilos.botonPrimario("✓  ACTUALIZAR CONFIGURACIÓN");
            btnGuardar.FontSize = 16;
            btnGuardar.HeightRequest = 60;
            btnGuardar.Clicked += (s, e) => guardar();
            formLayout.Add(btnGuardar);

            var btnReiniciar = Estilos.botonPeligro("REINICIAR SISTEMA");
            btnReiniciar.FontSize = 16;
            btnReiniciar.HeightRequest = 60;
            btnReiniciar.Clicked += async (s, e) =>
            {
                if (Application.Current?.MainPage == null) return;

                bool confirm1 = await Application.Current.MainPage.DisplayAlert(
                    "ADVERTENCIA 1 DE 3",
                    "¿estas seguro? se borraran todas las ventas y movimientos del sistema",
                    "Sí", "No"
                );

                if (confirm1)
                {
                    bool confirm2 = await Application.Current.MainPage.DisplayAlert(
                        "ADVERTENCIA 2 DE 3",
                        "esta accion no se puede deshacer; perderas todo el historial financiero, ¿continuar?",
                        "Sí", "No"
                    );

                    if (confirm2)
                    {
                        string pass = await Application.Current.MainPage.DisplayPromptAsync(
                            "ADVERTENCIA 3 DE 3",
                            "para confirmar escribe la palabra: ELIMINAR TODO"
                        );

                        if (pass != null && pass.Trim().Equals("ELIMINAR TODO", StringComparison.OrdinalIgnoreCase))
                        {
                            app.reiniciarSistemaCompleto();
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Aviso", "reinicio cancelado, la palabra no coincide", "OK");
                        }
                    }
                }
            };
            formLayout.Add(btnReiniciar);

            var scroll = new ScrollView
            {
                Content = formLayout
            };
            rootLayout.Add(scroll, 0, 2);

            Content = rootLayout;
        }

        private async void guardar()
        {
            string nuevoNombre = txtNombre.Text ?? "";
            string nuevaSucursal = txtSucursal.Text ?? "";
            string nuevoRFC = txtRFC.Text ?? "";

            if (string.IsNullOrEmpty(nuevoNombre) || string.IsNullOrEmpty(nuevaSucursal) || string.IsNullOrEmpty(nuevoRFC))
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("Error", "por favor llena todos los campos", "OK");
                return;
            }

            ConfiguracionTienda nuevaConfig = new ConfiguracionTienda(nuevoNombre, nuevaSucursal, nuevoRFC);
            app.guardarConfiguracion(nuevaConfig);
            app.getVentanaPrincipal().actualizarTitulo();

            if (Application.Current?.MainPage != null)
                await Application.Current.MainPage.DisplayAlert("Éxito", "configuracion guardada exitosamente en la base de datos", "OK");
        }

        private Label lbl(string t)
        {
            return new Label
            {
                Text = t.ToUpper(),
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE
            };
        }

        private Entry campo(string valor)
        {
            return new Entry
            {
                Text = valor,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Estilos.BG_ZINC_100,
                TextColor = Estilos.TEXTO_PRINCIPAL,
                HeightRequest = 56
            };
        }
    }
}
