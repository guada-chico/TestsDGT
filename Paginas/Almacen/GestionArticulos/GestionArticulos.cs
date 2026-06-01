using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestsDGT.Paginas.Suministro.Conformidad;
using TestsDGT.Paginas.Suministro.OrdenProduccion;
using TestsDGT.Paginas.Suministro.Proveedores;

namespace TestsDGT.Almacen.GestionArticulos;

[TestFixture]
public class GestionArticulosTest : BaseTest
{
    private GestionArticulosPage _gestionArticulosPage;

    [SetUp]
    public void SetupPagina()
    {
        _gestionArticulosPage = new GestionArticulosPage(Page);
    }

    
}