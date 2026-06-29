using Microsoft.EntityFrameworkCore;
using Embutidos_Vallejos.Models.Entities;

namespace Embutidos_Vallejos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Repartidor> Repartidores => Set<Repartidor>();
    public DbSet<CategoriaProducto> CategoriasProducto => Set<CategoriaProducto>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Entrega> Entregas => Set<Entrega>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Cliente");
            entity.HasKey(e => e.ClienteId);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Direccion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Rol");
            entity.HasKey(e => e.RolId);
            entity.Property(e => e.NombreRol).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.ToTable("Empleado");
            entity.HasKey(e => e.EmpleadoId);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FechaContratacion).IsRequired();
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(30);

            entity.HasOne(e => e.Rol)
                  .WithMany(r => r.Empleados)
                  .HasForeignKey(e => e.RolId);
        });

        modelBuilder.Entity<Repartidor>(entity =>
        {
            entity.ToTable("Repartidor");
            entity.HasKey(e => e.RepartidorId);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.PlacaVehiculo).HasMaxLength(20);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(30);

            entity.HasOne(e => e.Empleado)
                  .WithOne(emp => emp.Repartidor)
                  .HasForeignKey<Repartidor>(e => e.EmpleadoId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<CategoriaProducto>(entity =>
        {
            entity.ToTable("CategoriaProducto");
            entity.HasKey(e => e.CategoriaId);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Producto");
            entity.HasKey(e => e.ProductoId);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
            entity.Property(e => e.PrecioProduccion).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.PrecioVenta).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Imagen).HasMaxLength(255);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(30);

            entity.HasOne(e => e.Categoria)
                  .WithMany(c => c.Productos)
                  .HasForeignKey(e => e.CategoriaId);
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("Pedido");
            entity.HasKey(e => e.PedidoId);
            entity.Property(e => e.FechaPedido).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.Total).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(30);
            entity.Property(e => e.DireccionEntrega).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Cliente)
                  .WithMany(c => c.Pedidos)
                  .HasForeignKey(e => e.ClienteId);

            entity.HasOne(e => e.Repartidor)
                  .WithMany(r => r.Pedidos)
                  .HasForeignKey(e => e.RepartidorId)
                  .IsRequired(false);
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.ToTable("DetallePedido");
            entity.HasKey(e => e.DetalleId);
            entity.Property(e => e.PrecioUnitario).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Subtotal).IsRequired().HasColumnType("decimal(10,2)");

            entity.HasOne(e => e.Pedido)
                  .WithMany(p => p.DetallesPedido)
                  .HasForeignKey(e => e.PedidoId);

            entity.HasOne(e => e.Producto)
                  .WithMany(p => p.DetallesPedido)
                  .HasForeignKey(e => e.ProductoId);
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.ToTable("Pago");
            entity.HasKey(e => e.PagoId);
            entity.Property(e => e.Monto).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.TipoPago).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FechaPago).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.EstadoPago).IsRequired().HasMaxLength(30);
            entity.Property(e => e.CodigoTransaccion).HasMaxLength(100);
            entity.Property(e => e.ReferenciaPago).HasMaxLength(100);

            entity.HasIndex(e => e.PedidoId).IsUnique();

            entity.HasOne(e => e.Pedido)
                  .WithOne(p => p.Pago)
                  .HasForeignKey<Pago>(e => e.PedidoId);
        });

        modelBuilder.Entity<Entrega>(entity =>
        {
            entity.ToTable("Entrega");
            entity.HasKey(e => e.EntregaId);
            entity.Property(e => e.EstadoEntrega).IsRequired().HasMaxLength(30);
            entity.Property(e => e.Observaciones).HasColumnType("nvarchar(max)");

            entity.HasIndex(e => e.PedidoId).IsUnique();

            entity.HasOne(e => e.Pedido)
                  .WithOne(p => p.Entrega)
                  .HasForeignKey<Entrega>(e => e.PedidoId);

            entity.HasOne(e => e.Repartidor)
                  .WithMany(r => r.Entregas)
                  .HasForeignKey(e => e.RepartidorId);
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.ToTable("Venta");
            entity.HasKey(e => e.VentaId);
            entity.Property(e => e.FechaVenta).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.Total).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.MetodoPago).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(30);

            entity.HasOne(e => e.Cliente)
                  .WithMany(c => c.Ventas)
                  .HasForeignKey(e => e.ClienteId);

            entity.HasOne(e => e.Empleado)
                  .WithMany(emp => emp.Ventas)
                  .HasForeignKey(e => e.EmpleadoId);
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.ToTable("DetalleVenta");
            entity.HasKey(e => e.DetalleVentaId);
            entity.Property(e => e.PrecioUnitario).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Subtotal).IsRequired().HasColumnType("decimal(10,2)");

            entity.HasOne(e => e.Venta)
                  .WithMany(v => v.DetallesVenta)
                  .HasForeignKey(e => e.VentaId);

            entity.HasOne(e => e.Producto)
                  .WithMany(p => p.DetallesVenta)
                  .HasForeignKey(e => e.ProductoId);
        });
    }
}
