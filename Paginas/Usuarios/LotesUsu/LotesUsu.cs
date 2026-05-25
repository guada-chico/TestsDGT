using Microsoft.Playwright;
using NUnit.Framework;
using TestsDGT.Paginas;

namespace TestsDGT.Paginas.Usuarios.LotesUsu;

[TestFixture]
public class LotesUsuTest : BaseTest
{
    private LotesUsuPage _lotesPage;

    [SetUp]
    public void SetupPagina()
    {
        _lotesPage = new LotesUsuPage(Page);
    }

    [Test]
    public async Task PedidoOrdinarioExito()
    {
        await _lotesPage.AñadirProductoAlCarritoAsync("777", "M", "2");
        await _lotesPage.TramitarPedidoOrdinarioAsync();

        var mensaje = await _lotesPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido realizado con éxito."));
    }

    [Test]
    public async Task PedidoUrgenteExito()
    {
        await _lotesPage.AñadirProductoAlCarritoAsync("777", "M", "2");

        await _lotesPage.TramitarPedidoUrgenteAsync(
            motivo: "Accidente",
            instrucciones: "Pedido urgente",
            rutaArchivo: @"C:\repos\TestsDGT\6073873.png"
        );

        var mensaje = await _lotesPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido realizado con éxito."));
    }

    [Test]
    public async Task BorrarArticuloPedido()
    {
        await _lotesPage.AñadirProductoAlCarritoAsync("777", "M", "2");
        await _lotesPage.BorrarArticuloDeLaCestaAsync("Zapatillas Nike 3.0");

        var mensaje = await _lotesPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Producto eliminado con éxito"));
    }

    [Test]
    public async Task PedidoOtroUsuario()
    {
        await _lotesPage.AñadirProductoAlCarritoAsync("777", "M", "2");
        await _lotesPage.TramitarPedidoParaOtroUsuarioAsync("julia");

        var mensaje = await _lotesPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido realizado con éxito."));
    }

    [Test]

    public async Task FiltroPedidoNombreArticulo()
    {
        await _lotesPage.IrAMisPedidosAsync();
        await _lotesPage.FiltrarPorNombreArticuloAsync("DIVISA");

        bool existe = await _lotesPage.ExisteElementoEnTablaAsync("DIVISA");
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el nombre 'DIVISA' tras 5 segundos.");
    }

    [Test]

    public async Task FiltroPedidoCodigo()
    {
        await _lotesPage.IrAMisPedidosAsync();
        await _lotesPage.FiltrarPorCodigoAsync("21488");

        bool existe = await _lotesPage.ExisteElementoEnTablaAsync("21488");
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el Código '21488' tras 5 segundos.");
    }

    [Test]

    public async Task FiltroPedidoEstado()
    {
        await _lotesPage.IrAMisPedidosAsync();
        await _lotesPage.FiltrarPorEstadoAsync("En proceso");

        bool existe = await _lotesPage.ExisteElementoEnTablaAsync("En proceso");
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el estado 'En proceso' tras 5 segundos.");
    }

    [Test]
    public async Task FiltroFechaDesde()
    {
        await _lotesPage.IrAMisPedidosAsync();
        await _lotesPage.FiltrarPorRangoFechasAsync("18/04/2026", hasta: null);

        var fechasPedidos = await _lotesPage.ObtenerValoresDeColumnaAsync("Fecha");
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
        await _lotesPage.IrAMisPedidosAsync();
        await _lotesPage.FiltrarPorRangoFechasAsync(desde: null, "18/04/2026");

        var fechasPedidos = await _lotesPage.ObtenerValoresDeColumnaAsync("Fecha");
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
        await _lotesPage.IrAMisPedidosAsync();
        await _lotesPage.FiltrarPorRangoFechasAsync("18/04/2026", "26/04/2026");

        var fechasPedidos = await _lotesPage.ObtenerValoresDeColumnaAsync("Fecha");
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
}