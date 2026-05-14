using Microsoft.Playwright;

namespace TestsDGT.Paginas.Login;

public class LoginPage
{
    private readonly IPage _page;
    private ILocator InputUsuario => _page.GetByPlaceholder("Usuario (TIP)");
    private ILocator InputPassword => _page.Locator("input[type='password']");
    private ILocator BotonEntrar => _page.GetByRole(AriaRole.Button, new() { Name = "ENTRAR" });
    private ILocator ToastMensaje => _page.Locator(".custom-alert");
    private ILocator BotonCerrarSesionAnterior => _page.GetByRole(AriaRole.Button, new() { Name = " Cerrar sesion anterior " });
    public LoginPage(IPage page)
    {
        _page = page;
    }

    public async Task IrALoginAsync()
    {
        await _page.GotoAsync("http://192.168.200.51:7001/dgt-front/#/login");
    }
    public async Task RealizarLoginAsync(string usuario, string password)
    {
        await InputUsuario.FillAsync(usuario);
        await InputPassword.FillAsync(password);
        await BotonEntrar.ClickAsync();
        try
        {
            // Esperamos a que el botón sea visible
            await BotonCerrarSesionAnterior.WaitForAsync(new()
            {
                State = WaitForSelectorState.Visible,
                Timeout = 3000
            });

            // Si llegamos aquí, el botón existe. Lo pulsamos con "fuerza"
            await BotonCerrarSesionAnterior.ClickAsync(new() { Force = true });
        }
        catch (System.Exception)
        {
            // Si salta el timeout es que NO apareció el botón, 
            // así que el login probablemente fue directo. No hacemos nada.
        }

        // 2. IMPORTANTE: Esperamos a que la red se calme antes de terminar el método
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<string> ObtenerMensajeErrorAsync()
    {
        await ToastMensaje.WaitForAsync();
        return await ToastMensaje.InnerTextAsync();
    }
}