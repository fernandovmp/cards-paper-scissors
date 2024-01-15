using System.Collections.Generic;
using CardsPaperScissors.Game.Utils;
using Godot;

namespace CardsPaperScissors.Game.ui.matchInfo;

public partial class MatchInfoControl : Control
{
    private Label _name = default!;
    private TextureRect _line = default!;
    private HBoxContainer _pointsContainer = default!;
    private List<TextureRect> _pointsTextures = new List<TextureRect>();
    public int Points { get; private set; }

    public override void _Ready()
    {
        _name = GetNode<Label>("Name");
        _line = GetNode<TextureRect>("Line");
        _pointsContainer = GetNode<HBoxContainer>("Points");
    }

    public void Initialize(string name, bool flip, int matchPoint)
    {
        _name.Text = name;
        var model = GD.Load<PackedScene>(Constants.PointModel);
        for (int i = 0; i < matchPoint; i++)
        {
            var instance = model.Instantiate<TextureRect>();
            _pointsContainer.AddChild(instance);
            _pointsTextures.Add(instance);
        }
        if (flip)
        {
            _line.FlipH = true;
            _pointsContainer.Alignment = BoxContainer.AlignmentMode.End;
        }
    }

    public void MakePoint()
    {
        Points++;
        if (Points > 0 && Points <= _pointsTextures.Count)
        {
            int index = _line.FlipH ? _pointsTextures.Count - Points : Points - 1;
            _pointsTextures[index].Texture = ResourceLoader.Load<Texture2D>(Constants.PointTexture);
        }
    }
}