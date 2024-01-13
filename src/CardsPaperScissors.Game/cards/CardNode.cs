using System;
using System.Collections.Generic;
using Godot;

namespace CardsPaperScissors.Game.Cards;

public partial class CardNode : Area2D
{
	private static IReadOnlyDictionary<ECardOrigin, Color> OriginColors => new Dictionary<ECardOrigin, Color>()
	{
		{ ECardOrigin.Random, Color.FromString("#9BABB8", Colors.White) },
		{ ECardOrigin.Player, Color.FromString("#EADEA6", Colors.White) },
		{ ECardOrigin.Opponent, Color.FromString("#756AB6", Colors.White) }
	};
	
	private static IReadOnlyDictionary<ECardValue, string> ValueTextureResource => new Dictionary<ECardValue, string>()
	{
		{ ECardValue.Rock, "res://cards/template/rock.png" },
		{ ECardValue.Paper, "res://cards/template/paper.png" },
		{ ECardValue.Scissors, "res://cards/template/scissors.png" }
	};
	
	public Card? Card { get; set; }
	public Sprite2D? FrameSprite { get; set; }
	public Sprite2D? ValueContainerSprite { get; set; }
	public Sprite2D? ValueSprite { get; set; }
	[Export]
	public bool IsHidden { get; set; }
	private bool ShouldHide => IsHidden || Card == null;
	
	public Action<CardNode>? OnPlay { get; set;  } 

	public override void _Ready()
	{
		FrameSprite = GetNode<Sprite2D>("Frame");
		ValueContainerSprite = GetNode<Sprite2D>("ValueContainer");
		ValueSprite = GetNode<Sprite2D>("Value");
		RefreshState();
	}

	public void ShowValue()
	{
		IsHidden = false;
		RefreshState();
	}

	public void RefreshState()
	{
		string texturePath;
		if (ShouldHide)
		{
			ValueContainerSprite?.Hide();
			ValueSprite?.Hide();
			texturePath = "res://cards/template/card_back.png";
		}
		else
		{
			ValueContainerSprite?.Show();
			ValueSprite?.Show();
			texturePath = "res://cards/template/card_frame.png";
			if(ValueSprite != null && Card != null)
				ValueSprite.Texture = GD.Load<Texture2D>(ValueTextureResource[Card.Value]);
		}
		if (FrameSprite != null)
		{
			FrameSprite.Texture = GD.Load<Texture2D>(texturePath);
			FrameSprite.Modulate = OriginColors[Card?.Origin ?? ECardOrigin.Random];
		}
	}

	public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.IsReleased())
		{
			OnPlay?.Invoke(this);
		}
	}
}
