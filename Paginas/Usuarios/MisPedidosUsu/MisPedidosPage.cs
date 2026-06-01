using Microsoft.Playwright;

namespace TestsDGT.Paginas.Usuarios.MisPedidosUsu;

public class MisPedidosPage
{
    private readonly IPage _page;

    private ILocator InputFiltroNombre => _page.GetByPlaceholder("Nombre del artículo");
    private ILocator EstadoPedido => _page.Locator("p-dropdown").GetByRole(AriaRole.Combobox);
    private ILocator InputFiltroCodigo => _page.GetByPlaceholder("Código");
    private ILocator FechaDesde => _page.Locator("#startDate input[placeholder='dd/mm/aaaa']");
    private ILocator FechaHasta => _page.Locator("#endDate input[placeholder='dd/mm/aaaa']");
    private ILocator BotonFiltrar => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-filter") });
    private ILocator BotonLimpiarFiltros => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-times") });

    private ILocator BotonCatalogo => _page.GetByRole(AriaRole.Button, new() { Name = "Catálogo" });

    private ILocator BotonVerMas => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-ellipsis-v").First });
    private ILocator OpcionDetallePedido => _page.GetByRole(AriaRole.Menuitem, new() { Name = "Detalle del pedido" });
    private ILocator OpcionVerDevolucion => _page.GetByRole(AriaRole.Menuitem, new() { Name = "Ver devolución" });

    private ILocator BotonDescargarDocumentoPedido => _page.GetByRole(AriaRole.Button, new() { Name = "Descargar" });
    private ILocator BotonDescargarEtiquetaDevolucion => _page.GetByRole(AriaRole.Button, new() { Name = "Etiqueta de Envío" });
    private ILocator BotonImprimirEtiquetaDevolucion => _page.GetByRole(AriaRole.Button, new() { Name = "Imprimir Etiqueta" });
    private ILocator BotonVerAlbaran => _page.GetByRole(AriaRole.Button, new() { Name = "Ver Albarán" });
    private ILocator BotonImprimirAlbaranDevolucion => _page.GetByRole(AriaRole.Button, new() { Name = "Imprimir Albarán" });

    private ILocator InputComentarioInfo => _page.Locator("#nuevoComentario");
    private ILocator ImportArchivo => _page.Locator("#documentoJustificativo");
    private ILocator BotonEnviarAValidar => _page.GetByRole(AriaRole.Button, new() { Name = "Enviar a validar" });

    private ILocator CabecerasTabla => _page.Locator("th");
    private ILocator FilasTabla => _page.Locator("tbody tr");
    private ILocator CeldaTabla(string texto) => _page.Locator("td").Filter(new() { HasText = texto });

    private ILocator ToastMensaje => _page.Locator(".p-toast");

    public MisPedidosPage(IPage page)
    {
        _page = page;
    }

    public async Task FiltrarPorNombreArticuloAsync(string nombre)
    {
        await InputFiltroNombre.FillAsync(nombre);
        await BotonFiltrar.ClickAsync();
    }

    public async Task FiltrarPorEstadoAsync(string estado)
    {
        await EstadoPedido.ClickAsync();
        var opcion = _page.Locator("p-dropdownitem").GetByText(estado, new() { Exact = true });

        await opcion.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
        await opcion.ClickAsync();

        await BotonFiltrar.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task FiltrarPorRangoFechasAsync(string desde, string hasta)
    {
        if (!string.IsNullOrEmpty(desde))
        {
            await FechaDesde.ClearAsync();
            await FechaDesde.FillAsync(desde);
            await _page.Keyboard.PressAsync("Tab");
        }
        if (!string.IsNullOrEmpty(hasta))
        {
            await FechaHasta.ClearAsync();
            await FechaHasta.FillAsync(hasta);
            await _page.Keyboard.PressAsync("Tab");
        }
        await BotonFiltrar.ClickAsync();
    }

    public async Task FiltrarPorCodigoAsync(string codigo)
    {
        await InputFiltroCodigo.FillAsync(codigo);
        await BotonFiltrar.ClickAsync();
    }

    public async Task LimpiarFiltrosAsync()
    {
        await BotonLimpiarFiltros.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
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
        return await InputFiltroNombre.InputValueAsync();
    }

    public async Task<List<string>> ObtenerValoresDeColumnaAsync(string nombreColumna)
    {
        var cabeceras = await CabecerasTabla.AllInnerTextsAsync();
        int indiceColumna = cabeceras.ToList().FindIndex(t => t.Contains(nombreColumna));

        if (indiceColumna == -1) return new List<string>();

        await _page.Locator("tbody tr td").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        return (await _page.Locator($"tbody tr td:nth-child({indiceColumna + 1})").AllInnerTextsAsync()).ToList();
    }

    public async Task VerDetallePedidoDeMisPedidosAsync(string codigoPedido)
    {
        await FiltrarPorCodigoAsync(codigoPedido);

        var filaProducto = _page.GetByRole(AriaRole.Row).Filter(new() { HasText = codigoPedido }).First;
        await filaProducto.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

        await filaProducto.Locator(BotonVerMas).First.ClickAsync();
        await _page.WaitForTimeoutAsync(200); 

        await OpcionDetallePedido.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 4000 });
        await OpcionDetallePedido.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task DescargarDocumentoPedidoAsync()
    {
        await BotonDescargarDocumentoPedido.ClickAsync();
    }

    public async Task VerDevolucionPedidoDeMisPedidosAsync(string codigoPedido)
    {
        await FiltrarPorCodigoAsync(codigoPedido);

        var filaProducto = _page.GetByRole(AriaRole.Row).Filter(new() { HasText = codigoPedido }).First;
        await filaProducto.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
        
        await filaProducto.Locator(BotonVerMas).First.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        await OpcionVerDevolucion.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 4000 });
        await OpcionVerDevolucion.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task DescargarEtiquetaDevolucionAsync()
    {
        await BotonDescargarEtiquetaDevolucion.ClickAsync();
        await BotonImprimirEtiquetaDevolucion.ClickAsync();
    }

    public async Task ImprimirAlbaranDevolucionAsync()
    {
        await BotonVerAlbaran.ClickAsync();
        await BotonImprimirAlbaranDevolucion.ClickAsync();
    }

    public async Task RellenarInfoPedidoAsync(string comentario, string archivo)
    {
        await InputComentarioInfo.FillAsync(comentario);
        await ImportArchivo.SetInputFilesAsync(archivo);
        await BotonEnviarAValidar.ClickAsync();
    }

    public async Task<string> ObtenerMensajeToastAsync()
    {
        await ToastMensaje.WaitForAsync();
        return await ToastMensaje.InnerTextAsync();
    }

    public async Task IrAMisPedidosAsync()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/orders-list");
        await _page.WaitForURLAsync("**/orders-list");
    }

    public async Task IrACatalogoAsync()
    {
        await BotonCatalogo.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}