using Microsoft.Playwright;
using NUnit.Framework;
using TestsDGT.Paginas;
using TestsDGT.Paginas.Usuarios.Catalogo;

namespace TestsDGT.Paginas.Usuarios.MisPedidosUsu;

[TestFixture]
public class MisPedidosUsuTest : BaseTest
{
    private MisPedidosPage _pedidosPage;

    [SetUp]
    public void SetupPagina()
    {
        _pedidosPage = new MisPedidosPage(Page);
    }

    [Test]
    public async Task FiltroPedidoNombreArticulo()
    {
        await _pedidosPage.IrAMisPedidosAsync();
        await _pedidosPage.FiltrarPorNombreArticuloAsync("DIVISA");

        bool existe = await _pedidosPage.ExisteElementoEnTablaAsync("DIVISA");
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el nombre 'DIVISA' tras 5 segundos.");
    }

    [Test]
    public async Task FiltroPedidoEstado()
    {
        await _pedidosPage.IrAMisPedidosAsync();
        await _pedidosPage.FiltrarPorEstadoAsync("En proceso");

        bool existe = await _pedidosPage.ExisteElementoEnTablaAsync("En proceso");
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el estado 'En proceso' tras 5 segundos.");
    }

    [Test]
    public async Task FiltroFechaDesde()
    {
        await _pedidosPage.IrAMisPedidosAsync();
        await _pedidosPage.FiltrarPorRangoFechasAsync("18/04/2026", hasta: null);

        var fechasPedidos = await _pedidosPage.ObtenerValoresDeColumnaAsync("Fecha");
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
        await _pedidosPage.IrAMisPedidosAsync();
        await _pedidosPage.FiltrarPorRangoFechasAsync(desde: null, "18/04/2026");

        var fechasPedidos = await _pedidosPage.ObtenerValoresDeColumnaAsync("Fecha");
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
        await _pedidosPage.IrAMisPedidosAsync();
        await _pedidosPage.FiltrarPorRangoFechasAsync("18/04/2026", "26/04/2026");

        var fechasPedidos = await _pedidosPage.ObtenerValoresDeColumnaAsync("Fecha");
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
        await _pedidosPage.IrAMisPedidosAsync();
        await _pedidosPage.FiltrarPorCodigoAsync("21488");

        bool existe = await _pedidosPage.ExisteElementoEnTablaAsync("21488");
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el Código '21488' tras 5 segundos.");
    }

    [Test]
    public async Task LimpiarFiltrosCatalogo()
    {
        await _pedidosPage.IrACatalogoAsync();

        int totalFilasOriginales = await _pedidosPage.ObtenerNumeroFilasAsync();

        string codigoProd = "21488";
        await _pedidosPage.FiltrarPorCodigoAsync(codigoProd);

        int filasFiltradas = await _pedidosPage.ObtenerNumeroFilasAsync();

        await _pedidosPage.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _pedidosPage.ObtenerNumeroFilasAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
            "La tabla no recuperó todos los registros originales tras limpiar los filtros.");

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas),
            "El filtro de prueba no redujo el tamaño de la tabla, usa un artículo diferente.");
    }
}