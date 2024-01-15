using System;
using System.Collections.Generic;
using CardsPaperScissors.Game.Cards;
using CardsPaperScissors.Game.Scenes.Match;
using CardsPaperScissors.Game.settings;
using Godot;

namespace CardsPaperScissors.Game.Scenes.PreMatch;

public partial class DeckNode : Node2D
{
    private int _lastMarginX = 0;
    [Export]
    private PackedScene _cardModel = default!;
    public Deck Deck { get; set; } = new Deck();

    public override void _Ready()
    {
        var settings = MatchSettings.Default();
        Deck.AddRange(GetRandomCards(settings.RandomCards));
        Deck.Reveal(settings.RevealedCards);
        Deck.AddRange(GetRandomCards(settings.CardsToAdd, origin: ECardOrigin.Opponent));
        RenderCards();
    }
    
    private IEnumerable<Card> GetRandomCards(int amount, ECardOrigin origin = ECardOrigin.Random)
    {
        var random = new Random();
        for (int i = 0; i < amount; i++)
        {
            yield return new Card
            {
                Origin = origin,
                Value = (ECardValue) random.Next(1, 4)
            };
        }
    }

    public void RenderCards()
    {
        foreach (var card in Deck.Cards)
        {
            RenderCard(card);
        }
    }
    
    public void Add(Card card)
    {
        Deck.Add(card);
        RenderCard(card);
    }

    private void RenderCard(Card card)
    {
        var cardNode = _cardModel.Instantiate<CardNode>();
        cardNode.Card = card;
        cardNode.Position = new Vector2(_lastMarginX, 0);
        cardNode.IsHidden = !Deck.IsRevealed(card);
        cardNode._Ready();
        AddChild(cardNode);
        _lastMarginX += 100;
    }
}