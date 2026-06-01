using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using TestsDGT.Paginas.Usuarios.MisPedidosUsu;

namespace TestsDGT.Tests.Usuarios.MisPedidosUsu;

[TestFixture]
public class MisPedidosUsuTest : BaseTest
{
    private MisPedidosPage _misPedidosPage;

    [SetUp]
    public void SetupPagina()
    {
        _misPedidosPage = new MisPedidosPage(Page);
    }

    [Test]
    public async Task FiltroPedidoNombreArticulo()
    {
        await _misPedidosPage.IrAMisPedidosAsync();

        string nombreArticulo = "DIVISA";
        await _misPedidosPage.FiltrarPorNombreArticuloAsync(nombreArticulo);

        bool existe = await _misPedidosPage.ExisteElementoEnTablaAsync(nombreArticulo);
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el nombre 'DIVISA' tras 5 segundos.");
    }

    [Test]
    public async Task FiltroPedidoEstado()
    {
        await _misPedidosPage.IrAMisPedidosAsync();

        string estado = "En proceso";
        await _misPedidosPage.FiltrarPorEstadoAsync(estado);

        bool existe = await _misPedidosPage.ExisteElementoEnTablaAsync(estado);
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el estado 'En proceso' tras 5 segundos.");
    }

    [Test]
    public async Task FiltroFechaDesde()
    {
        await _misPedidosPage.IrAMisPedidosAsync();
        await _misPedidosPage.FiltrarPorRangoFechasAsync("18/04/2026", hasta: null);

        var fechasPedidos = await _misPedidosPage.ObtenerValoresDeColumnaAsync("Fecha");
        Assert.That(fechasPedidos, Is.Not.Empty, "El filtro no devolvió ningún pedido (la tabla está vacía).");

        DateTime fechaFiltro = new DateTime(2026, 4, 18);

        foreach (var fechaTexto in fechasPedidos)
        {
            if (DateTime.TryParse(fechaTexto.Trim(), out DateTime fechaPedido))
            {
                Assert.That(fechaPedido.Date >= fechaFiltro.Date,
                    $"Error: La fecha {fechaPedido:dd/MM/yyyy} es menor que el filtro {fechaFiltro:dd/MM/yyyy}");
            }
            else
            {
                Assert.Fail($"No se pudo procesar el formato de fecha: '{fechaTexto}'");
            }
        }
    }

    [Test]
    public async Task FiltroFechaHasta()
    {
        await _misPedidosPage.IrAMisPedidosAsync();
        await _misPedidosPage.FiltrarPorRangoFechasAsync(desde: null, "18/04/2026");

        var fechasPedidos = await _misPedidosPage.ObtenerValoresDeColumnaAsync("Fecha");
        Assert.That(fechasPedidos, Is.Not.Empty, "El filtro no devolvió ningún pedido (la tabla está vacía).");

        DateTime fechaFiltro = new DateTime(2026, 4, 18);

        foreach (var fechaTexto in fechasPedidos)
        {
            if (DateTime.TryParse(fechaTexto.Trim(), out DateTime fechaPedido))
            {
                Assert.That(fechaPedido.Date <= fechaFiltro.Date,
                    $"Error: La fecha {fechaPedido:dd/MM/yyyy} es mayor que el filtro {fechaFiltro:dd/MM/yyyy}");
            }
            else
            {
                Assert.Fail($"No se pudo procesar el formato de fecha: '{fechaTexto}'");
            }
        }
    }

    [Test]
    public async Task FiltroRangoFecha()
    {
        await _misPedidosPage.IrAMisPedidosAsync();
        await _misPedidosPage.FiltrarPorRangoFechasAsync("18/04/2026", "26/04/2026");

        var fechasPedidos = await _misPedidosPage.ObtenerValoresDeColumnaAsync("Fecha");
        Assert.That(fechasPedidos, Is.Not.Empty, "El filtro no devolvió ningún pedido (la tabla está vacía).");

        DateTime fechaFiltroDesde = new DateTime(2026, 4, 18);
        DateTime fechaFiltroHasta = new DateTime(2026, 4, 26);

        foreach (var fechaTexto in fechasPedidos)
        {
            if (DateTime.TryParse(fechaTexto.Trim(), out DateTime fechaPedido))
            {
                Assert.That(fechaPedido.Date >= fechaFiltroDesde.Date && fechaPedido.Date <= fechaFiltroHasta.Date,
                    $"Error: La fecha {fechaPedido:dd/MM/yyyy} está fuera del rango de filtros [{fechaFiltroDesde:dd/MM/yyyy}, {fechaFiltroHasta:dd/MM/yyyy}]");
            }
            else
            {
                Assert.Fail($"No se pudo procesar el formato de fecha: '{fechaTexto}'");
            }
        }
    }

    [Test]
    public async Task FiltroPedidoCodigo()
    {
        await _misPedidosPage.IrAMisPedidosAsync();

        string codigoProd = "21488";
        await _misPedidosPage.FiltrarPorCodigoAsync(codigoProd);

        bool existe = await _misPedidosPage.ExisteElementoEnTablaAsync(codigoProd);
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el Código '21488' tras 5 segundos.");
    }

    [Test]
    public async Task LimpiarFiltrosMisPedidos()
    {
        await _misPedidosPage.IrAMisPedidosAsync();

        int totalFilasOriginales = await _misPedidosPage.ObtenerNumeroFilasAsync();

        string codigoProd = "21488";
        await _misPedidosPage.FiltrarPorCodigoAsync(codigoProd);

        int filasFiltradas = await _misPedidosPage.ObtenerNumeroFilasAsync();

        await _misPedidosPage.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _misPedidosPage.ObtenerNumeroFilasAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
            "La tabla no recuperó todos los registros originales tras limpiar los filtros.");

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas),
            "El filtro de prueba no redujo el tamaño de la tabla, usa un artículo diferente.");
    }

    [Test]
    public async Task IrACatalogoDesdeMisPedidos()
    {
        await _misPedidosPage.IrAMisPedidosAsync();

        await _misPedidosPage.IrACatalogoAsync();

        await Assertions.Expect(Page).ToHaveURLAsync(new Regex(".*/catalogo.*"));
    }

    [Test]
    public async Task VerDetallePedido_MisPedidos() 
    {
        await _misPedidosPage.IrAMisPedidosAsync();
        string codigoProd = "21488";

        await _misPedidosPage.VerDetallePedidoDeMisPedidosAsync(codigoProd);

        await Assertions.Expect(Page).ToHaveURLAsync(new Regex(".*/app-order-detail/.*"));
    }

    [Test]
    public async Task DescargarDocumentoPedidoExtraordinario()
    {
        await _misPedidosPage.IrAMisPedidosAsync();
        string codigoProd = "307616";

        await _misPedidosPage.VerDetallePedidoDeMisPedidosAsync(codigoProd);

        var downloadTask = Page.WaitForDownloadAsync();
        await _misPedidosPage.DescargarDocumentoPedidoAsync();

        var download = await downloadTask;
        string fileName = download.SuggestedFilename;
        Assert.That(fileName, Does.Contain("6073873"), "El nombre del archivo descargado no contiene 'pedido_' como se esperaba.");
    }

    [Test]
    public async Task VerDevolucionPedido_MisPedidos()
    {
        await _misPedidosPage.IrAMisPedidosAsync();
        string codigoProd = "307654";

        await _misPedidosPage.VerDevolucionPedidoDeMisPedidosAsync(codigoProd);

        await Assertions.Expect(Page).ToHaveURLAsync(new Regex(".*/devoluciones/.*"));
    }

    [Test]
    public async Task DescargarEtiquetaDevolucion_MisPedidos()
    {
        await _misPedidosPage.IrAMisPedidosAsync();
        string codigoProd = "307654";

        await _misPedidosPage.VerDevolucionPedidoDeMisPedidosAsync(codigoProd);

        var esperarPestañaTask = Page.Context.WaitForPageAsync();
        await _misPedidosPage.DescargarEtiquetaDevolucionAsync();

        var nuevaPestaña = await esperarPestañaTask;
        await nuevaPestaña.WaitForLoadStateAsync(LoadState.Load);

        string urlImpresion = nuevaPestaña.Url;

        Console.WriteLine($"URL de la etiqueta capturada: {urlImpresion}");

        Assert.That(urlImpresion, Does.Contain("blob"),
            $"La pestaña se abrió pero no contiene un objeto Blob de impresión. URL obtenida: {urlImpresion}");
    }

    [Test]
    public async Task ImprimirAlbaranDevolucion_MisPedidos()
    {
        await _misPedidosPage.IrAMisPedidosAsync();
        string codigoProd = "307654";

        await _misPedidosPage.VerDevolucionPedidoDeMisPedidosAsync(codigoProd);

        var esperarPestañaTask = Page.Context.WaitForPageAsync();
        await _misPedidosPage.ImprimirAlbaranDevolucionAsync();

        var nuevaPestaña = await esperarPestañaTask;
        await nuevaPestaña.WaitForLoadStateAsync(LoadState.Load);

        string urlImpresion = nuevaPestaña.Url;

        Console.WriteLine($"URL de la etiqueta capturada: {urlImpresion}");

        Assert.That(urlImpresion, Does.Contain("blob"),
            $"La pestaña se abrió pero no contiene un objeto Blob de impresión. URL obtenida: {urlImpresion}");
    }

    [Test]
    public async Task RellenarSolicitudInfo_MisPedidos()
    {
        await _misPedidosPage.IrAMisPedidosAsync();
        string codigoProd = "22222";

        await _misPedidosPage.VerDetallePedidoDeMisPedidosAsync(codigoProd);

        var comentario = "Comentario de prueba para solicitud de información.";
        var archivo = @"C:\repos\TestsDGT\6073873.png";

        await _misPedidosPage.RellenarInfoPedidoAsync(comentario, archivo);

        var mensaje = await _misPedidosPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido enviado a validar correctamente"));
    }
}