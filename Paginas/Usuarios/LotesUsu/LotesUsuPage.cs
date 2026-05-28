using Microsoft.Playwright;

namespace TestsDGT.Paginas.Usuarios.LotesUsu;

public class LotesUsuPage
{
    private readonly IPage _page;

    private ILocator InputFiltroCodigoONombre => _page.GetByRole(AriaRole.Textbox, new() { Name = "Buscar por código o nombre" });
    private ILocator BotonFiltrar => _page.Locator(".pi-filter");
    private ILocator BotonLimpiarFiltros => _page.Locator(".pi-times");

    private ILocator BotonPaginaLotesDisponibles => _page.GetByRole(AriaRole.Button, new() { Name = "Lotes disponibles" });
    private ILocator BotonAgregarLote => _page.GetByRole(AriaRole.Button, new() { Name = "Añadir" });

    private ILocator IconoCesta => _page.Locator(".cesta");

    private ILocator ComboTalla => _page.GetByRole(AriaRole.Combobox, new() { Name = "" });
    private ILocator CheckConfirmarTalla => _page.GetByRole(AriaRole.Checkbox, new() { Name = "Confirmar talla" });
    private ILocator InputInstrucciones => _page.GetByRole(AriaRole.Textbox, new() { Name = "Añade instrucciones especiales para este artículo" });

    private ILocator BotonTramitar => _page.GetByRole(AriaRole.Button, new() { Name = "Tramitar" });
    private ILocator BotonBorrarLote => _page.Locator(".pi-trash");
    private ILocator ToastMensaje => _page.Locator(".p-toast");

    private ILocator CabecerasTabla => _page.Locator("th");
    private ILocator FilasTabla => _page.Locator("tbody tr");
    private ILocator CeldaTexto(string texto) => _page.Locator("td").Filter(new() { HasText = texto });

    public LotesUsuPage(IPage page)
    {
        _page = page;
    }

    public async Task DatosLoteCestaAsync(string talla, string instrucciones)
    {
        await IconoCesta.ClickAsync();
        await ComboTalla.ClickAsync();

        var opcion = _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").GetByText(talla, new() { Exact = true });
        await opcion.ClickAsync();

        if (!string.IsNullOrEmpty(instrucciones))
        {
            await InputInstrucciones.FillAsync(instrucciones);
        }

        await CheckConfirmarTalla.CheckAsync();
    }

    public async Task TramitarLoteAsync()
    {
        
        await BotonTramitar.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task BorrarArticuloDeLaCestaAsync(string nombreLote)
    {
        var filaLote = _page.GetByRole(AriaRole.Row).Filter(new() { HasText = nombreLote }).First;
        await filaLote.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

        await filaLote.Locator(BotonBorrarLote).ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<string> ObtenerMensajeToastAsync()
    {
        await ToastMensaje.WaitForAsync();
        return await ToastMensaje.InnerTextAsync();
    }

    public async Task FiltrarPorLoteAsync(string codigoNombre)
    {
        await InputFiltroCodigoONombre.FillAsync(codigoNombre);
        await BotonFiltrar.ClickAsync();
    }

    public async Task LimpiarFiltrosAsync()
    {
        await BotonLimpiarFiltros.ClickAsync();
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

    public async Task<int> ObtenerNumeroFilasLotesAsync()
    {
        return await FilasTabla.CountAsync();
    }

    public async Task<List<string>> ObtenerValoresDeColumnaAsync(string nombreColumna)
    {
        var cabeceras = await CabecerasTabla.AllInnerTextsAsync();
        int indiceColumna = cabeceras.ToList().FindIndex(t => t.Contains(nombreColumna));

        if (indiceColumna == -1) return new List<string>();

        await _page.Locator("tbody tr td").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        return (await _page.Locator($"tbody tr td:nth-child({indiceColumna + 1})").AllInnerTextsAsync()).ToList();
    }

    public async Task IrALotesDisponiblesAsync()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/catalogo");
        await _page.WaitForURLAsync("**/catalogo");

        await BotonPaginaLotesDisponibles.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}