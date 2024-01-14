using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsPaperScissors.Game.Cards;
using FernandoVmp.GodotUtils.Nodes;
using FernandoVmp.GodotUtils.Services;
using Godot;

namespace CardsPaperScissors.Game.Scenes.Match;

public partial class MatchScene : Node2D
{
	private List<ECardValue> _cards = new List<ECardValue>(3)
	{
		ECardValue.Rock,
		ECardValue.Paper,
		ECardValue.Scissors
	};

	private List<Card> PlayerCards => _cards.Select(x => new Card()
	{
		Origin = ECardOrigin.Player,
		Value = x
	}).ToList();
	
	private List<Card> OpponentCards => _cards.Select(x => new Card()
	{
		Origin = ECardOrigin.Opponent,
		Value = x
	}).ToList();
	
	private HandNode? PlayerHandNode { get; set; }
	private HandNode? OpponentHandNode { get; set; }
	[Export]
	private PackedScene? _cardModel;

	private Deck _deck = new Deck();
	private Node2D _playerField;
	private Node2D _opponentField;
	private MoveServiceNode _moveService = default!;
	
	private const float CardAnimationSpeed = 300;

	public override void _Ready()
	{
		_moveService = new MoveServiceNode();
		AddChild(_moveService);
		PlayerHandNode = GetNode<HandNode>("PlayerHand");
		OpponentHandNode = GetNode<HandNode>("OpponentHand");
		_playerField = GetNode<Node2D>("PlayerField");
		_opponentField = GetNode<Node2D>("OpponentField");

		var cache = new MemoryCacheService();
		_deck = cache.GetValueOrDefault<Deck>("deck") ?? new Deck();
		_deck.Shuffle();

		PlayerHandNode.OnPlay += OnPlay;
		PlayerHandNode.SetCards(_deck.Draw(3), _cardModel);		
		OpponentHandNode.SetCards(_deck.Draw(3), _cardModel);
	}

	private async void OnPlay(CardNode obj)
	{
		var opponentCard = OpponentHandNode.GetRandomCard();
		opponentCard.ShowValue();
		await Task.WhenAll(
			_moveService.MoveToAsync(obj, _playerField.GlobalPosition, CardAnimationSpeed),
			_moveService.MoveToAsync(opponentCard, _opponentField.GlobalPosition, CardAnimationSpeed)
		);
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		EvaluateWinner(obj, opponentCard);
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		PlayerHandNode.Remove(obj);
		OpponentHandNode.Remove(opponentCard);
	}

	private void EvaluateWinner(CardNode playerCard, CardNode opponentCard)
	{
		var value1 = playerCard.Card!.Value;
		var value2 = opponentCard!.Card!.Value;

		var winnervalue = EvaluateWinner(value1, value2);
		string resultText;
		if (winnervalue == null)
		{
			resultText = "Draw!";
		}
		else if (winnervalue == value1)
		{
			resultText = "Win!";
		}
		else
		{
			resultText = "Lose!";
		}
		GD.Print(resultText);
	}

	private static ECardValue? EvaluateWinner(ECardValue value1, ECardValue value2)
	{
		if (value1 == value2)
		{
			return null;
		}
		else if (value1 == ECardValue.Rock && value2 == ECardValue.Paper)
		{
			return value2;
		}
		else if (value1 == ECardValue.Rock && value2 == ECardValue.Scissors)
		{
			return value1;
		}
		else if (value1 == ECardValue.Paper && value2 == ECardValue.Rock)
		{
			return value1;
		}
		else if (value1 == ECardValue.Paper && value2 == ECardValue.Scissors)
		{
			return value2;
		}
		else if (value1 == ECardValue.Scissors && value2 == ECardValue.Rock)
		{
			return value2;
		}
		else if (value1 == ECardValue.Scissors && value2 == ECardValue.Paper)
		{
			return value1;
		}

		return null;
	}
}
