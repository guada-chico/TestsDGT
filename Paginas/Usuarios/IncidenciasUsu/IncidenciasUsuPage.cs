using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.IncidenciasUsu;

public class IncidenciasUsuPage
{
    private readonly IPage _page;
    
    private ILocator BotonNuevaIncidencia => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-plus") });
    private ILocator InputTituloIncidencia => _page.Locator("#titulo");
    private ILocator ComboPrioridad => _page.GetByRole(AriaRole.Combobox, new() { Name = "Media" });
    private ILocator InputMotivo => _page.Locator("#motivo");
    private ILocator InputIdPedido => _page.Locator("#idPedido");
    private ILocator InputArchivo => _page.Locator("input[type='file']");
    private ILocator InputDescripcion => _page.Locator("#descripcion");
    private ILocator BotonEnviarIncidencia => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-check") });
    private ILocator ToastMensaje => _page.Locator(".p-toast");

    private ILocator InputFiltroTituloIncidencia => _page.GetByPlaceholder("Buscar por título");
    private ILocator InputFiltroFechaIncidencia => _page.Locator("p-calendar input[placeholder='dd/mm/aaaa']");
    private ILocator BotonAplicarFiltro => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-filter") });
    private ILocator BotonLimpiarFiltros => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-times") });
    private ILocator FilasTablaIncidencias => _page.Locator("tbody tr");

    public IncidenciasUsuPage(IPage page)
    {
        _page = page;
    }

    public async Task CrearNuevaIncidenciaAsync(string titulo, string prioridad, string motivo, string idPedido, string rutaArchivo, string descripcion)
    {
        await BotonNuevaIncidencia.ClickAsync();

        if (!string.IsNullOrEmpty(titulo))
        {
            await InputTituloIncidencia.FillAsync(titulo);
        }
        else
        {
            await InputTituloIncidencia.FocusAsync();
            await _page.Keyboard.PressAsync("Space");
            await _page.Keyboard.PressAsync("Backspace");
        }

        await ComboPrioridad.ClickAsync();
        await _page.Locator($"#prioridad_{prioridad}").ClickAsync();

        await InputMotivo.FillAsync(motivo);
        await InputIdPedido.FillAsync(idPedido);
        await InputArchivo.SetInputFilesAsync(rutaArchivo);
        await InputDescripcion.FillAsync(descripcion);

        await BotonEnviarIncidencia.DispatchEventAsync("click");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<string> ObtenerMensajeToastAsync()
    {
        try
        {
            await Assertions.Expect(ToastMensaje).ToBeVisibleAsync(new() { Timeout = 5000 });
            return await ToastMensaje.InnerTextAsync();
        }
        catch (Exception)
        {
            Assert.Fail("No se mostró ningún mensaje toast tras 5 segundos.");
            return string.Empty; 
        }
    }

    public async Task FiltrarPorTituloIncidenciaAsync(string titulo)
    {
        await InputFiltroTituloIncidencia.FillAsync(titulo);
        await BotonAplicarFiltro.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task FiltrarPorEstadoIncidenciaAsync(string estado)
    {
        var comboFiltroEstado = _page.Locator("p-dropdown").Filter(new() { HasText = "Todos los estados" });

        if (await comboFiltroEstado.CountAsync() == 0)
        {
            comboFiltroEstado = _page.Locator("p-dropdown").GetByRole(AriaRole.Combobox);
        }

        await comboFiltroEstado.ClickAsync();

        var opcionSeleccionar = _page.Locator(".p-dropdown-item, p-dropdownitem").GetByText(estado, new() { Exact = true });

        await opcionSeleccionar.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
        await opcionSeleccionar.ClickAsync();

        await BotonAplicarFiltro.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task FiltrarPorFechaIncidenciaAsync(string fechaIncidencia)
    {
        await InputFiltroFechaIncidencia.ClearAsync();
        await InputFiltroFechaIncidencia.FillAsync(fechaIncidencia);
        await _page.Keyboard.PressAsync("Tab");
        await BotonAplicarFiltro.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> VerificarIncidenciaCreadaAsync(string titulo)
    {
        try
        {
            var celdaIncidencia = _page.Locator("td").GetByText(titulo, new() { Exact = true }).First;
            await Assertions.Expect(celdaIncidencia).ToBeVisibleAsync(new() { Timeout = 15000 });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<int> ObtenerNumeroFilasIncidenciasAsync()
    {
        return await FilasTablaIncidencias.CountAsync();
    }

    public async Task LimpiarFiltrosAsync()
    {
        await BotonLimpiarFiltros.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task IrAIncidencias()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/mis-incidencias");
        await _page.WaitForURLAsync("**/mis-incidencias");
    }
}