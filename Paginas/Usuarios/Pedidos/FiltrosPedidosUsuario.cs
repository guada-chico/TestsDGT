using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.Pedidos;

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

    [Test]
    public async Task FiltroFechaDesde()
    {
        await Page.GetByText("GUADALUPE").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Mis pedidos" }).ClickAsync();

        await Page.WaitForURLAsync("**/orders-list");

        await Page.Locator("#startDate").GetByRole(AriaRole.Button, new() { Name = "Choose Date" }).ClickAsync();
        await Page.FillAsync("#startDate input[placeholder='dd/mm/aaaa']", "18/04/2026");
        await Page.Keyboard.PressAsync("Tab");

        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-filter") }).ClickAsync();

        // Busca en qué posición está la columna que dice "Fecha"
        var cabeceras = await Page.Locator("th").AllInnerTextsAsync();
        int indiceColumna = cabeceras.ToList().FindIndex(t => t.Contains("Fecha"));

        Assert.That(indiceColumna, Is.GreaterThan(-1), "No se encontró la columna 'Fecha' en la tabla.");

        var primerCelda = Page.Locator("tbody tr td").First;
        await primerCelda.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var fechasPedidos = await Page.Locator($"tbody tr td:nth-child({indiceColumna + 1})").AllInnerTextsAsync();

        Assert.That(fechasPedidos, Is.Not.Empty, "El filtro no devolvió ningún pedido (la tabla está vacía).");

        DateTime fechaFiltro = new DateTime(2026, 4, 18);

        foreach (var fechaTexto in fechasPedidos)
        {
            if (DateTime.TryParse(fechaTexto.Trim(), out DateTime fechaPedido))
            {
                Assert.That(
                    fechaPedido.Date >= fechaFiltro.Date,
                    $"Error: La fecha {fechaPedido:dd/MM/yyyy} es menor que el filtro {fechaFiltro:dd/MM/yyyy}"
                );
            }
            else
            {
                Assert.Fail($"No se pudo procesar el formato de fecha: '{fechaTexto}'");
            }
        }
    }

    [Test]
    public async Task FiltroFechaHasta()
    {
        await Page.GetByText("GUADALUPE").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Mis pedidos" }).ClickAsync();

        await Page.WaitForURLAsync("**/orders-list");

        var inputHasta = Page.Locator("#endDate input[placeholder='dd/mm/aaaa']");

        await inputHasta.ClearAsync();
        await inputHasta.FillAsync("18/04/2026");

        await Page.Keyboard.PressAsync("Tab");

        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-filter") }).ClickAsync();

        var cabeceras = await Page.Locator("th").AllInnerTextsAsync();
        int indiceColumna = cabeceras.ToList().FindIndex(t => t.Contains("Fecha"));

        Assert.That(indiceColumna, Is.GreaterThan(-1), "No se encontró la columna 'Fecha' en la tabla.");

        var primerCelda = Page.Locator("tbody tr td").First;
        await primerCelda.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var fechasPedidos = await Page.Locator($"tbody tr td:nth-child({indiceColumna + 1})").AllInnerTextsAsync();

        Assert.That(fechasPedidos, Is.Not.Empty, "El filtro no devolvió ningún pedido (la tabla está vacía).");

        DateTime fechaFiltro = new DateTime(2026, 4, 18);

        foreach (var fechaTexto in fechasPedidos)
        {
            if (DateTime.TryParse(fechaTexto.Trim(), out DateTime fechaPedido))
            {
                Assert.That(
                    fechaPedido.Date <= fechaFiltro.Date,
                    $"Error: La fecha {fechaPedido:dd/MM/yyyy} es mayor que el filtro {fechaFiltro:dd/MM/yyyy}"
                );
            }
            else
            {
                Assert.Fail($"No se pudo procesar el formato de fecha: '{fechaTexto}'");
            }
        }
    }

    [Test]
    public async Task FiltroRangoFecha()
    {
        await Page.GetByText("GUADALUPE").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Mis pedidos" }).ClickAsync();

        await Page.WaitForURLAsync("**/orders-list");

        var inputDesde = Page.Locator("#startDate input[placeholder='dd/mm/aaaa']");

        await inputDesde.ClearAsync();
        await inputDesde.FillAsync("18/04/2026");

        await Page.Keyboard.PressAsync("Tab");

        var inputHasta = Page.Locator("#endDate input[placeholder='dd/mm/aaaa']");

        await inputHasta.ClearAsync();
        await inputHasta.FillAsync("26/04/2026");

        await Page.Keyboard.PressAsync("Tab");

        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-filter") }).ClickAsync();

        var cabeceras = await Page.Locator("th").AllInnerTextsAsync();
        int indiceColumna = cabeceras.ToList().FindIndex(t => t.Contains("Fecha"));

        Assert.That(indiceColumna, Is.GreaterThan(-1), "No se encontró la columna 'Fecha' en la tabla.");

        var primerCelda = Page.Locator("tbody tr td").First;
        await primerCelda.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var fechasPedidos = await Page.Locator($"tbody tr td:nth-child({indiceColumna + 1})").AllInnerTextsAsync();

        Assert.That(fechasPedidos, Is.Not.Empty, "El filtro no devolvió ningún pedido (la tabla está vacía).");

        DateTime fechaFiltroDesde = new DateTime(2026, 4, 18);
        DateTime fechaFiltroHasta = new DateTime(2026, 4, 26);

        foreach (var fechaTexto in fechasPedidos)
        {
            if (DateTime.TryParse(fechaTexto.Trim(), out DateTime fechaPedido))
            {
                Assert.That(
                    fechaPedido.Date >= fechaFiltroDesde.Date && fechaPedido.Date <= fechaFiltroHasta.Date,
                    $"Error: La fecha {fechaPedido:dd/MM/yyyy} está fuera del rango de filtros [{fechaFiltroDesde:dd/MM/yyyy}, {fechaFiltroHasta:dd/MM/yyyy}]"
                );
            }
            else
            {
                Assert.Fail($"No se pudo procesar el formato de fecha: '{fechaTexto}'");
            }
        }
    }

    [Test]

    public async Task FiltroPedidoCodigo()
    {
        await Page.GetByText("GUADALUPE").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Mis pedidos" }).ClickAsync();

        await Page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/orders-list");

        await Page.GetByPlaceholder("Código").FillAsync("21488");
        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-filter") }).ClickAsync();

        var celdaDivisa = Page.Locator("td").Filter(new() { HasText = "21488" });

        try
        {
            await Assertions.Expect(celdaDivisa).ToBeVisibleAsync(new() { Timeout = 5000 });
        }
        catch (Exception)
        {
            Assert.Fail("El filtro no devolvió la celda con el Código '21488' tras 5 segundos.");
        }
    }
}