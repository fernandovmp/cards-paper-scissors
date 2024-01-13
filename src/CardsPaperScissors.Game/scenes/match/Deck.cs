using System;
using System.Collections.Generic;
using System.Linq;
using CardsPaperScissors.Game.Cards;

namespace CardsPaperScissors.Game.Scenes.Match;

public class Deck
{
    private List<Card> _cards = new List<Card>();

    public void AddRange(IEnumerable<Card> cards) => _cards.AddRange(cards);
    
    public void Add(Card card) => _cards.Add(card);

    public void Shuffle()
    {
        var random = new Random();
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    public List<Card> Draw(int amount)
    {
        var cards = new List<Card>(amount);
        var index = _cards.Count - 1; 
        while (amount > 0)
        {
            cards.Add(_cards.ElementAt(index));
            _cards.RemoveAt(index);
            index--;
            amount--;
        }
        return cards;
    }
}