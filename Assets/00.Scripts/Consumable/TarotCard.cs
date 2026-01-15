using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]
public abstract class TarotCard : ConsumableCard
{
	public int MaxTarget = 1;

	public override bool CheckCondition(int selectedCount)
	{
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
public class Fool : TarotCard
{
	private ConsumableCard lastCard => ConsumableSystem.Instance.LastConsumableCard;

	public override string Description =>
		"Creates the last Tarot or Planet card used during this run\nThe Fool excluded";

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
		Inventory.Instance.Consumables.Add(lastCard);
	}
}
public class Magician : TarotCard
{
	public Magician()
	{
		MaxTarget = 2;
	}

	public override string Description => "Enhances 2 selected cards to Lucky Cards";

	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Lucky);
	}
}
public class HighPriestess : TarotCard
{
	public override string Description => "Creates up to 2 random Planet cards\n(Must have room)";

	public override bool CheckCondition(int selectedCount)
	{
		return Inventory.Instance.Consumables.IsFull == false || this.IsPlayerOwned;
	}
	
	protected override void Effect(List<Card> selectedCards)
	{
		Inventory.Instance.Consumables.Add(PlanetFactory.Instance.CreateRandom(false));
		Inventory.Instance.Consumables.Add(PlanetFactory.Instance.CreateRandom(false));
	}
}
public class Empress : TarotCard
{
	public override string Description => "Enhances 2 selected cards to Mult Cards";
	public Empress()
	{
		MaxTarget = 2;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Mult);
	}
}
public class Emperor : TarotCard
{
	public override string Description => "Creates up to 2 random Tarot cards\n(Must have room)";
	public override bool CheckCondition(int selectedCount)
	{
		return Inventory.Instance.Consumables.IsFull == false || this.IsPlayerOwned;
	}

	protected override void Effect(List<Card> selectedCards)
	{
		Inventory.Instance.Consumables.Add(TarotFactory.Instance.CreateRandom(false));
		Inventory.Instance.Consumables.Add(TarotFactory.Instance.CreateRandom(false));
	}
}
public class Hierophant : TarotCard
{
	public override string Description => "Enhances 2 selected cards to Bonus Cards";
	public Hierophant()
	{
		MaxTarget = 2;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Bonus);
	}
}
public class Lovers : TarotCard
{
	public override string Description => "Enhances 1 selected card into a Wild Card";
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Wild);
	}
}
public class Chariot : TarotCard
{
	public override string Description => "Enhances 1 selected card into a Steel Card";
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Steel);
	}
}
public class Justice : TarotCard
{
	public override string Description => "Enhances 1 selected card into a Glass Card";
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Glass);
	}
}
public class Hermit : TarotCard
{
	public override string Description => "The Hermit Doubles money\n(Max of $20)";
	public override bool CheckCondition(int selectedCount)
	{
		return true;
	}

	protected override void Effect(List<Card> selectedCards)
	{
		int value = Inventory.Instance.PlayerMoney;
		if (value < 0)
		{
			value = 0;
		}
		else if (value > 20)
		{
			value = 20;
		}

		Inventory.Instance.PlayerMoney += value;
	}
}
public class WheelOfFortune : TarotCard
{
	public override string Description => "1 in 4 chance to add Foil, Holographic, or Polychrome edition to a random Joker";
	public override bool CheckCondition(int selectedCount)
	{
		return Inventory.Instance.Jokers.Count > 0;
	}

	protected override void Effect(List<Card> selectedCards)
	{
		Chance.Roll(4, () =>
			{
				Debug.Log("Yes! Fortune!");
				Joker j = Inventory.Instance.Jokers.CloneList().GetRandom();
				j.Edition = (Edition)UnityEngine.Random.Range(1, 4);
			}
		);
	}
}
public class Strength : TarotCard
{
	public override string Description => "Increases rank of up to 2 selected cards by 1";
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
public class HangedMan : TarotCard
{
	public override string Description => "Destroys up to 2 selected cards";
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
		HandController.Instance.DeselectAllCards();
	}
}
public class Death : TarotCard
{
	public override string Description => "Select 2 cards, convert the left card into the right card\n(Drag to rearrange)";
	public override bool CheckCondition(int selectedCount)
	{
		return selectedCount == 2;
	}

	protected override void Effect(List<Card> selectedCards)
	{
		selectedCards[1].CopyTo(selectedCards[0]);
		HandController.Instance.DeselectAllCards();
	}
}
public class Temperance : TarotCard
{
	public override string Description => "Gives the total sell value of all current Jokers\n(Max of $50)";
	public override bool CheckCondition(int selectedCount)
	{
		return true;
	}

	protected override void Effect(List<Card> selectedCards)
	{
		var jokers = Inventory.Instance.Jokers.CloneList();
		int value = 0;
		foreach (Joker joker in jokers)
		{
			value += joker.Price;
		}

		if (value > 50)
		{
			value = 50;
		}
		
		Inventory.Instance.PlayerMoney += value;
	}
}
public class Devil : TarotCard
{
	public override string Description => "Enhances 1 selected card into a Gold Card";
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Gold);
	}
}
public class Tower : TarotCard
{
	public override string Description => "Enhances 1 selected card into a Stone Card";
	protected override void Effect(List<Card> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Stone);
	}
}
public class Star : TarotCard
{
	public override string Description => "Converts up to 3 selected cards to Diamonds";
	public Star()
	{
		MaxTarget = 3;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		TransformCardSuit(selectedCards, CardSuit.Diamond);
	}
}
public class Moon : TarotCard
{
	public override string Description => "Converts up to 3 selected cards to Clubs";
	public Moon()
	{
		MaxTarget = 3;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		TransformCardSuit(selectedCards, CardSuit.Club);
	}
}
public class Sun : TarotCard
{
	public override string Description => "Converts up to 3 selected cards to Hearts";
	public Sun()
	{
		MaxTarget = 3;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		TransformCardSuit(selectedCards, CardSuit.Heart);
	}
}
public class Judgement : TarotCard
{
	public override string Description => "Creates a random Joker card\n(Must have room)";
	public override bool CheckCondition(int selectedCount)
	{
		return Inventory.Instance.Jokers.IsFull == false;
	}

	protected override void Effect(List<Card> selectedCards)
	{
		Inventory.Instance.Jokers.Add(JokerFactory.Instance.CreateRandom(false));
	}
}
public class World : TarotCard
{
	public override string Description => "Converts up to 3 selected cards to Spades";
	public World()
	{
		MaxTarget = 3;
	}
	protected override void Effect(List<Card> selectedCards)
	{
		TransformCardSuit(selectedCards, CardSuit.Spade);
	}
}
public class Aura : TarotCard
{
	public override string Description => "Add Foil, Holographic, or Polychrome effect to 1 selected card in hand";
	protected override void Effect(List<Card> selectedCards)
	{
		selectedCards[0].Edition = (Edition)UnityEngine.Random.Range(0, 4);
	}
}