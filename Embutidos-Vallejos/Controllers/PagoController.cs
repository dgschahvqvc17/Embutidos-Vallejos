using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Embutidos_Vallejos.Controllers;

[Authorize]
public class PagoController : Controller
{
    private readonly IPagoService _pagoService;
    private readonly IPedidoService _pedidoService;
    private readonly IPayPalService _payPalService;
    private readonly IQRService _qrService;
    private readonly IVentaService _ventaService;
    private readonly IStripeService _stripeService;
    private readonly IConfiguration _config;

    public PagoController(
        IPagoService pagoService,
        IPedidoService pedidoService,
        IPayPalService payPalService,
        IQRService qrService,
        IVentaService ventaService,
        IStripeService stripeService,
        IConfiguration config)
    {
        _pagoService = pagoService;
        _pedidoService = pedidoService;
        _payPalService = payPalService;
        _qrService = qrService;
        _ventaService = ventaService;
        _stripeService = stripeService;
        _config = config;
    }

    public async Task<IActionResult> PayPal(int pedidoId)
    {
        var pedido = await _pedidoService.GetByIdAsync(pedidoId);
        if (pedido == null) return NotFound();

        return View(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> CrearOrdenPayPal(int pedidoId)
    {
        var pedido = await _pedidoService.GetByIdAsync(pedidoId);
        if (pedido == null) return Json(new { success = false, message = "Pedido no encontrado" });

        try
        {
            var orderId = await _payPalService.CrearOrdenAsync(pedido.Total);
            return Json(new { success = true, orderId });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error al crear orden PayPal: {ex.Message}" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CapturarOrdenPayPal(int pedidoId, string orderId)
    {
        try
        {
            var capturado = await _payPalService.CapturarOrdenAsync(orderId);
            if (!capturado)
                return Json(new { success = false, message = "Error al capturar el pago" });

            var pago = await _pagoService.GetByPedidoIdAsync(pedidoId);
            if (pago != null)
            {
                await _pagoService.ConfirmarPagoAsync(pago.PagoId, orderId);
            }

            await _pedidoService.UpdateEstadoAsync(pedidoId, "Pagado");

            return Json(new { success = true, redirect = Url.Action("Comprobante", new { pedidoId }) });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    public async Task<IActionResult> QR(int pedidoId, int? ventaId, bool esVenta = false)
    {
        if (esVenta && ventaId.HasValue)
        {
            var venta = await _ventaService.GetByIdAsync(ventaId.Value);
            if (venta == null) return NotFound();

            var referencia = $"VTA-{ventaId}-{DateTime.Now:yyyyMMddHHmmss}";
            var qrBase64 = _qrService.GenerarQRPago(ventaId.Value, venta.Total, referencia);

            ViewBag.QRCode = qrBase64;
            ViewBag.Referencia = referencia;
            ViewBag.EsVenta = true;
            ViewBag.VentaId = ventaId;

            return View("QRVenta", venta);
        }

        var pedido = await _pedidoService.GetByIdAsync(pedidoId);
        if (pedido == null) return NotFound();

        var refPedido = $"PED-{pedidoId}-{DateTime.Now:yyyyMMddHHmmss}";
        var qrBase64ped = _qrService.GenerarQRPago(pedidoId, pedido.Total, refPedido);

        ViewBag.QRCode = qrBase64ped;
        ViewBag.Referencia = refPedido;

        return View(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmarQRVenta(int ventaId)
    {
        TempData["Success"] = $"Venta #{ventaId} pagada con QR.";
        return RedirectToAction("Index", "Ventas");
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmarQR(int pedidoId, string referencia)
    {
        var pago = await _pagoService.GetByPedidoIdAsync(pedidoId);
        if (pago != null)
        {
            await _pagoService.ConfirmarPagoAsync(pago.PagoId, referencia);
        }

        await _pedidoService.UpdateEstadoAsync(pedidoId, "Pagado");

        TempData["Success"] = "Pago QR confirmado exitosamente.";
        return RedirectToAction("Comprobante", new { pedidoId });
    }

    public async Task<IActionResult> Efectivo(int pedidoId)
    {
        var pedido = await _pedidoService.GetByIdAsync(pedidoId);
        if (pedido == null) return NotFound();

        return View(pedido);
    }

    public async Task<IActionResult> Comprobante(int pedidoId)
    {
        var receipt = await _pagoService.GetReceiptAsync(pedidoId);
        if (receipt == null) return NotFound();

        return View(receipt);
    }

    public async Task<IActionResult> Stripe(int pedidoId)
    {
        var pedido = await _pedidoService.GetByIdAsync(pedidoId);
        if (pedido == null) return NotFound();

        var secretKey = _config["Stripe:SecretKey"];
        if (string.IsNullOrEmpty(secretKey) || secretKey == "TU_SECRET_KEY_DE_STRIPE" || secretKey.StartsWith("TU_"))
        {
            return RedirectToAction(nameof(StripeSimulado), new { pedidoId });
        }

        try
        {
            var successUrl = (Url.Action("StripeSuccess", "Pago", new { pedidoId = pedidoId }, Request.Scheme) ?? "") + "&sessionId={CHECKOUT_SESSION_ID}";
            var cancelUrl = Url.Action("StripeCancel", "Pago", new { pedidoId = pedidoId }, Request.Scheme) ?? "";

            var sessionUrl = await _stripeService.CrearCheckoutSessionAsync(pedido, successUrl, cancelUrl);
            return Redirect(sessionUrl);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al conectar con Stripe: {ex.Message}. Redireccionando a simulación local...";
            return RedirectToAction(nameof(StripeSimulado), new { pedidoId });
        }
    }

    public async Task<IActionResult> StripeSimulado(int pedidoId)
    {
        var pedido = await _pedidoService.GetByIdAsync(pedidoId);
        if (pedido == null) return NotFound();

        ViewBag.PublishableKey = _config["Stripe:PublishableKey"];
        return View(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmarPagoStripeSimulado(int pedidoId, string numeroTarjeta)
    {
        var pedido = await _pedidoService.GetByIdAsync(pedidoId);
        if (pedido == null) return NotFound();

        var pago = await _pagoService.GetByPedidoIdAsync(pedidoId);
        var mockTransactionId = $"ch_sim_{Guid.NewGuid().ToString("N").Substring(0, 16)}";
        
        if (pago != null)
        {
            await _pagoService.ConfirmarPagoAsync(pago.PagoId, mockTransactionId);
        }

        await _pedidoService.UpdateEstadoAsync(pedidoId, "Pagado");

        TempData["Success"] = "Pago simulado con tarjeta procesado exitosamente.";
        return RedirectToAction(nameof(Comprobante), new { pedidoId });
    }

    public async Task<IActionResult> StripeSuccess(int pedidoId, string sessionId)
    {
        var pedido = await _pedidoService.GetByIdAsync(pedidoId);
        if (pedido == null) return NotFound();

        var valid = await _stripeService.ValidarCheckoutSessionAsync(sessionId);
        if (!valid)
        {
            TempData["Error"] = "No se pudo verificar el pago de Stripe.";
            return RedirectToAction("Details", "Pedido", new { id = pedidoId });
        }

        var pago = await _pagoService.GetByPedidoIdAsync(pedidoId);
        if (pago != null)
        {
            await _pagoService.ConfirmarPagoAsync(pago.PagoId, sessionId);
        }

        await _pedidoService.UpdateEstadoAsync(pedidoId, "Pagado");

        TempData["Success"] = "Pago procesado y verificado exitosamente mediante Stripe.";
        return RedirectToAction(nameof(Comprobante), new { pedidoId });
    }

    public IActionResult StripeCancel(int pedidoId)
    {
        TempData["Error"] = "El pago mediante Stripe fue cancelado por el usuario.";
        return RedirectToAction("Details", "Pedido", new { id = pedidoId });
    }
}

