using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Embutidos_Vallejos.Models.Entities;

public class Repartidor
{
    public int RepartidorId { get; set; }

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Apellido { get; set; } = null!;

    [MaxLength(20)]
    public string? Telefono { get; set; }

    [MaxLength(20)]
    public string? PlacaVehiculo { get; set; }

    [Required, MaxLength(30)]
    public string Estado { get; set; } = null!;

    public int? EmpleadoId { get; set; }

    [ForeignKey("EmpleadoId")]
    public Empleado? Empleado { get; set; }

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();
}
