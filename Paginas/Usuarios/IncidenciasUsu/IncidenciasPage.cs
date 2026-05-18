using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.IncidenciasUsu;

public class IncidenciasPage
{
    private readonly IPage _page;
    
    private ILocator BotonNuevaIncidencia => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-plus") });
    private ILocator InputTituloIncidencia => _page.Locator("#titulo");
    private ILocator ComboPrioridad => _page.GetByRole(AriaRole.Combobox, new() { Name = "Media" });
    private ILocator InputMotivo => _page.Locator("#motivo");
    private ILocator InputIdPedido => _page.Locator("#idPedido");
    private ILocator InputArchivo => _page.Locator("input[type='file']");
    private ILocator InputDescripcion => _page.Locator("#descripcion");
    private ILocator BotonEnviarIncidencia => _page.Locator("button").Filter(new() { Has = _page.Locator(".pi-check") });
    private ILocator ToastMensaje => _page.Locator(".p-toast");
    private ILocator CeldaIncidencia => _page.Locator("td").GetByText("Camiseta mal tallada", new() { Exact = true }).First;

    public IncidenciasPage(IPage page)
    {
        _page = page;
    }

    public async Task CrearNuevaIncidenciaAsync(string titulo, string prioridad, string motivo, string idPedido, string rutaArchivo, string descripcion)
    {
        await BotonNuevaIncidencia.ClickAsync();
        await InputTituloIncidencia.FillAsync(titulo);
        await ComboPrioridad.ClickAsync();
        await _page.Locator($"#prioridad_{prioridad}").ClickAsync();
        await InputMotivo.FillAsync(motivo);
        await InputIdPedido.FillAsync(idPedido);
        await InputArchivo.SetInputFilesAsync(rutaArchivo);
        await InputDescripcion.FillAsync(descripcion);
        await BotonEnviarIncidencia.ClickAsync();
    }

    public async Task<string> ObtenerMensajeToastAsync()
    {
        try
        {
            await Assertions.Expect(ToastMensaje).ToBeVisibleAsync(new() { Timeout = 5000 });
            return await ToastMensaje.InnerTextAsync();
        }
        catch (Exception)
        {
            Assert.Fail("No se mostró ningún mensaje toast tras 5 segundos.");
            return string.Empty; 
        }
    }

    public async Task<bool> VerificarIncidenciaCreadaAsync(string titulo)
    {
        try
        {
            var celdaIncidencia = _page.Locator("td").GetByText(titulo, new() { Exact = true }).First;
            await Assertions.Expect(celdaIncidencia).ToBeVisibleAsync(new() { Timeout = 15000 });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task IrAIncidencias()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/mis-incidencias");
        await _page.WaitForURLAsync("**/mis-incidencias");
    }
}