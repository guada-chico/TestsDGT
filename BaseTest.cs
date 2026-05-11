using Microsoft.Playwright;
using NUnit.Framework;

namespace TestsDGT;

public class BaseTest
{
    protected IPlaywright Playwright;
    protected IBrowser Browser;
    protected IPage Page;

    [SetUp]
    public async Task Setup()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 500
        });

        Page = await Browser.NewPageAsync();

        // --- AQUÍ PONES TU LOGIN ÚNICO ---
        await Page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/login");
        await Page.GetByPlaceholder("Usuario (TIP)").FillAsync("A22222A");
        await Page.Locator("input[type='password']").FillAsync("Temporal123!");
        await Page.ClickAsync("text=ENTRAR");

        // Gestión de sesión anterior (el IF que ya tienes)
        var botonSesion = Page.GetByRole(AriaRole.Button, new() { Name = " Cerrar sesion anterior" });
        if (await botonSesion.IsVisibleAsync())
        {
            await botonSesion.ClickAsync();
        }

        await Page.WaitForURLAsync("**/catalogo");
    }

    [TearDown]
    public async Task Teardown()
    {
        await Browser.CloseAsync();
        Playwright.Dispose();
    }
}