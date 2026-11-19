using BrainRush.Models.Entities;

namespace BrainRush.Services;

/// <summary>
/// Keeps the current player ("you") in memory so the side menu and the pages share one copy.
/// There is no login yet, so "you" is the players row where is_you = true.
/// </summary>
public class PlayerSession(Supabase.Client db)
{
    public Player? Me { get; private set; }

    /// <summary>Raised after Me is reloaded, so the side menu can redraw its streak.</summary>
    public event Action? Changed;

    /// <summary>Loads "you" once. Later calls reuse the cached row.</summary>
    public async Task<Player?> GetMeAsync()
    {
        if (Me is null) await RefreshAsync();
        return Me;
    }

    /// <summary>Reloads "you" from the database (one GET) and tells listeners.</summary>
    public async Task RefreshAsync()
    {
        Me = await db.From<Player>()
            .Where(p => p.IsYou == true)
            .Single();
        Changed?.Invoke();
    }

    /// <summary>Time left until the daily board resets at midnight.</summary>
    public static string UntilReset()
    {
        var t = DateTime.Today.AddDays(1) - DateTime.Now;
        return $"{(int)t.TotalHours}h {t.Minutes:00}m";
    }
}
