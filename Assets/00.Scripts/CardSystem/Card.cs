using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Flags]
public enum CardSuit
{
	None = 0,
	Diamond = 1,
	Club = 2,
	Heart = 4,
	Spade = 8,
	Wild = Diamond | Club | Heart | Spade
}

// 2 ~ 14
public enum CardRank
{
	None = 0,
	Two = 2,
	Three,
	Four,
	Five,
	Six,
	Seven,
	Eight,
	Nine,
	Ten,
	Jack,
	Queen,
	King,
	Ace
}

[System.Serializable]
public class Card
{
	public static readonly CardSuit[] ALL_EACH_SUITS = new CardSuit[]{ CardSuit.Diamond , CardSuit.Club, CardSuit.Heart, CardSuit.Spade };
	[SerializeField] private CardSuit _suit = CardSuit.Spade;
	public CardSuit SuitOrigin => _suit;
	public CardSuit Suit
	{
		get
		{
			if (Enhancement == CardEnhancement.Wild)
			{
				return CardSuit.Wild;
			}
			return _suit;
		}
		set
		{
			_suit = value;
			View?.UpdatePicture();
		}
	}

	[SerializeField] private CardRank _rank = CardRank.King;
	public CardRank Rank
	{
		get => _rank;
		set
		{
			_rank = value;
			View?.UpdatePicture();
		}
	}
	public int Chip;
	
	[SerializeField] private CardEnhancement _enhancement;
	public CardEnhancement Enhancement
	{
		get => _enhancement;
		set
		{
			_enhancement = value;
			View?.UpdatePaper();
		}
	}

	public Edition Edition = Edition.None;

	[HideInInspector] public CardView View;

	public Card(CardSuit suit, CardRank rank)
	{
		Suit = suit;
		Rank = rank;

		if (rank == CardRank.Jack || rank == CardRank.Queen || rank == CardRank.King)
		{
			Chip = 10;
		}
		else if (rank == CardRank.Ace)
		{
			Chip = 11;
		}
		else
		{
			Chip = (int)rank;
		}
	}

	public Card(CardSuit suit, CardRank rank, int chip)
	{
		Suit = suit;
		Rank = rank;
		Chip = chip;
	}

	public void ActivateInPlay()
	{
		if (Enhancement != CardEnhancement.Stone)
		{
			ScoreCalculator.Instance.AddChip(Chip);
		}

		switch (Enhancement)
		{
			case CardEnhancement.Bonus:
				ScoreCalculator.Instance.AddChip(30);
				break;
			case CardEnhancement.Mult:
				ScoreCalculator.Instance.AddMult(4);
				break;
			case CardEnhancement.Glass:
				ScoreCalculator.Instance.ScaleMult(2);
				break;
			case CardEnhancement.Stone:
				ScoreCalculator.Instance.AddChip(50);
				break;
			case CardEnhancement.Lucky:
				Chance.Roll(5, () => ScoreCalculator.Instance.AddMult(20));
				Chance.Roll(15, () => Inventory.Instance.PlayerMoney += 20);
				break;
			default:
				break;
		}

		switch (Edition)
		{
			case Edition.Foil:
				ScoreCalculator.Instance.AddChip(50);
				break;
			case Edition.Holographic:
				ScoreCalculator.Instance.AddMult(10);
				break;
			case Edition.Polychrome:
				ScoreCalculator.Instance.ScaleMult(1.5d);
				break;
			default:
				break;
		}
		
		IngameEventManager.CallEvent(new CardScoredEventArgs(this));
	}
	
	public void ActivateInHeld()
	{
		if (Enhancement == CardEnhancement.Steel)
		{
			ScoreCalculator.Instance.ScaleMult(1.5d);
		}
		IngameEventManager.CallEvent(new CardHeldEventArgs(this));
	}

	public void Destroy()
	{
		Debug.Log($"{ToString()} Destroy");
		DeckManager.Instance.Deck.Remove(this);
		DeckManager.Instance.Hand.Remove(this);
	}

	public void CopyTo(Card other)
	{
		other.Suit =  Suit;
		other.Rank = Rank;
		other.Chip = Chip;
		other.Enhancement = Enhancement;
	}

	public override string ToString()
	{
		return $"{_rank} of {_suit}";
	}
}
