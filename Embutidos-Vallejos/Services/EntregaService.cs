using Embutidos_Vallejos.Data;
using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embutidos_Vallejos.Services;

public class EntregaService : IEntregaService
{
    private readonly AppDbContext _db;

    public EntregaService(AppDbContext db) => _db = db;

    public async Task<EntregaDto?> GetByPedidoIdAsync(int pedidoId)
    {
        return await _db.Entregas
            .Include(e => e.Repartidor)
            .Where(e => e.PedidoId == pedidoId)
            .Select(e => MapToDto(e))
            .FirstOrDefaultAsync();
    }

    public async Task<List<EntregaDto>> GetByRepartidorAsync(int repartidorId)
    {
        return await _db.Entregas
            .Include(e => e.Repartidor)
            .Where(e => e.RepartidorId == repartidorId)
            .OrderByDescending(e => e.FechaSalida)
            .Select(e => MapToDto(e))
            .ToListAsync();
    }

    public async Task<EntregaDto> CreateAsync(int pedidoId, int repartidorId)
    {
        var entrega = new Entrega
        {
            EstadoEntrega = "Pendiente",
            PedidoId = pedidoId,
            RepartidorId = repartidorId
        };

        _db.Entregas.Add(entrega);

        var pedido = await _db.Pedidos.FindAsync(pedidoId);
        if (pedido != null)
        {
            pedido.Estado = "En Camino";
            pedido.RepartidorId = repartidorId;
        }

        var repartidor = await _db.Repartidores.FindAsync(repartidorId);
        if (repartidor != null)
            repartidor.Estado = "Ocupado";

        await _db.SaveChangesAsync();

        return (await GetByPedidoIdAsync(pedidoId))!;
    }

    public async Task<bool> IniciarEntregaAsync(int entregaId)
    {
        var entrega = await _db.Entregas.FindAsync(entregaId);
        if (entrega == null) return false;

        entrega.FechaSalida = DateTime.Now;
        entrega.EstadoEntrega = "En Camino";

        var pedido = await _db.Pedidos.FindAsync(entrega.PedidoId);
        if (pedido != null) pedido.Estado = "En Camino";

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompletarEntregaAsync(int entregaId, string? observaciones)
    {
        var entrega = await _db.Entregas
            .Include(e => e.Repartidor)
            .FirstOrDefaultAsync(e => e.EntregaId == entregaId);

        if (entrega == null) return false;

        entrega.FechaEntrega = DateTime.Now;
        entrega.EstadoEntrega = "Entregado";
        entrega.Observaciones = observaciones;

        var pedido = await _db.Pedidos.FindAsync(entrega.PedidoId);
        if (pedido != null)
        {
            pedido.Estado = "Entregado";

            var pago = await _db.Pagos.FirstOrDefaultAsync(p => p.PedidoId == pedido.PedidoId);
            if (pago != null) pago.EstadoPago = "Completado";
        }

        if (entrega.Repartidor != null)
            entrega.Repartidor.Estado = "Disponible";

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<EntregaDto>> GetPendientesAsync()
    {
        return await _db.Entregas
            .Include(e => e.Repartidor)
            .Where(e => e.EstadoEntrega != "Entregado")
            .OrderBy(e => e.FechaSalida)
            .Select(e => MapToDto(e))
            .ToListAsync();
    }

    private static EntregaDto MapToDto(Entrega e) => new()
    {
        EntregaId = e.EntregaId,
        FechaSalida = e.FechaSalida,
        FechaEntrega = e.FechaEntrega,
        EstadoEntrega = e.EstadoEntrega,
        Observaciones = e.Observaciones,
        PedidoId = e.PedidoId,
        RepartidorId = e.RepartidorId,
        NombreRepartidor = $"{e.Repartidor.Nombre} {e.Repartidor.Apellido}",
        PlacaVehiculo = e.Repartidor.PlacaVehiculo
    };
}
