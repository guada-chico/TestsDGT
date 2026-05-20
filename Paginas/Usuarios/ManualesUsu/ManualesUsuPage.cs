using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.ManualesUsu;

public class    ManualesUsuPage
{
    private readonly IPage _page;
    
    private ILocator InputFiltroTituloManual => _page.GetByPlaceholder("Buscar por título");
    private ILocator InputFiltroFechaDesde => _page.Locator("p-calendar input[placeholder='Desde']");
    private ILocator InputFiltroFechaHasta => _page.Locator("p-calendar input[placeholder='Hasta']");
    private ILocator BotonAplicarFiltro => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-filter") });
    private ILocator BotonLimpiarFiltros => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-times") });
    private ILocator FilasTablaManual => _page.Locator("tbody tr");

    public ManualesUsuPage(IPage page)
    {
        _page = page;
    }

    public async Task FiltrarPorTituloManualAsync(string titulo)
    {
        await InputFiltroTituloManual.FillAsync(titulo);

        await BotonAplicarFiltro.DispatchEventAsync("click");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task FiltrarPorRangoFechaManualAsync(string fechaDesdeManual, string fechaHastaManual)
    {
        await InputFiltroFechaDesde.ClearAsync();
        await InputFiltroFechaDesde.FillAsync(fechaDesdeManual);
        await _page.Keyboard.PressAsync("Tab");
        await InputFiltroFechaHasta.ClearAsync();
        await InputFiltroFechaHasta.FillAsync(fechaHastaManual);
        await _page.Keyboard.PressAsync("Tab");

        await BotonAplicarFiltro.DispatchEventAsync("click");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> VerificarManualCreadaAsync(string titulo)
    {
        try
        {
            var celdaManual = _page.Locator("td").GetByText(titulo).First;
            await celdaManual.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<int> ObtenerNumeroFilasManualAsync()
    {
        return await FilasTablaManual.CountAsync();
    }

    public async Task LimpiarFiltrosAsync()
    {
        await BotonLimpiarFiltros.DispatchEventAsync("click");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task IrAManuales()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/manuales-user");
        await _page.WaitForURLAsync("**/manuales-user");
    }
}