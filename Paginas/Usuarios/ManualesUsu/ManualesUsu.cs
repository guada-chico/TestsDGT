using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.ManualesUsu;

[TestFixture]
public class ManualesUsuTest : BaseTest
{
    private ManualesUsuPage _manualesPage;

    [SetUp]
    public void SetupPagina()
    {
        _manualesPage = new ManualesUsuPage(Page);
    }

    [Test]

    public async Task FiltroManualTitulo()
    {
        await _manualesPage.IrAManuales();

        string tituloManual = "Manual Prueba";
        await _manualesPage.FiltrarPorTituloManualAsync(tituloManual);

        Assert.That(await _manualesPage.VerificarManualCreadaAsync(tituloManual), Is.True);
        Assert.That(await _manualesPage.ObtenerNumeroFilasManualAsync(), Is.GreaterThan(0));
    }

    [Test]

    public async Task FiltroManualFecha()
    {
        await _manualesPage.IrAManuales();

        string fechaDesdeManual = "25/03/2026";
        string fechaHastaManual = "19/05/2026";
        await _manualesPage.FiltrarPorRangoFechaManualAsync(fechaDesdeManual, fechaHastaManual);

        Assert.That(await _manualesPage.ObtenerNumeroFilasManualAsync(), Is.GreaterThan(0));
    }

    [Test]
    public async Task LimpiarFiltrosManuales()
    {
        await _manualesPage.IrAManuales();

        int totalFilasOriginales = await _manualesPage.ObtenerNumeroFilasManualAsync();

        string titulo = "Manual Prueba";
        await _manualesPage.FiltrarPorTituloManualAsync(titulo);

        int filasFiltradas = await _manualesPage.ObtenerNumeroFilasManualAsync();

        await _manualesPage.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _manualesPage.ObtenerNumeroFilasManualAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
            "La tabla no recuperó todos los registros originales tras limpiar los filtros.");

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas),
            "El filtro de prueba no redujo el tamaño de la tabla, usa un artículo diferente.");
    }
}