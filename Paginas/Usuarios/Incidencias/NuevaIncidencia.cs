using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Incidencias;

public class NuevaIncidenciaTest : BaseTest
{
    [Test]

    public async Task NuevaIncidencia()
    {
        await Page.GetByText("GUADALUPE").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Incidencias" }).ClickAsync();

        await Page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/mis-incidencias");

        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-plus") }).ClickAsync();

        await Page.Locator("#titulo").FillAsync("Camiseta mal tallada");

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Media" }).ClickAsync();
        await Page.Locator("#prioridad_2").ClickAsync();

        await Page.Locator("#motivo").FillAsync("Camiseta mal tallada");
        await Page.Locator("#descripcion").FillAsync("La talla S es muy grande");

        await Page.Locator("#idPedido").FillAsync("123");

        await Page.Locator("#archivo").SetInputFilesAsync(@"C:\repos\TestsDGT\6073873.png");

        await Page.Locator("#descripcion").FillAsync("descripción incidencia");

        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-check") }).ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var celdaIncidencia = Page.Locator("td").GetByText("Camiseta mal tallada", new() { Exact = true }).First;

        await Assertions.Expect(celdaIncidencia).ToBeVisibleAsync(new() { Timeout = 15000 });
    }

    [Test]

    public async Task NuevaIncidencia_ErrorFaltanDatos() // Falta Título incidencia
    {
        await Page.GetByText("GUADALUPE").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Incidencias" }).ClickAsync();

        await Page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/mis-incidencias");

        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-plus") }).ClickAsync();

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Media" }).ClickAsync();
        await Page.Locator("#prioridad_2").ClickAsync();

        await Page.Locator("#motivo").FillAsync("Camiseta mal tallada");
        await Page.Locator("#descripcion").FillAsync("La talla S es muy grande");

        await Page.Locator("#idPedido").FillAsync("123");

        await Page.Locator("#archivo").SetInputFilesAsync(@"C:\repos\TestsDGT\6073873.png");

        await Page.Locator("#descripcion").FillAsync("descripción incidencia");

        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-check") }).ClickAsync();

        var exito = Page.Locator(".p-toast");
        await exito.WaitForAsync();

        var textoExito = await exito.InnerTextAsync();

        Assert.That(textoExito, Does.Contain("El título es obligatorio."));
    }
}