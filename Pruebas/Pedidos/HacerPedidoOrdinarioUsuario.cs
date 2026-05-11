using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Pruebas.Pedidos;

public class PedidoOrdinarioUsuarioTest
{
    [Test]

    public async Task PedidoOrdinarioExito()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 500
        });

        var page = await browser.NewPageAsync();

        await page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/login");

        await page.GetByPlaceholder("Usuario (TIP)").FillAsync("A22222A");
        await page.Locator("input[type='password']").FillAsync("Temporal123!");

        await page.ClickAsync("text=ENTRAR");

        await page.WaitForURLAsync("**/catalogo");

        var filaProducto = page.GetByRole(AriaRole.Row).Filter(new() { HasText = "777" });

        await filaProducto.WaitForAsync();
        await filaProducto.Locator("button.p-button-icon-only").ClickAsync();

        await page.Locator("#talla").ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "M" }).ClickAsync();

        await page.Locator("#cantidad").ClickAsync();
        await page.GetByRole(AriaRole.Option, new() { Name = "2" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Añadir a la cesta" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Close" }).ClickAsync();

        await page.Locator(".cesta").ClickAsync();
        await page.GetByRole(AriaRole.Checkbox, new() { Name = "Confirmar talla" }).CheckAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Tramitar" }).ClickAsync();

        var exito = page.Locator(".p-toast");

        await exito.WaitForAsync();

        var textoExito = await exito.InnerTextAsync();

        Assert.That(textoExito, Does.Contain("Pedido realizado con éxito."));
    }
}