using System;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios;

public abstract class UsuariosBasePage
{
    protected readonly IPage _page;

    protected ILocator ToastMensaje => _page.Locator(".p-toast");
    protected ILocator FilasTabla => _page.Locator("tbody tr");
    protected ILocator CeldaTabla(string texto) => _page.Locator("td").Filter(new() { HasText = texto });

    protected UsuariosBasePage(IPage page)
    {
        _page = page;
    }

    public async Task IrACatalogoAsync()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/catalogo");
        await _page.WaitForURLAsync("**/catalogo");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task IrAMisPedidosAsync()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/orders-list");
        await _page.WaitForURLAsync("**/orders-list");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<string> ObtenerMensajeToastAsync()
    {
        await ToastMensaje.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
        return await ToastMensaje.InnerTextAsync();
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
}