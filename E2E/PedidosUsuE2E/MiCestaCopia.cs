using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.E2E.PedidosUsuE2E;

[TestFixture]
public class MiCestaCopiaTest : BaseTest
{
    private MiCestaPageCopia _catalogoPageCopia;

    [SetUp]
    public void SetupPagina()
    {
        _catalogoPageCopia = new MiCestaPageCopia(Page);
    }
    /*
    [Test]
    public async Task AgregarArticuloCarrito()
    {
        await _catalogoPageCopia.IrACatalogoAsync();

        await _catalogoPageCopia.AgregarProductoAlCarritoAsync("5117", "L", "2");

        var mensaje = await _catalogoPageCopia.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Se añadió CHALECO AIRBAG (talla L) x2 a la cesta."));
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
    public async Task AgregarLoteCarrito()
    {
        await _catalogoPageCopia.IrACatalogoAsync();
        await _catalogoPageCopia.IrALotesDisponiblesAsync();

        string codigo = "LT-00059";
        await _catalogoPageCopia.AgregarLoteAlCarritoAsync(codigo);

        var mensaje = await _catalogoPageCopia.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Lote \"Prueba Test\" añadido correctamente a la cesta."));
    }
    */
}