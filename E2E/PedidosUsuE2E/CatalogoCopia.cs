using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestsDGT.E2E.PedidosUsuE2E;

[TestFixture]
public class CatalogoCopiaTest : BaseTest
{
    private CatalogoPageCopia _catalogoPageCopia;

    [SetUp]
    public void SetupPagina()
    {
        _catalogoPageCopia = new CatalogoPageCopia(Page);
    }
    
    [Test]
    public async Task IrAMisPedidosDesdeCatalogoCopia()
    {
        await _catalogoPageCopia.IrACatalogoAsync();

        await _catalogoPageCopia.IrAMisPedidosAsync();

        await Assertions.Expect(Page).ToHaveURLAsync(new Regex(".*/orders-list.*"));
    }
    /*
    [Test]
    public async Task FiltroCatalogoCodigo()
    {
        await _catalogoPageCopia.IrACatalogoAsync();

        string codigoProd = "5117";
        await _catalogoPageCopia.FiltrarPorCodigoAsync(codigoProd);

        Assert.That(await _catalogoPageCopia.ExisteElementoEnTablaAsync(codigoProd), Is.True);
        Assert.That(await _catalogoPageCopia.ObtenerNumeroFilasAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task FiltroCatalogoNombre()
    {
        await _catalogoPageCopia.IrACatalogoAsync();

        string nombreProd = "CHALECO AIRBAG";
        await _catalogoPageCopia.FiltrarPorNombreAsync(nombreProd);

        Assert.That(await _catalogoPageCopia.ExisteElementoEnTablaAsync(nombreProd), Is.True);
        Assert.That(await _catalogoPageCopia.ObtenerNumeroFilasAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task FiltroCatalogoCodigo_SinResultados()
    {
        await _catalogoPageCopia.IrACatalogoAsync();

        string codigoProd = "CodigoInexistente";
        await _catalogoPageCopia.FiltrarPorCodigoAsync(codigoProd);

        var mensajeVacio = await _catalogoPageCopia.ExisteElementoEnTablaAsync("No hay resultados para tu búsqueda");
        Assert.That(mensajeVacio, Is.True);
    }

    [Test]
    public async Task FiltroCatalogoNombre_SinResultados()
    {
        await _catalogoPageCopia.IrACatalogoAsync();

        string nombreProd = "NombreInexistente";
        await _catalogoPageCopia.FiltrarPorNombreAsync(nombreProd);

        var mensajeVacio = await _catalogoPageCopia.ExisteElementoEnTablaAsync("No hay resultados para tu búsqueda");
        Assert.That(mensajeVacio, Is.True);
    }

    [Test]
    public async Task LimpiarFiltrosCatalogo()
    {
        await _catalogoPageCopia.IrACatalogoAsync();

        int totalFilasOriginales = await _catalogoPageCopia.ObtenerNumeroFilasAsync();

        string codigoProd = "5117";
        await _catalogoPageCopia.FiltrarPorCodigoAsync(codigoProd);

        int filasFiltradas = await _catalogoPageCopia.ObtenerNumeroFilasAsync();

        await _catalogoPageCopia.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _catalogoPageCopia.ObtenerNumeroFilasAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
            "La tabla no recuperó todos los registros originales tras limpiar los filtros.");

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas),
            "El filtro de prueba no redujo el tamaño de la tabla, usa un artículo diferente.");
    }

    [Test]
    public async Task AgregarPrendaCarrito()
    {
        await _catalogoPageCopia.IrACatalogoAsync();

        var producto = "5117";
        var talla = "XL";
        var cantidad = "2";

        await _catalogoPageCopia.SeleccionarProductoCatalogoAsync(producto);

        await _catalogoPageCopia.AgregarDatosProductoAsync(talla, cantidad);
        await _catalogoPageCopia.AgregarACestaAsync();

        var mensaje = await _catalogoPageCopia.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain($"Se añadió CHALECO AIRBAG (talla {talla}) x{cantidad} a la cesta."));
    }

    [Test]
    public async Task VolverAlCatalogo()
    {
        await _catalogoPageCopia.IrACatalogoAsync();

        var producto = "5117";

        await _catalogoPageCopia.SeleccionarProductoCatalogoAsync(producto);

        await _catalogoPageCopia.VolverAlCatalogoAsync();
    }

    [Test]
    public async Task FiltroLotes_PorCodigo()
    {
        await _catalogoPageCopia.IrACatalogoAsync();
        await _catalogoPageCopia.IrALotesDisponiblesAsync();

        string codigo = "LT-00056";
        await _catalogoPageCopia.FiltrarPorLoteAsync(codigo);

        bool existe = await _catalogoPageCopia.ExisteElementoEnTablaAsync(codigo);
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el Código 'LT-00057' tras 5 segundos.");
    }

    [Test]
    public async Task FiltroLotes_PorNombre()
    {
        await _catalogoPageCopia.IrACatalogoAsync();
        await _catalogoPageCopia.IrALotesDisponiblesAsync();

        string nombre = "Sudaderas2pruebaaa";
        await _catalogoPageCopia.FiltrarPorLoteAsync(nombre);

        bool existe = await _catalogoPageCopia  .ExisteElementoEnTablaAsync(nombre);
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el nombre 'Lote prueba1' tras 5 segundos.");
    }

    [Test]
    public async Task FiltroLotes_SinResultados()
    {
        await _catalogoPageCopia.IrACatalogoAsync();
        await _catalogoPageCopia.IrALotesDisponiblesAsync();

        string nombre = "lote inventado";
        await _catalogoPageCopia.FiltrarPorLoteAsync(nombre);

        var mensajeVacio = await _catalogoPageCopia.ExisteElementoEnTablaAsync(nombre);
        Assert.That(mensajeVacio, Is.True);
    }

    [Test]
    public async Task LimpiarFiltrosLotes()
    {
        await _catalogoPageCopia.IrACatalogoAsync();
        await _catalogoPageCopia.IrALotesDisponiblesAsync();

        int totalFilasOriginales = await _catalogoPageCopia.ObtenerNumeroFilasAsync();

        string nombre = "Sudaderas2pruebaaa";
        await _catalogoPageCopia.FiltrarPorLoteAsync(nombre);

        int filasFiltradas = await _catalogoPageCopia.ObtenerNumeroFilasAsync();

        await _catalogoPageCopia.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _catalogoPageCopia.ObtenerNumeroFilasAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
            "La tabla no recuperó todos los registros originales tras limpiar los filtros.");

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas),
            "El filtro de prueba no redujo el tamaño de la tabla, usa un artículo diferente.");
    }

    [Test]
    public async Task AgregarLoteCarrito()
    {
        await _catalogoPageCopia.IrACatalogoAsync();
        await _catalogoPageCopia.IrALotesDisponiblesAsync();

        string nombre = "Prueba Test";
        await _catalogoPageCopia.AgregarLoteAlCarritoAsync(nombre);

        var mensaje = await _catalogoPageCopia.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.EndWith("añadido correctamente a la cesta."));
    }
    */
}