using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using punto_de_venta_C_.Modelo;
using punto_de_venta_C_.Controlador;

namespace punto_de_venta_C_.Vista
{
    public class UsuariosPanel : ContentView
    {
        private readonly UsuarioControlador ctrl;
        private VerticalStackLayout listUsuarios;

        public UsuariosPanel(UsuarioControlador ctrl)
        {
            this.ctrl = ctrl;
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
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 150 }
                }
            };

            var tit = new Label
            {
                Text = "RRHH  —  Equipo de Operación",
                FontSize = Estilos.FUENTE_TITULO_SIZE,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };
            header.Add(tit, 0, 0);

            var btnNuevo = Estilos.botonPrimario("+ Alta Personal");
            btnNuevo.HeightRequest = 42;
            btnNuevo.Clicked += (s, e) => abrirFormulario(null);
            header.Add(btnNuevo, 1, 0);

            rootLayout.Add(header, 0, 0);

            listUsuarios = new VerticalStackLayout();
            var scroll = new ScrollView
            {
                Content = listUsuarios,
                BackgroundColor = Estilos.BG_BLANCO
            };

            rootLayout.Add(scroll, 0, 2);

            Content = rootLayout;

            refrescar();
        }

        public void refrescar()
        {
            if (listUsuarios == null)
                return;

            listUsuarios.Children.Clear();

            var header = new Grid
            {
                BackgroundColor = Estilos.BG_ZINC_100,
                HeightRequest = 35,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 60 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 160 },
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = 70 },
                    new ColumnDefinition { Width = 100 }
                }
            };
            header.Add(new Label { Text = "ID", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
            header.Add(new Label { Text = "Nombre", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 1);
            header.Add(new Label { Text = "Rol", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 2);
            header.Add(new Label { Text = "Usuario Red", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 3);
            header.Add(new Label { Text = "Edad", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 4);
            header.Add(new Label { Text = "Sexo", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 5);
            listUsuarios.Children.Add(header);

            foreach (Usuario u in ctrl.getUsuarios())
            {
                var rowGrid = new Grid
                {
                    HeightRequest = 44,
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = 60 },
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = 160 },
                        new ColumnDefinition { Width = 150 },
                        new ColumnDefinition { Width = 70 },
                        new ColumnDefinition { Width = 100 }
                    }
                };

                rowGrid.Add(new Label { Text = u.getId().ToString(), VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) }, 0);
                rowGrid.Add(new Label { Text = u.getNombre(), VerticalOptions = LayoutOptions.Center }, 1);
                rowGrid.Add(new Label { Text = u.getRol().ToString(), VerticalOptions = LayoutOptions.Center }, 2);
                rowGrid.Add(new Label { Text = u.getUsername(), VerticalOptions = LayoutOptions.Center }, 3);
                rowGrid.Add(new Label { Text = u.getEdad().ToString(), VerticalOptions = LayoutOptions.Center }, 4);
                rowGrid.Add(new Label { Text = u.getSexo(), VerticalOptions = LayoutOptions.Center }, 5);

                var tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
                Usuario currentUser = u;
                tap.Tapped += (s, e) => abrirFormulario(currentUser);
                rowGrid.GestureRecognizers.Add(tap);

                listUsuarios.Children.Add(rowGrid);
            }
        }

        private async void abrirFormulario(Usuario u)
        {
            var dlg = new UsuarioDialog(u, ctrl);
            await dlg.mostrar();
            refrescar();
        }
    }
}
