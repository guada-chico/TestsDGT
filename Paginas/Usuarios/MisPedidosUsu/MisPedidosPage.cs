using Microsoft.Playwright;

namespace TestsDGT.Paginas.Usuarios.MisPedidosUsu;

public class MisPedidosPage
{
    private readonly IPage _page;

    private ILocator InputTalla => _page.Locator("#talla");
    private ILocator InputCantidad => _page.Locator("#cantidad");
    private ILocator BotonAgregarCesta => _page.GetByRole(AriaRole.Button, new() { Name = "Añadir a la cesta" });
    private ILocator BotonCloseModal => _page.GetByRole(AriaRole.Button, new() { Name = "Close" });
    private ILocator IconoCesta => _page.Locator(".cesta");
    private ILocator CheckConfirmarTalla => _page.GetByRole(AriaRole.Checkbox, new() { Name = "Confirmar talla" });
    private ILocator BotonTramitar => _page.GetByRole(AriaRole.Button, new() { Name = "Tramitar" });
    private ILocator ToastMensaje => _page.Locator(".p-toast");

    private ILocator RadioUrgente => _page.Locator("input[value='Urgente']");
    private ILocator ComboMotivo => _page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccionar motivo" });
    private ILocator InputInstrucciones => _page.GetByRole(AriaRole.Textbox, new() { Name = "Añade instrucciones" });
    private ILocator InputArchivo => _page.Locator("input[type='file']");

    private ILocator RadioOtroUsuario => _page.GetByRole(AriaRole.Radio, new() { Name = "Otro usuario" });
    private ILocator ComboSeleccionarUsuario => _page.GetByRole(AriaRole.Combobox, new() { Name = "-- Selecciona un usuario --" });
    private ILocator BuscadorUsuario => _page.GetByRole(AriaRole.Searchbox, new() { Name = "Buscar usuario..." });

    private ILocator InputFiltroNombre => _page.GetByPlaceholder("Nombre del artículo");
    private ILocator InputFiltroCodigo => _page.GetByPlaceholder("Código");
    private ILocator EstadoPedido => _page.Locator("p-dropdown").GetByRole(AriaRole.Combobox);
    private ILocator FechaDesde => _page.Locator("#startDate input[placeholder='dd/mm/aaaa']");
    private ILocator FechaHasta => _page.Locator("#endDate input[placeholder='dd/mm/aaaa']");
    private ILocator BotonFiltrar => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-filter") });

    private ILocator CabecerasTabla => _page.Locator("th");
    private ILocator FilasTabla => _page.Locator("tbody tr");
    private ILocator CeldaTexto(string texto) => _page.Locator("td").Filter(new() { HasText = texto });

    public MisPedidosPage(IPage page)
    {
        _page = page;
    }

    public async Task AgregarProductoAlCarritoAsync(string codigoProd, string talla, string cantidad)
    {
        var filaProducto = _page.GetByRole(AriaRole.Row).Filter(new() { HasText = codigoProd });
        await filaProducto.WaitForAsync();
        await filaProducto.Locator("button.p-button-icon-only").ClickAsync();

        await InputTalla.ClickAsync();
        await _page.GetByRole(AriaRole.Option, new() { Name = talla }).ClickAsync();

        await InputCantidad.ClickAsync();
        await _page.GetByRole(AriaRole.Option, new() { Name = cantidad }).ClickAsync();

        await BotonAgregarCesta.ClickAsync();
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

    public async Task IrAMisPedidosAsync()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/orders-list");
        await _page.WaitForURLAsync("**/orders-list");
    }

    public async Task FiltrarPorNombreArticuloAsync(string nombre)
    {
        await InputFiltroNombre.FillAsync(nombre);
        await BotonFiltrar.ClickAsync();
    }

    public async Task FiltrarPorCodigoAsync(string codigo)
    {
        await InputFiltroCodigo.FillAsync(codigo);
        await BotonFiltrar.ClickAsync();
    }

    public async Task FiltrarPorEstadoAsync(string estado)
    {
        await EstadoPedido.ClickAsync();
        var opcion = _page.Locator("p-dropdownitem").GetByText(estado, new() { Exact = true });

        await opcion.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
        await opcion.ClickAsync();

        await BotonFiltrar.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task FiltrarPorRangoFechasAsync(string desde, string hasta)
    {
        if (!string.IsNullOrEmpty(desde))
        {
            await FechaDesde.ClearAsync();
            await FechaDesde.FillAsync(desde);
            await _page.Keyboard.PressAsync("Tab");
        }
        if (!string.IsNullOrEmpty(hasta))
        {
            await FechaHasta.ClearAsync();
            await FechaHasta.FillAsync(hasta);
            await _page.Keyboard.PressAsync("Tab");
        }
        await BotonFiltrar.ClickAsync();
    }

    public async Task<bool> ExisteElementoEnTablaAsync(string texto)
    {
        try
        {
            await CeldaTexto(texto).First.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
            return true;
        }
        catch { return false; }
    }

    public async Task<List<string>> ObtenerValoresDeColumnaAsync(string nombreColumna)
    {
        var cabeceras = await CabecerasTabla.AllInnerTextsAsync();
        int indiceColumna = cabeceras.ToList().FindIndex(t => t.Contains(nombreColumna));

        if (indiceColumna == -1) return new List<string>();

        await _page.Locator("tbody tr td").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        return (await _page.Locator($"tbody tr td:nth-child({indiceColumna + 1})").AllInnerTextsAsync()).ToList();
    }
}