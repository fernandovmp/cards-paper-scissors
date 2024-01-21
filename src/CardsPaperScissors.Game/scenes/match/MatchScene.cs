using System.Threading.Tasks;
using CardsPaperScissors.Game.Cards;
using CardsPaperScissors.Game.Scenes.Match.Ia;
using CardsPaperScissors.Game.settings;
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

	private MoveServiceNode _moveService = default!;
	
	private const float CardAnimationSpeed = 400;
	private bool _canPlay = true;
	private Label _resultText = default!;
	private MatchSettings _matchSettings = MatchSettings.Default();
	private Board _board = default!;
	private PlayerContext _playerContext = default!;
	private PlayerContext _opponentContext = default!;
	private GuessingPlayerAI _opponentAi = default!;
	private AudioStreamPlayer _cardSfx = default!;

	public override void _Ready()
	{
		var cache = new MemoryCacheService();
		var deck = cache.GetValueOrDefault<Deck>("deck") ?? new Deck();
		
		_moveService = new MoveServiceNode();
		AddChild(_moveService);
		
		_resultText = GetNode<Label>("UI/Result");
		_cardSfx = GetNode<AudioStreamPlayer>("CardSfxPlayer");
		
		_playerContext = PlayerContext.CreateFrom("Player", this);
		_opponentContext = PlayerContext.CreateFrom("Opponent", this);
		
		_opponentAi = new GuessingPlayerAI(deck, ECardOrigin.Opponent, _opponentContext.Hand, _playerContext.Hand);
		_opponentContext.Ai = _opponentAi;
		_opponentAi.Initialize();
		
		_board = new Board(this);
		_board.Initialize(new BoardContext(deck, _matchSettings, _moveService, OnPlay, _cardModel, _playerContext, _opponentContext));
		_opponentAi.UpdateWithHand();
		
		_playerContext.Info.Initialize("YOU", flip: true, _matchSettings.MatchPoint);
		_opponentContext.Info.Initialize("Opponent", flip: false, _matchSettings.MatchPoint);
		_resultText.Hide();
	}

	private async void OnPlay(PlayContext playerPlayContext)
	{
		if (!_canPlay) return;
		_canPlay = false;
		var opponentPlay = _board.Opponent.MakePlay();
		opponentPlay.Card.ShowValue();
		_cardSfx.Play();
		await Task.WhenAll(
			MoveCardAsync(playerPlayContext),
			MoveCardAsync(opponentPlay)
		);
		_opponentAi.UpdateWith(playerPlayContext.Card.Card!);
		await this.WaitForSeconds(0.5);
		await _board.EvaluateWinnerAsync(playerPlayContext, opponentPlay);
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

	public async Task MoveCardAsync(PlayContext context) =>
		await _moveService.MoveToAsync(context.Card, context.Owner.Field.GlobalPosition, CardAnimationSpeed); 

	private bool ValidateWinner()
	{
		bool emptyHand = _board.Player.Hand.IsEmpty();
		string result;
		if (_playerContext.Info.Points >= _matchSettings.MatchPoint || (emptyHand && _playerContext.Info.Points > _opponentContext.Info.Points))
		{
			result = "You won!";
		}
		else if (_opponentContext.Info.Points >= _matchSettings.MatchPoint || (emptyHand && _opponentContext.Info.Points > _playerContext.Info.Points))
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
