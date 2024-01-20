using System.Collections.Generic;
using System.Linq;
using CardsPaperScissors.Game.Cards;

namespace CardsPaperScissors.Game.Scenes.Match.Ia;

public class GuessingPlayerAI : IPlayerAI
{
    private readonly Deck _deck;
    private readonly ECardOrigin _knowOrigin;
    private readonly HandNode _guessingHand;
    private readonly HandNode _hand;
    private Dictionary<ECardOrigin, OriginKnowledge> _knowledge = new Dictionary<ECardOrigin, OriginKnowledge>();

    private class OriginKnowledge
    {
        public LinkedList<Card> KnowCards { get; } = new LinkedList<Card>();
        public int Total { get; set; }

        public List<(ECardValue, double)> GetChance() =>
            KnowCards
                .GroupBy(chance => chance.Value)
                .Select(chance => (chance.Key, chance.Count() / (double)Total))
                .ToList();
    }

    public GuessingPlayerAI(Deck deck, ECardOrigin knowOrigin, HandNode hand, HandNode guessingHand)
    {
        _deck = deck;
        _knowOrigin = knowOrigin;
        _guessingHand = guessingHand;
        _hand = hand;
    }
    
    public CardNode MakePlay()
    {
        var chances = new List<(ECardValue, double)>();
        foreach (var card in _guessingHand.Cards)
        {
            chances.AddRange(_knowledge[card.Origin].GetChance());
        }

        var finalChances = chances
            .GroupBy(chance => chance.Item1)
            .Select(chance => (chance.Key, chance.Sum(y => y.Item2) / chances.Count))
            .OrderByDescending(chance => chance.Item2)
            .ToList();
        
        if(finalChances.Count == 0)
            return _hand.GetRandomCard();
        return _hand.GetCardAgainst(finalChances[0].Item1);
    }

    public void Initialize()
    {
        foreach (var card in _deck.Cards)
        {
            if (!_knowledge.ContainsKey(card.Origin))
            {
                _knowledge.Add(card.Origin, new OriginKnowledge());
            }
            if (_deck.IsRevealed(card) || card.Origin == _knowOrigin)
            {
                _knowledge[card.Origin].KnowCards.AddLast(card);
            }
            _knowledge[card.Origin].Total++;
        }
    }

    public void UpdateWithHand()
    {
        foreach (var card in _hand.Cards)
        {
            RemoveCard(card);
        }
    }

    public void UpdateWith(Card card) => RemoveCard(card);
    
    private void RemoveCard(Card card)
    {
        var knowledge = _knowledge[card.Origin];
        var item = knowledge.KnowCards.FirstOrDefault(x => x.Origin == card.Origin && x.Value == card.Value);
        if (item != null)
        {
            knowledge.KnowCards.Remove(item);
            knowledge.Total--;
        }
    }

}