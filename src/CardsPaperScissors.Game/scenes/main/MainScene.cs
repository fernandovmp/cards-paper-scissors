using CardsPaperScissors.Game.Utils;
using FernandoVmp.GodotUtils.Scene;
using Godot;

namespace CardsPaperScissors.Game.scenes.main;

public partial class MainScene : Node2D
{
    private Node _root = default!;

    public override void _Ready()
    {
        _root = GetNode(Constants.RootNode);
        SceneLoader.LoadInto(_root, Constants.MenuScene);
    }
}