using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.MiCesta;

[TestFixture]
public class MiCestaTest : BaseTest
{
    private MiCestaPage _catalogoPage;

    [SetUp]
    public void SetupPagina()
    {
        _catalogoPage = new MiCestaPage(Page);
    }

    [Test]
    public async Task AgregarArticuloCarrito()
    {
        await _catalogoPage.IrACatalogoAsync();

        await _catalogoPage.AgregarProductoAlCarritoAsync("5117", "L", "2");

        var mensaje = await _catalogoPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Se añadió CHALECO AIRBAG (talla L) x2 a la cesta."));
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