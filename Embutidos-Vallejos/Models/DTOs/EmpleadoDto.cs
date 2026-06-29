using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.DTOs;

public class EmpleadoDto
{
    public int EmpleadoId { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string? Telefono { get; set; }
    public string Email { get; set; } = null!;
    public DateTime FechaContratacion { get; set; }
    public string Estado { get; set; } = null!;
    public string NombreRol { get; set; } = null!;
    public string? PlacaVehiculo { get; set; }
}

public class EmpleadoCreateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(100)]
    public string Apellido { get; set; } = null!;

    [MaxLength(20)]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contrasena es requerida")]
    [StringLength(255, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    public DateTime FechaContratacion { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "El estado es requerido")]
    [MaxLength(30)]
    public string Estado { get; set; } = "Activo";

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol")]
    public int RolId { get; set; }

    [MaxLength(20)]
    public string? PlacaVehiculo { get; set; }
}

public class EmpleadoUpdateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(100)]
    public string Apellido { get; set; } = null!;

    [MaxLength(20)]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = null!;

    [Required]
    public DateTime FechaContratacion { get; set; }

    [Required(ErrorMessage = "El estado es requerido")]
    [MaxLength(30)]
    public string Estado { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol")]
    public int RolId { get; set; }

    [MaxLength(20)]
    public string? PlacaVehiculo { get; set; }
}
