using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Pruebas.Usuarios.Pedidos;

public class PedidoExtraordinarioUsuarioTest : BaseTest
{
    [Test]

    public async Task PedidoExtraordinario_OrdinarioExito()
    {
        await Page.GetByText("GUADALUPE").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Pedido extraordinario" }).ClickAsync();

        await Page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/pedido-extraordinario");

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccione un artículo" }).ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "CHAQUETA TECNICA" }).ClickAsync();

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccione una talla" }).ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "M/L" }).ClickAsync();

        await Page.GetByRole(AriaRole.Spinbutton, new() { Name = "Cantidad de artículos*" }).FillAsync("3");

        await Page.Locator("input[type='file']").SetInputFilesAsync(@"C:\repos\TestsDGT\6073873.png");

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "He confirmado mi talla*" }).CheckAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Motivo del pedido*" }).FillAsync("Necesito más");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Realizar" }).ClickAsync();

        var exito = Page.Locator(".p-toast");

        await exito.WaitForAsync();

        var textoExito = await exito.InnerTextAsync();

        Assert.That(textoExito, Does.Contain("Pedido enviado"));
    }

    [Test]

    public async Task PedidoExtraordinario_UrgenteExito()
    {
        await Page.GetByText("GUADALUPE").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Pedido extraordinario" }).ClickAsync();

        await Page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/pedido-extraordinario");

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccione un artículo" }).ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "CHAQUETA TECNICA" }).ClickAsync();

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccione una talla" }).ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "M/L" }).ClickAsync();

        await Page.GetByRole(AriaRole.Radio, new() { Name = "Urgente" }).CheckAsync();

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccionar motivo" }).ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "Deterioro prematuro de las" }).ClickAsync();

        await Page.Locator("#archivoUrgencia").SetInputFilesAsync(@"C:\repos\TestsDGT\6073873.png");

        await Page.GetByRole(AriaRole.Spinbutton, new() { Name = "Cantidad de artículos*" }).FillAsync("3");

        await Page.Locator("#archivo").SetInputFilesAsync(@"C:\repos\TestsDGT\6073873.png");

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "He confirmado mi talla*" }).CheckAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Motivo del pedido*" }).FillAsync("Necesito más");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Realizar" }).ClickAsync();

        var exito = Page.Locator(".p-toast");

        await exito.WaitForAsync();

        var textoExito = await exito.InnerTextAsync();

        Assert.That(textoExito, Does.Contain("Pedido enviado"));
    }

    [Test]

    public async Task PedidoExtraordinario_Error()
    {
        await Page.GetByText("GUADALUPE").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Pedido extraordinario" }).ClickAsync();

        await Page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/pedido-extraordinario");

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccione un artículo" }).ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "CHAQUETA TECNICA" }).ClickAsync();

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccione una talla" }).ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "M/L" }).ClickAsync();

        await Page.GetByRole(AriaRole.Radio, new() { Name = "Urgente" }).CheckAsync();

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccionar motivo" }).ClickAsync();
        await Page.GetByRole(AriaRole.Option, new() { Name = "Deterioro prematuro de las" }).ClickAsync();

        await Page.Locator("#archivoUrgencia").SetInputFilesAsync(@"C:\repos\TestsDGT\6073873.png");

        await Page.GetByRole(AriaRole.Spinbutton, new() { Name = "Cantidad de artículos*" }).FillAsync("3");

        await Page.Locator("#archivo").SetInputFilesAsync(@"C:\repos\TestsDGT\6073873.png");

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Motivo del pedido*" }).FillAsync("Necesito más");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Realizar" }).ClickAsync();

        var exito = Page.Locator(".p-toast");

        await exito.WaitForAsync();

        var textoExito = await exito.InnerTextAsync();

        Assert.That(textoExito, Does.Contain("Error en el formulario"));
    }
}