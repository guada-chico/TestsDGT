using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Pruebas.Pedidos;

public class FiltrosPedidosUsuarioTest : BaseTest
{
    [Test]

    public async Task FiltroPedidoNombreArticulo()
    {
        await Page.GetByText("GUADALUPE").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Mis pedidos" }).ClickAsync();

        await Page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/orders-list");

        await Page.GetByPlaceholder("Nombre del artículo").FillAsync("DIVISA");
        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-filter") }).ClickAsync();

        var celdaDivisa = Page.Locator("td").Filter(new() { HasText = "DIVISA" });

        try
        {
            await Assertions.Expect(celdaDivisa).ToBeVisibleAsync(new() { Timeout = 5000 });
        }
        catch (Exception)
        {
            Assert.Fail("El filtro no devolvió la celda con el nombre 'DIVISA' tras 5 segundos.");
        }
    }

    [Test]

    public async Task FiltroPedidoEstado()
    {
        await Page.GetByText("GUADALUPE").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Mis pedidos" }).ClickAsync();

        await Page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/orders-list");

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Todos" }).ClickAsync();
        await Page.GetByText("En proceso").ClickAsync();

        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-filter") }).ClickAsync();

        var celdaEstado = Page.Locator("tbody td").Filter(new() { HasText = "En proceso" }).First;

        try
        {
            await Assertions.Expect(celdaEstado).ToBeVisibleAsync(new() { Timeout = 5000 });
        }
        catch (Exception)
        {
            Assert.Fail("El filtro no devolvió la celda con el estado 'En proceso' tras 5 segundos.");
        }
    }
}