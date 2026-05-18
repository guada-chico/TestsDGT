using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.PedidosExtraordinarioUsu;

public class PedidoExtraordinarioPage
{
    private readonly IPage _page;

    private ILocator InputArticulo => _page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccione un artículo" });
    private ILocator InputTalla => _page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccione una talla" });
    private ILocator InputCantidad => _page.GetByRole(AriaRole.Spinbutton, new() { Name = "Cantidad de artículos*" });
    private ILocator InputArchivoJustificante => _page.Locator("#archivoUrgencia");
    private ILocator InputArchivoPliego => _page.Locator("#archivo");
    private ILocator CheckConfirmarTalla => _page.GetByRole(AriaRole.Checkbox, new() { Name = "He confirmado mi talla*" });
    private ILocator InputMotivo => _page.GetByRole(AriaRole.Textbox, new() { Name = "Motivo del pedido*" });
    private ILocator RadioUrgente => _page.GetByRole(AriaRole.Radio, new() { Name = "Urgente" });
    private ILocator ComboMotivoUrgencia => _page.GetByRole(AriaRole.Combobox, new() { Name = "Seleccionar motivo" });
    private ILocator BotonRealizar => _page.GetByRole(AriaRole.Button, new() { Name = "Realizar" });
    private ILocator ToastMensaje => _page.Locator(".p-toast");

    public PedidoExtraordinarioPage(IPage page)
    {
        _page = page;
    }

    public async Task RealizarPedidoExtraordinarioAsync(string articulo, string talla, string cantidad, string motivo, bool urgente = false)
    {
        // 1. Selección de Artículo (Solo si no viene vacío en el test de error)
        if (!string.IsNullOrEmpty(articulo))
        {
            await InputArticulo.ClickAsync();
            await _page.GetByRole(AriaRole.Option, new() { Name = articulo }).ClickAsync();
        }

        // 2. Selección de Talla
        if (!string.IsNullOrEmpty(talla))
        {
            await InputTalla.ClickAsync();
            await _page.GetByRole(AriaRole.Option, new() { Name = talla }).ClickAsync();
        }

        // 3. Si es urgente, activamos los campos extra de urgencia que tenía el HTML
        if (urgente)
        {
            await RadioUrgente.CheckAsync();
            await ComboMotivoUrgencia.ClickAsync();
            // Seleccionamos la primera opción por defecto del motivo de urgencia
            await _page.GetByRole(AriaRole.Option, new() { Name = "Deterioro prematuro de las" }).ClickAsync();

            // Subimos el justificante de urgencia obligatorio (#archivoUrgencia)
            await InputArchivoJustificante.SetInputFilesAsync(@"C:\repos\TestsDGT\6073873.png");
        }

        // 4. Rellenar cantidad y el archivo principal del pedido (#archivo)
        await InputCantidad.FillAsync(cantidad);
        await InputArchivoPliego.SetInputFilesAsync(@"C:\repos\TestsDGT\6073873.png");

        // 5. Confirmación y motivo final
        await CheckConfirmarTalla.CheckAsync();
        await InputMotivo.FillAsync(motivo);

        // 6. Enviar
        await BotonRealizar.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<string> ObtenerMensajeToastAsync()
    {
        await ToastMensaje.WaitForAsync();
        return await ToastMensaje.InnerTextAsync();
    }

    public async Task IrAPedidoExtraordinarioAsync()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/pedido-extraordinario");
        await _page.WaitForURLAsync("**/pedido-extraordinario");
    }
}