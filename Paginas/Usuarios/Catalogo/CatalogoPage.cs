using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.Catalogo;

public class CatalogoPage : UsuariosBasePage
{
    private ILocator InputBuscarCodigo => _page.GetByPlaceholder("Buscar por código");
    private ILocator InputBuscarNombre => _page.GetByPlaceholder("Buscar por nombre");
    private ILocator BotonFiltrar => _page.Locator(".pi-filter");
    private ILocator BotonLimpiarFiltros => _page.Locator(".pi-times");

    private ILocator BotonVerDetalle => _page.Locator("button").Filter(new() { Has = _page.Locator("button.p-button-icon-only") });

    private ILocator ComboTalla => _page.Locator("#talla");
    private ILocator InputCantidad => _page.Locator("#cantidad");
    private ILocator BotonAgregarCesta => _page.GetByRole(AriaRole.Button, new() { Name = "Añadir a la cesta" });
    private ILocator BotonVolverCatalogo => _page.GetByRole(AriaRole.Button, new() { Name = "Volver al catálogo" });

    private ILocator InputFiltroLote => _page.GetByRole(AriaRole.Textbox, new() { Name = "Buscar por código o nombre" });
    private ILocator BotonPaginaLotesDisponibles => _page.GetByRole(AriaRole.Button, new() { Name = "Lotes disponibles" });
    private ILocator BotonAgregarLote => _page.GetByRole(AriaRole.Button, new() { Name = "Añadir" });

    public CatalogoPage(IPage page) : base(page) { }

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

    public async Task IrALotesDisponiblesAsync()
    {
        await BotonPaginaLotesDisponibles.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}