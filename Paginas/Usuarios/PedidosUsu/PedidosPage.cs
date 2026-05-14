using Microsoft.Playwright;

namespace TestsDGT.Paginas;

public class PedidosPage
{
    private readonly IPage _page;

    // Localizadores Básicos
    private ILocator InputTalla => _page.Locator("#talla");
    private ILocator InputCantidad => _page.Locator("#cantidad");
    private ILocator BotonAnadirCesta => _page.GetByRole(AriaRole.Button, new() { Name = "Añadir a la cesta" });
    private ILocator BotonCloseModal => _page.GetByRole(AriaRole.Button, new() { Name = "Close" });
    private ILocator IconoCesta => _page.Locator(".cesta");
    private ILocator CheckConfirmarTalla => _page.GetByRole(AriaRole.Checkbox, new() { Name = "Confirmar talla" });
    private ILocator BotonTramitar => _page.GetByRole(AriaRole.Button, new() { Name = "Tramitar" });
    private ILocator ToastMensaje => _page.Locator(".p-toast");

    // Localizadores de Pedido Urgente
    private ILocator RadioUrgente => _page.Locator("input[value='Urgente']");
    private ILocator ComboMotivo => _page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccionar motivo" });
    private ILocator InputInstrucciones => _page.GetByRole(AriaRole.Textbox, new() { Name = "Añade instrucciones" });
    private ILocator InputArchivo => _page.Locator("input[type='file']");

    // Localizadores de Otro Usuario
    private ILocator RadioOtroUsuario => _page.GetByRole(AriaRole.Radio, new() { Name = "Otro usuario" });
    private ILocator ComboSeleccionarUsuario => _page.GetByRole(AriaRole.Combobox, new() { Name = "-- Selecciona un usuario --" });
    private ILocator BuscadorUsuario => _page.GetByRole(AriaRole.Searchbox, new() { Name = "Buscar usuario..." });

    public PedidosPage(IPage page)
    {
        _page = page;
    }

    public async Task AñadirProductoAlCarritoAsync(string codigoProd, string talla, string cantidad)
    {
        var filaProducto = _page.GetByRole(AriaRole.Row).Filter(new() { HasText = codigoProd });
        await filaProducto.WaitForAsync();
        await filaProducto.Locator("button.p-button-icon-only").ClickAsync();

        await InputTalla.ClickAsync();
        await _page.GetByRole(AriaRole.Option, new() { Name = talla }).ClickAsync();

        await InputCantidad.ClickAsync();
        await _page.GetByRole(AriaRole.Option, new() { Name = cantidad }).ClickAsync();

        await BotonAnadirCesta.ClickAsync();
        await BotonCloseModal.ClickAsync();
        await IconoCesta.ClickAsync();
    }

    public async Task TramitarPedidoOrdinarioAsync()
    {
        await CheckConfirmarTalla.CheckAsync();
        await BotonTramitar.ClickAsync();
    }

    public async Task TramitarPedidoUrgenteAsync(string motivo, string instrucciones, string rutaArchivo)
    {
        await CheckConfirmarTalla.CheckAsync();
        await RadioUrgente.CheckAsync(new() { Force = true });
        await ComboMotivo.ClickAsync();
        await _page.GetByRole(AriaRole.Option, new() { Name = motivo }).ClickAsync();
        await InputInstrucciones.FillAsync(instrucciones);
        await InputArchivo.SetInputFilesAsync(rutaArchivo);
        await BotonTramitar.ClickAsync();
    }

    public async Task TramitarPedidoParaOtroUsuarioAsync(string nombreUsuario)
    {
        await RadioOtroUsuario.CheckAsync();
        await ComboSeleccionarUsuario.ClickAsync();
        await BuscadorUsuario.FillAsync(nombreUsuario);
        await _page.GetByRole(AriaRole.Option, new() { Name = nombreUsuario.ToUpper(), Exact = true }).ClickAsync();
        await CheckConfirmarTalla.CheckAsync();
        await BotonTramitar.ClickAsync();
    }

    public async Task BorrarArticuloDeLaCestaAsync(string nombreArticulo)
    {
        var filaArticulo = _page.GetByRole(AriaRole.Row).Filter(new() { HasText = nombreArticulo });
        await filaArticulo.Locator("button").Filter(new() { Has = _page.Locator(".pi-trash") }).ClickAsync();
    }

    public async Task<string> ObtenerMensajeToastAsync()
    {
        await ToastMensaje.WaitForAsync();
        return await ToastMensaje.InnerTextAsync();
    }
}