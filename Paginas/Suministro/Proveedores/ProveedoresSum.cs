using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace TestsDGT.Paginas.Suministro.Proveedores;

[TestFixture]
public class ProvedoresSumTest : BaseTest
{
    private ProveedoresSumPage _proveedoresSumPage;

    [SetUp]
    public void SetupPagina()
    {
        _proveedoresSumPage = new ProveedoresSumPage(Page);
    }

    [Test]
    public async Task NuevoProveedor()
    {
        await _proveedoresSumPage.IrAProveedoresSum();

        await _proveedoresSumPage.CrearNuevoProveedorAsync(
            nombre: "Proveedor de Prueba Test",
            razonSocial: "S.L.",
            cifNif: "A1234567B",
            web: "www.proveedorprueba.com",
            direccion: "Calle de la Prueba, 123",
            telefono: "912345678",
            contacto: "Domingo Dominguez",
            correoElectronico: "proveedor.prueba@example.com",
            estado: "Inactivo",
            observaciones: "Observaciones prueba"
        );

        await _proveedoresSumPage.GuadarNuevoProveedorAsync();

        var mensaje = await _proveedoresSumPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Proveedor guardado"));
    }

    [Test]
    public async Task CancelarProcesoNuevoProveedor()
    {
        await _proveedoresSumPage.IrAProveedoresSum();

        int filasIniciales = await _proveedoresSumPage.ObtenerNumeroFilasProveedoresAsync();

        await _proveedoresSumPage.CrearNuevoProveedorAsync(
            nombre: "Proveedor Cancelar Creación",
            razonSocial: "S.L.",
            cifNif: "A1234567B",
            web: "www.proveedorprueba.com",
            direccion: "Calle de la Prueba, 123",
            telefono: "912345678",
            contacto: "Domingo Dominguez",
            correoElectronico: "proveedor.prueba@example.com",
            estado: "Inactivo",
            observaciones: "Observaciones prueba"
        );

        await _proveedoresSumPage.CancelarCreacionProveedorAsync();

        string nombreProveedor = "Proveedor Cancelar Creación";

        bool existe = await _proveedoresSumPage.VerificarProveedorCreadaAsync(nombreProveedor);
        Assert.That(existe, Is.False, "El proveedor se ha guardado a pesar de haber pulsado cancelar.");

        int filasFinales = await _proveedoresSumPage.ObtenerNumeroFilasProveedoresAsync();
        Assert.That(filasFinales, Is.EqualTo(filasIniciales), "El número de filas ha cambiado, ¡se ha creado un proveedor por error!");
    }

    [Test]
    public async Task FiltroCodigoProveedor()
    {
        await _proveedoresSumPage.IrAProveedoresSum();

        string codigo = "305";
        await _proveedoresSumPage.FiltrarPorCodigoNombreProveedorAsync(codigo);

        Assert.That(await _proveedoresSumPage.VerificarProveedorCreadaAsync(codigo), Is.True);
        Assert.That(await _proveedoresSumPage.ObtenerNumeroFilasProveedoresAsync(), Is.GreaterThan(0));
    }

    [Test]
    public async Task FiltroNombreProveedor()
    {
        await _proveedoresSumPage.IrAProveedoresSum();

        string nombre = "RopaMuyGuay";
        await _proveedoresSumPage.FiltrarPorCodigoNombreProveedorAsync(nombre);

        Assert.That(await _proveedoresSumPage.VerificarProveedorCreadaAsync(nombre), Is.True);
        Assert.That(await _proveedoresSumPage.ObtenerNumeroFilasProveedoresAsync(), Is.GreaterThan(0));
    }

    [Test]
    public async Task LimpiarFiltrosProveedores()
    {
        await _proveedoresSumPage.IrAProveedoresSum();

        await _proveedoresSumPage.LimpiarFiltrosAsync();
        int totalFilasOriginales = await _proveedoresSumPage.ObtenerNumeroFilasProveedoresAsync();

        string codigo = "305";
        await _proveedoresSumPage.FiltrarPorCodigoNombreProveedorAsync(codigo);

        int filasFiltradas = await _proveedoresSumPage.ObtenerNumeroFilasProveedoresAsync();

        Assert.That(totalFilasOriginales, Is.GreaterThan(filasFiltradas), "El filtro no redujo las filas.");

        await _proveedoresSumPage.LimpiarFiltrosAsync();

        int filasTrasLimpiar = await _proveedoresSumPage.ObtenerNumeroFilasProveedoresAsync();

        Assert.That(filasTrasLimpiar, Is.EqualTo(totalFilasOriginales),
        $"Esperábamos {totalFilasOriginales} filas, pero se obtuvieron {filasTrasLimpiar}");
    }

    [Test]
    public async Task EditarFichaProveedor()
    {
        await _proveedoresSumPage.IrAProveedoresSum();

        string nombreOriginal = "Prueba Test";
        string nombreNuevo = "RopitaChulita";

        await _proveedoresSumPage.FiltrarPorCodigoNombreProveedorAsync(nombreOriginal);

        await _proveedoresSumPage.EditarProveedorAsync(nombreOriginal, nombreNuevo);

        await _proveedoresSumPage.GuardarCambiosProveedorsync();

        var mensaje = await _proveedoresSumPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Proveedor actualizado"));

        await _proveedoresSumPage.LimpiarFiltrosAsync();
        await _proveedoresSumPage.FiltrarPorCodigoNombreProveedorAsync(nombreNuevo);
        Assert.That(await _proveedoresSumPage.VerificarProveedorCreadaAsync(nombreNuevo), Is.True);
    }

    [Test]
    public async Task EditarFichaProveedor_CancelarEdicion()
    {
        await _proveedoresSumPage.IrAProveedoresSum();

        string nombreOriginal = "RopitaChulita";
        string nombreNoGuardado = "Proveedor no guarda";

        await _proveedoresSumPage.FiltrarPorCodigoNombreProveedorAsync(nombreOriginal);

        await _proveedoresSumPage.EditarProveedorAsync(nombreOriginal, nombreNoGuardado);

        await _proveedoresSumPage.CancelarEdicionProveedorAsync();

        bool existeNuevo = await _proveedoresSumPage.VerificarProveedorCreadaAsync(nombreNoGuardado);
        Assert.That(existeNuevo, Is.False, "El nombre nuevo no debería existir porque se canceló.");

        bool existeOriginal = await _proveedoresSumPage.VerificarProveedorCreadaAsync(nombreOriginal);
        Assert.That(existeOriginal, Is.True, "El nombre original debería persistir.");
    }

    [Test]
    public async Task EliminarProveedor()
    {
        await _proveedoresSumPage.IrAProveedoresSum();

        string nombreProveedor = "RopitaChulita";
        await _proveedoresSumPage.FiltrarPorCodigoNombreProveedorAsync(nombreProveedor);
        await _proveedoresSumPage.EliminarProveedorAsync(nombreProveedor);

        var mensaje = await _proveedoresSumPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("Proveedor eliminado correctamente"));

        await _proveedoresSumPage.LimpiarFiltrosAsync();
        await _proveedoresSumPage.FiltrarPorCodigoNombreProveedorAsync(nombreProveedor);
        Assert.That(await _proveedoresSumPage.VerificarProveedorCreadaAsync(nombreProveedor), Is.False);
    }

    [Test]
    public async Task EliminarProveedor_ErrorPorTenerOrdenesAsociadas()
    {
        await _proveedoresSumPage.IrAProveedoresSum();

        string nombreProveedor = "pepe";
        await _proveedoresSumPage.FiltrarPorCodigoNombreProveedorAsync(nombreProveedor);
        await _proveedoresSumPage.EliminarProveedorAsync(nombreProveedor);

        var mensaje = await _proveedoresSumPage.ObtenerMensajeToastAsync();
        Assert.That(mensaje, Does.Contain("No se puede eliminar el proveedor porque tiene órdenes de entrega asociadas."));
    }
}