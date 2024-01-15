using CardsPaperScissors.Game.Utils;
using FernandoVmp.GodotUtils.Scene;
using Godot;

namespace CardsPaperScissors.Game.scenes.Menu;

public partial class MenuScene : CanvasLayer
{
    private Button _startButton = default!;

    public override void _Ready()
    {
        _startButton = GetNode<Button>("Start");
        var root = GetNode(Constants.RootNode);
        _startButton.Pressed += () => SceneLoader.LoadInto(root, Constants.PreMatchScene);
    }
}