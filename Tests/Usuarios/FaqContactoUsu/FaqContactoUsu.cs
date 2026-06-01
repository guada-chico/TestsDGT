using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using TestsDGT.Paginas.Usuarios.FaqContactoUsu;

namespace TestsDGT.Tests.Usuarios.FaqContactoUsu;

[TestFixture]
public class FaqContactoUsuTest : BaseTest
{
    private FaqContactoUsuPage _faqContactoUsuPage;

    [SetUp]
    public void SetupPagina()
    {
        _faqContactoUsuPage = new FaqContactoUsuPage(Page);
    }

    [Test]

    public async Task BuscarPregunta()
    {
        await _faqContactoUsuPage.IrAFaqContacto();

        string buscarTexto = "incidencia";
        await _faqContactoUsuPage.BuscarPreguntaAsync(buscarTexto);

        int preguntasVisibles = await _faqContactoUsuPage.ObtenerNumeroPreguntasVisiblesAsync();
        Assert.That(preguntasVisibles, Is.GreaterThan(0));
    }

    [Test]

    public async Task BuscarPregunta_SinRespuesta()
    {
        await _faqContactoUsuPage.IrAFaqContacto();

        string terminoInexistente = "Pregunta Inventada";
        await _faqContactoUsuPage.BuscarPreguntaAsync(terminoInexistente);

        int preguntasTrasFiltroVacio = await _faqContactoUsuPage.ObtenerNumeroPreguntasVisiblesAsync();
        Assert.That(preguntasTrasFiltroVacio, Is.EqualTo(0));
    }

    [Test]

    public async Task DesplegarPregunta()
    {
        await _faqContactoUsuPage.IrAFaqContacto();

        string textoPregunta = "¿Cómo puedo abrir una incidencia?";
        await _faqContactoUsuPage.DesplegarPreguntaPorTextoAsync(textoPregunta);

        string respuesta = await _faqContactoUsuPage.ObtenerRespuestaDePreguntaAsync(textoPregunta);
        Assert.That(respuesta, Does.Contain("Para abrir una incidencia, dirígete al apartado “Soporte” o “Centro de ayuda” del portal. Completa el formulario indicado."));
    }

    [Test]
    public async Task MandarContacto()
    {
        await _faqContactoUsuPage.IrAFaqContacto();

        string nombre = "Fernando Fernández";
        string email = "fernando.fernandez@example.com";
        string asunto = "Consulta sobre incidencia";
        string mensaje = "Tengo una pregunta sobre una incidencia que he reportado.";
        await _faqContactoUsuPage.MandarContactoAsync(nombre, email, asunto, mensaje);

        var mensajeToast = await _faqContactoUsuPage.ObtenerMensajeToastAsync();
        Assert.That(mensajeToast, Does.Contain("Tu mensaje se ha enviado correctamente."));   
    }

    [Test]
    public async Task MandarContacto_FaltanDatos()
    {
        await _faqContactoUsuPage.IrAFaqContacto();

        string nombre = "";
        string email = "fernando.fernandez@example.com";
        string asunto = "Consulta sobre incidencia";
        string mensaje = "Tengo una pregunta sobre una incidencia que he reportado.";
        await _faqContactoUsuPage.MandarContactoAsync(nombre, email, asunto, mensaje);

        var mensajeToastError = await _faqContactoUsuPage.ObtenerMensajeToastAsync();
        Assert.That(mensajeToastError, Does.Contain("Rellena nombre, correo electronico, asunto y mensaje."));
    }
}