using System.Security.Claims;
using Embutidos_Vallejos.Data;
using Embutidos_Vallejos.Models;
using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Embutidos_Vallejos.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    private readonly IClienteService _clienteService;

    public AccountController(AppDbContext db, IClienteService clienteService)
    {
        _db = db;
        _clienteService = clienteService;
    }

    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var cliente = await _db.Clientes
            .FirstOrDefaultAsync(c => c.Email == model.Email);

        if (cliente != null && IsBcrypt(cliente.Password) && BCrypt.Net.BCrypt.Verify(model.Password, cliente.Password))
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, $"{cliente.Nombre} {cliente.Apellido}"),
                new(ClaimTypes.Email, cliente.Email),
                new(ClaimTypes.NameIdentifier, cliente.ClienteId.ToString()),
                new("UserType", "Cliente")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = model.RememberMe });

            return RedirectToAction("Index", "Home");
        }

        var empleado = await _db.Empleados
            .Include(e => e.Rol)
            .FirstOrDefaultAsync(e => e.Email == model.Email);

        if (empleado != null && IsBcrypt(empleado.Password) && BCrypt.Net.BCrypt.Verify(model.Password, empleado.Password))
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, $"{empleado.Nombre} {empleado.Apellido}"),
                new(ClaimTypes.Email, empleado.Email),
                new(ClaimTypes.NameIdentifier, empleado.EmpleadoId.ToString()),
                new(ClaimTypes.Role, empleado.Rol.NombreRol),
                new("UserType", "Empleado")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = model.RememberMe });

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Correo o contrasena incorrectos.");
        return View(model);
    }

    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await _clienteService.EmailExistsAsync(model.Email))
        {
            ModelState.AddModelError("Email", "Este correo ya esta registrado.");
            return View(model);
        }

        var cliente = await _clienteService.RegisterAsync(model);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, $"{cliente.Nombre} {cliente.Apellido}"),
            new(ClaimTypes.Email, cliente.Email),
            new(ClaimTypes.NameIdentifier, cliente.ClienteId.ToString()),
            new("UserType", "Cliente")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = false });

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    private static bool IsBcrypt(string password)
    {
        return password.StartsWith("$2");
    }
}
