using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using punto_de_venta_C_.Modelo;

namespace punto_de_venta_C_.Util
{
    public class CreadorDocumentos
    {
        public static string crearDocumentoVentas(List<Venta> ventas, ConfiguracionTienda configuracion, string fechaInicio, string fechaFin)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            string rutaArchivo = Path.Combine(Path.GetTempPath(), "ReporteVentas_" + Guid.NewGuid().ToString() + ".pdf");

            double ingresosTotales = ventas.Sum(v => v.getTotal());
            int numeroVentas = ventas.Count;
            double promedioVenta = numeroVentas > 0 ? ingresosTotales / numeroVentas : 0;

            Document.Create(documento =>
            {
                documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.Letter.Landscape());
                    pagina.Margin(1, Unit.Centimetre);
                    pagina.PageColor(QuestPDF.Helpers.Colors.White);
                    pagina.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    pagina.Header().Column(col =>
                    {
                        col.Item().Text(configuracion.getNombre()).FontSize(18).SemiBold().FontColor(QuestPDF.Helpers.Colors.Blue.Darken2);
                        col.Item().Text($"Sucursal: {configuracion.getSucursal()}");
                        col.Item().Text($"RFC: {configuracion.getRfc()}");
                        col.Item().Text($"Periodo: {fechaInicio} al {fechaFin}");
                        col.Item().PaddingTop(10).Text($"Ventas totales: {numeroVentas}");
                        col.Item().Text($"Ingresos totales: {ingresosTotales:C}");
                        col.Item().Text($"Promedio por venta: {promedioVenta:C}");
                        col.Item().PaddingBottom(10);
                    });

                    pagina.Content().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(40);
                            cols.ConstantColumn(50);
                            cols.ConstantColumn(40);
                            cols.ConstantColumn(70);
                            cols.ConstantColumn(60);
                            cols.ConstantColumn(50);
                            cols.ConstantColumn(50);
                            cols.RelativeColumn();
                            cols.ConstantColumn(40);
                            cols.ConstantColumn(50);
                            cols.ConstantColumn(50);
                        });

                        tabla.Header(cabecera =>
                        {
                            cabecera.Cell().Element(EstiloCelda).Text("ID Venta");
                            cabecera.Cell().Element(EstiloCelda).Text("Fecha");
                            cabecera.Cell().Element(EstiloCelda).Text("Hora");
                            cabecera.Cell().Element(EstiloCelda).Text("Cajero");
                            cabecera.Cell().Element(EstiloCelda).Text("Metodo");
                            cabecera.Cell().Element(EstiloCelda).Text("Total");
                            cabecera.Cell().Element(EstiloCelda).Text("SKU");
                            cabecera.Cell().Element(EstiloCelda).Text("Producto");
                            cabecera.Cell().Element(EstiloCelda).Text("Cant");
                            cabecera.Cell().Element(EstiloCelda).Text("Precio");
                            cabecera.Cell().Element(EstiloCelda).Text("Subtotal");

                            static QuestPDF.Infrastructure.IContainer EstiloCelda(QuestPDF.Infrastructure.IContainer contenedor)
                            {
                                return contenedor.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Black);
                            }
                        });

                        foreach (var v in ventas)
                        {
                            string fecha = v.getFecha().ToString("dd/MM/yyyy");
                            string hora = v.getFecha().ToString("HH:mm:ss");

                            if (v.getItems() == null || v.getItems().Count == 0)
                            {
                                tabla.Cell().Element(EstiloCuerpo).Text(v.getId().ToString());
                                tabla.Cell().Element(EstiloCuerpo).Text(fecha);
                                tabla.Cell().Element(EstiloCuerpo).Text(hora);
                                tabla.Cell().Element(EstiloCuerpo).Text(v.getCajero());
                                tabla.Cell().Element(EstiloCuerpo).Text(v.getMetodoPago());
                                tabla.Cell().Element(EstiloCuerpo).Text(v.getTotal().ToString("C"));
                                tabla.Cell().Element(EstiloCuerpo).Text("");
                                tabla.Cell().Element(EstiloCuerpo).Text("Sin detalles");
                                tabla.Cell().Element(EstiloCuerpo).Text("");
                                tabla.Cell().Element(EstiloCuerpo).Text("");
                                tabla.Cell().Element(EstiloCuerpo).Text("");
                            }
                            else
                            {
                                foreach (var item in v.getItems())
                                {
                                    tabla.Cell().Element(EstiloCuerpo).Text(v.getId().ToString());
                                    tabla.Cell().Element(EstiloCuerpo).Text(fecha);
                                    tabla.Cell().Element(EstiloCuerpo).Text(hora);
                                    tabla.Cell().Element(EstiloCuerpo).Text(v.getCajero());
                                    tabla.Cell().Element(EstiloCuerpo).Text(v.getMetodoPago());
                                    tabla.Cell().Element(EstiloCuerpo).Text(v.getTotal().ToString("C"));
                                    tabla.Cell().Element(EstiloCuerpo).Text(item.getProducto().getId());
                                    tabla.Cell().Element(EstiloCuerpo).Text(item.getProducto().getNombre());
                                    tabla.Cell().Element(EstiloCuerpo).Text(item.getCantidad().ToString("0.00"));
                                    tabla.Cell().Element(EstiloCuerpo).Text(item.getProducto().getPrecio().ToString("C"));
                                    tabla.Cell().Element(EstiloCuerpo).Text(item.getSubtotal().ToString("C"));
                                }
                            }
                        }

                        if (ventas.Count == 0)
                        {
                            tabla.Cell().ColumnSpan(11).PaddingVertical(10).AlignCenter().Text("Sin ventas en el periodo");
                        }

                        static QuestPDF.Infrastructure.IContainer EstiloCuerpo(QuestPDF.Infrastructure.IContainer contenedor)
                        {
                            return contenedor.BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(3);
                        }
                    });

                    pagina.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Pagina ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            })
            .GeneratePdf(rutaArchivo);

            return rutaArchivo;
        }
    }
}
