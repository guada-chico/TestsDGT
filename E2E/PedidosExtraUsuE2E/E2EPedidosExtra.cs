using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestsDGT.Paginas.Suministro.Conformidad;
using TestsDGT.Paginas.Suministro.OrdenProduccion;
using TestsDGT.Paginas.Suministro.Proveedores;

namespace TestsDGT.E2E.PedidosUsuE2E;

[TestFixture]
public class E2EPedidosExtraTest : BaseTest
{
    private E2EPedidosExtraPage _e2ePedidosExtraPage;

    [SetUp]
    public void SetupPagina()
    {
        _e2ePedidosExtraPage = new E2EPedidosExtraPage(Page);
    }

    [Test]
    public async Task NuevoProveedor_E2E()
    {
        await _e2ePedidosExtraPage.IrAProveedoresSum();

        await _e2ePedidosExtraPage.CrearNuevoProveedorAsync(
            nombre: "Proveedor Prueba Test",
            razonSocial: "S.L.",
            cifNif: "A1234567B",
            web: "www.proveedorprueba.com",
            direccion: "Calle de la Prueba, 123",
            telefono: "912345678",
            contacto: "Lope López",
            correoElectronico: "proveedor.prueba@example.com",
            estado: "Activo",
            observaciones: "Observaciones prueba"
        );

        await _e2ePedidosExtraPage.GuadarNuevoProveedorAsync();

        var mensaje = await _e2ePedidosExtraPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Proveedor guardado"));
    }

    [Test]
    public async Task NuevaOrdenProduccion_PorCodigo_E2E()
    {
        await _e2ePedidosExtraPage.IrAOrdenProduccion();

        await _e2ePedidosExtraPage.IrACrearOrdenProduccionAsync();

        await _e2ePedidosExtraPage.DatosNuevaOrdenProduccionAsync(
            expediente: "891",
            lote: "Lote Prueba 2",
            duracionContrato: 12,
            observaciones: "Observaciones prueba",
            proveedor: "RopitaChulita"
        );

        await _e2ePedidosExtraPage.ArticuloNuevaOrdenProduccionAsync(
            valorPrenda: "777",
            talla: "M",
            cantidad: 1
        );

        await _e2ePedidosExtraPage.GuardarOrdenAsync();

        var mensaje = await _e2ePedidosExtraPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Orden guardada correctamente. El envio de correo no bloquea la creacion."));
    }

    [Test]
    public async Task NuevaVerificacionConformidadSum()
    {
        await _e2ePedidosExtraPage.IrAConformidadSuministro();

        string expedienteConformidadSum = "96";
        await _e2ePedidosExtraPage.VerDetalleConformidadSumAsync(expedienteConformidadSum);

        await _e2ePedidosExtraPage.NuevaVerificacionSumAsync(
            "15/09/2024",
            "Verificación de prueba",
            @"C:\repos\TestsDGT\DGT3.\DGT3.xlsx",
            "1"
        );


    }
}