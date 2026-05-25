using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestsDGT.Paginas.Suministro.OrdenProduccion;
using TestsDGT.Paginas.Suministro.Proveedores;

namespace TestsDGT.Paginas.Suministro.Conformidad;

[TestFixture]
public class ConformidadSumTest : BaseTest
{
    private ConformidadSumPage _conformidadSumPage;

    [SetUp]
    public void SetupPagina()
    {
        _conformidadSumPage = new ConformidadSumPage(Page);
    }

    [Test]
    public async Task FiltroCodigoOrden()
    {
        await _conformidadSumPage.IrAConformidadSuministro();

        string codigo = "77";
        await _conformidadSumPage.FiltrarPorCodigoOrdenAsync(codigo);

        Assert.That(await _conformidadSumPage.VerificarConformidadSumAsync(codigo), Is.True);
        Assert.That(await _conformidadSumPage.ObtenerNumeroFilasConformidadSumAsync(), Is.GreaterThan(0));
    }

    [Test]
    public async Task FiltroProveedor()
    {
        await _conformidadSumPage.IrAConformidadSuministro();

        await _conformidadSumPage.FiltrarPorProveedorAsync("Proveedor Siete");

        Assert.That(await _conformidadSumPage.ObtenerNumeroFilasConformidadSumAsync(), Is.GreaterThan(0));
        Assert.That(await _conformidadSumPage.VerificarTextoEnTablaAsync("Proveedor Siete"), Is.True);
    }

    [Test]
    public async Task LimpiarFiltrosConformidadSum()
    {
        await _conformidadSumPage.IrAConformidadSuministro();

        int totalFilasOriginales = await _conformidadSumPage.ObtenerNumeroFilasConformidadSumAsync();

        string codigo = "77";
        await _conformidadSumPage.FiltrarPorCodigoOrdenAsync(codigo);

        int filasFiltradas = await _conformidadSumPage.ObtenerNumeroFilasConformidadSumAsync();

        await _conformidadSumPage.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _conformidadSumPage.ObtenerNumeroFilasConformidadSumAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
            "La tabla no recuperó todos los registros originales tras limpiar los filtros.");

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas),
            "El filtro de prueba no redujo el tamaño de la tabla, usa un artículo diferente.");
    }

    [Test]
    public async Task VerDetalleConformidadSum()
    {
        await _conformidadSumPage.IrAConformidadSuministro();

        string expedienteConformidadSum = "80";
        await _conformidadSumPage.VerDetalleConformidadSumAsync(expedienteConformidadSum);

        await Page.WaitForURLAsync("**/verificacion-conformidad?id=80", new() { Timeout = 7000 });
    }

    [Test]
    public async Task NuevaVerificacionConformidadSum()
    {
        await _conformidadSumPage.IrAConformidadSuministro();

        string expedienteConformidadSum = "80";
        await _conformidadSumPage.VerDetalleConformidadSumAsync(expedienteConformidadSum);


        var mensaje = await _conformidadSumPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("La conformidad de suministro ya tiene una verificación asociada. No se puede crear una nueva."));
    }

    [Test]
    public async Task CancelarVerificacionConformidadSum()
    {
        await _conformidadSumPage.IrAConformidadSuministro();

        string expedienteConformidadSum = "80";
        await _conformidadSumPage.VerDetalleConformidadSumAsync(expedienteConformidadSum);
        await _conformidadSumPage.CancelarVerificacionConformidadSumAsync();

        bool existe = await _conformidadSumPage.VerificarConformidadSumAsync("9999");
        Assert.That(existe, Is.False, "La conformidad de suministro se creó a pesar de cancelar el proceso.");
    }

    /*
    [Test]
    public async Task CambiarEstadoOrdenProduccion()
    {
        await _conformidadSumPage.IrAConformidadSuministro();

        string expedienteConformidadSum = "92";
        await _conformidadSumPage.VerDetalleConformidadSumAsync(expedienteConformidadSum);

        string nuevoEstado = "Entrega parcial";
        await _conformidadSumPage.CambiarEstadoConformidadSumAsync(nuevoEstado);

        var mensaje = await _conformidadSumPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Estado actualizado correctamente"));
    }

    [Test]
    public async Task ImprimirOrdenProduccion_DesdeVerDetalles()
    {
        await _conformidadSumPage.IrAConformidadSuministro();

        string expedienteConformidadSum = "92";
        await _conformidadSumPage.VerDetalleConformidadSumAsync(expedienteConformidadSum);

        var esperarPestañaTask = Page.Context.WaitForPageAsync();

        await _conformidadSumPage.ImprimirDesdeVerDetallesOP();

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
        await _conformidadSumPage.IrAConformidadSuministro();

        string expedienteOrdenProduccion = "92";

        await _conformidadSumPage.ImprimirDesdeMasOpcionesOPAsync(expedienteOrdenProduccion);

        var esperarPestañaTask = Page.Context.WaitForPageAsync();

        var nuevaPestaña = await esperarPestañaTask;

        await nuevaPestaña.WaitForLoadStateAsync(LoadState.Load);

        string urlImpresion = nuevaPestaña.Url;

        Console.WriteLine($"URL de la pestaña de impresión capturada: {urlImpresion}");

        Assert.That(urlImpresion, Does.Contain("blob"),
                $"La pestaña se abrió pero no parece ser un objeto de impresión en memoria. URL: {urlImpresion}");
    }

    [Test]
    public async Task EnviarCorreoAProveedor() // Bug abierto
    {
        await _conformidadSumPage.IrAConformidadSuministro();

        string expedienteConformidadSum = "92";
        await _conformidadSumPage.VerDetalleConformidadSumAsync(expedienteConformidadSum);

        await _conformidadSumPage.EnviarCorreoAProveedorAsync();

        var mensaje = await _conformidadSumPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("No se pudo enviar el correo."));
    }
    */
}