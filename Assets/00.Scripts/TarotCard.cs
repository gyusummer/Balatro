using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	protected void TransformCardSuit(List<PlayingCard> cards, CardSuit suit)
	{
		foreach (PlayingCard card in cards)
		{
			card.Suit = suit;
		}
	}
	
	protected void EnhanceCard(List<PlayingCard> cards, CardEnhancement enhance)
	{
		foreach (PlayingCard card in cards)
		{
			card.Enhancement = enhance;
		}
	}
}

#region Tarots
// UNDONE: Complete function

// The Fool	Creates the last Tarot or Planet card used during this run
// The Fool excluded
public class Fool : TarotCard
{
	public override bool CheckCondition(int selectedCount)
	{
		ConsumableCard last = ConsumableSystem.Instance.LastConsumableCard;
		if (last is not Fool)
		{
			return true;
		}
		return false;
	}

	protected override void Effect(List<PlayingCard> selectedCards)
	{
		// Add LastCard to player inventory
	}
}

// The Magician	Enhances 2 selected cards to Lucky Cards
public class Magician : TarotCard
{
	public Magician()
	{
		MaxTarget = 2;
	}

	protected override void Effect(List<PlayingCard> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Lucky);
	}
}

// The High Priestess	Creates up to 2 random Planet cards
// (Must have room)
public class HighPriestess : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
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
	protected override void Effect(List<PlayingCard> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Mult);
	}
}

// The Emperor	Creates up to 2 random Tarot cards
// (Must have room)
public class Emperor : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
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
	protected override void Effect(List<PlayingCard> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Bonus);
	}
}

// The Lovers	Enhances 1 selected card into a Wild Card
public class Lovers : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Wild);
	}
}

// The Chariot	Enhances 1 selected card into a Steel Card
public class Chariot : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Steel);
	}
}

// Justice	Enhances 1 selected card into a Glass Card
public class Justice : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Glass);
	}
}

// The Hermit	Doubles money
// (Max of $20)
public class Hermit : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
	{
	}
}

// The Wheel of Fortune	1 in 4 chance to add Foil, Holographic, or Polychrome edition to a random Joker
public class WheelOfFortune : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
	{
	}
}

// Strength	Increases rank of up to 2 selected cards by 1
public class Strength : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
	{
		foreach (PlayingCard card in selectedCards)
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
	}
}

// The Hanged Man	Destroys up to 2 selected cards
public class HangedMan : TarotCard
{
	public HangedMan()
	{
		MaxTarget = 2;
	}
	protected override void Effect(List<PlayingCard> selectedCards)
	{
		foreach (PlayingCard card in selectedCards)
		{
			card.Destroy();
		}
	}
}

// Death	Select 2 cards, convert the left card into the right card
// (Drag to rearrange)
public class Death : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
	{
	}
}

// Temperance	Gives the total sell value of all current Jokers
// (Max of $50)
public class Temperance : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
	{
	}
}
// The Devil	Enhances 1 selected card into a Gold Card
public class Devil : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
	{
		EnhanceCard(selectedCards, CardEnhancement.Gold);
	}
}
// The Tower	Enhances 1 selected card into a Stone Card
public class Tower : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
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
	protected override void Effect(List<PlayingCard> selectedCards)
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
	protected override void Effect(List<PlayingCard> selectedCards)
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
	protected override void Effect(List<PlayingCard> selectedCards)
	{
		TransformCardSuit(selectedCards, CardSuit.Heart);
	}
}

// Judgement	Creates a random Joker card
// 	(Must have room)
public class Judgement : TarotCard
{
	protected override void Effect(List<PlayingCard> selectedCards)
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
	protected override void Effect(List<PlayingCard> selectedCards)
	{
		TransformCardSuit(selectedCards, CardSuit.Spade);
	}
}
#endregion

public static class TarotFactory
{
	// 딕셔너리: 카드 ID 또는 Enum을 키로, 해당 카드의 타입을 값으로 저장합니다.
	// 하지만 타입을 저장하는 대신, 바로 생성하는 델리게이트를 저장하는 것이 더 유연합니다.
	private static readonly Dictionary<int, Func<TarotCard>> CardConstructors = new Dictionary<int, Func<TarotCard>>()
	{
		// 모든 22개 카드의 생성자를 람다 표현식으로 등록
		{ 0, () => new Fool() },
		{ 1, () => new Magician() },
		{ 2, () => new HighPriestess() },
		{ 3, () => new Empress() },
		{ 4, () => new Emperor() },
		{ 5, () => new Hierophant() },
		{ 6, () => new Lovers() },
		{ 7, () => new Chariot() },
		{ 8, () => new Strength() },
		{ 9, () => new Hermit() },
		{ 10, () => new WheelOfFortune() },
		{ 11, () => new Justice() },
		{ 12, () => new HangedMan() },
		{ 13, () => new Death() },
		{ 14, () => new Temperance() },
		{ 15, () => new Devil() },
		{ 16, () => new Tower() },
		{ 17, () => new Star() },
		{ 18, () => new Moon() },
		{ 19, () => new Sun() },
		{ 20, () => new Judgement() },
		{ 21, () => new World() }
	};

	private static readonly System.Random rng = new System.Random();
	
	/// <summary>
	/// 랜덤한 타로카드 객체를 생성하여 반환합니다.
	/// </summary>
	public static TarotCard CreateRandomCard()
	{
		// 1. 딕셔너리의 키(ID) 목록을 가져옵니다.
		List<int> availableKeys = new List<int>(CardConstructors.Keys);
        
		// 2. 랜덤한 키(카드 ID)를 선택합니다.
		int randomIndex = rng.Next(availableKeys.Count);
		int randomCardId = availableKeys[randomIndex];

		// 3. 해당 키에 등록된 생성자(Func<Tarot>)를 호출하여 객체를 생성합니다.
		return CardConstructors[randomCardId].Invoke();
	}
}