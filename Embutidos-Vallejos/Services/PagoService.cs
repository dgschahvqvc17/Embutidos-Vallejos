using Embutidos_Vallejos.Data;
using Embutidos_Vallejos.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Embutidos_Vallejos.Services;

public class PagoService : IPagoService
{
    private readonly AppDbContext _db;
    private readonly IQRService _qrService;

    public PagoService(AppDbContext db, IQRService qrService)
    {
        _db = db;
        _qrService = qrService;
    }

    public async Task<PagoDto?> GetByPedidoIdAsync(int pedidoId)
    {
        return await _db.Pagos
            .Where(p => p.PedidoId == pedidoId)
            .Select(p => new PagoDto
            {
                PagoId = p.PagoId,
                Monto = p.Monto,
                TipoPago = p.TipoPago,
                FechaPago = p.FechaPago,
                EstadoPago = p.EstadoPago,
                CodigoTransaccion = p.CodigoTransaccion,
                ReferenciaPago = p.ReferenciaPago,
                PedidoId = p.PedidoId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<PagoDto>> GetAllAsync()
    {
        return await _db.Pagos
            .OrderByDescending(p => p.FechaPago)
            .Select(p => new PagoDto
            {
                PagoId = p.PagoId,
                Monto = p.Monto,
                TipoPago = p.TipoPago,
                FechaPago = p.FechaPago,
                EstadoPago = p.EstadoPago,
                CodigoTransaccion = p.CodigoTransaccion,
                ReferenciaPago = p.ReferenciaPago,
                PedidoId = p.PedidoId
            })
            .ToListAsync();
    }

    public async Task<PagoDto> CreateAsync(PagoCreateDto dto)
    {
        var pago = new Models.Entities.Pago
        {
            Monto = dto.Monto,
            TipoPago = dto.TipoPago,
            FechaPago = DateTime.Now,
            EstadoPago = dto.TipoPago == "Efectivo" ? "Pendiente" : "Completado",
            CodigoTransaccion = dto.CodigoTransaccion,
            ReferenciaPago = dto.ReferenciaPago,
            PedidoId = dto.PedidoId
        };

        _db.Pagos.Add(pago);
        await _db.SaveChangesAsync();

        return (await GetByPedidoIdAsync(dto.PedidoId))!;
    }

    public async Task<bool> ConfirmarPagoAsync(int pagoId, string codigoTransaccion)
    {
        var pago = await _db.Pagos.FindAsync(pagoId);
        if (pago == null) return false;

        pago.EstadoPago = "Completado";
        pago.CodigoTransaccion = codigoTransaccion;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<PagoReceiptDto?> GetReceiptAsync(int pedidoId)
    {
        return await _db.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.DetallesPedido).ThenInclude(d => d.Producto)
            .Include(p => p.Pago)
            .Where(p => p.PedidoId == pedidoId)
            .Select(p => new PagoReceiptDto
            {
                PedidoId = p.PedidoId,
                FechaPedido = p.FechaPedido,
                FechaPago = p.Pago!.FechaPago,
                Monto = p.Pago.Monto,
                TipoPago = p.Pago.TipoPago,
                EstadoPago = p.Pago.EstadoPago,
                CodigoTransaccion = p.Pago.CodigoTransaccion,
                ReferenciaPago = p.Pago.ReferenciaPago,
                NombreCliente = $"{p.Cliente.Nombre} {p.Cliente.Apellido}",
                DireccionEntrega = p.DireccionEntrega,
                Detalles = p.DetallesPedido.Select(d => new DetallePedidoDto
                {
                    DetalleId = d.DetalleId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre
                }).ToList(),
                CodigoQR = _qrService.GenerarQRPago(p.PedidoId, p.Pago.Monto, p.Pago.ReferenciaPago ?? $"PED-{p.PedidoId}")
            })
            .FirstOrDefaultAsync();
    }
}
