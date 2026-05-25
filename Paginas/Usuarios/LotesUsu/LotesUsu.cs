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
    public async Task FiltroLotes_PorCodigo()
    {
        await _lotesPage.IrALotesDisponiblesAsync();

        string codigo = "LT-00057";
        await _lotesPage.FiltrarPorCodigoONombreAsync(codigo);

        bool existe = await _lotesPage.ExisteElementoEnTablaAsync(codigo);
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el Código 'LT-00057' tras 5 segundos.");
    }

    [Test]

    public async Task FiltroLotes_PorNombre()
    {
        await _lotesPage.IrALotesDisponiblesAsync();

        string nombre = "Lote prueba1";
        await _lotesPage.FiltrarPorCodigoONombreAsync(nombre);

        bool existe = await _lotesPage.ExisteElementoEnTablaAsync(nombre);
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el nombre 'Lote prueba1' tras 5 segundos.");
    }

    [Test]
    public async Task LimpiarFiltrosLotes()
    {
        await _lotesPage.IrALotesDisponiblesAsync();

        int totalFilasOriginales = await _lotesPage.ObtenerNumeroFilasLotesAsync();

        string nombre = "Lote prueba1";
        await _lotesPage.FiltrarPorCodigoONombreAsync(nombre);

        int filasFiltradas = await _lotesPage.ObtenerNumeroFilasLotesAsync();

        await _lotesPage.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _lotesPage.ObtenerNumeroFilasLotesAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
            "La tabla no recuperó todos los registros originales tras limpiar los filtros.");

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas),
            "El filtro de prueba no redujo el tamaño de la tabla, usa un artículo diferente.");
    }

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
}