using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.MiCesta;

public class MiCestaPage
{
    private readonly IPage _page;

    private ILocator CheckConfirmarTalla => _page.GetByRole(AriaRole.Checkbox, new() { Name = "Confirmar talla" });
    private ILocator BotonTramitar => _page.GetByRole(AriaRole.Button, new() { Name = "Tramitar" });

    private ILocator BotonCatalogo => _page.GetByRole(AriaRole.Button, new() { Name = "Catálogo" });

    private ILocator RadioUrgente => _page.Locator("input[value='Urgente']");
    private ILocator ComboMotivo => _page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccionar motivo" });
    private ILocator InputInstrucciones => _page.GetByRole(AriaRole.Textbox, new() { Name = "Añade instrucciones" });
    private ILocator InputArchivo => _page.Locator("input[type='file']");

    private ILocator RadioOtroUsuario => _page.GetByRole(AriaRole.Radio, new() { Name = "Otro usuario" });
    private ILocator ComboSeleccionarUsuario => _page.GetByRole(AriaRole.Combobox, new() { Name = "-- Selecciona un usuario --" });
    private ILocator BuscadorUsuario => _page.GetByRole(AriaRole.Searchbox, new() { Name = "Buscar usuario..." });


    private ILocator FilasTabla => _page.Locator("tbody tr");
    private ILocator CeldaTabla (string texto) => _page.Locator("td").Filter(new() { HasText = texto });
    private ILocator ToastMensaje => _page.Locator(".p-toast");

    public MiCestaPage(IPage page)
    {
        _page = page;
    }

    public async Task TramitarPedidoOrdinarioAsync()
    {
        await CheckConfirmarTalla.CheckAsync();
        await BotonTramitar.ClickAsync();
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
    /*
    public async Task<string> ObtenerTextoFiltroNombreAsync()
    {
        return await InputBuscarNombre.InputValueAsync();
    }
    */
    public async Task<string> ObtenerMensajeToastAsync()
    {
        await ToastMensaje.WaitForAsync();
        return await ToastMensaje.InnerTextAsync();
    }

    public async Task IrACestaAsync()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/app-cart");
        await _page.WaitForURLAsync("**/app-cart");

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task IrACatalogoAsync()
    {
        await BotonCatalogo.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}