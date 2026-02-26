using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace LanceMudCapstone.Services;

public class LogoutService
{
    private readonly DialogService _dialog;
    private readonly SessionRepository _sessionRepo;
    private readonly SessionState _sessionState;
    private readonly NavigationManager _nav;
    private readonly IJSRuntime _js;

    public LogoutService(
        DialogService dialogService,
        SessionRepository sessionRepository,
        SessionState sessionState,
        NavigationManager navigationManager,
        IJSRuntime jSRuntime)
    {
        _dialog = dialogService;
        _sessionRepo = sessionRepository;
        _sessionState = sessionState;
        _js = jSRuntime;
        _nav = navigationManager;
    }

    public async Task RequestLogoutAsync()
    {
        var confirmed = await _dialog.Confirm(
            GetMessage(),
            "Logout?",
            new ConfirmOptions()
            {
                OkButtonText = "OK",
                CancelButtonText = "Cancel"
            });
        if (confirmed == true)
        {
            await PerformLogoutAsync();
        }
    }
    private async Task PerformLogoutAsync()
    {
        var cookieValue = await _js.InvokeAsync<string>("cookieHelper.readCookie", "session");
        if (!string.IsNullOrEmpty(cookieValue))
        {
            _sessionRepo.DeleteSession(cookieValue);
        }

        await _js.InvokeVoidAsync("cookieHelper.eraseCookie", "session");
        await _sessionState.ClearCharacterAsync();

        _nav.NavigateTo("/login", forceLoad: true);
    }
    private RenderFragment GetMessage() => builder =>
    {
        builder.AddContent(0, "Are you sure?");
        builder.OpenComponent<RadzenImage>(1);
        builder.AddAttribute(2, "Path", "/images/classes/default.svg");
        builder.AddAttribute(3, "Style", "width: 1rem; height: 1rem;");
        builder.CloseComponent();
    };
}
