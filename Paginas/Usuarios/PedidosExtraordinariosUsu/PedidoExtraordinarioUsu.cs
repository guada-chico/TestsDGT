using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using TestsDGT.Paginas.Usuarios.PedidosExtraordinarioUsu;

namespace TestsDGT.Paginas.Usuarios.PedidosExtraordinariosUsu;

[TestFixture]
public class PedidoExtraordinarioUsuTest : BaseTest
{
    private PedidoExtraordinarioPage _pedidoExtraordinarioPage;

    [SetUp]
    public void SetupPagina()
    {
        _pedidoExtraordinarioPage = new PedidoExtraordinarioPage(Page);
    }

    [Test]

    public async Task PedidoExtraordinario_OrdinarioExito()
    {
        await _pedidoExtraordinarioPage.IrAPedidoExtraordinarioAsync();

        await _pedidoExtraordinarioPage.RealizarPedidoExtraordinarioAsync(
            articulo: "CHAQUETA TECNICA",
            talla: "M/L",
            cantidad: "3",
            motivo: "Necesito más"
        );

        var mensaje = await _pedidoExtraordinarioPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido enviado"));
    }

    [Test]
    public async Task PedidoExtraordinario_UrgenteExito()
    {
        await _pedidoExtraordinarioPage.IrAPedidoExtraordinarioAsync();

        await _pedidoExtraordinarioPage.RealizarPedidoExtraordinarioAsync(
            articulo: "CHAQUETA TECNICA",
            talla: "M/L",
            cantidad: "3",
            motivo: "Necesito más",
            urgente: true
        );

        var mensaje = await _pedidoExtraordinarioPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido enviado"));
    }

    [Test]

    public async Task PedidoExtraordinario_Error()
    {
        await _pedidoExtraordinarioPage.IrAPedidoExtraordinarioAsync();

        await _pedidoExtraordinarioPage.RealizarPedidoExtraordinarioAsync(
            articulo: "CHAQUETA TECNICA",
            talla: "",
            cantidad: "3",
            motivo: "Necesito más"
        );

        var mensajeError = await _pedidoExtraordinarioPage.ObtenerMensajeToastAsync();
        Assert.That(mensajeError, Does.Contain("Error en el formulario"));
    }
}