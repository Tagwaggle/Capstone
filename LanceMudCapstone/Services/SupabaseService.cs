using Supabase;

public class SupabaseService
{
    private readonly IConfiguration _config;
    private Client? _client;

    public SupabaseService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> TestConnectionAsync()
    {
        var url = _config["Supabase:Url"];
        var key = _config["Supabase:Key"];

        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
            return false;

        _client = new Client(url, key);

        try
        {
            // This will throw if the URL or key is invalid
            await _client.InitializeAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
