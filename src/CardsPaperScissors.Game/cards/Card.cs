using Godot;

namespace CardsPaperScissors.Game.Cards;

public class Card
{
	public Card()
	{
	}

	public Card(ECardValue value, ECardOrigin origin)
	{
		Value = value;
		Origin = origin;
	}
	
	public ECardValue Value { get; set; }
	public ECardOrigin Origin { get; set; }
}
