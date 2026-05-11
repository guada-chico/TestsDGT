using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Pruebas.Login;

public class LoginCorrectoTests
{
    [Test]

    public async Task LoginCorrecto()
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
        await page.Locator("input[type='password']").FillAsync("temporal123");

        await page.ClickAsync("text=ENTRAR");

        Assert.That(page.Url, Does.Contain("/dgt-front/#/catalogo"));
    }
}