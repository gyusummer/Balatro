using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class TarotCard : ConsumableCard
{
	public int MaxTarget = 1;

	public override bool CheckCondition(int selectedCount)
	{
		Debug.Log("this is Tarot CheckCondition");
		if (selectedCount != 0 && selectedCount <= MaxTarget)
		{
			return true;
		}
		return false;
	}

	protected void TransformCardSuit(List<Card> cards, CardSuit suit)
	{
		foreach (Card card in cards)
		{
			card.Suit = suit;
		}
		
		HandController.Instance.DeselectAllCards();
	}
	
	protected void EnhanceCard(List<Card> cards, CardEnhancement enhance)
	{
		foreach (Card card in cards)
		{
			card.Enhancement = enhance;
		}
		
		HandController.Instance.DeselectAllCards();
	}
}

// UNDONE: Complete function / After : Money, Joker, Rearrange, Inventory

// The Fool	Creates the last Tarot or Planet card used during this run
// The Fool excluded
public class Fool : TarotCard
{
	private ConsumableCard lastCard => ConsumableSystem.Instance.LastConsumableCard;
	public override bool CheckCondition(int selectedCount)
	{
		if (lastCard is not Fool and not null)
		{
			return true;
		}
		
		Debug.Log("last card is Tarot or null");
		return false;
	}

	protected override void Effect(List<Card> selectedCards)
	{
		// Add LastCard to player inventory
		ConsumableSystem.Instance.Print(lastCard);
	}
}

// The Magician	Enhances 2 selected cards to Lucky Cards
public class Magician : TarotCard
{
	public Magician()
	{
		MaxTarget = 2;
	}

	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Lucky);
	}
}

// The High Priestess	Creates up to 2 random Planet cards
// (Must have room)
public class HighPriestess : TarotCard
{
	protected override void Effect(List<Card> selectedCards)
	{
	}
}

// The Empress	Enhances 2 selected cards to Mult Cards
public class Empress : TarotCard
{
	public Empress()
	{
		MaxTarget = 2;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Mult);
	}
}

// The Emperor	Creates up to 2 random Tarot cards
// (Must have room)
public class Emperor : TarotCard
{
	protected override void Effect(List<Card> selectedCards)
	{
	}
}

// The Hierophant	Enhances 2 selected cards to Bonus Cards
public class Hierophant : TarotCard
{
	public Hierophant()
	{
		MaxTarget = 2;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Bonus);
	}
}

// The Lovers	Enhances 1 selected card into a Wild Card
public class Lovers : TarotCard
{
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Wild);
	}
}

// The Chariot	Enhances 1 selected card into a Steel Card
public class Chariot : TarotCard
{
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Steel);
	}
}

// Justice	Enhances 1 selected card into a Glass Card
public class Justice : TarotCard
{
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Glass);
	}
}

// The Hermit	Doubles money
// (Max of $20)
public class Hermit : TarotCard
{
	protected override void Effect(List<Card> selectedCards)
	{
	}
}

// The Wheel of Fortune	1 in 4 chance to add Foil, Holographic, or Polychrome edition to a random Joker
public class WheelOfFortune : TarotCard
{
	protected override void Effect(List<Card> selectedCards)
	{
	}
}

// Strength	Increases rank of up to 2 selected cards by 1
public class Strength : TarotCard
{
	public Strength()
	{
		MaxTarget = 2;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		foreach (Card card in selectedCards)
		{
			if (card.Rank == CardRank.Ace)
			{
				card.Rank = CardRank.Two;
			}
			else
			{
				card.Rank += 1;
			}
		}
		HandController.Instance.DeselectAllCards();
	}
}

// The Hanged Man	Destroys up to 2 selected cards
public class HangedMan : TarotCard
{
	public HangedMan()
	{
		MaxTarget = 2;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		foreach (Card card in selectedCards)
		{
			card.Destroy();
		}
	}
}

// Death	Select 2 cards, convert the left card into the right card
// (Drag to rearrange)
public class Death : TarotCard
{
	protected override void Effect(List<Card> selectedCards)
	{
	}
}

// Temperance	Gives the total sell value of all current Jokers
// (Max of $50)
public class Temperance : TarotCard
{
	protected override void Effect(List<Card> selectedCards)
	{
	}
}
// The Devil	Enhances 1 selected card into a Gold Card
public class Devil : TarotCard
{
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Gold);
	}
}
// The Tower	Enhances 1 selected card into a Stone Card
public class Tower : TarotCard
{
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Stone);
	}
}

// The Star	Converts up to 3 selected cards to  Diamonds
public class Star : TarotCard
{
	public Star()
	{
		MaxTarget = 3;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		TransformCardSuit(selectedCards, CardSuit.Diamond);
	}
}

// The Moon	Converts up to 3 selected cards to  Clubs
public class Moon : TarotCard
{
	public Moon()
	{
		MaxTarget = 3;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		TransformCardSuit(selectedCards, CardSuit.Club);
	}
}

// The Sun	Converts up to 3 selected cards to  Hearts
public class Sun : TarotCard
{
	public Sun()
	{
		MaxTarget = 3;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		TransformCardSuit(selectedCards, CardSuit.Heart);
	}
}

// Judgement	Creates a random Joker card
// 	(Must have room)
public class Judgement : TarotCard
{
	protected override void Effect(List<Card> selectedCards)
	{
	}
}

// The World	Converts up to 3 selected cards to  Spades
public class World : TarotCard
{
	public World()
	{
		MaxTarget = 3;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		TransformCardSuit(selectedCards, CardSuit.Spade);
	}
}