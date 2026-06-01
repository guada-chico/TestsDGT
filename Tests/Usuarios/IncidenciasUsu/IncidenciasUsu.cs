using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using TestsDGT.Paginas.Usuarios.IncidenciasUsu;

namespace TestsDGT.Tests.Usuarios.IncidenciasUsu;

[TestFixture]
public class IncidenciasUsuTest : BaseTest
{
    private IncidenciasUsuPage _incidenciasPage;

    [SetUp]
    public void SetupPagina()
    {
        _incidenciasPage = new IncidenciasUsuPage(Page);
    }

    [Test]

    public async Task NuevaIncidencia()
    {
        await _incidenciasPage.IrAIncidencias();

        await _incidenciasPage.CrearNuevaIncidenciaAsync(
            titulo: "Incidencia Prueba",
            prioridad: "2",
            motivo: "Motivo incidencia",
            idPedido: "123",
            rutaArchivo: @"C:\repos\TestsDGT\6073873.png",
            descripcion: "descripción incidencia"
        );

        await _incidenciasPage.VerificarIncidenciaCreadaAsync("Incidencia Prueba");
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

    public async Task FiltroIncidenciaTitulo()
    {
        await _incidenciasPage.IrAIncidencias();

        string tituloIncidencia = "Incidencia Prueba";
        await _incidenciasPage.FiltrarPorTituloIncidenciaAsync(tituloIncidencia);

        Assert.That(await _incidenciasPage.VerificarIncidenciaCreadaAsync(tituloIncidencia), Is.True);
        Assert.That(await _incidenciasPage.ObtenerNumeroFilasIncidenciasAsync(), Is.GreaterThan(0));
    }

    [Test]

    public async Task FiltroIncidenciaFecha()
    {
        await _incidenciasPage.IrAIncidencias();

        string fechaIncidencia = "18/05/2026";
        await _incidenciasPage.FiltrarPorFechaIncidenciaAsync(fechaIncidencia);

        Assert.That(await _incidenciasPage.ObtenerNumeroFilasIncidenciasAsync(), Is.GreaterThan(0));
    }

    [Test]

    public async Task FiltroIncidenciaEstado()
    {
        await _incidenciasPage.IrAIncidencias();

        string estado = "En curso";
        await _incidenciasPage.FiltrarPorEstadoIncidenciaAsync(estado);

        Assert.That(await _incidenciasPage.ObtenerNumeroFilasIncidenciasAsync(), Is.GreaterThan(0));
        Assert.That(await _incidenciasPage.VerificarIncidenciaCreadaAsync(estado), Is.True);
    }

    [Test]
    public async Task LimpiarFiltrosIncidencias()
    {
        await _incidenciasPage.IrAIncidencias();

        int totalFilasOriginales = await _incidenciasPage.ObtenerNumeroFilasIncidenciasAsync();

        string titulo = "Incidencia Prueba";
        await _incidenciasPage.FiltrarPorTituloIncidenciaAsync(titulo);

        int filasFiltradas = await _incidenciasPage.ObtenerNumeroFilasIncidenciasAsync();

        await _incidenciasPage.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _incidenciasPage.ObtenerNumeroFilasIncidenciasAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
            "La tabla no recuperó todos los registros originales tras limpiar los filtros.");

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas),
            "El filtro de prueba no redujo el tamaño de la tabla, usa un artículo diferente.");
    }
}