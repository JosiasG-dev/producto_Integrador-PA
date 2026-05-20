using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;
using punto_de_venta_C_.Util;

namespace punto_de_venta_C_.Vista
{
    public class ProductoDialog : ContentPage
    {
        private readonly InventarioControlador ctrl;
        private Producto producto;

        private Entry txtId, txtNombre, txtPrecio, txtStock, txtStockMin;
        private Picker cmbCategoria, cmbUnidad;
        private Button btnImagen;
        private string rutaImagen = "";
        private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public ProductoDialog(Producto p, InventarioControlador ctrl)
        {
            this.ctrl = ctrl;
            this.producto = p;
            Title = p == null ? "Nuevo Producto" : "Editar Producto";
            BackgroundColor = Estilos.BG_BLANCO;
            construir();
            if (p != null)
                cargar(p);
        }

        private void construir()
        {
            var absoluteLayout = new AbsoluteLayout
            {
                WidthRequest = 480,
                HeightRequest = 580,
                BackgroundColor = Estilos.BG_BLANCO
            };

            var tit = new Label
            {
                Text = producto == null ? "NUEVO PRODUCTO" : "EDITAR PRODUCTO",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            AbsoluteLayout.SetLayoutBounds(tit, new Rect(28, 18, 420, 30));
            AbsoluteLayout.SetLayoutFlags(tit, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(tit);

            int x = 28, aw = 424, fh = 36;

            // SKU
            var lblSku = lbl("SKU / Codigo");
            AbsoluteLayout.SetLayoutBounds(lblSku, new Rect(x, 60, 300, 14));
            AbsoluteLayout.SetLayoutFlags(lblSku, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblSku);

            txtId = campo();
            txtId.Text = producto == null ? ctrl.generarNuevoId() : "";
            txtId.IsEnabled = producto == null;
            AbsoluteLayout.SetLayoutBounds(txtId, new Rect(x, 76, aw, fh));
            AbsoluteLayout.SetLayoutFlags(txtId, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtId);

            // Nombre
            var lblNombre = lbl("Nombre del Producto");
            AbsoluteLayout.SetLayoutBounds(lblNombre, new Rect(x, 124, 300, 14));
            AbsoluteLayout.SetLayoutFlags(lblNombre, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblNombre);

            txtNombre = campo();
            AbsoluteLayout.SetLayoutBounds(txtNombre, new Rect(x, 140, aw, fh));
            AbsoluteLayout.SetLayoutFlags(txtNombre, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtNombre);

            // Precio
            var lblPrecio = lbl("Precio Publico ($)");
            AbsoluteLayout.SetLayoutBounds(lblPrecio, new Rect(x, 188, 200, 14));
            AbsoluteLayout.SetLayoutFlags(lblPrecio, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblPrecio);

            txtPrecio = campo();
            txtPrecio.Text = "0.00";
            AbsoluteLayout.SetLayoutBounds(txtPrecio, new Rect(x, 204, 200, fh));
            AbsoluteLayout.SetLayoutFlags(txtPrecio, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtPrecio);

            // Existencia
            var lblStock = lbl("Existencia Actual");
            AbsoluteLayout.SetLayoutBounds(lblStock, new Rect(x + 210, 188, 214, 14));
            AbsoluteLayout.SetLayoutFlags(lblStock, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblStock);

            txtStock = campo();
            txtStock.Text = "0";
            AbsoluteLayout.SetLayoutBounds(txtStock, new Rect(x + 210, 204, 214, fh));
            AbsoluteLayout.SetLayoutFlags(txtStock, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtStock);

            // Stock Minimo
            var lblStockMin = lbl("Stock Minimo (limite de alerta)");
            AbsoluteLayout.SetLayoutBounds(lblStockMin, new Rect(x, 252, 300, 14));
            AbsoluteLayout.SetLayoutFlags(lblStockMin, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblStockMin);

            txtStockMin = campo();
            txtStockMin.Text = "5";
            AbsoluteLayout.SetLayoutBounds(txtStockMin, new Rect(x, 268, 200, fh));
            AbsoluteLayout.SetLayoutFlags(txtStockMin, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(txtStockMin);

            // Categoria
            var lblCat = lbl("Categoria");
            AbsoluteLayout.SetLayoutBounds(lblCat, new Rect(x, 316, 200, 14));
            AbsoluteLayout.SetLayoutFlags(lblCat, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblCat);

            cmbCategoria = new Picker { FontAttributes = FontAttributes.Bold };
            foreach (var cat in DatosIniciales.getCategorias())
            {
                cmbCategoria.Items.Add(cat);
            }
            if (cmbCategoria.Items.Count > 0) cmbCategoria.SelectedIndex = 0;
            AbsoluteLayout.SetLayoutBounds(cmbCategoria, new Rect(x, 332, 200, fh));
            AbsoluteLayout.SetLayoutFlags(cmbCategoria, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(cmbCategoria);

            // Unidad
            var lblUni = lbl("Unidad de Medida");
            AbsoluteLayout.SetLayoutBounds(lblUni, new Rect(x + 210, 316, 214, 14));
            AbsoluteLayout.SetLayoutFlags(lblUni, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblUni);

            cmbUnidad = new Picker { FontAttributes = FontAttributes.Bold };
            foreach (var unit in DatosIniciales.getUnidades())
            {
                cmbUnidad.Items.Add(unit);
            }
            if (cmbUnidad.Items.Count > 0) cmbUnidad.SelectedIndex = 0;
            AbsoluteLayout.SetLayoutBounds(cmbUnidad, new Rect(x + 210, 332, 214, fh));
            AbsoluteLayout.SetLayoutFlags(cmbUnidad, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(cmbUnidad);

            // Imagen
            var lblImg = lbl("Imagen del Producto");
            AbsoluteLayout.SetLayoutBounds(lblImg, new Rect(x, 380, 300, 14));
            AbsoluteLayout.SetLayoutFlags(lblImg, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(lblImg);

            btnImagen = new Button
            {
                Text = "Seleccionar Imagen",
                FontSize = Estilos.FUENTE_XS_SIZE,
                BackgroundColor = Estilos.BG_ZINC_100,
                TextColor = Estilos.TEXTO_PRINCIPAL
            };
            btnImagen.Clicked += async (s, e) => await seleccionarImagen();
            AbsoluteLayout.SetLayoutBounds(btnImagen, new Rect(x, 396, 200, 38));
            AbsoluteLayout.SetLayoutFlags(btnImagen, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btnImagen);

            // Actions Panel
            var btns = new HorizontalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.End
            };

            if (producto != null)
            {
                var btnEliminar = Estilos.botonPeligro("Eliminar");
                btnEliminar.HeightRequest = 38;
                btnEliminar.WidthRequest = 100;
                btnEliminar.Clicked += async (s, e) =>
                {
                    bool ok = await DisplayAlert("Confirmar", "Eliminar producto " + producto.getNombre() + "?", "Sí", "No");
                    if (ok)
                    {
                        ctrl.eliminarProducto(producto.getId());
                        tcs.SetResult(true);
                        await Navigation.PopModalAsync();
                    }
                };
                btns.Add(btnEliminar);
            }

            var btnCancelar = Estilos.botonSecundario("Cancelar");
            btnCancelar.HeightRequest = 38;
            btnCancelar.WidthRequest = 100;
            btnCancelar.Clicked += async (s, e) =>
            {
                tcs.SetResult(false);
                await Navigation.PopModalAsync();
            };
            btns.Add(btnCancelar);

            var btnGuardar = Estilos.botonPrimario("Guardar");
            btnGuardar.HeightRequest = 38;
            btnGuardar.WidthRequest = 102;
            btnGuardar.Clicked += (s, e) => guardar();
            btns.Add(btnGuardar);

            AbsoluteLayout.SetLayoutBounds(btns, new Rect(28, 480, 424, 38));
            AbsoluteLayout.SetLayoutFlags(btns, AbsoluteLayoutFlags.None);
            absoluteLayout.Add(btns);

            Content = new Grid
            {
                Children = { absoluteLayout },
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private async Task seleccionarImagen()
        {
            try
            {
                var options = new PickOptions
                {
                    PickerTitle = "Seleccionar Imagen",
                    FileTypes = FilePickerFileType.Images
                };
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    string destDir = RutaBase.getImages();
                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }
                    string destPath = Path.Combine(destDir, result.FileName);
                    using (var sourceStream = await result.OpenReadAsync())
                    using (var destStream = File.Create(destPath))
                    {
                        await sourceStream.CopyToAsync(destStream);
                    }
                    this.rutaImagen = "Images/" + result.FileName;
                    btnImagen.Text = "✅ " + result.FileName;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al copiar la imagen: " + ex.Message, "OK");
            }
        }

        private void cargar(Producto p)
        {
            txtId.Text = p.getId();
            txtNombre.Text = p.getNombre();
            txtPrecio.Text = p.getPrecio().ToString("0.00");
            txtStock.Text = p.getStock().ToString("0.1");
            txtStockMin.Text = p.getStockMinimo().ToString("0");
            if (cmbCategoria.Items.Contains(p.getCategoria()))
                cmbCategoria.SelectedItem = p.getCategoria();
            if (cmbUnidad.Items.Contains(p.getUnidad()))
                cmbUnidad.SelectedItem = p.getUnidad();
            this.rutaImagen = p.getImagenRuta();
            if (!string.IsNullOrEmpty(rutaImagen))
            {
                btnImagen.Text = "✅ " + Path.GetFileName(rutaImagen);
            }
        }

        private async void guardar()
        {
            try
            {
                string id = txtId.Text?.Trim() ?? "";
                string nom = txtNombre.Text?.Trim() ?? "";
                
                double precio;
                if (!double.TryParse(txtPrecio.Text?.Trim() ?? "0", out precio))
                {
                    await DisplayAlert("Error", "Precio invalido", "OK");
                    return;
                }

                double stock;
                if (!double.TryParse(txtStock.Text?.Trim() ?? "0", out stock))
                {
                    await DisplayAlert("Error", "Existencia invalida", "OK");
                    return;
                }

                double stockMin;
                if (!double.TryParse(txtStockMin.Text?.Trim() ?? "0", out stockMin))
                {
                    await DisplayAlert("Error", "Stock minimo invalido", "OK");
                    return;
                }

                string cat = cmbCategoria.SelectedItem?.ToString() ?? "";
                string unidad = cmbUnidad.SelectedItem?.ToString() ?? "";

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(nom))
                {
                    await DisplayAlert("Error", "SKU y Nombre son obligatorios", "OK");
                    return;
                }

                Producto p = new Producto(id, nom, precio, stock, cat, unidad, rutaImagen);
                p.setStockMinimo(stockMin);
                ctrl.guardarProducto(p);
                tcs.SetResult(true);
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Datos numéricos inválidos: " + ex.Message, "OK");
            }
        }

        private Label lbl(string texto)
        {
            return new Label
            {
                Text = texto.ToUpper(),
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
