using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.Entities;

public class Empleado
{
    public int EmpleadoId { get; set; }

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

    [Required]
    public DateTime FechaContratacion { get; set; }

    [Required, MaxLength(30)]
    public string Estado { get; set; } = null!;

    public int RolId { get; set; }

    public Rol Rol { get; set; } = null!;
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    public Repartidor? Repartidor { get; set; }
}
