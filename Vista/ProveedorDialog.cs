using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System;
using System.Threading.Tasks;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;

namespace punto_de_venta_C_.Vista
{
    public class ProveedorDialog : ContentPage
    {
        private readonly ProveedorControlador ctrl;
        private readonly Proveedor proveedor;
        private Entry txtNombre, txtContacto, txtTelefono, txtEmail, txtDireccion;
        private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public ProveedorDialog(Proveedor p, ProveedorControlador ctrl)
        {
            this.ctrl = ctrl;
            this.proveedor = p;
            Title = p == null ? "Nuevo Proveedor" : "Editar Proveedor";
            BackgroundColor = Estilos.BG_BLANCO;
            construir();
            if (p != null)
                cargar(p);
        }

        private void construir()
        {
            var absoluteLayout = new AbsoluteLayout
            {
                WidthRequest = 500,
                HeightRequest = 520,
                BackgroundColor = Estilos.BG_BLANCO
            };

            var tit = new Label
            {
                Text = proveedor == null ? "NUEVO PROVEEDOR" : "EDITAR PROVEEDOR",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(tit, new Rect(32, 28, 436, 30));
            AbsoluteLayout.SetLayoutFlags(tit, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(tit);

            double startY = 78;
            double fieldGap = 64;

            // Empresa
            var lblNombre = lbl("Empresa / Razón Social");
            AbsoluteLayout.SetLayoutBounds(lblNombre, new Rect(32, startY, 436, 14));
            AbsoluteLayout.SetLayoutFlags(lblNombre, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblNombre);

            txtNombre = campo();
            AbsoluteLayout.SetLayoutBounds(txtNombre, new Rect(32, startY + 18, 436, 40));
            AbsoluteLayout.SetLayoutFlags(txtNombre, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtNombre);

            startY += fieldGap;

            // Contacto
            var lblContacto = lbl("Nombre de Contacto");
            AbsoluteLayout.SetLayoutBounds(lblContacto, new Rect(32, startY, 436, 14));
            AbsoluteLayout.SetLayoutFlags(lblContacto, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblContacto);

            txtContacto = campo();
            AbsoluteLayout.SetLayoutBounds(txtContacto, new Rect(32, startY + 18, 436, 40));
            AbsoluteLayout.SetLayoutFlags(txtContacto, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtContacto);

            startY += fieldGap;

            // Teléfono
            var lblTelefono = lbl("Teléfono");
            AbsoluteLayout.SetLayoutBounds(lblTelefono, new Rect(32, startY, 436, 14));
            AbsoluteLayout.SetLayoutFlags(lblTelefono, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblTelefono);

            txtTelefono = campo();
            AbsoluteLayout.SetLayoutBounds(txtTelefono, new Rect(32, startY + 18, 436, 40));
            AbsoluteLayout.SetLayoutFlags(txtTelefono, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtTelefono);

            startY += fieldGap;

            // Email
            var lblEmail = lbl("Email Corporativo");
            AbsoluteLayout.SetLayoutBounds(lblEmail, new Rect(32, startY, 436, 14));
            AbsoluteLayout.SetLayoutFlags(lblEmail, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblEmail);

            txtEmail = campo();
            AbsoluteLayout.SetLayoutBounds(txtEmail, new Rect(32, startY + 18, 436, 40));
            AbsoluteLayout.SetLayoutFlags(txtEmail, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtEmail);

            startY += fieldGap;

            // Dirección
            var lblDireccion = lbl("Dirección / Matriz");
            AbsoluteLayout.SetLayoutBounds(lblDireccion, new Rect(32, startY, 436, 14));
            AbsoluteLayout.SetLayoutFlags(lblDireccion, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblDireccion);

            txtDireccion = campo();
            AbsoluteLayout.SetLayoutBounds(txtDireccion, new Rect(32, startY + 18, 436, 40));
            AbsoluteLayout.SetLayoutFlags(txtDireccion, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtDireccion);

            // Botones
            var btns = new HorizontalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.End
            };

            if (proveedor != null)
            {
                var btnElim = Estilos.botonPeligro("Eliminar");
                btnElim.HeightRequest = 42;
                btnElim.WidthRequest = 110;
                btnElim.Clicked += async (s, e) =>
                {
                    ctrl.eliminarProveedor(proveedor.getId());
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

            AbsoluteLayout.SetLayoutBounds(btns, new Rect(32, 450, 436, 52));
            AbsoluteLayout.SetLayoutFlags(btns, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btns);

            Content = new Grid
            {
                Children = { absoluteLayout },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private void cargar(Proveedor p)
        {
            txtNombre.Text = p.getNombre();
            txtContacto.Text = p.getContacto();
            txtTelefono.Text = p.getTelefono();
            txtEmail.Text = p.getEmail();
            txtDireccion.Text = p.getDireccion();
        }

        private async void guardar()
        {
            string nombre = txtNombre.Text?.Trim().ToUpper() ?? "";
            if (string.IsNullOrEmpty(nombre))
            {
                await DisplayAlert("Error", "El nombre es obligatorio.", "OK");
                return;
            }

            int id = proveedor != null ? proveedor.getId() : ctrl.generarIdProveedor();
            Proveedor p = new Proveedor(
                id, 
                nombre, 
                txtContacto.Text?.Trim() ?? "", 
                txtTelefono.Text?.Trim() ?? "",
                txtEmail.Text?.Trim() ?? "", 
                txtDireccion.Text?.Trim() ?? "", 
                true
            );

            ctrl.guardarProveedor(p);
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
