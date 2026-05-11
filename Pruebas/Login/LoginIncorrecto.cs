using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Pruebas.Login;

public class LoginIncorrectoTests : BaseTest
{
    [Test]

    public async Task LoginIncorrecto_PasswordIncorrecto()
    {
        var error = Page.Locator(".custom-alert");
        await error.WaitForAsync();

        var textoError = await error.InnerTextAsync();

        Assert.That(textoError, Does.Contain("Usuario (TIP) o contraseña incorrectos"));
    }

    [Test]

    public async Task LoginIncorrecto_UsuarioIncorrecto()
    {
        var error = Page.Locator(".custom-alert");
        await error.WaitForAsync();

        var textoError = await error.InnerTextAsync();

        Assert.That(textoError, Does.Contain("Usuario (TIP) o contraseña incorrectos"));
    }
}