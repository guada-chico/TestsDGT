using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.Catalogo;

public class CatalogoPage
{
    private readonly IPage _page;

    private ILocator InputBuscarCodigo => _page.GetByPlaceholder("Buscar por código");
    private ILocator InputBuscarNombre => _page.GetByPlaceholder("Buscar por nombre");
    private ILocator BotonFiltrar => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-filter") });
    private ILocator BotonLimpiar => _page.Locator("button.btn-clear");
    private ILocator FilasTabla => _page.Locator("tbody tr");
    private ILocator CeldaTabla (string texto) => _page.Locator("td").Filter(new() { HasText = texto });

    public CatalogoPage(IPage page)
    {
        _page = page;
    }

    public async Task FiltrarPorCodigoAsync(string codigo)
    {
        await InputBuscarCodigo.FillAsync(codigo);
        await BotonFiltrar.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task FiltrarPorNombreAsync(string nombre)
    {
        await InputBuscarNombre.FillAsync(nombre);
        await BotonFiltrar.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task LimpiarFiltrosAsync()
    {
        await BotonLimpiar.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> ExisteTextoEnTablaAsync(string texto)
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
        return await InputBuscarNombre.InputValueAsync();
    }
}