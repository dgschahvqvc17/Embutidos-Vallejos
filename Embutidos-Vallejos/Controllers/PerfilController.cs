using System.Security.Claims;
using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Embutidos_Vallejos.Controllers;

[Authorize]
public class PerfilController : Controller
{
    private readonly IClienteService _clienteService;
    private readonly IEmpleadoService _empleadoService;

    public PerfilController(IClienteService clienteService, IEmpleadoService empleadoService)
    {
        _clienteService = clienteService;
        _empleadoService = empleadoService;
    }

    private string GetUserType() => User.FindFirstValue("UserType") ?? "";
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        PerfilDto? perfil = null;
        var userType = GetUserType();

        if (userType == "Cliente")
            perfil = await _clienteService.GetProfileAsync(GetUserId());
        else if (userType == "Empleado")
            perfil = await _empleadoService.GetProfileAsync(GetUserId());

        if (perfil == null) return RedirectToAction("Index", "Home");

        return View(perfil);
    }

    public async Task<IActionResult> Editar()
    {
        PerfilDto? perfil = null;
        var userType = GetUserType();

        if (userType == "Cliente")
            perfil = await _clienteService.GetProfileAsync(GetUserId());
        else if (userType == "Empleado")
            perfil = await _empleadoService.GetProfileAsync(GetUserId());

        if (perfil == null) return RedirectToAction("Index", "Home");

        var dto = new PerfilUpdateDto
        {
            Nombre = perfil.Nombre,
            Apellido = perfil.Apellido,
            Telefono = perfil.Telefono,
            Direccion = userType == "Cliente" ? perfil.Direccion : null
        };

        ViewBag.Email = perfil.Email;
        ViewBag.UserType = userType;

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(PerfilUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Email = User.FindFirstValue(ClaimTypes.Email);
            ViewBag.UserType = GetUserType();
            return View(dto);
        }

        var userType = GetUserType();
        var success = userType == "Cliente"
            ? await _clienteService.UpdateProfileAsync(GetUserId(), dto)
            : await _empleadoService.UpdateProfileAsync(GetUserId(), dto);

        if (success)
        {
            TempData["PerfilMensaje"] = "Perfil actualizado correctamente.";

            var name = $"{dto.Nombre} {dto.Apellido}";
            var identity = (ClaimsIdentity)User.Identity!;
            var nameClaim = identity.FindFirst(ClaimTypes.Name);
            if (nameClaim != null)
                identity.RemoveClaim(nameClaim);
            identity.AddClaim(new Claim(ClaimTypes.Name, name));

            await HttpContext.SignOutAsync();
            await HttpContext.SignInAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }
        else
        {
            ModelState.AddModelError("", "Error al actualizar el perfil.");
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult CambiarPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CambiarPassword(CambiarPasswordDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        if (dto.PasswordActual != dto.PasswordActualConfirm)
        {
            ModelState.AddModelError("PasswordActualConfirm", "Las contrasenas actuales no coinciden.");
            return View(dto);
        }

        var userType = GetUserType();
        var userId = GetUserId();

        var verified = userType == "Cliente"
            ? await _clienteService.VerifyPasswordAsync(userId, dto.PasswordActual)
            : await _empleadoService.VerifyPasswordAsync(userId, dto.PasswordActual);

        if (!verified)
        {
            ModelState.AddModelError("PasswordActual", "La contrasena actual es incorrecta.");
            return View(dto);
        }

        var success = userType == "Cliente"
            ? await _clienteService.CambiarPasswordAsync(userId, dto.NuevaPassword)
            : await _empleadoService.CambiarPasswordAsync(userId, dto.NuevaPassword);

        if (success)
        {
            TempData["PerfilMensaje"] = "Contrasena cambiada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Error al cambiar la contrasena.");
        return View(dto);
    }
}
