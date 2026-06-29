using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido")]
    [MaxLength(100)]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = "El telefono es requerido")]
    [RegularExpression(@"^\+591\d{8}$", ErrorMessage = "Debe ingresar +591 seguido de 8 digitos")]
    [Display(Name = "Telefono")]
    public string Telefono { get; set; } = "+591";

    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "Formato de correo invalido")]
    [MaxLength(150)]
    [Display(Name = "Correo electronico")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La direccion es requerida")]
    [Display(Name = "Direccion")]
    public string Direccion { get; set; } = null!;

    [Required(ErrorMessage = "La contrasena es requerida")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "La contrasena debe tener al menos 6 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Contrasena")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirma tu contrasena")]
    [Compare("Password", ErrorMessage = "Las contrasenas no coinciden")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contrasena")]
    public string ConfirmPassword { get; set; } = null!;
}
