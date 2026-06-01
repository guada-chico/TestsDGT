using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestsDGT.E2E.PedidosUsuE2E;

[TestFixture]
public class E2EPedidosExtraTest : BaseTest
{
    private E2EPedidosExtraPage e2ePedidosExtraPage;

    [SetUp]
    public void SetupPagina()
    {
        e2ePedidosExtraPage = new E2EPedidosExtraPage(Page);
    }
    
    
}