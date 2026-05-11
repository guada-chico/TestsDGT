using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Pruebas.Pedidos;

public class CancelarPedidoUsuarioTest : BaseTest
{
    [Test]

    public async Task CancelarPedido()
    {
        var filaProducto = Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "777" });

        await filaProducto.WaitForAsync();
        await filaProducto.Locator("button.p-button-icon-only").ClickAsync();

        await Page.Locator("#talla").ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "M" }).ClickAsync();

        await Page.Locator("#cantidad").ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "2" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Añadir a la cesta" }).ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Close" }).ClickAsync();

        await Page.Locator(".cesta").ClickAsync();
        var filaArticulo = Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "Zapatillas Nike 3.0" });
        await filaArticulo.Locator("button").Filter(new() { Has = Page.Locator(".pi-trash") }).ClickAsync();

        var exito = Page.Locator(".p-toast");

        await exito.WaitForAsync();

        var textoExito = await exito.InnerTextAsync();

        Assert.That(textoExito, Does.Contain("Producto eliminado con éxito"));
    }
}