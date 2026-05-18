using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Usuarios.IncidenciasUsu;

[TestFixture]
public class IncidenciasUsuTest : BaseTest
{
    private IncidenciasPage _incidenciasPage;

    [SetUp]
    public void SetupPagina()
    {
        _incidenciasPage = new IncidenciasPage(Page);
    }

    [Test]

    public async Task NuevaIncidencia()
    {
        await _incidenciasPage.IrAIncidencias();

        await _incidenciasPage.CrearNuevaIncidenciaAsync(
            titulo: "Camiseta mal tallada",
            prioridad: "2",
            motivo: "Motivo incidencia",
            idPedido: "123",
            rutaArchivo: @"C:\repos\TestsDGT\6073873.png",
            descripcion: "descripción incidencia"
        );

        await _incidenciasPage.VerificarIncidenciaCreadaAsync("Camiseta mal tallada");
    }

    [Test]

    public async Task NuevaIncidencia_ErrorFaltanDatos()
    {
        await _incidenciasPage.IrAIncidencias();
        await _incidenciasPage.CrearNuevaIncidenciaAsync(
            titulo: "",
            prioridad: "2",
            motivo: "Motivo incidencia",
            idPedido: "123",
            rutaArchivo: @"C:\repos\TestsDGT\6073873.png",
            descripcion: "descripción incidencia"
        );

        var mensajeError = await _incidenciasPage.ObtenerMensajeToastAsync();
        Assert.That(mensajeError, Does.Contain("El título es obligatorio."));
    }

    [Test]

    public async Task FiltroIncidenciaNombreArticulo()
    {
        await _incidenciasPage.IrAIncidencias();

        await _incidenciasPage.VerificarIncidenciaCreadaAsync("Camiseta mal tallada");

        await Page.GetByPlaceholder("Nombre del artículo").FillAsync("DIVISA");
        await Page.Locator("button").Filter(new() { Has = Page.Locator(".pi-filter") }).ClickAsync();

        var celdaDivisa = Page.Locator("td").Filter(new() { HasText = "DIVISA" });

        try
        {
            await Assertions.Expect(celdaDivisa).ToBeVisibleAsync(new() { Timeout = 5000 });
        }
        catch (Exception)
        {
            Assert.Fail("El filtro no devolvió la celda con el nombre 'DIVISA' tras 5 segundos.");
        }
    }
}