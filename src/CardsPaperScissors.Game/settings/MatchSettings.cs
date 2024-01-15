namespace CardsPaperScissors.Game.settings;

public class MatchSettings
{
    public int RandomCards { get; set; }
    public int CardsToAdd { get; set; }
    public int RevealedCards { get; set; }
    public int HandSize { get; set; }

    public static MatchSettings Default() => new MatchSettings
    {
        RandomCards = 4,
        RevealedCards = 2,
        CardsToAdd = 3,
        HandSize = 3
    };

    public int MatchPoint => HandSize / 2 + 1;
}