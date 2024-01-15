using System.Threading.Tasks;
using CardsPaperScissors.Game.Cards;
using CardsPaperScissors.Game.ui.matchInfo;
using FernandoVmp.GodotUtils.Extensions;
using FernandoVmp.GodotUtils.Nodes;
using Godot;

namespace CardsPaperScissors.Game.Scenes.Match;

public class Board
{
    private readonly Node2D _node;
    private MoveServiceNode _moveService = default!;
    private BoardContext _context = default!;
    public HandNode PlayerHandNode { get; set; } = default!;
    public HandNode OpponentHandNode { get; set; } = default!;
    public Node2D PlayerField { get; set; } = default!;
    public Node2D OpponentField { get; set; } = default!;

    public Board(Node2D node)
    {
        _node = node;
    }

    public void Initialize(BoardContext context)
    {
        _moveService = context.MoveService;
        _context = context;
        PlayerHandNode = GetNode<HandNode>("PlayerHand");
        OpponentHandNode = GetNode<HandNode>("OpponentHand");
        PlayerField = GetNode<Node2D>("PlayerField");
        OpponentField = GetNode<Node2D>("OpponentField");

        PlayerHandNode.OnPlay += context.OnPlay;
        var deck = context.Deck;
        var matchSettings = context.MatchSettings;
        var cardModel = context.CardModel;
        PlayerHandNode.SetCards(deck.Draw(matchSettings.HandSize), cardModel);		
        OpponentHandNode.SetCards(deck.Draw(matchSettings.HandSize), cardModel);
    }

    private T GetNode<T>(string path) where T : Node => _node.GetNode<T>(path);
    
    public async Task EvaluateWinnerAsync(CardNode playerCard, CardNode opponentCard)
    {
        var value1 = playerCard.Card!.Value;
        var value2 = opponentCard!.Card!.Value;

        var winnervalue = Board.EvaluateWinner(value1, value2);
        if (winnervalue == null)
        {
            await _node.WaitForSeconds(1);
            PlayerHandNode.Remove(playerCard);
            OpponentHandNode.Remove(opponentCard);
        }
        else if (winnervalue == value1)
        {
            await AnimateCardWinAsync(playerCard, opponentCard, PlayerHandNode, OpponentHandNode, _context.PlayerInfo, -1);
        }
        else
        {
            await AnimateCardWinAsync(opponentCard, playerCard, OpponentHandNode, PlayerHandNode, _context.OpponentInfo, 1);
        }
    }
    
    private async Task AnimateCardWinAsync(CardNode winnerNode, CardNode loserNode, HandNode winnerHand, HandNode loserHand,
        MatchInfoControl winnerInfo,
        int direction)
    {
        Vector2 offset = new Vector2(100, 0);
        Vector2 dest = winnerNode.GlobalPosition + (offset * direction);
        await _moveService.MoveToAsync(winnerNode, dest, 300);
        dest = winnerNode.GlobalPosition - (offset * direction);
        await _moveService.MoveToAsync(winnerNode, dest, 300);
        loserHand.Remove(loserNode);
        winnerInfo.MakePoint();
        await _node.WaitForSeconds(1);
        winnerHand.Remove(winnerNode);
    }
    
    public static ECardValue? EvaluateWinner(ECardValue value1, ECardValue value2)
    {
        if (value1 == value2)
        {
            return null;
        }
        if (value1 == ECardValue.Rock && value2 == ECardValue.Paper)
        {
            return value2;
        }
        if (value1 == ECardValue.Rock && value2 == ECardValue.Scissors)
        {
            return value1;
        }
        if (value1 == ECardValue.Paper && value2 == ECardValue.Rock)
        {
            return value1;
        }
        if (value1 == ECardValue.Paper && value2 == ECardValue.Scissors)
        {
            return value2;
        }
        if (value1 == ECardValue.Scissors && value2 == ECardValue.Rock)
        {
            return value2;
        }
        if (value1 == ECardValue.Scissors && value2 == ECardValue.Paper)
        {
            return value1;
        }

        return null;
    }
}