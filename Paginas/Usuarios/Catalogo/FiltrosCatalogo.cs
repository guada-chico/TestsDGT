using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.Catalogo;

[TestFixture]
public class FiltrosCatalogoTest : BaseTest
{
    private CatalogoPage _catalogoPage;

    [SetUp]
    public void SetupPagina()
    {
        _catalogoPage = new CatalogoPage(Page);
    }

    [Test]

    public async Task FiltroCatalogoCodigo()
    {
        string codigoProd = "777";
        await _catalogoPage.FiltrarPorCodigoAsync(codigoProd);

        Assert.That(await _catalogoPage.ExisteTextoEnTablaAsync(codigoProd), Is.True);
        Assert.That(await _catalogoPage.ObtenerNumeroFilasAsync(), Is.EqualTo(1));
    }

    [Test]

    public async Task FiltroCatalogoNombre()
    {
        string nombreProd = "Zapatillas deportivas de Nike";
        await _catalogoPage.FiltrarPorNombreAsync(nombreProd);

        Assert.That(await _catalogoPage.ExisteTextoEnTablaAsync(nombreProd), Is.True);
        Assert.That(await _catalogoPage.ObtenerNumeroFilasAsync(), Is.EqualTo(1));

        await _catalogoPage.LimpiarFiltrosAsync();
    }

    [Test]

    public async Task FiltroCatalogoCodigo_SinResultados()
    {
        await _catalogoPage.FiltrarPorCodigoAsync("CodigoInexistente");

        var mensajeVacio = await _catalogoPage.ExisteTextoEnTablaAsync("No hay resultados para tu búsqueda");
        Assert.That(mensajeVacio, Is.True);

        await _catalogoPage.LimpiarFiltrosAsync();
    }

    [Test]

    public async Task FiltroCatalogoNombre_SinResultados()
    {
        await _catalogoPage.FiltrarPorNombreAsync("NombreInexistente");

        var mensajeVacio = await _catalogoPage.ExisteTextoEnTablaAsync("No hay resultados para tu búsqueda");
        Assert.That(mensajeVacio, Is.True);

        await _catalogoPage.LimpiarFiltrosAsync();
    }
}