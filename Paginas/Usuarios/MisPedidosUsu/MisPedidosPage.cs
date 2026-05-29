using Microsoft.Playwright;

namespace TestsDGT.Paginas.Usuarios.MisPedidosUsu;

public class MisPedidosPage
{
    private readonly IPage _page;

    private ILocator InputFiltroNombre => _page.GetByPlaceholder("Nombre del artículo");
    private ILocator EstadoPedido => _page.Locator("p-dropdown").GetByRole(AriaRole.Combobox);
    private ILocator InputFiltroCodigo => _page.GetByPlaceholder("Código");
    private ILocator FechaDesde => _page.Locator("#startDate input[placeholder='dd/mm/aaaa']");
    private ILocator FechaHasta => _page.Locator("#endDate input[placeholder='dd/mm/aaaa']");
    private ILocator BotonFiltrar => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-filter") });
    private ILocator BotonLimpiarFiltros => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-times") });

    private ILocator BotonCatalogo => _page.GetByRole(AriaRole.Button, new() { Name = "Catálogo" });

    private ILocator CabecerasTabla => _page.Locator("th");
    private ILocator FilasTabla => _page.Locator("tbody tr");
    private ILocator CeldaTabla(string texto) => _page.Locator("td").Filter(new() { HasText = texto });

    public MisPedidosPage(IPage page)
    {
        _page = page;
    }

    public async Task FiltrarPorNombreArticuloAsync(string nombre)
    {
        await InputFiltroNombre.FillAsync(nombre);
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

    public async Task FiltrarPorCodigoAsync(string codigo)
    {
        await InputFiltroCodigo.FillAsync(codigo);
        await BotonFiltrar.ClickAsync();
    }

    public async Task LimpiarFiltrosAsync()
    {
        await BotonLimpiarFiltros.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> ExisteElementoEnTablaAsync(string texto)
    {
        try
        {
            await CeldaTabla(texto).First.WaitForAsync(new() { Timeout = 5000 });
            return true;
        }
        catch { return false; }
    }

    public async Task<int> ObtenerNumeroFilasAsync()
    {
        return await FilasTabla.CountAsync();
    }

    public async Task<string> ObtenerTextoFiltroNombreAsync()
    {
        return await InputFiltroNombre.InputValueAsync();
    }

    public async Task IrAMisPedidosAsync()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/orders-list");
        await _page.WaitForURLAsync("**/orders-list");
    }

    public async Task IrACatalogoAsync()
    {
        await BotonCatalogo.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}