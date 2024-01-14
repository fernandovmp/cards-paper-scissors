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
        int childCount = _pointsContainer.GetChildCount();
        for (int i = 0; i < childCount; i++)
        {
            _pointsTextures.Add(_pointsContainer.GetChild<TextureRect>(i));
        }
    }

    public void Initialize(string name, bool flip)
    {
        _name.Text = name;
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
            GD.Print(index);
            _pointsTextures[index].Texture = ResourceLoader.Load<Texture2D>(Constants.PointTexture);
        }
    }
}