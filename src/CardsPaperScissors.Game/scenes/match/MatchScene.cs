using System.Threading.Tasks;
using CardsPaperScissors.Game.Cards;
using CardsPaperScissors.Game.settings;
using CardsPaperScissors.Game.ui.matchInfo;
using CardsPaperScissors.Game.Utils;
using FernandoVmp.GodotUtils.Extensions;
using FernandoVmp.GodotUtils.Nodes;
using FernandoVmp.GodotUtils.Scene;
using FernandoVmp.GodotUtils.Services;
using Godot;

namespace CardsPaperScissors.Game.Scenes.Match;

public partial class MatchScene : Node2D
{
	[Export]
	private PackedScene _cardModel = default!;

	private Deck _deck = new Deck();
	private MoveServiceNode _moveService = default!;
	
	private const float CardAnimationSpeed = 300;
	private bool _canPlay = true;
	private MatchInfoControl _playerInfo = default!;
	private MatchInfoControl _opponentInfo = default!;
	private Label _resultText = default!;
	private MatchSettings _matchSettings = MatchSettings.Default();
	private Board _board = default!;

	public override void _Ready()
	{
		var cache = new MemoryCacheService();
		_deck = cache.GetValueOrDefault<Deck>("deck") ?? new Deck();
		_deck.Shuffle();
		
		_moveService = new MoveServiceNode();
		AddChild(_moveService);
		
		_playerInfo = GetNode<MatchInfoControl>("UI/PlayerInfo");
		_opponentInfo = GetNode<MatchInfoControl>("UI/OpponentInfo");
		_resultText = GetNode<Label>("UI/Result");

		_board = new Board(this);
		_board.Initialize(new BoardContext(_deck, _matchSettings, _moveService, OnPlay, _cardModel, _playerInfo, _opponentInfo));
		
		_playerInfo.Initialize("YOU", flip: true, _matchSettings.MatchPoint);
		_opponentInfo.Initialize("Opponent", flip: false, _matchSettings.MatchPoint);
		_resultText.Hide();
	}

	private async void OnPlay(CardNode obj)
	{
		if (!_canPlay) return;
		_canPlay = false;
		var opponentCard = _board.OpponentHandNode.GetRandomCard();
		opponentCard.ShowValue();
		await Task.WhenAll(
			_moveService.MoveToAsync(obj, _board.PlayerField.GlobalPosition, CardAnimationSpeed),
			_moveService.MoveToAsync(opponentCard, _board.OpponentField.GlobalPosition, CardAnimationSpeed)
		);
		await this.WaitForSeconds(0.5);
		await _board.EvaluateWinnerAsync(obj, opponentCard);
		bool hasWinner = ValidateWinner();
		if (hasWinner)
		{
			await this.WaitForSeconds(3);
			var root = GetNode(Constants.RootNode);
			SceneLoader.LoadInto(root, Constants.PreMatchScene);
		}
		else
		{
			_canPlay = true;
		}
	}

	private bool ValidateWinner()
	{
		bool emptyHand = _board.PlayerHandNode.IsEmpty();
		string result;
		if (_playerInfo.Points >= _matchSettings.MatchPoint || (emptyHand && _playerInfo.Points > _opponentInfo.Points))
		{
			result = "You won!";
		}
		else if (_opponentInfo.Points >= _matchSettings.MatchPoint || (emptyHand && _opponentInfo.Points > _playerInfo.Points))
		{
			result = "You lost!";
		}
		else if(emptyHand)
		{
			result = "Draw!";
		}
		else
		{
			return false;
		}

		_resultText.Text = result;
		_resultText.Show();
		return true;
	}
}
