using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Suministro.Conformidad;

public class ConformidadSumPage : SuministroBasePage
{
    private ILocator InputFiltroCodigoOrden => _page.GetByPlaceholder("Buscar por Código de Orden");
    private ILocator FiltroComboProveedor => _page.Locator("p-dropdown[placeholder='Todos los proveedores']");

    private ILocator FechaVerificacion => _page.Locator("#fechaVerificacion-0 input[placeholder='dd/mm/aaaa']");
    private ILocator InputObservaciones => _page.GetByPlaceholder("Escriba aquí las observaciones sobre esta verficación...");
    private ILocator AdjuntarFichero => _page.Locator("#archivo-0");
    private ILocator InputCantidadRecibida => _page.GetByPlaceholder("0");
    private ILocator BotonRegistrarConformidad => _page.GetByRole(AriaRole.Button, new() { Name = "Registrar conformidad" });
    private ILocator BotonCancelarVerificacion => _page.GetByRole(AriaRole.Button, new() { Name = "Cancelar" });

   
    private ILocator FilasTablaConformidadSum => _page.Locator("tbody tr");

    private ILocator OpcionVerDetalleConformidadSum => _page.GetByRole(AriaRole.Menuitem, new() { Name = "Ver detalle" });


    public ConformidadSumPage(IPage page) : base(page) { }

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

    public async Task<int> ObtenerNumeroFilasConformidadSumAsync()
    {
        return await FilasTablaConformidadSum.CountAsync();
    }
}