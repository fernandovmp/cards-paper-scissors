using System;
using System.Collections.Generic;
using CardsPaperScissors.Game.Utils;
using Godot;

namespace CardsPaperScissors.Game.Cards;

public partial class HandNode : Node2D
{
    private List<CardNode> _cards = new List<CardNode>(0);
    [Export]
    public bool IsHidden { get; set; }
    public Action<CardNode>? OnPlay { get; set; }

    public void SetCards(List<Card> cards, PackedScene cardModel)
    {
        int marginX = 0;
        _cards = new List<CardNode>(cards.Count);
        foreach (var card in cards)
        {
            var cardNode = cardModel!.Instantiate<CardNode>();
            cardNode.Card = card;
            cardNode.Position = new Vector2(marginX, 0);
            cardNode.IsHidden = IsHidden;
            if (OnPlay != null)
            {
                cardNode.OnPlay = OnPlay;
            }
            cardNode._Ready();
            AddChild(cardNode);
            _cards.Add(cardNode);
            marginX += Constants.CardGap;
        }
    }

    public void Remove(CardNode card)
    {
        _cards.Remove(card);
        RemoveChild(card);
        card.QueueFree();
    }

    public CardNode GetRandomCard()
    {
        int indice = new Random().Next(_cards.Count);
        return _cards[indice];
    }

    public bool HasCard() => !IsEmpty();
    public bool IsEmpty() => _cards.Count == 0;
}