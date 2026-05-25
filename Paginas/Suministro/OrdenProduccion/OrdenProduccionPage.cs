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

    private ILocator FilasFormularioArticulos => _page.Locator(".fila-articulo");
    private ILocator ComboCodigoPrenda => _page.Locator("p-dropdown[placeholder='Código Prenda']");
    private ILocator ComboArticulo => _page.Locator("p-dropdown[placeholder='Artículo']");
    private ILocator ComboTalla => _page.Locator("p-dropdown[name='talla']");
    private ILocator InputCantidadOrden => _page.Locator("input[name='cantidad']");

    private ILocator BotonAgregarArticulo => _page.GetByRole(AriaRole.Button, new() { Name = "Agregar", Exact = true });
    private ILocator BotonEliminarArticulos => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-trash") });
    private ILocator BotonGuardarOrden => _page.GetByRole(AriaRole.Button, new() { Name = "Guardar", Exact = true });
    private ILocator BotonCancelarOrden => _page.GetByRole(AriaRole.Button, new() { Name = "Cancelar", Exact = true });

    private ILocator BotonConfirmarEnvioOP => _page.Locator("button.p-confirmdialog-accept-button");
    private ILocator BotonCancelarEnvioOP => _page.Locator("button.p-confirmdialog-reject-button");

    private ILocator ToastMensaje => _page.Locator(".p-toast:visible");
    private ILocator ToastMensajeDetalle => ToastMensaje.Locator(".p-toast-detail"); 
    
    private ILocator InputFiltroCodigoExpedienteOP => _page.GetByPlaceholder("Buscar por código o expediente");
    private ILocator ComboFiltroProveedorOD => _page.Locator("p-dropdown[placeholder='Todos los proveedores']");
    private ILocator BotonAplicarFiltro => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-filter") });
    private ILocator BotonLimpiarFiltros => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-times") });
    private ILocator FilasTablaOrdenProduccion => _page.Locator("tbody tr");

    private ILocator BotonMasOpciones => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-ellipsis-v") });
    private ILocator BotonImprimirDesdeMasOpciones => _page.GetByRole(AriaRole.Menuitem, new() { Name = "Imprimir" });
    private ILocator OpcionVerDetalleOrdenProduccion => _page.GetByRole(AriaRole.Menuitem, new() { Name = "Ver detalle" });

    private ILocator ComboCambiarEstadoOrdenProduccion => _page.Locator("p-dropdown[placeholder='Seleccione un estado']");
    private ILocator BotonActualizarEstado => _page.GetByRole(AriaRole.Button, new() { Name = "Actualizar" });
    private ILocator BotonImprimirOrdenProduccion => _page.GetByRole(AriaRole.Button, new() { Name = "Imprimir" });
    private ILocator BotonEnviarCorreoAProveedor => _page.GetByRole(AriaRole.Button, new() { Name = "Enviar" });

    public OrdenProduccionPage(IPage page)
    {
        _page = page;
    }

    public async Task IrACrearOrdenProduccionAsync()
    {
        await BotonNuevaOrdenProduccion.ClickAsync();
    }

    public async Task DatosNuevaOrdenProduccionAsync(string expediente, string lote, int duracionContrato, string observaciones, string proveedor)
    {
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
    }

    public async Task ArticuloNuevaOrdenProduccionAsync(string valorPrenda, string talla, int cantidad, bool esPorCodigo = true)
    {
        var filaActiva = FilasFormularioArticulos.First;

        if (esPorCodigo)
        {
            await filaActiva.Locator(ComboCodigoPrenda).Locator(".p-select-dropdown, .p-dropdown-trigger").ClickAsync();
        }
        else
        {
            await filaActiva.Locator(ComboArticulo).Locator(".p-select-dropdown, .p-dropdown-trigger").ClickAsync();
        }

        var filtroPrenda = _page.Locator(".p-dropdown-filter:visible, .p-select-filter:visible");
        await filtroPrenda.FillAsync(valorPrenda);
        await _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").First.ClickAsync();
        await _page.WaitForTimeoutAsync(500);

        await filaActiva.Locator(ComboTalla).Locator(".p-select-dropdown, .p-dropdown-trigger").ClickAsync();
        var opcionTalla = _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").GetByText(talla, new() { Exact = true });
        await opcionTalla.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
        await opcionTalla.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        await filaActiva.Locator(InputCantidadOrden).FillAsync(cantidad.ToString());
    }

    public async Task AgregarArticuloODAsync()
    {
        await BotonAgregarArticulo.ClickAsync();
        await _page.WaitForTimeoutAsync(600);
    }

    public async Task GuardarNuevaOrdenProduccionAsync()
    {
        await FinalizarYGuardarOrdenAsync();
    }

    public async Task CancelarNuevaOrdenProduccionAsync()
    {
        await BotonCancelarOrden.ClickAsync();
    }

    public async Task EliminarPrimerArticuloDeTablaAsync()
    {
        await BotonEliminarArticulos.First.ClickAsync();
        await _page.WaitForTimeoutAsync(400);
    }

    public async Task VerDetalleOrdenProduccionAsync(string expedienteOrdenProduccion)
    {
        var filaOrdenProduccion = _page.Locator("tr").Filter(new() { HasText = expedienteOrdenProduccion });

        await filaOrdenProduccion.Locator(BotonMasOpciones).ClickAsync();

        await OpcionVerDetalleOrdenProduccion.ClickAsync();  
    }

    public async Task CambiarEstadoOrdenProduccionAsync(string nuevoEstado)
    {
        await ComboCambiarEstadoOrdenProduccion.Locator(".p-select-dropdown").First.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        var opcionSeleccionar = _page.Locator(".p-dropdown-item:visible, .p-select-option:visible").GetByText(nuevoEstado, new() { Exact = true });
        await opcionSeleccionar.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
        await opcionSeleccionar.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        await BotonActualizarEstado.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ImprimirDesdeVerDetallesOP()
    {
        await BotonImprimirOrdenProduccion.ClickAsync();
    }

    public async Task ImprimirDesdeMasOpcionesOPAsync(string expedienteOrdenProduccion)
    {
        var filaOrdenProduccion = _page.Locator("tr").Filter(new() { HasText = expedienteOrdenProduccion });

        await filaOrdenProduccion.Locator(BotonMasOpciones).ClickAsync();

        await BotonImprimirDesdeMasOpciones.ClickAsync();
    }

    public async Task EnviarCorreoAProveedorAsync()
    {
        await BotonEnviarCorreoAProveedor.ClickAsync();
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

    public async Task FinalizarYGuardarOrdenAsync()
    {
        await BotonGuardarOrden.ClickAsync();

        await BotonConfirmarEnvioOP.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

        await BotonConfirmarEnvioOP.ClickAsync();
    }

    public async Task FiltrarPorCodigoExpedienteODAsync(string codigoExpediente)
    {
        await InputFiltroCodigoExpedienteOP.FillAsync(codigoExpediente);
        await BotonAplicarFiltro.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task FiltrarPorProveedorOPAsync(string proveedor)
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