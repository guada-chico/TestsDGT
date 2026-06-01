using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Suministro.Proveedores;

public class ProveedoresSumPage : SuministroBasePage
{
    private ILocator BotonNuevoProveedor => _page.GetByRole(AriaRole.Button, new() { Name = "Añadir", Exact = true });
    private ILocator InputNombreProveedor => _page.Locator("#nombre");
    private ILocator InputRazonSocialProveedor => _page.Locator("#razonSocial"); 
    private ILocator InputCifNifProveedor => _page.Locator("#cifNif");
    private ILocator InputWebProveedor => _page.Locator("#web");
    private ILocator InputDireccionProveedor => _page.Locator("#direccion");
    private ILocator InputTelefonoProveedor => _page.Locator("#telefono");
    private ILocator InputContactoProveedor => _page.Locator("#contacto");
    private ILocator InputEmailProveedor => _page.Locator("#correoElectronico");
    private ILocator EstadoProveedor => _page.Locator("#estado"); 
    private ILocator ObservacionesProveedor => _page.Locator("#observaciones");

    private ILocator BotonGuardarProveedor => _page.GetByRole(AriaRole.Button, new() { Name = "Guardar", Exact = true });
    private ILocator BotonCancelarEdicionProveedor => _page.GetByRole(AriaRole.Button, new() { Name = "Cancelar", Exact = true });
    private ILocator BotónConfirmarCancelarProveedor => _page.GetByRole(AriaRole.Button, new() { Name = "Salir sin guardar", Exact = true });

    private ILocator InputFiltroCodigoNombreProveedor => _page.GetByPlaceholder("Buscar por código o nombre");

    private ILocator OpcionEditarProveedor => _page.GetByRole(AriaRole.Menuitem, new() { Name = "Editar", Exact = true });
    private ILocator BotonEliminarProveedor => _page.Locator("button.p-button-danger");
    private ILocator PopupEliminarProveedor => _page.Locator(".p-dialog-footer");
    private ILocator BotonConfirmarEliminarProveedor => _page.Locator("button.p-confirmdialog-accept-button");

    public ProveedoresSumPage(IPage page) : base(page) { }

    public async Task CrearNuevoProveedorAsync(string nombre, string razonSocial, string cifNif, string web, string direccion, string telefono, string contacto, string correoElectronico, string estado, string observaciones)
    {
        await BotonNuevoProveedor.ClickAsync();

        if (!string.IsNullOrEmpty(nombre))
        {
            await InputNombreProveedor.FillAsync(nombre);
        }
        else
        {
            await InputNombreProveedor.FocusAsync();
            await _page.Keyboard.PressAsync("Space");
            await _page.Keyboard.PressAsync("Backspace");
        }

        await InputRazonSocialProveedor.FillAsync(razonSocial);
        await InputCifNifProveedor.FillAsync(cifNif);
        await InputWebProveedor.FillAsync(web);
        await InputDireccionProveedor.FillAsync(direccion);
        await InputTelefonoProveedor.FillAsync(telefono);
        await InputContactoProveedor.FillAsync(contacto);
        await InputEmailProveedor.FillAsync(correoElectronico);

        await EstadoProveedor.Locator(".p-dropdown-trigger, .p-select-dropdown").First.ClickAsync();

        var opcionEstado = _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").GetByText(estado, new() { Exact = true });
        await opcionEstado.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
        await opcionEstado.ClickAsync();

        await _page.WaitForTimeoutAsync(300);

        await ObservacionesProveedor.FillAsync(observaciones);
    }

    public async Task GuadarNuevoProveedorAsync()
    {
        await BotonGuardarProveedor.ClickAsync();
    }

    public async Task CancelarCreacionProveedorAsync()
    {
        await BotonCancelarEdicionProveedor.ClickAsync();
        await BotónConfirmarCancelarProveedor.ClickAsync();
    }

    public async Task FiltrarPorCodigoNombreProveedorAsync(string codigoProveedor)
    {
        await InputFiltroCodigoNombreProveedor.FillAsync(codigoProveedor);
        await BotonFiltrar.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> VerificarProveedorCreadaAsync(string nombreProveedor)
    {
        return await _page.Locator("td").GetByText(nombreProveedor, new() { Exact = true }).IsVisibleAsync();
    }

    public async Task<int> ObtenerNumeroFilasProveedoresAsync()
    {
        return await FilasTabla.CountAsync();
    }

    public async Task EditarProveedorAsync(string nombreProveedor, string nuevoNombre)
    {
        var filaProveedor = _page.Locator("tr").Filter(new() { HasText = nombreProveedor });

        await BotonMasOpciones.ClickAsync();

        await OpcionEditarProveedor.ClickAsync();

        await InputNombreProveedor.ClickAsync();
        await InputNombreProveedor.ClearAsync();
        await InputNombreProveedor.FillAsync(nuevoNombre);
    }

    public async Task GuardarCambiosProveedorsync()
    {
        await BotonGuardarProveedor.ClickAsync();
    }

    public async Task CancelarEdicionProveedorAsync()
    {
        await BotonCancelarEdicionProveedor.ClickAsync();
        await BotónConfirmarCancelarProveedor.ClickAsync();

        await BotonNuevoProveedor.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task EliminarProveedorAsync(string nombreProveedor)
    {
        var filaProveedor = _page.Locator("tr").Filter(new() { HasText = nombreProveedor });
        await BotonMasOpciones.ClickAsync();
        await OpcionEditarProveedor.ClickAsync();

        await BotonEliminarProveedor.ClickAsync();

        await BotonConfirmarEliminarProveedor.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

        await BotonConfirmarEliminarProveedor.ClickAsync();

        await PopupEliminarProveedor.WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 5000 });
    }

    public async Task IrAProveedoresSum()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/suppliers");
        await _page.WaitForURLAsync("**/suppliers");
    }
}