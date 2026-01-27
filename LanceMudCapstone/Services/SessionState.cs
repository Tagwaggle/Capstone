namespace LanceMudCapstone.Services;

public class SessionState
{
    public int? UserId { get; private set; }
    public string? UserName { get; private set; }
    public bool IsLoggedIn => UserId.HasValue;

    public event Action? OnChange;

    public void SetUser(int userId, string username)
    {
        UserId = userId; UserName = username; NotifyStateChange();
    }
    public void Logout()
    {
        UserId = null; UserName = null; NotifyStateChange();
    }

    private void NotifyStateChange() => OnChange?.Invoke();
}
