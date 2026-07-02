using Embutidos_Vallejos.Models.DTOs;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Embutidos_Vallejos.Services;

public class ReporteExportacionService : IReporteExportacionService
{
    public ReporteExportacionService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> ExportarVentasPdfAsync(ReporteVentasDto reporte)
    {
        return await Task.FromResult(Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposeHeader(c, "Reporte de Ventas"));
                page.Content().Element(c => ComposeVentasContent(c, reporte));
                page.Footer().Element(ComposeFooter);
            });
        }).GeneratePdf());
    }

    public async Task<byte[]> ExportarInventarioPdfAsync(ReporteInventarioDto reporte)
    {
        return await Task.FromResult(Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposeHeader(c, "Reporte de Inventario"));
                page.Content().Element(c => ComposeInventarioContent(c, reporte));
                page.Footer().Element(ComposeFooter);
            });
        }).GeneratePdf());
    }

    public async Task<byte[]> ExportarVentasExcelAsync(ReporteVentasDto reporte)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("Ventas");

        ws.Cell(1, 1).Value = "Reporte de Ventas";
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Range(1, 1, 1, 5).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        ws.Cell(3, 1).Value = "Total Ventas";
        ws.Cell(3, 2).Value = reporte.TotalVentas;
        ws.Cell(4, 1).Value = "Monto Total";
        ws.Cell(4, 2).Value = reporte.MontoTotal;
        ws.Cell(5, 1).Value = "Total Pedidos";
        ws.Cell(5, 2).Value = reporte.TotalPedidos;
        ws.Cell(6, 1).Value = "Ingreso Pedidos";
        ws.Cell(6, 2).Value = reporte.IngresoPedidos;
        ws.Cell(7, 1).Value = "Promedio Venta";
        ws.Cell(7, 2).Value = reporte.PromedioVenta;

        if (reporte.VentasRecientes.Any())
        {
            var headerRow = 9;
            ws.Cell(headerRow, 1).Value = "ID";
            ws.Cell(headerRow, 2).Value = "Fecha";
            ws.Cell(headerRow, 3).Value = "Cliente";
            ws.Cell(headerRow, 4).Value = "Total";
            ws.Cell(headerRow, 5).Value = "Metodo Pago";
            ws.Range(headerRow, 1, headerRow, 5).Style.Font.Bold = true;
            ws.Range(headerRow, 1, headerRow, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(0xFF, 0x1A, 0x1D, 0x24);

            for (var i = 0; i < reporte.VentasRecientes.Count; i++)
            {
                var v = reporte.VentasRecientes[i];
                var row = headerRow + 1 + i;
                ws.Cell(row, 1).Value = v.VentaId;
                ws.Cell(row, 2).Value = v.Fecha.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(row, 3).Value = v.Cliente;
                ws.Cell(row, 4).Value = v.Total;
                ws.Cell(row, 5).Value = v.MetodoPago;
            }
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return await Task.FromResult(ms.ToArray());
    }

    public async Task<byte[]> ExportarInventarioExcelAsync(ReporteInventarioDto reporte)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("Inventario");

        ws.Cell(1, 1).Value = "Reporte de Inventario";
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Range(1, 1, 1, 5).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        ws.Cell(3, 1).Value = "Total Productos";
        ws.Cell(3, 2).Value = reporte.TotalProductos;
        ws.Cell(4, 1).Value = "Productos Activos";
        ws.Cell(4, 2).Value = reporte.ProductosActivos;
        ws.Cell(5, 1).Value = "Stock Bajo";
        ws.Cell(5, 2).Value = reporte.ProductosStockBajo;
        ws.Cell(6, 1).Value = "Sin Stock";
        ws.Cell(6, 2).Value = reporte.ProductosSinStock;
        ws.Cell(7, 1).Value = "Valor Inventario";
        ws.Cell(7, 2).Value = reporte.ValorInventario;

        if (reporte.ProductosStockBajoLista.Any())
        {
            var headerRow = 9;
            ws.Cell(headerRow, 1).Value = "ID";
            ws.Cell(headerRow, 2).Value = "Nombre";
            ws.Cell(headerRow, 3).Value = "Categoria";
            ws.Cell(headerRow, 4).Value = "Stock";
            ws.Cell(headerRow, 5).Value = "Stock Minimo";
            ws.Range(headerRow, 1, headerRow, 5).Style.Font.Bold = true;
            ws.Range(headerRow, 1, headerRow, 5).Style.Fill.BackgroundColor = XLColor.FromArgb(0xFF, 0x1A, 0x1D, 0x24);

            for (var i = 0; i < reporte.ProductosStockBajoLista.Count; i++)
            {
                var p = reporte.ProductosStockBajoLista[i];
                var row = headerRow + 1 + i;
                ws.Cell(row, 1).Value = p.ProductoId;
                ws.Cell(row, 2).Value = p.Nombre;
                ws.Cell(row, 3).Value = p.NombreCategoria;
                ws.Cell(row, 4).Value = p.Stock;
                ws.Cell(row, 5).Value = p.StockMinimo;
            }
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return await Task.FromResult(ms.ToArray());
    }

    public async Task<byte[]> ExportarReporteCompletoPdfAsync(ReporteVentasDto ventas, ReporteInventarioDto inventario)
    {
        return await Task.FromResult(Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposeHeader(c, "Reporte General"));
                page.Content().Element(c => ComposeGeneralContent(c, ventas, inventario));
                page.Footer().Element(ComposeFooter);
            });
        }).GeneratePdf());
    }

    public async Task<byte[]> ExportarReporteCompletoExcelAsync(ReporteVentasDto ventas, ReporteInventarioDto inventario)
    {
        using var workbook = new XLWorkbook();

        var wsVentas = workbook.AddWorksheet("Ventas");
        wsVentas.Cell(1, 1).Value = "Reporte de Ventas";
        wsVentas.Cell(1, 1).Style.Font.Bold = true;
        wsVentas.Cell(1, 1).Style.Font.FontSize = 14;
        wsVentas.Range(1, 1, 1, 5).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        wsVentas.Cell(3, 1).Value = "Total Ventas";
        wsVentas.Cell(3, 2).Value = ventas.TotalVentas;
        wsVentas.Cell(4, 1).Value = "Monto Total";
        wsVentas.Cell(4, 2).Value = ventas.MontoTotal;
        wsVentas.Cell(5, 1).Value = "Total Pedidos";
        wsVentas.Cell(5, 2).Value = ventas.TotalPedidos;
        wsVentas.Cell(6, 1).Value = "Ingreso Pedidos";
        wsVentas.Cell(6, 2).Value = ventas.IngresoPedidos;
        wsVentas.Cell(7, 1).Value = "Promedio Venta";
        wsVentas.Cell(7, 2).Value = ventas.PromedioVenta;
        wsVentas.Columns().AdjustToContents();

        var wsInventario = workbook.AddWorksheet("Inventario");
        wsInventario.Cell(1, 1).Value = "Reporte de Inventario";
        wsInventario.Cell(1, 1).Style.Font.Bold = true;
        wsInventario.Cell(1, 1).Style.Font.FontSize = 14;
        wsInventario.Range(1, 1, 1, 5).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        wsInventario.Cell(3, 1).Value = "Total Productos";
        wsInventario.Cell(3, 2).Value = inventario.TotalProductos;
        wsInventario.Cell(4, 1).Value = "Productos Activos";
        wsInventario.Cell(4, 2).Value = inventario.ProductosActivos;
        wsInventario.Cell(5, 1).Value = "Stock Bajo";
        wsInventario.Cell(5, 2).Value = inventario.ProductosStockBajo;
        wsInventario.Cell(6, 1).Value = "Sin Stock";
        wsInventario.Cell(6, 2).Value = inventario.ProductosSinStock;
        wsInventario.Cell(7, 1).Value = "Valor Inventario";
        wsInventario.Cell(7, 2).Value = inventario.ValorInventario;
        wsInventario.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return await Task.FromResult(ms.ToArray());
    }

    private static void ComposeHeader(IContainer container, string title)
    {
        container.Row(row =>
        {
            row.RelativeColumn().Column(col =>
            {
                col.Item().Text("Embutidos Vallejos").FontFamily("Arial").FontSize(16).Bold().FontColor(Colors.Black);
                col.Item().Text(title).FontFamily("Arial").FontSize(12).FontColor(Colors.Grey.Darken2);
            });
            row.ConstantItem(150).AlignRight().Column(col =>
            {
                col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken2);
            });
        });
    }

    private static void ComposeVentasContent(IContainer container, ReporteVentasDto reporte)
    {
        container.PaddingVertical(20).Column(col =>
        {
            col.Spacing(10);

            col.Item().Text("Resumen").FontSize(14).Bold().FontColor(Colors.Black);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Element(CellStyleMetric).Text("Total Ventas").FontColor(Colors.Grey.Darken2);
                table.Cell().Element(CellStyleMetric).AlignRight().Text("Monto Total").FontColor(Colors.Grey.Darken2);

                table.Cell().Element(CellStyleMetric).Text("Total Pedidos").FontColor(Colors.Grey.Darken2);
                table.Cell().Element(CellStyleMetric).AlignRight().Text("Ingreso Pedidos").FontColor(Colors.Grey.Darken2);

                table.Cell().Element(CellStyleMetric).Text("Promedio Venta").FontColor(Colors.Grey.Darken2);
                table.Cell().Element(CellStyleMetric);
            });

            col.Item().PaddingTop(20).Text("Ventas Recientes").FontSize(14).Bold().FontColor(Colors.Black);

            if (reporte.VentasRecientes.Any())
            {
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("ID");
                        header.Cell().Element(CellStyle).Text("Fecha");
                        header.Cell().Element(CellStyle).Text("Cliente");
                        header.Cell().Element(CellStyle).Text("Total");
                        header.Cell().Element(CellStyle).Text("Metodo Pago");
                    });

                    foreach (var v in reporte.VentasRecientes)
                    {
                        table.Cell().Element(CellStyle).Text(v.VentaId.ToString());
                        table.Cell().Element(CellStyle).Text(v.Fecha.ToString("dd/MM/yyyy HH:mm"));
                        table.Cell().Element(CellStyle).Text(v.Cliente);
                        table.Cell().Element(CellStyle).Text(v.Total.ToString("C"));
                        table.Cell().Element(CellStyle).Text(v.MetodoPago);
                    }
                });
            }
        });
    }

    private static void ComposeInventarioContent(IContainer container, ReporteInventarioDto reporte)
    {
        container.PaddingVertical(20).Column(col =>
        {
            col.Spacing(10);

            col.Item().Text("Resumen").FontSize(14).Bold().FontColor(Colors.Black);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Element(CellStyleMetric).Text("Total Productos").FontColor(Colors.Grey.Darken2);
                table.Cell().Element(CellStyleMetric).AlignRight().Text("Productos Activos").FontColor(Colors.Grey.Darken2);

                table.Cell().Element(CellStyleMetric).Text("Stock Bajo").FontColor(Colors.Grey.Darken2);
                table.Cell().Element(CellStyleMetric).AlignRight().Text("Sin Stock").FontColor(Colors.Grey.Darken2);

                table.Cell().Element(CellStyleMetric).Text("Valor Inventario").FontColor(Colors.Grey.Darken2);
                table.Cell().Element(CellStyleMetric);
            });

            col.Item().PaddingTop(20).Text("Productos con Stock Bajo").FontSize(14).Bold().FontColor(Colors.Black);

            if (reporte.ProductosStockBajoLista.Any())
            {
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("ID");
                        header.Cell().Element(CellStyle).Text("Nombre");
                        header.Cell().Element(CellStyle).Text("Categoria");
                        header.Cell().Element(CellStyle).Text("Stock");
                        header.Cell().Element(CellStyle).Text("Stock Minimo");
                    });

                    foreach (var p in reporte.ProductosStockBajoLista)
                    {
                        table.Cell().Element(CellStyle).Text(p.ProductoId.ToString());
                        table.Cell().Element(CellStyle).Text(p.Nombre);
                        table.Cell().Element(CellStyle).Text(p.NombreCategoria);
                        table.Cell().Element(CellStyle).Text(p.Stock.ToString());
                        table.Cell().Element(CellStyle).Text(p.StockMinimo.ToString());
                    }
                });
            }
        });
    }

    private static void ComposeGeneralContent(IContainer container, ReporteVentasDto ventas, ReporteInventarioDto inventario)
    {
        container.PaddingVertical(20).Column(col =>
        {
            col.Spacing(20);

            col.Item().Text("Ventas").FontSize(14).Bold().FontColor(Colors.Black);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Element(CellStyleMetric).Text("Total Ventas").FontColor(Colors.Grey.Darken2);
                table.Cell().Element(CellStyleMetric).AlignRight().Text("Monto Total").FontColor(Colors.Grey.Darken2);

                table.Cell().Element(CellStyleMetric).Text("Total Pedidos").FontColor(Colors.Grey.Darken2);
                table.Cell().Element(CellStyleMetric).AlignRight().Text("Ingreso Pedidos").FontColor(Colors.Grey.Darken2);

                table.Cell().Element(CellStyleMetric).Text("Promedio Venta").FontColor(Colors.Grey.Darken2);
                table.Cell().Element(CellStyleMetric);
            });

            col.Item().Text("Inventario").FontSize(14).Bold().FontColor(Colors.Black);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Element(CellStyleMetric).Text("Total Productos").FontColor(Colors.Grey.Darken2);
                table.Cell().Element(CellStyleMetric).AlignRight().Text("Productos Activos").FontColor(Colors.Grey.Darken2);

                table.Cell().Element(CellStyleMetric).Text("Stock Bajo").FontColor(Colors.Grey.Darken2);
                table.Cell().Element(CellStyleMetric).AlignRight().Text("Sin Stock").FontColor(Colors.Grey.Darken2);

                table.Cell().Element(CellStyleMetric).Text("Valor Inventario").FontColor(Colors.Grey.Darken2);
                table.Cell().Element(CellStyleMetric);
            });
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container.AlignRight().Text(text =>
        {
            text.Span("Pagina ").FontColor(Colors.Grey.Darken2);
            text.CurrentPageNumber();
        });
    }

    private static IContainer CellStyle(IContainer container) => container.Padding(6).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

    private static IContainer CellStyleMetric(IContainer container) => container.Padding(8).Border(1).BorderColor(Colors.Grey.Lighten2);
}
