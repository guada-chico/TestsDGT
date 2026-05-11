using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Pruebas.Login;

public class LoginIncorrectoTests
{
    [Test]

    public async Task LoginIncorrecto_PasswordIncorrecto()
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
        await page.Locator("input[type='password']").FillAsync("incorrecto");

        await page.ClickAsync("text=ENTRAR");

        var error = page.Locator(".custom-alert");
        await error.WaitForAsync();

        var textoError = await error.InnerTextAsync();

        Assert.That(textoError, Does.Contain("Usuario (TIP) o contraseña incorrectos"));
    }

    [Test]

    public async Task LoginIncorrecto_UsuarioIncorrecto()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 500
        });

        var page = await browser.NewPageAsync();

        await page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/login");

        await page.GetByPlaceholder("Usuario (TIP)").FillAsync("incorrecto");
        await page.Locator("input[type='password']").FillAsync("Temporal123!");

        await page.ClickAsync("text=ENTRAR");

        var error = page.Locator(".custom-alert");
        await error.WaitForAsync();

        var textoError = await error.InnerTextAsync();

        Assert.That(textoError, Does.Contain("Usuario (TIP) o contraseña incorrectos"));
    }
}