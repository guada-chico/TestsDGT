using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestsDGT.Paginas.Suministro.Proveedores;

namespace TestsDGT.Paginas.Suministro.OrdenProduccion;

[TestFixture]
public class OrdenProduccionTest : BaseTest
{
    private OrdenProduccionPage _ordenProduccionPage;

    [SetUp]
    public void SetupPagina()
    {
        _ordenProduccionPage = new OrdenProduccionPage(Page);
    }

    [Test]
    public async Task NuevaOrdenProduccion_PorCodigo()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        await _ordenProduccionPage.IrACrearOrdenProduccionAsync();

        await _ordenProduccionPage.DatosNuevaOrdenProduccionAsync(
            expediente: "891",
            lote: "Lote Prueba",
            duracionContrato: 12,
            observaciones: "Observaciones prueba",
            proveedor: "RopitaChulita"
        );
        
        await _ordenProduccionPage.ArticuloNuevaOrdenProduccionAsync(
            valorPrenda: "777",
            talla: "M",
            cantidad: 5
        );

        await _ordenProduccionPage.GuardarNuevaOrdenProduccionAsync();

        var mensaje = await _ordenProduccionPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Orden guardada correctamente. El envio de correo no bloquea la creacion."));
    }

    [Test]
    public async Task NuevaOrdenProduccion_PorArticulo()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        await _ordenProduccionPage.IrACrearOrdenProduccionAsync();

        await _ordenProduccionPage.DatosNuevaOrdenProduccionAsync(
            expediente: "2365",
            lote: "Lote Prueba Articulo",
            duracionContrato: 12,
            observaciones: "Observaciones prueba 2",
            proveedor: "RopaMuyGuay"
        );

        await _ordenProduccionPage.ArticuloNuevaOrdenProduccionAsync(
            valorPrenda: "JERSEY VERDE DE CUELLO CISNE",
            talla: "L",
            cantidad: 5,
            esPorCodigo: false
        );

        await _ordenProduccionPage.GuardarNuevaOrdenProduccionAsync();

        var mensaje = await _ordenProduccionPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Orden guardada correctamente. El envio de correo no bloquea la creacion."));
    }

    [Test]
    public async Task CancelarNuevaOrdenProduccion()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        await _ordenProduccionPage.IrACrearOrdenProduccionAsync();

        await _ordenProduccionPage.DatosNuevaOrdenProduccionAsync(
            expediente: "9999",
            lote: "Lote Prueba Cancelar",
            duracionContrato: 12,
            observaciones: "Observaciones prueba cancelar",
            proveedor: "RopaMuyGuay"
        );
        await _ordenProduccionPage.ArticuloNuevaOrdenProduccionAsync(
            valorPrenda: "777",
            talla: "M",
            cantidad: 5,
            esPorCodigo: true
        );

        await _ordenProduccionPage.CancelarNuevaOrdenProduccionAsync();

        bool existe = await _ordenProduccionPage.VerificarOrdenProduccionCreadaAsync("9999");
        Assert.That(existe, Is.False, "La orden de producción se creó a pesar de cancelar el proceso.");
    }
    

    [Test]
    public async Task AgregarArticulosOD()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        await _ordenProduccionPage.IrACrearOrdenProduccionAsync();

        await _ordenProduccionPage.DatosNuevaOrdenProduccionAsync(
            expediente: "7890",
            lote: "Lote Prueba Agregar Articulos",
            duracionContrato: 12,
            observaciones: "Observaciones prueba agregar artículos",
            proveedor: "RopaMuyGuay"
        ); 

        await _ordenProduccionPage.ArticuloNuevaOrdenProduccionAsync(
            valorPrenda: "246",
            talla: "M",
            cantidad: 5,
            esPorCodigo: true
        );

        await _ordenProduccionPage.AgregarArticuloODAsync();

        await _ordenProduccionPage.ArticuloNuevaOrdenProduccionAsync(
            valorPrenda: "JERSEY VERDE DE CUELLO CISNE",
            talla: "L",
            cantidad: 3,
            esPorCodigo: false
        );

        await _ordenProduccionPage.FinalizarYGuardarOrdenAsync();

        var mensaje = await _ordenProduccionPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Orden guardada correctamente. El envio de correo no bloquea la creacion."));
    }

    [Test]
    public async Task VerDetalleOrdenProduccion()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        string expedienteOrdenProduccion = "92";
        await _ordenProduccionPage.VerDetalleOrdenProduccionAsync(expedienteOrdenProduccion);

        await Page.WaitForURLAsync("**/orden-produccion/detalle/92", new() { Timeout = 7000 });
    }

    [Test]
    public async Task CambiarEstadoOrdenProduccion()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        string expedienteOrdenProduccion = "92";
        await _ordenProduccionPage.VerDetalleOrdenProduccionAsync(expedienteOrdenProduccion);

        string nuevoEstado = "Entrega parcial";
        await _ordenProduccionPage.CambiarEstadoOrdenProduccionAsync(nuevoEstado);

        var mensaje = await _ordenProduccionPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Estado actualizado correctamente"));
    }

    [Test]
    public async Task ImprimirOrdenProduccion_DesdeVerDetalles()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        string expedienteOrdenProduccion = "92";
        await _ordenProduccionPage.VerDetalleOrdenProduccionAsync(expedienteOrdenProduccion);

        var esperarPestañaTask = Page.Context.WaitForPageAsync();

        await _ordenProduccionPage.ImprimirDesdeVerDetallesOP();

        var nuevaPestaña = await esperarPestañaTask;

        await nuevaPestaña.WaitForLoadStateAsync(LoadState.Load);

        string urlImpresion = nuevaPestaña.Url;

        Console.WriteLine($"URL de la pestaña de impresión capturada: {urlImpresion}");

        Assert.That(urlImpresion, Does.Contain("blob"),
                $"La pestaña se abrió pero no parece ser un objeto de impresión en memoria. URL: {urlImpresion}");
    }

    [Test]
    public async Task ImprimirOrdenProduccion_DesdeMasOpciones()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        string expedienteOrdenProduccion = "92";

        await _ordenProduccionPage.ImprimirDesdeMasOpcionesOPAsync(expedienteOrdenProduccion);

        var esperarPestañaTask = Page.Context.WaitForPageAsync();

        var nuevaPestaña = await esperarPestañaTask;

        await nuevaPestaña.WaitForLoadStateAsync(LoadState.Load);

        string urlImpresion = nuevaPestaña.Url;

        Console.WriteLine($"URL de la pestaña de impresión capturada: {urlImpresion}");

        Assert.That(urlImpresion, Does.Contain("blob"),
                $"La pestaña se abrió pero no parece ser un objeto de impresión en memoria. URL: {urlImpresion}");
    }

    [Test]
    public async Task EnviarCorreoAProveedor()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        string expedienteOrdenProduccion = "92";
        await _ordenProduccionPage.VerDetalleOrdenProduccionAsync(expedienteOrdenProduccion);

        await _ordenProduccionPage.EnviarCorreoAProveedorAsync();

        var mensaje = await _ordenProduccionPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Correo enviado correctamente al proveedor"));
    }

    [Test]
    public async Task FiltroCodigoOD()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        string codigo = "39";
        await _ordenProduccionPage.FiltrarPorCodigoExpedienteODAsync(codigo);

        Assert.That(await _ordenProduccionPage.VerificarOrdenProduccionCreadaAsync(codigo), Is.True);
        Assert.That(await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync(), Is.GreaterThan(0));
    }

    [Test]
    public async Task FiltroExpedienteOD() // Bug abierto
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        string expediente = "4561";
        await _ordenProduccionPage.FiltrarPorCodigoExpedienteODAsync(expediente);

        Assert.That(await _ordenProduccionPage.VerificarOrdenProduccionCreadaAsync(expediente), Is.True);
        Assert.That(await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync(), Is.GreaterThan(0));
    }

    [Test]
    public async Task FiltroProveedorOD()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        await _ordenProduccionPage.FiltrarPorProveedorOPAsync("Proveedor Siete");

        Assert.That(await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync(), Is.GreaterThan(0));
        Assert.That(await _ordenProduccionPage.VerificarTextoEnTablaAsync("Proveedor Siete"), Is.True);
    }

    [Test]
    public async Task LimpiarFiltrosOrdenProduccion()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        int totalFilasOriginales = await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync();

        string codigo = "39";
        await _ordenProduccionPage.FiltrarPorCodigoExpedienteODAsync(codigo);

        int filasFiltradas = await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync();

        await _ordenProduccionPage.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
            "La tabla no recuperó todos los registros originales tras limpiar los filtros.");

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas),
            "El filtro de prueba no redujo el tamaño de la tabla, usa un artículo diferente.");
    }
}