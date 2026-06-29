using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.Entities;

public class Venta
{
    public int VentaId { get; set; }

    public DateTime FechaVenta { get; set; }

    [Required]
    public decimal Total { get; set; }

    [Required, MaxLength(50)]
    public string MetodoPago { get; set; } = null!;

    [Required, MaxLength(30)]
    public string Estado { get; set; } = null!;

    public int ClienteId { get; set; }
    public int EmpleadoId { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public Empleado Empleado { get; set; } = null!;
    public ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
}
