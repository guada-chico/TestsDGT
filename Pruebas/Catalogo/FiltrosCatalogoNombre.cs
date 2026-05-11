using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Pruebas.Catalogo;

public class FiltrosCatalogoNombreTest
{
    [Test]

    public async Task FiltrosCatalogoNombre()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
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

        await page.GetByPlaceholder("Buscar por nombre").FillAsync("Zapatillas deportivas de Nike");
        await page.Locator("button").Filter(new() { Has = page.Locator(".pi-filter") }).ClickAsync();

        var celdaNombre = page.Locator("td").Filter(new() { HasText = "Zapatillas deportivas de Nike" });

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
        var filasDatos = page.Locator("tbody tr");
        await Assertions.Expect(filasDatos).ToHaveCountAsync(1); // Validamos que SOLO hay 1 fila

        await page.Locator("button.btn-clear").ClickAsync();
    }
}