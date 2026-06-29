# Embutidos Vallejos

Sistema web de gestion para venta de embutidos con pedidos en linea,
pagos via PayPal, QR y efectivo.

---

## Requisitos

- .NET 8 SDK
- SQL Server (LocalDB, Express o superior)
- Visual Studio 2022 / VS Code

---

## 4 Pasos para ejecutar la app

### 1. Clonar y restaurar paquetes

```bash
git clone <repo-url>
cd Embutidos-Vallejos/Embutidos-Vallejos
dotnet restore
```

### 2. Configurar la cadena de conexion

Editar `appsettings.json` y ajustar la conexion a SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=EmbutidosVallejosDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

Para SQL Server LocalDB:
```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EmbutidosVallejosDB;Trusted_Connection=True;TrustServerCertificate=True"
```

### 3. Configurar PayPal (opcional)

En `appsettings.json`, reemplaza con tus credenciales de PayPal Sandbox:

```json
"PayPal": {
  "ClientId": "TU_CLIENT_ID",
  "Secret": "TU_SECRET",
  "Url": "https://api-m.sandbox.paypal.com"
}
```

### 4. Ejecutar la aplicacion

```bash
dotnet run
```

La base de datos se crea automaticamente al iniciar por primera vez
junto con los datos iniciales (roles, categorias y administrador).

---

## Credenciales iniciales

| Rol | Email | Contrasena |
|-----|-------|------------|
| Administrador | admin@vallejos.com | admin123 |

---

## Roles del sistema

| RolId | Nombre | Descripcion |
|-------|--------|-------------|
| 1 | Administrador | Gestion total del sistema |
| 2 | Produccion | Gestion de produccion |
| 3 | Ventas | Atencion de ventas presenciales |
| 4 | Almacen | Control de inventario |

---

## Estructura del proyecto (MVC)

```
Embutidos-Vallejos/
├── Controllers/        # Controladores
│   ├── AccountController.cs    # Login/Registro
│   ├── AdminController.cs      # Panel admin
│   ├── HomeController.cs       # Inicio
│   ├── ProductoController.cs   # Catalogo
│   ├── CarritoController.cs    # Carrito (Session)
│   ├── PedidoController.cs     # Pedidos
│   └── PagoController.cs       # Pagos (PayPal/QR/Efectivo)
├── Models/
│   ├── Entities/       # Entidades EF Core (12 tablas)
│   ├── DTOs/           # Objetos de transferencia
│   └── ViewModels/     # Modelos de vista
├── Services/
│   ├── I*Service.cs    # Interfaces de servicios
│   ├── *Service.cs     # Implementaciones
│   └── DbInitializer.cs # Seed data automatico
├── Data/
│   └── AppDbContext.cs # Contexto EF Core
├── Views/              # Vistas Razor
│   ├── Admin/          # Dashboard, CRUDs, Reportes
│   ├── Producto/       # Catalogo
│   ├── Carrito/        # Carrito de compras
│   ├── Pedido/         # Checkout, historial
│   └── Pago/           # PayPal, QR, Efectivo, Comprobante
└── wwwroot/            # Archivos estaticos
```

## Modulos del sistema

- **Gestion de Empleados** - Solo el Administrador puede crear empleados
- **Gestion de Productos** - CRUD con control de stock e inventario
- **Gestion de Categorias** - Clasificacion de productos
- **Gestion de Repartidores** - CRUD con estados (Disponible/Ocupado)
- **Pedidos en Linea** - Clientes realizan pedidos desde el catalogo
- **Carrito de Compras** - Basado en session con AJAX
- **Pagos** - PayPal, Codigo QR (con comprobante), Efectivo (contra entrega)
- **Entregas** - Asignacion de repartidores y seguimiento
- **Reportes** - Dashboard, ventas, inventario, alertas de stock bajo
- **Autenticacion** - Login por roles con cookies

## Tecnologias

- ASP.NET Core 8 MVC
- Entity Framework Core
- SQL Server
- Bootstrap 5
- Font Awesome 6
- QRCoder
- PayPal REST API
- BCrypt.Net
