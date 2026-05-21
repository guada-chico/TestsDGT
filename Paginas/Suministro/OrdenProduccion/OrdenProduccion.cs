using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Suministro.OrdenProduccion;

[TestFixture]
public class OrdenProduccionTest : BaseTest
{
    private OrdenProduccionPage _ordenProduccionPage;

    [SetUp]
    public void SetupPagina()
    {
        _ordenProduccionPage = new OrdenProduccionPage(Page);
    }

    [Test]
    public async Task NuevaOrdenProduccion_PorCodigo()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        bool guardadoExitoso = await _ordenProduccionPage.CrearNuevaOrdenProduccionAsync(
            expediente: "4561",
            lote: "Lote Prueba",
            duracionContrato: 12,
            observaciones: "Observaciones prueba",
            proveedor: "RopaMuyGuay",
            valorPrenda: "777",
            talla: "M",
            cantidad: 5,
            esPorCodigo: true
        );

        Assert.That(guardadoExitoso, Is.True,
            "El servidor de la DGT rechazó la petición de creación de la orden de producción por Código.");
    }

    [Test]
    public async Task NuevaOrdenProduccion_PorArticulo()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        bool guardadoExitoso = await _ordenProduccionPage.CrearNuevaOrdenProduccionAsync(
            expediente: "2365",
            lote: "Lote Prueba Articulo",
            duracionContrato: 12,
            observaciones: "Observaciones prueba 2",
            proveedor: "RopaMuyGuay",
            valorPrenda: "JERSEY VERDE DE CUELLO CISNE",
            talla: "L",
            cantidad: 5,
            esPorCodigo: false
        );

        Assert.That(guardadoExitoso, Is.True,
            "El servidor de la DGT rechazó la petición de creación de la orden de producción por Artículo.");
    }

    [Test]
    public async Task AgregarArticulosOD()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        string expedientePrueba = "7890";

        await _ordenProduccionPage.CrearNuevaOrdenProduccionAsync(
            expediente: expedientePrueba,
            lote: "Lote Prueba Agregar Articulos",
            duracionContrato: 12,
            observaciones: "Observaciones prueba agregar artículos",
            proveedor: "RopaMuyGuay",
            valorPrenda: "777",
            talla: "M",
            cantidad: 5,
            esPorCodigo: true
        );

        await _ordenProduccionPage.AgregarArticuloODAsync(
            valorPrenda: "JERSEY VERDE DE CUELLO CISNE",
            talla: "L",
            cantidad: 3,
            esPorCodigo: false
        );

        bool guardadoExitoso = await _ordenProduccionPage.FinalizarYGuardarOrdenAsync();

        Assert.That(guardadoExitoso, Is.True,
            "El servidor de la DGT rechazó la creación de la orden con múltiples artículos.");

        await _ordenProduccionPage.IrAOrdenProduccion();
        await _ordenProduccionPage.FiltrarPorCodigoExpedienteODAsync(expedientePrueba);

        Assert.That(await _ordenProduccionPage.VerificarOrdenProduccionCreadaAsync(expedientePrueba), Is.True);
        Assert.That(await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync(), Is.GreaterThan(0));
    }

    [Test]
    public async Task FiltroCodigoOD()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        string codigo = "39";
        await _ordenProduccionPage.FiltrarPorCodigoExpedienteODAsync(codigo);

        Assert.That(await _ordenProduccionPage.VerificarOrdenProduccionCreadaAsync(codigo), Is.True);
        Assert.That(await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync(), Is.GreaterThan(0));
    }

    [Test]
    public async Task FiltroExpedienteOD()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        string expediente = "4561";
        await _ordenProduccionPage.FiltrarPorCodigoExpedienteODAsync(expediente);

        Assert.That(await _ordenProduccionPage.VerificarOrdenProduccionCreadaAsync(expediente), Is.True);
        Assert.That(await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync(), Is.GreaterThan(0));
    }

    [Test]
    public async Task FiltroProveedorOD()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        await _ordenProduccionPage.FiltrarPorProveedorODAsync("Proveedor Siete");

        Assert.That(await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync(), Is.GreaterThan(0));
        Assert.That(await _ordenProduccionPage.VerificarTextoEnTablaAsync("Proveedor Siete"), Is.True);
    }

    [Test]
    public async Task LimpiarFiltrosOrdenProduccion()
    {
        await _ordenProduccionPage.IrAOrdenProduccion();

        int totalFilasOriginales = await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync();

        string codigo = "39";
        await _ordenProduccionPage.FiltrarPorCodigoExpedienteODAsync(codigo);

        int filasFiltradas = await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync();

        await _ordenProduccionPage.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _ordenProduccionPage.ObtenerNumeroFilasOrdenesProduccionAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
            "La tabla no recuperó todos los registros originales tras limpiar los filtros.");

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas),
            "El filtro de prueba no redujo el tamaño de la tabla, usa un artículo diferente.");
    }
}