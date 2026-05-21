using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Suministro.Proveedores;

public class ProveedoresSumPage
{
    private readonly IPage _page;

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
    private ILocator BotonCancelarProveedor => _page.GetByRole(AriaRole.Button, new() { Name = "Cancelar", Exact = true });
    private ILocator BotónConfirmarCancelarProveedor => _page.GetByRole(AriaRole.Button, new() { Name = "Salir sin guardar", Exact = true });
    private ILocator ToastMensaje => _page.Locator(".p-toast");

    private ILocator InputFiltroCodigoNombreProveedor => _page.GetByPlaceholder("Buscar por código o nombre");
    private ILocator BotonAplicarFiltro => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-filter") });
    private ILocator BotonLimpiarFiltros => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-times") });
    private ILocator FilasTablaProveedores => _page.Locator("tbody tr");

    private ILocator BotonMasOpcionesProveedor => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-ellipsis-v") }).First;
    private ILocator OpcionEditarProveedor => _page.GetByRole(AriaRole.Menuitem, new() { Name = "Editar", Exact = true });
    private ILocator BotonEliminarProveedor => _page.GetByRole(AriaRole.Button, new() { Name = "Eliminar", Exact = true }).First;
    private ILocator BotonConfirmarEliminarProveedor => _page.GetByRole(AriaRole.Button, new() { Name = "Eliminar", Exact = true });

    public ProveedoresSumPage(IPage page)
    {
        _page = page;
    }

    public async Task CrearFormularioNuevoProveedorAsync(string nombre, string razonSocial, string cifNif, string web, string direccion, string telefono, string contacto, string correoElectronico, string estado, string observaciones)
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

    public async Task CrearNuevoProveedorAsync(string nombre, string razonSocial, string cifNif, string web, string direccion, string telefono, string contacto, string correoElectronico, string estado, string observaciones)
    {
        await CrearFormularioNuevoProveedorAsync(nombre, razonSocial, cifNif, web, direccion, telefono, contacto, correoElectronico, estado, observaciones);
        await BotonGuardarProveedor.ClickAsync();
    }

    public async Task CancelarCreacionProveedorAsync(string nombre, string razonSocial, string cifNif, string web, string direccion, string telefono, string contacto, string correoElectronico, string estado, string observaciones)
    {
        await CrearFormularioNuevoProveedorAsync(nombre, razonSocial, cifNif, web, direccion, telefono, contacto, correoElectronico, estado, observaciones);
        await BotonCancelarProveedor.ClickAsync();
        await BotónConfirmarCancelarProveedor.ClickAsync();
    }

    public async Task<string> ObtenerMensajeToastAsync()
    {
        await ToastMensaje.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 8000 });
        return await ToastMensaje.InnerTextAsync();
    }

    public async Task FiltrarPorCodigoNombreProveedorAsync(string codigoProveedor)
    {
        await InputFiltroCodigoNombreProveedor.FillAsync(codigoProveedor);
        await BotonAplicarFiltro.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> VerificarProveedorCreadaAsync(string nombreProveedor)
    {
        try
        {
            var celdaProveedor = _page.Locator("td").GetByText(nombreProveedor, new() { Exact = true }).First;
            await Assertions.Expect(celdaProveedor).ToBeVisibleAsync(new() { Timeout = 15000 });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
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

    public async Task<int> ObtenerNumeroFilasProveedoresAsync()
    {
        return await FilasTablaProveedores.CountAsync();
    }

    public async Task LimpiarFiltrosAsync()
    {
        await BotonLimpiarFiltros.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task EditarProveedorAsync(string nombreProveedor, string nuevoNombre)
    {
        var filaProveedor = _page.Locator("tr").Filter(new() { HasText = nombreProveedor });

        await BotonMasOpcionesProveedor.ClickAsync();

        await OpcionEditarProveedor.ClickAsync();

        await InputNombreProveedor.ClickAsync();
        await InputNombreProveedor.ClearAsync();
        await InputNombreProveedor.FillAsync(nuevoNombre);

        await BotonGuardarProveedor.ClickAsync();
    }

    public async Task EliminarProveedorAsync(string nombreProveedor)
    {
        var filaProveedor = _page.Locator("tr").Filter(new() { HasText = nombreProveedor });
        await BotonMasOpcionesProveedor.ClickAsync();
        await OpcionEditarProveedor.ClickAsync();

        await BotonEliminarProveedor.ClickAsync();

        await BotonConfirmarEliminarProveedor.ClickAsync();
    }

    public async Task IrAProveedoresSum()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/suppliers");
        await _page.WaitForURLAsync("**/suppliers");
    }
}