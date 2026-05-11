using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Pruebas.Catalogo;

public class FiltrosCatalogoTest : BaseTest
{
    [Test]

    public async Task FiltrosCatalogoCodigo()
    {
        await Page.GetByPlaceholder("Buscar por código").FillAsync("777");
        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-filter") }).ClickAsync();

        var celda777 = Page.Locator("td").Filter(new() { HasText = "777" });

        try
        {
            await Assertions.Expect(celda777).ToBeVisibleAsync(new() { Timeout = 5000 });
        }
        catch (Exception)
        {
            // Si falla aquí, sabremos que el filtro no devolvió resultados o el botón no hizo nada
            Assert.Fail("El filtro no devolvió la celda con el código 777 tras 5 segundos.");
        }

        // 2. Si encontró la celda, ahora buscamos su fila para contar
        var filasDatos = Page.Locator("tbody tr");
        await Assertions.Expect(filasDatos).ToHaveCountAsync(1); // Validamos que SOLO hay 1 fila

        await Page.Locator("button.btn-clear").ClickAsync();
    }

    [Test]

    public async Task FiltrosCatalogoNombre()
    {
        await Page.GetByPlaceholder("Buscar por nombre").FillAsync("Zapatillas deportivas de Nike");
        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-filter") }).ClickAsync();

        var celdaNombre = Page.Locator("td").Filter(new() { HasText = "Zapatillas deportivas de Nike" });

        try
        {
            await Assertions.Expect(celdaNombre).ToBeVisibleAsync(new() { Timeout = 5000 });
        }
        catch (Exception)
        {
            // Si falla aquí, sabremos que el filtro no devolvió resultados o el botón no hizo nada
            Assert.Fail("El filtro no devolvió la celda con el nombre 'Zapatillas deportivas de Nike' tras 5 segundos.");
        }

        // 2. Si encontró la celda, ahora buscamos su fila para contar
        var filasDatos = Page.Locator("tbody tr");
        await Assertions.Expect(filasDatos).ToHaveCountAsync(1); // Validamos que SOLO hay 1 fila

        await Page.Locator("button.btn-clear").ClickAsync();
    }
}