using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.DTOs;

public class CategoriaDto
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int CantidadProductos { get; set; }
}

public class CategoriaCreateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }
}

public class CategoriaUpdateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }
}
