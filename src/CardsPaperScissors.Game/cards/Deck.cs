using System;
using System.Collections.Generic;
using System.Linq;

namespace CardsPaperScissors.Game.Cards;

public class Deck
{
    private List<Card> _cards = new List<Card>();
    private HashSet<Card> _revealedCards = new HashSet<Card>();
    public IReadOnlyCollection<Card> Cards => _cards;
    public IReadOnlyCollection<Card> RevealedCards => _revealedCards;

    public void AddRange(IEnumerable<Card> cards) => _cards.AddRange(cards);
    
    public void Add(Card card) => _cards.Add(card);

    public void Reveal(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            _revealedCards.Add(_cards[_cards.Count - 1 - i]);
        }
    }

    public bool IsRevealed(Card card) => _revealedCards.Contains(card);

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