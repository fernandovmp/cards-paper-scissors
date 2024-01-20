using System.Threading.Tasks;
using CardsPaperScissors.Game.Cards;
using FernandoVmp.GodotUtils.Extensions;
using FernandoVmp.GodotUtils.Nodes;
using Godot;

namespace CardsPaperScissors.Game.Scenes.Match;

public class Board
{
    private readonly Node2D _node;
    private MoveServiceNode _moveService = default!;
    public PlayerContext Player { get; set; } = default!;
    public PlayerContext Opponent { get; set; } = default!;
    public Deck Deck { get; set; } = default!;

    public Board(Node2D node)
    {
        _node = node;
    }

    public void Initialize(BoardContext context)
    {
        _moveService = context.MoveService;

        Player = context.Player;
        Opponent = context.Opponent;
        Deck = context.Deck;
        
        var matchSettings = context.MatchSettings;
        var cardModel = context.CardModel;
        
        Deck.Shuffle();
        
        Player.OnPlay += context.OnPlay;
        Player.Hand.SetCards(Deck.Draw(matchSettings.HandSize), cardModel);		
        Opponent.Hand.SetCards(Deck.Draw(matchSettings.HandSize), cardModel);
    }
    
    public async Task EvaluateWinnerAsync(PlayContext player, PlayContext opponent)
    {
        var value1 = player.Card.Card!.Value;
        var value2 = opponent.Card.Card!.Value;

        var winnervalue = Board.EvaluateWinner(value1, value2);
        if (winnervalue == null)
        {
            await _node.WaitForSeconds(1);
            DestoryCard(player);
            DestoryCard(opponent);
        }
        else if (winnervalue == value1)
        {
            await AnimateCardWinAsync(player, opponent, -1);
        }
        else
        {
            await AnimateCardWinAsync(opponent, player, 1);
        }
    }
    
    private async Task AnimateCardWinAsync(PlayContext winnerContext, PlayContext loserContext, int direction)
    {
        Vector2 offset = new Vector2(100, 0);
        Vector2 dest = winnerContext.Card.GlobalPosition + (offset * direction);
        await _moveService.MoveToAsync(winnerContext.Card, dest, 300);
        dest = winnerContext.Card.GlobalPosition - (offset * direction);
        await _moveService.MoveToAsync(winnerContext.Card, dest, 300);
        DestoryCard(loserContext);
        winnerContext.Owner.Info.MakePoint();
        await _node.WaitForSeconds(1);
        DestoryCard(winnerContext);
    }

    private static void DestoryCard(PlayContext context) => context.Owner.Hand.Remove(context.Card);
    
    public static ECardValue? EvaluateWinner(ECardValue value1, ECardValue value2)
    {
        if (value1 == Card.GetWinningValue(value2))
            return value1;
        if (value1 == value2)
            return null;
        return value2;
    }
}