using NUnit.Framework;
using TestsDGT.Paginas;

namespace TestsDGT.Paginas.Usuarios.Pedidos;

public class PedidosUsuarioTest : BaseTest
{
    private PedidosPage _pedidosPage;

    [SetUp]
    public void SetupPagina()
    {
        _pedidosPage = new PedidosPage(Page);
    }

    [Test]
    public async Task PedidoOrdinarioExito()
    {
        await _pedidosPage.AñadirProductoAlCarritoAsync("777", "M", "2");
        await _pedidosPage.TramitarPedidoOrdinarioAsync();

        var mensaje = await _pedidosPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido realizado con éxito."));
    }

    [Test]
    public async Task PedidoUrgenteExito()
    {
        await _pedidosPage.AñadirProductoAlCarritoAsync("777", "M", "2");

        await _pedidosPage.TramitarPedidoUrgenteAsync(
            motivo: "Accidente",
            instrucciones: "Pedido urgente",
            rutaArchivo: @"C:\repos\TestsDGT\6073873.png"
        );

        var mensaje = await _pedidosPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido realizado con éxito."));
    }

    [Test]
    public async Task BorrarArticuloPedido()
    {
        await _pedidosPage.AñadirProductoAlCarritoAsync("777", "M", "2");
        await _pedidosPage.BorrarArticuloDeLaCestaAsync("Zapatillas Nike 3.0");

        var mensaje = await _pedidosPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Producto eliminado con éxito"));
    }

    [Test]
    public async Task PedidoOtroUsuario()
    {
        await _pedidosPage.AñadirProductoAlCarritoAsync("777", "M", "2");
        await _pedidosPage.TramitarPedidoParaOtroUsuarioAsync("julia");

        var mensaje = await _pedidosPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido realizado con éxito."));
    }
}