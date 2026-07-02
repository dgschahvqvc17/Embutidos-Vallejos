using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.Entities;

public class Rol
{
    public int RolId { get; set; }

    [Required, MaxLength(50)]
    public string NombreRol { get; set; } = null!;

    public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
