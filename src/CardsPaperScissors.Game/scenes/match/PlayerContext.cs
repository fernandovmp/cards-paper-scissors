using System;
using CardsPaperScissors.Game.Cards;
using CardsPaperScissors.Game.ui.matchInfo;
using Godot;

namespace CardsPaperScissors.Game.Scenes.Match;

public record PlayerContext(MatchInfoControl Info, HandNode Hand, Node2D Field)
{
    public Action<PlayContext>? OnPlay { get; set; }
    public static PlayerContext CreateFrom(string name, Node root)
    {
        var hand = root.GetNode<HandNode>($"{name}Hand");
        var field = root.GetNode<Node2D>($"{name}Field");
        var info = root.GetNode<MatchInfoControl>($"UI/{name}Info");
        var ctx = new PlayerContext(info, hand, field);
        hand.OnPlay += ctx.OnHandPlay;
        return ctx;
    }

    private void OnHandPlay(CardNode card)
    {
        OnPlay?.Invoke(new PlayContext(this, card));
    }

    public PlayContext GetRandomPlay() => new PlayContext(this, Hand.GetRandomCard());
};

public record PlayContext(PlayerContext Owner, CardNode Card);