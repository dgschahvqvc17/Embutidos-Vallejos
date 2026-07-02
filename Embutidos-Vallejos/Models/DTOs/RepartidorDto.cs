using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.DTOs;

public class RepartidorDto
{
    public int RepartidorId { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? PlacaVehiculo { get; set; }
    public string Estado { get; set; } = null!;
    public string NombreCompleto => $"{Nombre} {Apellido}";
    public int PedidosAsignados { get; set; }
    public string? Email { get; set; }
}

public class RepartidorCreateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(100)]
    public string Apellido { get; set; } = null!;

    [MaxLength(20)]
    public string? Telefono { get; set; }

    [MaxLength(20)]
    public string? PlacaVehiculo { get; set; }

    [Required(ErrorMessage = "El estado es requerido")]
    [MaxLength(30)]
    public string Estado { get; set; } = "Disponible";

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contrasena es requerida")]
    [StringLength(255, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}

public class RepartidorUpdateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(100)]
    public string Apellido { get; set; } = null!;

    [MaxLength(20)]
    public string? Telefono { get; set; }

    [MaxLength(20)]
    public string? PlacaVehiculo { get; set; }

    [Required(ErrorMessage = "El estado es requerido")]
    [MaxLength(30)]
    public string Estado { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = null!;
}
