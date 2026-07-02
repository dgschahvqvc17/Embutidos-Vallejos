using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.Entities;

public class Cliente
{
    public int ClienteId { get; set; }

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Apellido { get; set; } = null!;

    [MaxLength(20)]
    public string? Telefono { get; set; }

    [Required, MaxLength(150)]
    public string Email { get; set; } = null!;

    [Required, MaxLength(255)]
    public string Password { get; set; } = null!;

    public string? Direccion { get; set; }

    public DateTime FechaRegistro { get; set; }

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
