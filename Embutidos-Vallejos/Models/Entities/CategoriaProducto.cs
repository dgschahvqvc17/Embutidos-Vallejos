using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.Entities;

public class CategoriaProducto
{
    public int CategoriaId { get; set; }

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
