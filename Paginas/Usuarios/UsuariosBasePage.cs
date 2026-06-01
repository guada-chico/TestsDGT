using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios;

public abstract class UsuariosBasePage
{
    protected readonly IPage _page;
    protected ILocator InputBuscarCodigo => _page.GetByPlaceholder("Buscar por código");
    protected ILocator InputBuscarNombre => _page.GetByPlaceholder("Buscar por nombre");
    protected ILocator BotonFiltrar => _page.Locator(".pi-filter");
    protected ILocator BotonLimpiarFiltros => _page.Locator(".pi-times");

    protected ILocator BotonVerDetalle => _page.Locator("button").Filter(new() { Has = _page.Locator("button.p-button-icon-only") });

    protected ILocator FilasTabla => _page.Locator("tbody tr");
    protected ILocator CeldaTabla(string texto) => _page.Locator("td").Filter(new() { HasText = texto });
    protected ILocator ToastMensaje => _page.Locator(".p-toast");


    public UsuariosBasePage(IPage page)
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
        await BotonLimpiarFiltros.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task SeleccionarProductoCatalogoAsync(string codigoProd)
    {
        var filaProducto = _page.GetByRole(AriaRole.Row).Filter(new() { HasText = codigoProd });
        await filaProducto.WaitForAsync();
        await filaProducto.Locator(BotonVerDetalle).ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task AgregarDatosProductoAsync(string talla, string cantidad)
    {
        await ComboTalla.Locator(".p-select-dropdown, .p-dropdown-trigger").ClickAsync();

        var opcionTalla = _page.Locator(".p-dropdown-item:visible, .p-select-option:visible")
                           .GetByText(talla, new() { Exact = true });
        await opcionTalla.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 4000 });
        await opcionTalla.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        await InputCantidad.Locator(".p-select-dropdown, .p-dropdown-trigger").ClickAsync();

        var opcionCantidad = _page.Locator(".p-dropdown-item:visible, .p-select-option:visible")
                                   .GetByText(cantidad, new() { Exact = true });
        await opcionCantidad.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 4000 });
        await opcionCantidad.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task AgregarACestaAsync()
    {
        await BotonAgregarCesta.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task VolverAlCatalogoAsync()
    {
        await BotonVolverCatalogo.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task IrAMisPedidosAsync()
    {
        await BotonIrAMisPedidos.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task FiltrarPorLoteAsync(string codigoNombre)
    {
        await InputFiltroLote.FillAsync(codigoNombre);
        await BotonFiltrar.ClickAsync();
    }

    public async Task AgregarLoteAlCarritoAsync(string codigoLote)
    {
        var filaLote = _page.GetByRole(AriaRole.Row).Filter(new() { HasText = codigoLote }).First;
        await filaLote.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        await filaLote.Locator(BotonAgregarLote).ClickAsync();
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
        return await InputBuscarNombre.InputValueAsync();
    }
    public async Task<string> ObtenerMensajeToastAsync()
    {
        await ToastMensaje.WaitForAsync();
        return await ToastMensaje.InnerTextAsync();
    }

    public async Task IrACatalogoAsync()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/catalogo");
        await _page.WaitForURLAsync("**/catalogo");

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task IrALotesDisponiblesAsync()
    {
        await BotonPaginaLotesDisponibles.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}