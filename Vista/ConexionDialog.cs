using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Threading.Tasks;
using punto_de_venta_C_.ConexionBD;

namespace punto_de_venta_C_.Vista
{
    public class ConexionDialog : ContentPage
    {
        private bool confirmado = false;
        private Entry campoHost;
        private Entry campoPuerto;
        private Entry campoBase;
        private Entry campoUsuario;
        private Entry campoContrasena;
        private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public ConexionDialog()
        {
            Title = "Configuracion de Conexion";
            NavigationPage.SetHasNavigationBar(this, false);
            construirUI();
        }

        private void construirUI()
        {
            var layout = new VerticalStackLayout
            {
                Spacing = 12,
                Padding = new Thickness(24),
                BackgroundColor = Estilos.BG_BLANCO,
                WidthRequest = 450,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var titulo = new Label
            {
                Text = "Selecciona el motor de base de datos",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            layout.Add(titulo);

            layout.Add(new Label { Text = "Motor (Solo MySQL):", FontSize = Estilos.FUENTE_XS_SIZE, TextColor = Estilos.TEXTO_TENUE });
            var entryMotor = new Entry { Text = "MySQL", IsReadOnly = true, BackgroundColor = Estilos.BG_ZINC_100 };
            layout.Add(entryMotor);

            layout.Add(new Label { Text = "Host:", FontSize = Estilos.FUENTE_XS_SIZE, TextColor = Estilos.TEXTO_TENUE });
            campoHost = new Entry { Text = "localhost" };
            layout.Add(campoHost);

            layout.Add(new Label { Text = "Puerto:", FontSize = Estilos.FUENTE_XS_SIZE, TextColor = Estilos.TEXTO_TENUE });
            campoPuerto = new Entry { Text = "3306", Keyboard = Keyboard.Numeric };
            layout.Add(campoPuerto);

            layout.Add(new Label { Text = "Base de datos:", FontSize = Estilos.FUENTE_XS_SIZE, TextColor = Estilos.TEXTO_TENUE });
            campoBase = new Entry { Text = "corporativo_pos" };
            layout.Add(campoBase);

            layout.Add(new Label { Text = "Usuario:", FontSize = Estilos.FUENTE_XS_SIZE, TextColor = Estilos.TEXTO_TENUE });
            campoUsuario = new Entry { Text = "root" };
            layout.Add(campoUsuario);

            layout.Add(new Label { Text = "Contrasena:", FontSize = Estilos.FUENTE_XS_SIZE, TextColor = Estilos.TEXTO_TENUE });
            campoContrasena = new Entry { Text = "2306", IsPassword = true };
            layout.Add(campoContrasena);

            var panelBotones = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 100 },
                    new ColumnDefinition { Width = 8 },
                    new ColumnDefinition { Width = 100 }
                }
            };

            var btnSalir = Estilos.botonPeligro("Salir");
            btnSalir.Clicked += (s, e) => Environment.Exit(0);
            panelBotones.Add(btnSalir, 1, 0);

            var btnConectar = Estilos.botonPrimario("Conectar");
            btnConectar.Clicked += (s, e) => intentarConexion();
            panelBotones.Add(btnConectar, 3, 0);

            layout.Add(panelBotones);

            Content = new Grid
            {
                BackgroundColor = Estilos.BG_CLARO,
                Children = { layout },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private async void intentarConexion()
        {
            string host = campoHost.Text?.Trim() ?? "";
            string baseDatos = campoBase.Text?.Trim() ?? "";
            string puerto = campoPuerto.Text?.Trim() ?? "";
            string usr = campoUsuario.Text?.Trim() ?? "";
            string pass = campoContrasena.Text ?? "";

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(baseDatos) || string.IsNullOrEmpty(puerto) || string.IsNullOrEmpty(usr))
            {
                await DisplayAlert("Aviso", "Completa todos los campos.", "OK");
                return;
            }

            Conexion.configurar(Conexion.TipoMotor.MYSQL, Conexion.TipoAutenticacion.CREDENCIALES, host, puerto, baseDatos, usr, pass);

            var conn = Conexion.getConexion();
            if (conn != null)
            {
                confirmado = true;
                tcs.SetResult(true);
            }
            else
            {
                await DisplayAlert("Error de Conexion", "No se pudo conectar.\nRevisa los datos e intenta de nuevo.", "OK");
                Conexion.cerrar();
            }
        }

        public async Task<bool> mostrar()
        {
            if (Application.Current != null)
            {
                Application.Current.MainPage = new NavigationPage(this);
            }
            return await tcs.Task;
        }

        public bool fueConfirmado()
        {
            return confirmado;
        }
    }
}
