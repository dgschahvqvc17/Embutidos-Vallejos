using Embutidos_Vallejos.Data;
using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embutidos_Vallejos.Services;

public class PedidoService : IPedidoService
{
    private readonly AppDbContext _db;

    public PedidoService(AppDbContext db) => _db = db;

    public async Task<List<PedidoDto>> GetAllAsync()
    {
        return await _db.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Repartidor)
            .Include(p => p.DetallesPedido).ThenInclude(d => d.Producto)
            .Include(p => p.Pago)
            .Include(p => p.Entrega)
            .OrderByDescending(p => p.FechaPedido)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<List<PedidoDto>> GetByClienteAsync(int clienteId)
    {
        return await _db.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Repartidor)
            .Include(p => p.DetallesPedido).ThenInclude(d => d.Producto)
            .Include(p => p.Pago)
            .Include(p => p.Entrega)
            .Where(p => p.ClienteId == clienteId)
            .OrderByDescending(p => p.FechaPedido)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<List<PedidoDto>> GetByEstadoAsync(string estado)
    {
        return await _db.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Repartidor)
            .Include(p => p.DetallesPedido).ThenInclude(d => d.Producto)
            .Include(p => p.Pago)
            .Include(p => p.Entrega)
            .Where(p => p.Estado == estado)
            .OrderByDescending(p => p.FechaPedido)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<PedidoDto?> GetByIdAsync(int id)
    {
        return await _db.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Repartidor)
            .Include(p => p.DetallesPedido).ThenInclude(d => d.Producto)
            .Include(p => p.Pago)
            .Include(p => p.Entrega)
            .Where(p => p.PedidoId == id)
            .Select(p => MapToDto(p))
            .FirstOrDefaultAsync();
    }

    public async Task<PedidoDto> CreateAsync(int clienteId, PedidoCreateDto dto)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var pedido = new Pedido
            {
                FechaPedido = DateTime.Now,
                Total = dto.Items.Sum(i => i.Cantidad * i.PrecioUnitario),
                Estado = "Pendiente",
                DireccionEntrega = dto.DireccionEntrega,
                ClienteId = clienteId
            };

            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync();

            foreach (var item in dto.Items)
            {
                var producto = await _db.Productos.FindAsync(item.ProductoId);
                if (producto == null || producto.Stock < item.Cantidad)
                    throw new InvalidOperationException($"Stock insuficiente para {producto?.Nombre ?? "producto desconocido"}");

                producto.Stock -= item.Cantidad;

                _db.DetallesPedido.Add(new DetallePedido
                {
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario,
                    Subtotal = item.Cantidad * item.PrecioUnitario,
                    PedidoId = pedido.PedidoId,
                    ProductoId = item.ProductoId
                });
            }

            await _db.SaveChangesAsync();

            var pago = new Pago
            {
                Monto = pedido.Total,
                TipoPago = dto.TipoPago,
                FechaPago = DateTime.Now,
                EstadoPago = dto.TipoPago == "Efectivo" ? "Pendiente" : "Completado",
                PedidoId = pedido.PedidoId
            };

            _db.Pagos.Add(pago);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return (await GetByIdAsync(pedido.PedidoId))!;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateEstadoAsync(int id, string estado)
    {
        var pedido = await _db.Pedidos.FindAsync(id);
        if (pedido == null) return false;

        pedido.Estado = estado;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AsignarRepartidorAsync(int pedidoId, int repartidorId)
    {
        var pedido = await _db.Pedidos.FindAsync(pedidoId);
        if (pedido == null) return false;

        pedido.RepartidorId = repartidorId;
        pedido.Estado = "En Preparacion";
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelarAsync(int id)
    {
        var pedido = await _db.Pedidos
            .Include(p => p.DetallesPedido)
            .FirstOrDefaultAsync(p => p.PedidoId == id);

        if (pedido == null || pedido.Estado == "Entregado" || pedido.Estado == "Cancelado")
            return false;

        foreach (var detalle in pedido.DetallesPedido)
        {
            var producto = await _db.Productos.FindAsync(detalle.ProductoId);
            if (producto != null)
                producto.Stock += detalle.Cantidad;
        }

        pedido.Estado = "Cancelado";
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<PedidoDto>> GetDisponiblesParaRepartoAsync()
    {
        return await _db.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Repartidor)
            .Include(p => p.DetallesPedido).ThenInclude(d => d.Producto)
            .Include(p => p.Pago)
            .Include(p => p.Entrega)
            .Where(p => p.RepartidorId == null && (p.Estado == "Pagado" || p.Estado == "En Preparacion"))
            .OrderByDescending(p => p.FechaPedido)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    private static PedidoDto MapToDto(Pedido p) => new()
    {
        PedidoId = p.PedidoId,
        FechaPedido = p.FechaPedido,
        Total = p.Total,
        Estado = p.Estado,
        DireccionEntrega = p.DireccionEntrega,
        ClienteId = p.ClienteId,
        NombreCliente = $"{p.Cliente.Nombre} {p.Cliente.Apellido}",
        RepartidorId = p.RepartidorId,
        NombreRepartidor = p.Repartidor != null ? $"{p.Repartidor.Nombre} {p.Repartidor.Apellido}" : null,
        Detalles = p.DetallesPedido.Select(d => new DetallePedidoDto
        {
            DetalleId = d.DetalleId,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario,
            Subtotal = d.Subtotal,
            ProductoId = d.ProductoId,
            NombreProducto = d.Producto.Nombre,
            ImagenProducto = d.Producto.Imagen
        }).ToList(),
        Pago = p.Pago != null ? new PagoDto
        {
            PagoId = p.Pago.PagoId,
            Monto = p.Pago.Monto,
            TipoPago = p.Pago.TipoPago,
            FechaPago = p.Pago.FechaPago,
            EstadoPago = p.Pago.EstadoPago,
            CodigoTransaccion = p.Pago.CodigoTransaccion,
            ReferenciaPago = p.Pago.ReferenciaPago,
            PedidoId = p.Pago.PedidoId
        } : null,
        Entrega = p.Entrega != null ? new EntregaDto
        {
            EntregaId = p.Entrega.EntregaId,
            FechaSalida = p.Entrega.FechaSalida,
            FechaEntrega = p.Entrega.FechaEntrega,
            EstadoEntrega = p.Entrega.EstadoEntrega,
            Observaciones = p.Entrega.Observaciones,
            PedidoId = p.Entrega.PedidoId,
            RepartidorId = p.Entrega.RepartidorId,
            NombreRepartidor = p.Repartidor != null ? $"{p.Repartidor.Nombre} {p.Repartidor.Apellido}" : null,
            PlacaVehiculo = p.Repartidor?.PlacaVehiculo
        } : null
    };
}
