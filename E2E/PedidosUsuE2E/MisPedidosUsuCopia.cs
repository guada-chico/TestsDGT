using Microsoft.Playwright;
using NUnit.Framework;
using TestsDGT.Paginas;

namespace TestsDGT.E2E.PedidosUsuE2E;

[TestFixture]
public class MisPedidosUsuCopiaTest : BaseTest
{
    private MisPedidosPageCopia _pedidosPageCopia;

    [SetUp]
    public void SetupPagina()
    {
        _pedidosPageCopia = new MisPedidosPageCopia(Page);
    }
    /*
    [Test]
    public async Task PedidoOrdinarioExito()
    {
        await _pedidosPageCopia.AgregarProductoAlCarritoAsync("777", "M", "2");
        await _pedidosPageCopia.TramitarPedidoOrdinarioAsync();

        var mensaje = await _pedidosPageCopia.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido realizado con éxito."));
    }

    [Test]
    public async Task PedidoUrgenteExito()
    {
        await _pedidosPageCopia.AgregarProductoAlCarritoAsync("777", "M", "2");

        await _pedidosPageCopia.TramitarPedidoUrgenteAsync(
            motivo: "Accidente",
            instrucciones: "Pedido urgente",
            rutaArchivo: @"C:\repos\TestsDGT\6073873.png"
        );

        var mensaje = await _pedidosPageCopia.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido realizado con éxito."));
    }

    [Test]
    public async Task BorrarArticuloPedido()
    {
        await _pedidosPageCopia.AgregarProductoAlCarritoAsync("777", "M", "2");
        await _pedidosPageCopia.BorrarArticuloDeLaCestaAsync("Zapatillas Nike 3.0");

        var mensaje = await _pedidosPageCopia.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Producto eliminado con éxito"));
    }

    [Test]
    public async Task PedidoOtroUsuario()
    {
        await _pedidosPageCopia.AgregarProductoAlCarritoAsync("777", "M", "2");
        await _pedidosPageCopia.TramitarPedidoParaOtroUsuarioAsync("julia");

        var mensaje = await _pedidosPageCopia.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido realizado con éxito."));
    }

    [Test]

    public async Task FiltroPedidoNombreArticulo()
    {
        await _pedidosPageCopia.IrAMisPedidosAsync();
        await _pedidosPageCopia.FiltrarPorNombreArticuloAsync("DIVISA");

        bool existe = await _pedidosPageCopia.ExisteElementoEnTablaAsync("DIVISA");
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el nombre 'DIVISA' tras 5 segundos.");
    }

    [Test]

    public async Task FiltroPedidoCodigo()
    {
        await _pedidosPageCopia.IrAMisPedidosAsync();
        await _pedidosPageCopia.FiltrarPorCodigoAsync("21488");

        bool existe = await _pedidosPageCopia.ExisteElementoEnTablaAsync("21488");
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el Código '21488' tras 5 segundos.");
    }

    [Test]

    public async Task FiltroPedidoEstado()
    {
        await _pedidosPageCopia.IrAMisPedidosAsync();
        await _pedidosPageCopia.FiltrarPorEstadoAsync("En proceso");

        bool existe = await _pedidosPageCopia.ExisteElementoEnTablaAsync("En proceso");
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el estado 'En proceso' tras 5 segundos.");
    }

    [Test]
    public async Task FiltroFechaDesde()
    {
        await _pedidosPageCopia.IrAMisPedidosAsync();
        await _pedidosPageCopia.FiltrarPorRangoFechasAsync("18/04/2026", hasta: null);

        var fechasPedidos = await _pedidosPageCopia.ObtenerValoresDeColumnaAsync("Fecha");
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
        await _pedidosPageCopia.IrAMisPedidosAsync();
        await _pedidosPageCopia.FiltrarPorRangoFechasAsync(desde: null, "18/04/2026");

        var fechasPedidos = await _pedidosPageCopia.ObtenerValoresDeColumnaAsync("Fecha");
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
        await _pedidosPageCopia.IrAMisPedidosAsync();
        await _pedidosPageCopia.FiltrarPorRangoFechasAsync("18/04/2026", "26/04/2026");

        var fechasPedidos = await _pedidosPageCopia.ObtenerValoresDeColumnaAsync("Fecha");
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
    */
}