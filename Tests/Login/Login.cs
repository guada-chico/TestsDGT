using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using TestsDGT.Paginas.Login;

namespace TestsDGT.Tests.Login;

[TestFixture]
public class LoginTests
{
    protected IPlaywright Playwright;
    protected IBrowser Browser;
    protected IPage Page;
    private LoginPage _loginPage;

    [SetUp]
    public async Task SetupPagina()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        bool enServidor = Environment.GetEnvironmentVariable("AZURE_DEVOPS") == "true";

        Browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = enServidor,
            SlowMo = enServidor ? 0 : 500
        });

        Page = await Browser.NewPageAsync();

        _loginPage = new LoginPage(Page);
    }

    [TearDown]
    public async Task Teardown()
    {
        if (Browser != null) await Browser.CloseAsync();
        Playwright?.Dispose();
    }

    [Test]

    public async Task LoginCorrecto()
    {
        await _loginPage.IrALoginAsync();
        await _loginPage.RealizarLoginAsync("A22222A", "Temporal123!");
        await Assertions.Expect(Page).ToHaveURLAsync(new Regex(".*catalogo"), new() { Timeout = 15000 });
    }

    [Test]

    public async Task LoginIncorrecto_Credenciales()
    {
        await _loginPage.IrALoginAsync();
        await _loginPage.RealizarLoginAsync("usuarioIncorrecto", "contraseñaIncorrecta");

        string textoError = await _loginPage.ObtenerMensajeErrorAsync();

        Assert.That(textoError, Does.Contain("Usuario (TIP) o contraseña incorrectos"));
    }

    [Test]

    public async Task LoginIncorrecto_Password()
    {
        await _loginPage.IrALoginAsync();
        await _loginPage.RealizarLoginAsync("A22222A", "contraseñaIncorrecta");

        string textoError = await _loginPage.ObtenerMensajeErrorAsync();

        Assert.That(textoError, Does.Contain("Usuario (TIP) o contraseña incorrectos"));
    }

    [Test]

    public async Task LoginIncorrecto_Usuario()
    {
        await _loginPage.IrALoginAsync();
        await _loginPage.RealizarLoginAsync("usuarioIncorrecto", "Temporal123!");

        string textoError = await _loginPage.ObtenerMensajeErrorAsync();

        Assert.That(textoError, Does.Contain("Usuario (TIP) o contraseña incorrectos"));
    }
}