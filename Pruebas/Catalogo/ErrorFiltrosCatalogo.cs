using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Pruebas.Catalogo;

public class ErrorFiltrosCatalogoTest : BaseTest
{
    [Test]

    public async Task ErrorFiltrosCatalogo_SinResultados()
    {
        string busquedaErronea = "ProductoInexistente";

        await Page.GetByPlaceholder("Buscar por nombre").FillAsync(busquedaErronea);

        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-filter") }).ClickAsync();

        var mensajeVacio = Page.Locator("td").Filter(new() { HasText = "No hay resultados para tu búsqueda" });
        await Assertions.Expect(mensajeVacio).ToBeVisibleAsync();

        await Page.Locator("button.btn-clear").ClickAsync();
    }
}