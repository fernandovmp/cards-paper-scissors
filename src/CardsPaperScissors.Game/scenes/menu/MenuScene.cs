using CardsPaperScissors.Game.Utils;
using FernandoVmp.GodotUtils.Scene;
using Godot;

namespace CardsPaperScissors.Game.scenes.Menu;

public partial class MenuScene : CanvasLayer
{
    private Button _startButton = default!;
    private Button _creditsButton = default!;
    private Control _titleRoot = default!;
    private Control _creditsRoot = default!;

    public override void _Ready()
    {
        _startButton = GetNode<Button>("Title/Start");
        _creditsButton = GetNode<Button>("Title/Credits");
        _titleRoot = GetNode<Control>("Title");
        _creditsRoot = GetNode<Control>("Credits");
        
        var root = GetNode(Constants.RootNode);
        _startButton.Pressed += () => SceneLoader.LoadInto(root, Constants.PreMatchScene);
        _creditsButton.Pressed += () => ShowUi(title: false);
        
        using var file = FileAccess.Open("res://credits.txt", FileAccess.ModeFlags.Read);
        var credits = file.GetAsText();
        var label = _creditsRoot.GetNode<Label>("Label");
        label.Text = credits;

        _creditsRoot.GetNode<Button>("Back").Pressed += () => ShowUi(title: true);

        ShowUi(title: true);
    }

    private void ShowUi(bool title)
    {
        _creditsRoot.Visible = !title;
        _titleRoot.Visible = title;
    }
}