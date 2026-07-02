using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.DTOs;

public class VentaDto
{
    public int VentaId { get; set; }
    public DateTime FechaVenta { get; set; }
    public decimal Total { get; set; }
    public string MetodoPago { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public int ClienteId { get; set; }
    public string? NombreCliente { get; set; }
    public int EmpleadoId { get; set; }
    public string? NombreEmpleado { get; set; }
    public List<DetalleVentaDto> Detalles { get; set; } = new();
}

public class DetalleVentaDto
{
    public int DetalleVentaId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = null!;
    public string? ImagenProducto { get; set; }
}

public class VentaCreateDto
{
    [Required]
    public decimal Total { get; set; }

    [Required(ErrorMessage = "El metodo de pago es requerido")]
    [MaxLength(50)]
    public string MetodoPago { get; set; } = null!;

    [Required]
    public int ClienteId { get; set; }

    [Required]
    public int EmpleadoId { get; set; }

    public List<VentaDetalleCreateDto> Detalles { get; set; } = new();
}

public class VentaDetalleCreateDto
{
    [Required]
    public int Cantidad { get; set; }

    [Required]
    public decimal PrecioUnitario { get; set; }

    [Required]
    public int ProductoId { get; set; }
}
