using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TestsDGT.Paginas.Usuarios.FaqContactoUsu;

public class FaqContactoUsuPage
{
    private readonly IPage _page;
    
    private ILocator InputBuscarPregunta => _page.GetByPlaceholder("Buscar en las preguntas frecuentes...");
    private ILocator BloquesPreguntas => _page.Locator("p-accordiontab:visible, .p-accordion-header:visible");
    private ILocator InputNombreContacto => _page.Locator("input[name='nombre']");
    private ILocator InputEmailContacto => _page.Locator("input[name='correoElectronico']");
    private ILocator InputAsuntoContacto => _page.Locator("input[name='asunto']");
    private ILocator InputMensajeContacto => _page.Locator("textarea[name='mensaje']");
    private ILocator BotonEnviarContacto => _page.GetByRole(AriaRole.Button, new() { Name = "Enviar" });
    private ILocator ToastMensaje => _page.Locator(".p-toast");

    public FaqContactoUsuPage(IPage page)
    {
        _page = page;
    }

    public async Task BuscarPreguntaAsync(string texto)
    {
        await InputBuscarPregunta.ClearAsync();
        await InputBuscarPregunta.FillAsync(texto);

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await _page.WaitForTimeoutAsync(500);
    }

    public async Task DesplegarPreguntaPorTextoAsync(string textoPregunta)
    {
        var cabeceraPregunta = _page.Locator("p-accordiontab").GetByText(textoPregunta.Trim());
        await cabeceraPregunta.ClickAsync();
        await _page.WaitForTimeoutAsync(500);
    }

    public async Task<string> ObtenerRespuestaDePreguntaAsync(string textoPregunta)
    {
        var contenedorRespuesta = _page.Locator(".faq-content:visible");

        await contenedorRespuesta.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

        return await contenedorRespuesta.InnerTextAsync();
    }

    public async Task MandarContactoAsync(string nombre, string email, string asunto, string mensaje)
    {
        await InputNombreContacto.FillAsync(nombre);
        await InputEmailContacto.FillAsync(email);
        await InputAsuntoContacto.FillAsync(asunto);
        await InputMensajeContacto.FillAsync(mensaje);

        await BotonEnviarContacto.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<string> ObtenerMensajeToastAsync()
    {
        await ToastMensaje.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
        return await ToastMensaje.InnerTextAsync();
    }

    public async Task<int> ObtenerNumeroPreguntasVisiblesAsync()
    {
        return await BloquesPreguntas.CountAsync();
    }

    public async Task IrAFaqContacto()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/consultas-contacto");
        await _page.WaitForURLAsync("**/consultas-contacto");
    }
}