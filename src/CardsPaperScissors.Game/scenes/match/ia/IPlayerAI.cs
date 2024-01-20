using CardsPaperScissors.Game.Cards;

namespace CardsPaperScissors.Game.Scenes.Match.Ia;

public interface IPlayerAI
{
    CardNode MakePlay();
}