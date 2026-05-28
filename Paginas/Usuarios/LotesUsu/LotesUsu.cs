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
    /*
    [Test]
    public async Task PedidoLoteExito()
    {
        await _lotesPage.IrALotesDisponiblesAsync();

        await _lotesPage.AgregarLoteAlCarritoAsync("LT-00059");

        await _lotesPage.DatosLoteCestaAsync("M", "Instrucciones de prueba");

        await _lotesPage.TramitarLoteAsync();

        var mensaje = await _lotesPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido realizado con éxito"));
    }

    [Test]
    public async Task BorrarArticuloLote()
    {
        await _lotesPage.IrALotesDisponiblesAsync();

        await _lotesPage.AgregarLoteAlCarritoAsync("LT-00060");

        await _lotesPage.DatosLoteCestaAsync("M", "Instrucciones de prueba");

        await _lotesPage.BorrarArticuloDeLaCestaAsync("prueba test no");

        var mensaje = await _lotesPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Lote eliminado con éxito"));
    }

    */
}