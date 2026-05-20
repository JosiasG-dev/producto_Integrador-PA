using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;

namespace punto_de_venta_C_.Vista
{
    public class LoginVista : ContentPage
    {
        private readonly ControladorPrincipal app;
        private Entry txtUsuario;
        private Entry txtPassword;
        private Label lblError;

        public LoginVista(ControladorPrincipal app)
        {
            this.app = app;
            NavigationPage.SetHasNavigationBar(this, false);
            construir();
        }

        private void construir()
        {
            var fondo = new AbsoluteLayout
            {
                BackgroundColor = Estilos.BG_OSCURO,
                WidthRequest = 460,
                HeightRequest = 520
            };

            var tarjeta = new AbsoluteLayout
            {
                BackgroundColor = Estilos.BG_BLANCO,
                WidthRequest = 372,
                HeightRequest = 440
            };
            AbsoluteLayout.SetLayoutBounds(tarjeta, new Rect(44, 30, 372, 440));
            AbsoluteLayout.SetLayoutFlags(tarjeta, AbsoluteLayoutFlags.None);
            fondo.Add(tarjeta);

            var icono = new Label
            {
                Text = "PUNTO DE VENTA",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.INDIGO,
                WidthRequest = 372,
                HeightRequest = 30
            };
            AbsoluteLayout.SetLayoutBounds(icono, new Rect(0, 24, 372, 30));
            AbsoluteLayout.SetLayoutFlags(icono, AbsoluteLayoutFlags.None);
            tarjeta.Add(icono);

            var titulo = new Label
            {
                Text = "Terminal Corporativa",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL,
                WidthRequest = 372,
                HeightRequest = 28
            };
            AbsoluteLayout.SetLayoutBounds(titulo, new Rect(0, 60, 372, 28));
            AbsoluteLayout.SetLayoutFlags(titulo, AbsoluteLayoutFlags.None);
            tarjeta.Add(titulo);

            var sub = new Label
            {
                Text = "SISTEMA POS",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Estilos.FUENTE_XS_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.INDIGO,
                WidthRequest = 372,
                HeightRequest = 16
            };
            AbsoluteLayout.SetLayoutBounds(sub, new Rect(0, 92, 372, 16));
            AbsoluteLayout.SetLayoutFlags(sub, AbsoluteLayoutFlags.None);
            tarjeta.Add(sub);

            var lblUsr = new Label
            {
                Text = "USUARIO",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE,
                WidthRequest = 300,
                HeightRequest = 14
            };
            AbsoluteLayout.SetLayoutBounds(lblUsr, new Rect(36, 132, 300, 14));
            AbsoluteLayout.SetLayoutFlags(lblUsr, AbsoluteLayoutFlags.None);
            tarjeta.Add(lblUsr);

            txtUsuario = Estilos.campo("Usuario");
            AbsoluteLayout.SetLayoutBounds(txtUsuario, new Rect(36, 150, 300, 44));
            AbsoluteLayout.SetLayoutFlags(txtUsuario, AbsoluteLayoutFlags.None);
            tarjeta.Add(txtUsuario);

            var lblPass = new Label
            {
                Text = "CONTRASEÑA",
                FontSize = Estilos.FUENTE_XS_SIZE,
                TextColor = Estilos.TEXTO_TENUE,
                WidthRequest = 300,
                HeightRequest = 14
            };
            AbsoluteLayout.SetLayoutBounds(lblPass, new Rect(36, 206, 300, 14));
            AbsoluteLayout.SetLayoutFlags(lblPass, AbsoluteLayoutFlags.None);
            tarjeta.Add(lblPass);

            txtPassword = Estilos.campo("Contraseña");
            txtPassword.IsPassword = true;
            AbsoluteLayout.SetLayoutBounds(txtPassword, new Rect(36, 224, 300, 44));
            AbsoluteLayout.SetLayoutFlags(txtPassword, AbsoluteLayoutFlags.None);
            tarjeta.Add(txtPassword);

            txtPassword.Completed += (s, e) => intentarLogin();

            lblError = new Label
            {
                Text = " ",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Estilos.FUENTE_SMALL_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.ROSE,
                WidthRequest = 300,
                HeightRequest = 16
            };
            AbsoluteLayout.SetLayoutBounds(lblError, new Rect(36, 276, 300, 16));
            AbsoluteLayout.SetLayoutFlags(lblError, AbsoluteLayoutFlags.None);
            tarjeta.Add(lblError);

            var btnEntrar = Estilos.botonPrimario("ENTRAR AL SISTEMA");
            AbsoluteLayout.SetLayoutBounds(btnEntrar, new Rect(36, 302, 300, 48));
            AbsoluteLayout.SetLayoutFlags(btnEntrar, AbsoluteLayoutFlags.None);
            btnEntrar.Clicked += (s, e) => intentarLogin();
            tarjeta.Add(btnEntrar);

            Content = new Grid
            {
                BackgroundColor = Estilos.BG_OSCURO,
                Children = { fondo },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private void intentarLogin()
        {
            string user = txtUsuario.Text?.Trim() ?? "";
            string pass = txtPassword.Text?.Trim() ?? "";
            Usuario u = app.autenticar(user, pass);
            if (u != null)
            {
                lblError.Text = " ";
                app.onLoginExitoso(u);
            }
            else
            {
                lblError.Text = "Credenciales invalidas";
            }
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

        public void limpiar()
        {
            txtUsuario.Text = "";
            txtPassword.Text = "";
            lblError.Text = " ";
        }
    }
}
