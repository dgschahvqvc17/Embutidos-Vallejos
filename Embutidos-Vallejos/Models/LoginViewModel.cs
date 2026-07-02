using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "Formato de correo invalido")]
    [Display(Name = "Correo electronico")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contrasena es requerida")]
    [DataType(DataType.Password)]
    [Display(Name = "Contrasena")]
    public string Password { get; set; } = null!;

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }
}
