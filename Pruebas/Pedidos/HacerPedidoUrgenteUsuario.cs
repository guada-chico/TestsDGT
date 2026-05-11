using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Pruebas.Pedidos;

public class PedidoUrgenteUsuarioTest : BaseTest
{
    [Test]

    public async Task PedidoUrgenteExito()
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
       
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Confirmar talla" }).CheckAsync();

        await Page.Locator("input[value='Urgente']").CheckAsync(new() { Force = true });
        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccionar motivo" }).ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "Accidente" }).ClickAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Añade instrucciones" }).FillAsync("Pedido urgente");

        await Page.Locator("input[type='file']").SetInputFilesAsync(@"C:\repos\TestsDGT\6073873.png");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Tramitar" }).ClickAsync();

        var exito = Page.Locator(".p-toast");
        await exito.WaitForAsync();

        var textoExito = await exito.InnerTextAsync();

        Assert.That(textoExito, Does.Contain("Pedido realizado con éxito."));
    }
}