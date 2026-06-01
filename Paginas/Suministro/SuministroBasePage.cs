using System;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Suministro;

public abstract class SuministroBasePage 
{
    protected readonly IPage _page; 

    protected ILocator BotonFiltrar => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-filter") });
    protected ILocator BotonLimpiarFiltro => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-times") });
    protected ILocator ToastMensaje => _page.Locator(".p-toast:visible");
    protected ILocator ToastMensajeDetalle => ToastMensaje.Locator(".p-toast-detail");
    protected ILocator FilasTabla => _page.Locator("tbody tr");
    protected ILocator BotonMasOpciones => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-ellipsis-v") });
    protected ILocator OpcionVerDetalle => _page.GetByRole(AriaRole.Menuitem, new() { Name = "Ver detalle" });

    protected SuministroBasePage(IPage page)
    {
        _page = page;
    }

    public async Task LimpiarFiltrosAsync()
    {
        await BotonLimpiarFiltro.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> VerificarTextoEnTablaAsync(string textoBuscar)
    {
        try
        {
            var celda = _page.Locator("td").GetByText(textoBuscar, new() { Exact = false }).First;
            await Assertions.Expect(celda).ToBeVisibleAsync(new() { Timeout = 5000 });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<string> ObtenerMensajeToastAsync()
    {
        var ultimoToast = ToastMensajeDetalle.Last;
        await ultimoToast.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 15000 });
        return await ultimoToast.InnerTextAsync();
    }

    public async Task<int> ObtenerNumeroFilasTablaAsync()
    {
        return await FilasTabla.CountAsync();
    }
}