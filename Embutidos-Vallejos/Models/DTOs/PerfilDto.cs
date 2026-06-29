using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.DTOs;

public class PerfilDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string? Telefono { get; set; }
    public string Email { get; set; } = null!;
    public string? Direccion { get; set; }
    public string UserType { get; set; } = null!;
    public string? RolNombre { get; set; }
}

public class PerfilUpdateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(100)]
    public string Apellido { get; set; } = null!;

    [MaxLength(20)]
    public string? Telefono { get; set; }

    [MaxLength(500)]
    public string? Direccion { get; set; }
}

public class CambiarPasswordDto
{
    [Required(ErrorMessage = "Ingresa tu contrasena actual")]
    [DataType(DataType.Password)]
    [Display(Name = "Contrasena actual")]
    public string PasswordActual { get; set; } = null!;

    [Required(ErrorMessage = "Confirma tu contrasena actual")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contrasena actual")]
    public string PasswordActualConfirm { get; set; } = null!;

    [Required(ErrorMessage = "La nueva contrasena es requerida")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Minimo 6 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contrasena")]
    public string NuevaPassword { get; set; } = null!;

    [Required(ErrorMessage = "Confirma la nueva contrasena")]
    [Compare("NuevaPassword", ErrorMessage = "Las contrasenas no coinciden")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nueva contrasena")]
    public string ConfirmarNuevaPassword { get; set; } = null!;
}
