using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.Catalogo;

[TestFixture]
public class CatalogoTest : BaseTest
{
    private CatalogoPage _catalogoPage;

    [SetUp]
    public void SetupPagina()
    {
        _catalogoPage = new CatalogoPage(Page);
    }

    [Test]
    public async Task IrAMisPedidosDesdeCatalogo()
    {
        await _catalogoPage.IrACatalogoAsync();

        await _catalogoPage.IrAMisPedidosAsync();

        await Assertions.Expect(Page).ToHaveURLAsync(new Regex(".*/pedidos.*"));
    }

    [Test]
    public async Task FiltroCatalogoCodigo()
    {
        await _catalogoPage.IrACatalogoAsync();

        string codigoProd = "5117";
        await _catalogoPage.FiltrarPorCodigoAsync(codigoProd);

        Assert.That(await _catalogoPage.ExisteElementoEnTablaAsync(codigoProd), Is.True);
        Assert.That(await _catalogoPage.ObtenerNumeroFilasAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task FiltroCatalogoNombre()
    {
        await _catalogoPage.IrACatalogoAsync();

        string nombreProd = "CHALECO AIRBAG";
        await _catalogoPage.FiltrarPorNombreAsync(nombreProd);

        Assert.That(await _catalogoPage.ExisteElementoEnTablaAsync(nombreProd), Is.True);
        Assert.That(await _catalogoPage.ObtenerNumeroFilasAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task FiltroCatalogoCodigo_SinResultados()
    {
        await _catalogoPage.IrACatalogoAsync();

        string codigoProd = "CodigoInexistente";
        await _catalogoPage.FiltrarPorCodigoAsync(codigoProd);

        var mensajeVacio = await _catalogoPage.ExisteElementoEnTablaAsync("No hay resultados para tu búsqueda");
        Assert.That(mensajeVacio, Is.True);
    }

    [Test]
    public async Task FiltroCatalogoNombre_SinResultados()
    {
        await _catalogoPage.IrACatalogoAsync();

        string nombreProd = "NombreInexistente";
        await _catalogoPage.FiltrarPorNombreAsync(nombreProd);

        var mensajeVacio = await _catalogoPage.ExisteElementoEnTablaAsync("No hay resultados para tu búsqueda");
        Assert.That(mensajeVacio, Is.True);
    }

    [Test]
    public async Task LimpiarFiltrosCatalogo()
    {
        await _catalogoPage.IrACatalogoAsync();

        int totalFilasOriginales = await _catalogoPage.ObtenerNumeroFilasAsync();

        string codigoProd = "5117";
        await _catalogoPage.FiltrarPorCodigoAsync(codigoProd);

        int filasFiltradas = await _catalogoPage.ObtenerNumeroFilasAsync();

        await _catalogoPage.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _catalogoPage.ObtenerNumeroFilasAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
            "La tabla no recuperó todos los registros originales tras limpiar los filtros.");

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas),
            "El filtro de prueba no redujo el tamaño de la tabla, usa un artículo diferente.");
    }

    [Test]
    public async Task AgregarPrendaCarrito()
    {
        await _catalogoPage.IrACatalogoAsync();

        var producto = "5117";
        var talla = "L";
        var cantidad = "2";

        await _catalogoPage.SeleccionarProductoCatalogoAsync(producto);

        await _catalogoPage.AgregarDatosProductoAsync(talla, cantidad);
        await _catalogoPage.AgregarACestaAsync();

        var mensaje = await _catalogoPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Se añadió CHALECO AIRBAG (talla L) x" + cantidad + " a la cesta."));
    }

    [Test]
    public async Task VolverAlCatalogo()
    {
        await _catalogoPage.IrACatalogoAsync();

        var producto = "5117";

        await _catalogoPage.SeleccionarProductoCatalogoAsync(producto);

        await _catalogoPage.VolverAlCatalogoAsync();
    }

    [Test]
    public async Task FiltroLotes_PorCodigo()
    {
        await _catalogoPage.IrACatalogoAsync();
        await _catalogoPage.IrALotesDisponiblesAsync();

        string codigo = "LT-00056";
        await _catalogoPage.FiltrarPorLoteAsync(codigo);

        bool existe = await _catalogoPage.ExisteElementoEnTablaAsync(codigo);
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el Código 'LT-00057' tras 5 segundos.");
    }

    [Test]
    public async Task FiltroLotes_PorNombre()
    {
        await _catalogoPage.IrACatalogoAsync();
        await _catalogoPage.IrALotesDisponiblesAsync();

        string nombre = "Sudaderas2pruebaaa";
        await _catalogoPage.FiltrarPorLoteAsync(nombre);

        bool existe = await _catalogoPage.ExisteElementoEnTablaAsync(nombre);
        Assert.That(existe, Is.True, "El filtro no devolvió la celda con el nombre 'Lote prueba1' tras 5 segundos.");
    }

    [Test]
    public async Task FiltroLotes_SinResultados()
    {
        await _catalogoPage.IrACatalogoAsync();
        await _catalogoPage.IrALotesDisponiblesAsync();

        string nombre = "lote inventado";
        await _catalogoPage.FiltrarPorLoteAsync(nombre);

        var mensajeVacio = await _catalogoPage.ExisteElementoEnTablaAsync(nombre);
        Assert.That(mensajeVacio, Is.True);
    }

    [Test]
    public async Task LimpiarFiltrosLotes()
    {
        await _catalogoPage.IrACatalogoAsync();
        await _catalogoPage.IrALotesDisponiblesAsync();

        int totalFilasOriginales = await _catalogoPage.ObtenerNumeroFilasAsync();

        string nombre = "Sudaderas2pruebaaa";
        await _catalogoPage.FiltrarPorLoteAsync(nombre);

        int filasFiltradas = await _catalogoPage.ObtenerNumeroFilasAsync();

        await _catalogoPage.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _catalogoPage.ObtenerNumeroFilasAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
            "La tabla no recuperó todos los registros originales tras limpiar los filtros.");

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas),
            "El filtro de prueba no redujo el tamaño de la tabla, usa un artículo diferente.");
    }

    [Test]
    public async Task AgregarLoteCarrito()
    {
        await _catalogoPage.IrACatalogoAsync();
        await _catalogoPage.IrALotesDisponiblesAsync();

        string codigo = "LT-00059";
        await _catalogoPage.AgregarLoteAlCarritoAsync(codigo);

        var mensaje = await _catalogoPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Lote \"Prueba Test\" añadido correctamente a la cesta."));
    }
}