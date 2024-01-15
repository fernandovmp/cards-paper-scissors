using System.Collections.Generic;
using CardsPaperScissors.Game.Cards;
using CardsPaperScissors.Game.settings;
using CardsPaperScissors.Game.Utils;
using FernandoVmp.GodotUtils.Scene;
using FernandoVmp.GodotUtils.Services;
using Godot;

namespace CardsPaperScissors.Game.Scenes.PreMatch;

public partial class PreMatchScene : Node2D
{
	private DeckNode _deckNode = default!;
	private HandNode _selectionHand = default!;
	
	[Export]
	private PackedScene _cardModel = default!;

	private bool _canAdd = true;
	private int _addedCards = 0;
	private MatchSettings _matchSettings = MatchSettings.Default();

	private static List<Card> _selectableCards = new List<Card>
	{
		new Card(ECardValue.Rock, ECardOrigin.Player),
		new Card(ECardValue.Paper, ECardOrigin.Player),
		new Card(ECardValue.Scissors, ECardOrigin.Player)
	};
	
	public override void _Ready()
	{
		_deckNode = GetNode<DeckNode>("DeckNode");
		_selectionHand = GetNode<HandNode>("SelectionHand");
		_selectionHand.OnPlay += OnSelect;
		_selectionHand.SetCards(_selectableCards, _cardModel);
	}

	private async void OnSelect(CardNode card)
	{
		if (!_canAdd) return;
		_canAdd = false;
		_deckNode.Add(card.Card!);
		_addedCards++;
		if (_addedCards == _matchSettings.CardsToAdd)
		{
			await ToSignal(GetTree().CreateTimer(1), "timeout");
			StartMatch(_deckNode.Deck);
		}
		else
		{
			_canAdd = true;
		}
	}

	private void StartMatch(Deck deck)
	{
		var cache = new MemoryCacheService();
		cache.AddOrReplace(Constants.DeckKey, deck);
		var root = GetNode(Constants.RootNode);
		SceneLoader.LoadInto(root, Constants.MatchScene);
	}
}
