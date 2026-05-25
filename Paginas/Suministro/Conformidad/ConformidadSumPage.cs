using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Suministro.Conformidad;

public class ConformidadSumPage
{
    private readonly IPage _page;

    private ILocator InputFiltroCodigoOrden => _page.GetByPlaceholder("Buscar por Código de Orden");
    private ILocator FiltroComboProveedor => _page.Locator("p-dropdown[placeholder='Todos los proveedores']");
    private ILocator BotonFiltrar => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-filter") });
    private ILocator BotonLimpiarFiltro => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-times") });

    private ILocator FechaVerificacion => _page.Locator("#fechaVerificacion-0 input[placeholder='dd/mm/aaaa']");
    private ILocator InputObservaciones => _page.GetByPlaceholder("Escriba aquí las observaciones sobre esta verficación...");
    private ILocator AdjuntarFichero => _page.Locator("#archivo-0");
    private ILocator InputCantidadRecibida => _page.GetByPlaceholder("0");
    private ILocator BotonRegistrarConformidad => _page.GetByRole(AriaRole.Button, new() { Name = "Registrar conformidad" });
    private ILocator BotonCancelarVerificacion => _page.GetByRole(AriaRole.Button, new() { Name = "Cancelar" });
    private ILocator AgregarNuevaVerificacion => _page.GetByRole(AriaRole.Button, new() { Name = "Añadir Nueva Verificación" });


    private ILocator ToastMensaje => _page.Locator(".p-toast:visible");
    private ILocator ToastMensajeDetalle => ToastMensaje.Locator(".p-toast-detail"); 
   
    private ILocator FilasTablaConformidadSum => _page.Locator("tbody tr");

    private ILocator BotonMasOpciones => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-ellipsis-v") });
    private ILocator OpcionVerDetalleConformidadSum => _page.GetByRole(AriaRole.Menuitem, new() { Name = "Ver detalle" });


    public ConformidadSumPage(IPage page)
    {
        _page = page;
    }

    public async Task FiltrarPorCodigoOrdenAsync(string codigo)
    {
        await InputFiltroCodigoOrden.FillAsync(codigo);
        await BotonFiltrar.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task FiltrarPorProveedorAsync(string proveedor)
    {
        await FiltroComboProveedor.Locator(".p-select-dropdown, .p-dropdown-trigger").First.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        var opcionSeleccionar = _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").GetByText(proveedor, new() { Exact = true });
        await opcionSeleccionar.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
        await opcionSeleccionar.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        await BotonFiltrar.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task LimpiarFiltrosAsync()
    {
        await BotonLimpiarFiltro.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task VerDetalleConformidadSumAsync(string expedienteConformidadSum)
    {
        var filaConformidadSum = _page.Locator("tr").Filter(new() { HasText = expedienteConformidadSum });

        await filaConformidadSum.Locator(BotonMasOpciones).ClickAsync();

        await OpcionVerDetalleConformidadSum.ClickAsync();
    }

    public async Task NuevaVerificacionSumAsync(string fecha, string observaciones, string rutaArchivo, string cantidadRecibida)
    {
        await FechaVerificacion.FillAsync(fecha);
        await InputObservaciones.FillAsync(observaciones);
        await AdjuntarFichero.SetInputFilesAsync(rutaArchivo);
        await InputCantidadRecibida.FillAsync(cantidadRecibida);

        await BotonRegistrarConformidad.ClickAsync();
    }

    public async Task CancelarVerificacionConformidadSumAsync()
    {
        await BotonCancelarVerificacion.ClickAsync();
    }

    

    /*
    public async Task FinalizarYGuardarOrdenAsync()
    {
        await BotonGuardarOrden.ClickAsync();

        await BotonConfirmarEnvioOP.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

        await BotonConfirmarEnvioOP.ClickAsync();
    }
    */

    public async Task<bool> VerificarConformidadSumAsync(string expediente)
    {
        try
        {
            var celdaOrden = _page.Locator("td").GetByText(expediente, new() { Exact = true }).First;
            return await celdaOrden.IsVisibleAsync();
        }
        catch { return false; }
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

    public async Task<int> ObtenerNumeroFilasConformidadSumAsync()
    {
        return await FilasTablaConformidadSum.CountAsync();
    }

    public async Task IrAConformidadSuministro()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/conformidad-suministro");
        await _page.WaitForURLAsync("**/conformidad-suministro");
    }
}