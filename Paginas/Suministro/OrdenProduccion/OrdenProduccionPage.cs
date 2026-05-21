using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Suministro.OrdenProduccion;

public class OrdenProduccionPage
{
    private readonly IPage _page;

    private ILocator BotonNuevaOrdenProduccion => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-plus") });
    private ILocator InputExpedienteOrden => _page.GetByPlaceholder("Introduzca el expediente");
    private ILocator InputLoteOrden => _page.GetByPlaceholder("Introduzca el lote");
    private ILocator InputDuracionContrato => _page.GetByPlaceholder("Ej: 30");
    private ILocator InputObservacionesOrden => _page.GetByPlaceholder("Máx. 500 caracteres");
    private ILocator ComboProveedor => _page.Locator("p-dropdown[formcontrolname='idProveedor']");
    private ILocator ComboCodigoPrenda => _page.Locator("p-dropdown[placeholder='Código Prenda']");
    private ILocator ComboArticulo => _page.Locator("p-dropdown[placeholder='Artículo']");
    private ILocator ComboTalla => _page.Locator("p-dropdown[name='talla']");
    private ILocator InputCantidadOrden => _page.Locator("input[name='cantidad']");
    private ILocator BotonAgregarArticulo => _page.GetByRole(AriaRole.Button, new() { Name = "Agregar", Exact = true });
    private ILocator BotonEliminarArticulos => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-trash") });
    private ILocator BotonGuardarOrden => _page.GetByRole(AriaRole.Button, new() { Name = "Guardar", Exact = true });
    private ILocator BotonCancelarOrden => _page.Locator("button").Filter(new() { Has = _page.Locator(".p-button-secondary") });
    private ILocator BotonConfirmarEnvioOP => _page.Locator("button.p-confirmdialog-accept-button");
    private ILocator BotonCancelarEnvioOP => _page.Locator("button.p-confirmdialog-reject-button");
    private ILocator ToastMensaje => _page.Locator(".p-toast");

    private ILocator InputFiltroCodigoExpedienteOP => _page.GetByPlaceholder("Buscar por código o expediente");
    private ILocator ComboFiltroProveedorOD => _page.Locator("p-dropdown[placeholder='Todos los proveedores']");
    private ILocator BotonAplicarFiltro => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-filter") });
    private ILocator BotonLimpiarFiltros => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-times") });
    private ILocator FilasTablaOrdenProduccion => _page.Locator("tbody tr");

    public OrdenProduccionPage(IPage page)
    {
        _page = page;
    }

    public async Task<bool> CrearNuevaOrdenProduccionAsync(string expediente, string lote, int duracionContrato, string observaciones, string proveedor, string valorPrenda, string talla, int cantidad, bool esPorCodigo = true)
    {
        await BotonNuevaOrdenProduccion.ClickAsync();

        if (!string.IsNullOrEmpty(expediente))
        {
            await InputExpedienteOrden.FillAsync(expediente);
        }
        else
        {
            await InputExpedienteOrden.FocusAsync();
            await _page.Keyboard.PressAsync("Space");
            await _page.Keyboard.PressAsync("Backspace");
        }

        await InputLoteOrden.FillAsync(lote);
        await InputDuracionContrato.FillAsync(duracionContrato.ToString());
        await InputObservacionesOrden.FillAsync(observaciones);

        await ComboProveedor.Locator(".p-select-dropdown").ClickAsync();
        var filtroProveedor = _page.Locator(".p-dropdown-filter:visible, .p-select-filter:visible");
        await filtroProveedor.FillAsync(proveedor);
        await _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").First.ClickAsync();
        await _page.WaitForTimeoutAsync(300);

        if (esPorCodigo)
        {
            await ComboCodigoPrenda.Locator(".p-select-dropdown").ClickAsync();
        }
        else
        {
            await ComboArticulo.Locator(".p-select-dropdown").ClickAsync();
        }

        var filtroPrenda = _page.Locator(".p-dropdown-filter:visible, .p-select-filter:visible");
        await filtroPrenda.FillAsync(valorPrenda);
        await _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").First.ClickAsync();
        await _page.WaitForTimeoutAsync(500);

        await ComboTalla.Locator(".p-select-dropdown").ClickAsync();
        var opcionTalla = _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").GetByText(talla, new() { Exact = true });
        await opcionTalla.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
        await opcionTalla.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        await InputCantidadOrden.FillAsync(cantidad.ToString());

        await BotonAgregarArticulo.ClickAsync();
        await _page.WaitForTimeoutAsync(400);

        return await FinalizarYGuardarOrdenAsync();
    }

    public async Task IniciarNuevaOrdenSinGuardarAsync(string expediente, string lote, int duracionContrato, string observaciones, string proveedor, string valorPrenda, string talla, int cantidad, bool esPorCodigo = true)
    {
        await BotonNuevaOrdenProduccion.ClickAsync();

        if (!string.IsNullOrEmpty(expediente))
        {
            await InputExpedienteOrden.FillAsync(expediente);
        }
        else
        {
            await InputExpedienteOrden.FocusAsync();
            await _page.Keyboard.PressAsync("Space");
            await _page.Keyboard.PressAsync("Backspace");
        }

        await InputLoteOrden.FillAsync(lote);
        await InputDuracionContrato.FillAsync(duracionContrato.ToString());
        await InputObservacionesOrden.FillAsync(observaciones);

        await ComboProveedor.Locator(".p-select-dropdown").First.ClickAsync();
        var filtroProveedor = _page.Locator(".p-dropdown-filter:visible, .p-select-filter:visible");
        await filtroProveedor.FillAsync(proveedor);
        await _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").First.ClickAsync();
        await _page.WaitForTimeoutAsync(300);

        // Capturamos únicamente el contenedor de la primera fila del formulario
        var primeraFilaFormulario = _page.Locator(".fila-articulo").First;

        if (esPorCodigo)
        {
            // Buscamos el combo de código SÓLO dentro del ámbito de la primera fila
            await primeraFilaFormulario.Locator("p-dropdown[placeholder='Código Prenda'] .p-select-dropdown").ClickAsync();
        }
        else
        {
            // Buscamos el combo de artículo SÓLO dentro del ámbito de la primera fila
            await primeraFilaFormulario.Locator("p-dropdown[placeholder='Artículo'] .p-select-dropdown").ClickAsync();
        }

        var filtroPrenda = _page.Locator(".p-dropdown-filter:visible, .p-select-filter:visible");
        await filtroPrenda.FillAsync(valorPrenda);
        await _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").First.ClickAsync();
        await _page.WaitForTimeoutAsync(500);

        // Buscamos la talla SÓLO dentro del ámbito de la primera fila
        await primeraFilaFormulario.Locator("p-dropdown[name='talla'] .p-select-dropdown").ClickAsync();
        var opcionTalla = _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").GetByText(talla, new() { Exact = true });
        await opcionTalla.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
        await opcionTalla.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        // Buscamos el input de cantidad SÓLO dentro del ámbito de la primera fila
        await primeraFilaFormulario.Locator("input[name='cantidad']").FillAsync(cantidad.ToString());

        await BotonAgregarArticulo.ClickAsync();
        await _page.WaitForTimeoutAsync(600);
    }

    public async Task AgregarArticuloODAsync(string valorPrenda, string talla, int cantidad, bool esPorCodigo = true)
    {
        // Volvemos a fijar el foco de forma estricta en el contenedor de la fila superior limpia
        var primeraFilaFormulario = _page.Locator(".fila-articulo").First;

        if (esPorCodigo)
        {
            await primeraFilaFormulario.Locator("p-dropdown[placeholder='Código Prenda'] .p-select-dropdown").ClickAsync();
        }
        else
        {
            await primeraFilaFormulario.Locator("p-dropdown[placeholder='Artículo'] .p-select-dropdown").ClickAsync();
        }

        var filtroPrenda = _page.Locator(".p-dropdown-filter:visible, .p-select-filter:visible");
        await filtroPrenda.FillAsync(valorPrenda);
        await _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").First.ClickAsync();
        await _page.WaitForTimeoutAsync(500);

        await primeraFilaFormulario.Locator("p-dropdown[name='talla'] .p-select-dropdown").ClickAsync();
        var opcionTalla = _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").GetByText(talla, new() { Exact = true });
        await opcionTalla.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
        await opcionTalla.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        await primeraFilaFormulario.Locator("input[name='cantidad']").FillAsync(cantidad.ToString());

        await BotonAgregarArticulo.ClickAsync();
        await _page.WaitForTimeoutAsync(600);
    }

    public async Task EliminarPrimerArticuloDeTablaAsync()
    {
        await BotonEliminarArticulos.First.ClickAsync();
        await _page.WaitForTimeoutAsync(400);
    }

    public async Task CancelarEnPopUpYSalirAsync()
    {
        await BotonGuardarOrden.DispatchEventAsync("click");
        await BotonConfirmarEnvioOP.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 4000 });

        await BotonCancelarEnvioOP.ClickAsync();
        await _page.WaitForTimeoutAsync(300);

        await BotonCancelarOrden.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> FinalizarYGuardarOrdenAsync()
    {
        await BotonGuardarOrden.DispatchEventAsync("click");
        await BotonConfirmarEnvioOP.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 4000 });

        var respuestaServidorTask = _page.WaitForResponseAsync(r =>
            string.Equals(r.Request.Method, "POST", StringComparison.OrdinalIgnoreCase));

        await BotonConfirmarEnvioOP.ClickAsync();

        var respuesta = await respuestaServidorTask;

        return respuesta.Status == 200 || respuesta.Status == 201;
    }

    public async Task FiltrarPorCodigoExpedienteODAsync(string codigoExpediente)
    {
        await InputFiltroCodigoExpedienteOP.FillAsync(codigoExpediente);
        await BotonAplicarFiltro.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task FiltrarPorProveedorODAsync(string proveedor)
    {
        await ComboFiltroProveedorOD.Locator(".p-select-dropdown, .p-dropdown-trigger").First.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        var opcionSeleccionar = _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").GetByText(proveedor, new() { Exact = true });
        await opcionSeleccionar.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
        await opcionSeleccionar.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        await BotonAplicarFiltro.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> VerificarOrdenProduccionCreadaAsync(string expediente)
    {
        try
        {
            var celdaOrden = _page.Locator("td").GetByText(expediente, new() { Exact = true }).First;
            await Assertions.Expect(celdaOrden).ToBeVisibleAsync(new() { Timeout = 15000 });
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

    public async Task<int> ObtenerNumeroFilasOrdenesProduccionAsync()
    {
        return await FilasTablaOrdenProduccion.CountAsync();
    }

    public async Task LimpiarFiltrosAsync()
    {
        await BotonLimpiarFiltros.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task IrAOrdenProduccion()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/orden-produccion/listado");
        await _page.WaitForURLAsync("**/orden-produccion/listado");
    }
}