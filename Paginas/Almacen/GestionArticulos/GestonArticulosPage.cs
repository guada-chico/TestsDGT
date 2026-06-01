using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Almacen.GestionArticulos;

public class GestionArticulosPage
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
    private ILocator BotonGuardarOrden => _page.GetByRole(AriaRole.Button, new() { Name = "Guardar", Exact = true });
    private ILocator BotonConfirmarEnvioOP => _page.Locator("button.p-confirmdialog-accept-button");

    private ILocator InputFiltroCodigoOrden => _page.GetByPlaceholder("Buscar por Código de Orden");
    private ILocator FechaVerificacion => _page.Locator("#fechaVerificacion-0 input[placeholder='dd/mm/aaaa']");
    private ILocator InputObservaciones => _page.GetByPlaceholder("Escriba aquí las observaciones sobre esta verficación...");
    private ILocator AdjuntarFichero => _page.Locator("#archivo-0");
    private ILocator InputCantidadRecibida => _page.GetByPlaceholder("0");
    private ILocator BotonRegistrarConformidad => _page.GetByRole(AriaRole.Button, new() { Name = "Registrar conformidad" });
    private ILocator BotonMasOpciones => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-ellipsis-v") });
    private ILocator OpcionVerDetalleConformidadSum => _page.GetByRole(AriaRole.Menuitem, new() { Name = "Ver detalle" });

    private ILocator ToastMensaje => _page.Locator(".p-toast:visible");
    private ILocator ToastMensajeDetalle => ToastMensaje.Locator(".p-toast-detail");

    public GestionArticulosPage(IPage page)
    {
        _page = page;
    }

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

    public async Task<string> ObtenerMensajeToastAsync()
    {
        await ToastMensaje.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 8000 });
        return await ToastMensaje.InnerTextAsync();
    }

    public async Task IrAProveedoresSum()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/suppliers");
        await _page.WaitForURLAsync("**/suppliers");
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

    public async Task GuardarOrdenAsync()
    {
        await BotonGuardarOrden.ClickAsync();

        await BotonConfirmarEnvioOP.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

        await BotonConfirmarEnvioOP.ClickAsync();
    }

    public async Task IrAOrdenProduccion()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/orden-produccion/listado");
        await _page.WaitForURLAsync("**/orden-produccion/listado");
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

    public async Task IrAGestionDeArticulos()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/gestion-articulos");
        await _page.WaitForURLAsync("**/gestion-articulos");
    }
}