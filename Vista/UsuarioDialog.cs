using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System;
using System.Threading.Tasks;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;

namespace punto_de_venta_C_.Vista
{
    public class UsuarioDialog : ContentPage
    {
        private readonly UsuarioControlador ctrl;
        private readonly Usuario usuario;

        private Entry txtNombre, txtUsername, txtPassword, txtEdad;
        private Picker cmbRol, cmbSexo;
        private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public UsuarioDialog(Usuario u, UsuarioControlador ctrl)
        {
            this.ctrl = ctrl;
            this.usuario = u;
            Title = u == null ? "Alta Personal" : "Editar Personal";
            BackgroundColor = Estilos.BG_BLANCO;
            construir();
            if (u != null)
                cargar(u);
        }

        private void construir()
        {
            var absoluteLayout = new AbsoluteLayout
            {
                WidthRequest = 480,
                HeightRequest = 520,
                BackgroundColor = Estilos.BG_BLANCO
            };

            var tit = new Label
            {
                Text = usuario == null ? "ALTA PERSONAL" : "EDITAR PERSONAL",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(tit, new Rect(32, 28, 416, 30));
            AbsoluteLayout.SetLayoutFlags(tit, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(tit);

            double startY = 78;
            double fieldGap = 64;

            // Nombre Completo
            var lblNombre = lbl("Nombre Completo");
            AbsoluteLayout.SetLayoutBounds(lblNombre, new Rect(32, startY, 416, 14));
            AbsoluteLayout.SetLayoutFlags(lblNombre, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblNombre);

            txtNombre = campo();
            AbsoluteLayout.SetLayoutBounds(txtNombre, new Rect(32, startY + 18, 416, 40));
            AbsoluteLayout.SetLayoutFlags(txtNombre, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtNombre);

            startY += fieldGap;

            // Rol
            var lblRol = lbl("Rol en el Sistema");
            AbsoluteLayout.SetLayoutBounds(lblRol, new Rect(32, startY, 416, 14));
            AbsoluteLayout.SetLayoutFlags(lblRol, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblRol);

            cmbRol = new Picker { FontAttributes = FontAttributes.Bold };
            cmbRol.Items.Add("CAJERO");
            cmbRol.Items.Add("ADMINISTRADOR");
            cmbRol.SelectedIndex = 0;
            AbsoluteLayout.SetLayoutBounds(cmbRol, new Rect(32, startY + 18, 416, 40));
            AbsoluteLayout.SetLayoutFlags(cmbRol, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(cmbRol);

            startY += fieldGap;

            // Usuario
            var lblUser = lbl("Usuario de Acceso (Red)");
            AbsoluteLayout.SetLayoutBounds(lblUser, new Rect(32, startY, 416, 14));
            AbsoluteLayout.SetLayoutFlags(lblUser, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblUser);

            txtUsername = campo();
            AbsoluteLayout.SetLayoutBounds(txtUsername, new Rect(32, startY + 18, 416, 40));
            AbsoluteLayout.SetLayoutFlags(txtUsername, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtUsername);

            startY += fieldGap;

            // Contraseña
            var lblPass = lbl("Contraseña");
            AbsoluteLayout.SetLayoutBounds(lblPass, new Rect(32, startY, 416, 14));
            AbsoluteLayout.SetLayoutFlags(lblPass, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblPass);

            txtPassword = campo();
            AbsoluteLayout.SetLayoutBounds(txtPassword, new Rect(32, startY + 18, 416, 40));
            AbsoluteLayout.SetLayoutFlags(txtPassword, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtPassword);

            startY += fieldGap;

            // Row for Edad and Sexo
            var lblEdad = lbl("Edad");
            AbsoluteLayout.SetLayoutBounds(lblEdad, new Rect(32, startY, 190, 14));
            AbsoluteLayout.SetLayoutFlags(lblEdad, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblEdad);

            txtEdad = campo();
            txtEdad.Text = "18";
            txtEdad.Keyboard = Keyboard.Numeric;
            AbsoluteLayout.SetLayoutBounds(txtEdad, new Rect(32, startY + 18, 190, 40));
            AbsoluteLayout.SetLayoutFlags(txtEdad, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtEdad);

            var lblSexo = lbl("Sexo");
            AbsoluteLayout.SetLayoutBounds(lblSexo, new Rect(258, startY, 190, 14));
            AbsoluteLayout.SetLayoutFlags(lblSexo, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblSexo);

            cmbSexo = new Picker { FontAttributes = FontAttributes.Bold };
            cmbSexo.Items.Add("Femenino");
            cmbSexo.Items.Add("Masculino");
            cmbSexo.Items.Add("Otro");
            cmbSexo.SelectedIndex = 0;
            AbsoluteLayout.SetLayoutBounds(cmbSexo, new Rect(258, startY + 18, 190, 40));
            AbsoluteLayout.SetLayoutFlags(cmbSexo, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(cmbSexo);

            // Botones
            var btns = new HorizontalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.End
            };

            if (usuario != null && usuario.getId() != ctrl.getUsuarioActivo().getId())
            {
                var btnElim = Estilos.botonPeligro("Eliminar");
                btnElim.HeightRequest = 42;
                btnElim.WidthRequest = 110;
                btnElim.Clicked += async (s, e) =>
                {
                    ctrl.eliminarUsuario(usuario.getId());
                    tcs.SetResult(true);
                    await Navigation.PopModalAsync();
                };
                btns.Add(btnElim);
            }

            var btnCancelar = Estilos.botonSecundario("Cancelar");
            btnCancelar.HeightRequest = 42;
            btnCancelar.WidthRequest = 110;
            btnCancelar.Clicked += async (s, e) =>
            {
                tcs.SetResult(false);
                await Navigation.PopModalAsync();
            };
            btns.Add(btnCancelar);

            var btnGuardar = Estilos.botonPrimario("Guardar");
            btnGuardar.HeightRequest = 42;
            btnGuardar.WidthRequest = 120;
            btnGuardar.Clicked += (s, e) => guardar();
            btns.Add(btnGuardar);

            AbsoluteLayout.SetLayoutBounds(btns, new Rect(32, 450, 416, 52));
            AbsoluteLayout.SetLayoutFlags(btns, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btns);

            Content = new Grid
            {
                Children = { absoluteLayout },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private void cargar(Usuario u)
        {
            txtNombre.Text = u.getNombre();
            if (cmbRol.Items.Contains(u.getRol()))
                cmbRol.SelectedItem = u.getRol();
            txtUsername.Text = u.getUsername();
            txtPassword.Text = u.getPassword();
            txtEdad.Text = u.getEdad().ToString();
            if (cmbSexo.Items.Contains(u.getSexo()))
                cmbSexo.SelectedItem = u.getSexo();
        }

        private async void guardar()
        {
            string nombre = txtNombre.Text?.Trim() ?? "";
            string username = txtUsername.Text?.Trim() ?? "";
            string password = txtPassword.Text?.Trim() ?? "";

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Nombre, usuario y contraseña son obligatorios.", "OK");
                return;
            }

            int edad;
            if (!int.TryParse(txtEdad.Text?.Trim() ?? "18", out edad) || edad < 16 || edad > 100)
            {
                await DisplayAlert("Error", "Edad debe ser un número válido entre 16 y 100.", "OK");
                return;
            }

            int id = usuario != null ? usuario.getId() : ctrl.generarId();
            Usuario u = new Usuario(
                id, 
                username, 
                password, 
                cmbRol.SelectedItem?.ToString() ?? "CAJERO", 
                nombre,
                edad, 
                cmbSexo.SelectedItem?.ToString() ?? "Otro"
            );

            ctrl.guardarUsuario(u);
            tcs.SetResult(true);
            await Navigation.PopModalAsync();
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

        private Entry campo()
        {
            return new Entry
            {
                FontSize = Estilos.FUENTE_BOLD_SIZE,
                BackgroundColor = Estilos.BG_ZINC_100,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
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
