using CrossyRoadApi.Models.Game.Map;

namespace CrossyRoadApi.Models.Database;

public class CrossyGame
{
    public Guid Id { get; set; }
    public Guid HostId { get; set; }
    public int Seed { get; set; }
    public int Score { get; set; }
    public int Taler { get; set; }
    public CrossyTheme Theme { get; set; }
    // TODO: Create Dictionary<int, char> to string converter => "msSinceStart:char;msSinceLast:char;msSinceLast:char;..."
    public string Keystrokes { get; set; }
}