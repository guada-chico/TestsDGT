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

        bool enServidor = Environment.GetEnvironmentVariable("AZURE_DEVOPS") == "true";
        Browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = enServidor,
            SlowMo = enServidor ? 0 : 500
        });

        Page = await Browser.NewPageAsync();

        await Page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/login");
        await Page.GetByPlaceholder("Usuario (TIP)").FillAsync("A22222A");
        await Page.Locator("input[type='password']").FillAsync("Temporal123!");
        await Page.ClickAsync("text=ENTRAR");

        var botonSesion = Page.GetByRole(AriaRole.Button, new() { Name = " Cerrar sesion anterior " });
        if (await botonSesion.IsVisibleAsync())
        {
            await botonSesion.ClickAsync();
        }

        await Page.WaitForURLAsync("**/catalogo");
    }

    [TearDown]
    public async Task Teardown()
    {
        if (Page != null)
        {
            try
            {
                await Page.GetByText("GUADALUPE").ClickAsync();

                await Page.GetByText("Cerrar sesión").ClickAsync();

                var botonSi = Page.Locator("button.btn-si");

                await botonSi.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 2000 });

                await botonSi.ClickAsync();

                await Page.WaitForURLAsync(new System.Text.RegularExpressions.Regex(".*login"), new() { Timeout = 4000 });
            }
            catch (Exception)
            {
                // Si el test falló a mitad y no estamos en la pantalla donde se puede cerrar sesión,
                // el catch captura el error silenciosamente para que NUnit no se queje en el reporte.
            }
        }

        if (Browser != null) await Browser.CloseAsync();
        Playwright?.Dispose();
    }
}