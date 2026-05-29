using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.MiCesta;

[TestFixture]
public class MiCestaTest : BaseTest
{
    private MiCestaPage _miCestaPage;

    [SetUp]
    public void SetupPagina()
    {
        _miCestaPage = new MiCestaPage(Page);
    }

    [Test]
    public async Task IrACesta()
    {
        await _miCestaPage.IrACestaAsync();

        await Assertions.Expect(Page).ToHaveURLAsync(new Regex(".*/app-cart.*"));
    }

    [Test]
    public async Task PedidoOrdinarioExito()
    {
        await _miCestaPage.IrACestaAsync();

        await _miCestaPage.TramitarPedidoOrdinarioAsync();

        var mensaje = await _miCestaPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Pedido realizado con éxito."));
    }
}