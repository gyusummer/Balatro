using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
	[SerializeField] private CardRank _rank = CardRank.King;
	[SerializeField] private CardEnhancement _enhancement;
	[SerializeField] private Edition _edition =  Edition.None;
	public int Chip;
	
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
	public CardRank Rank
	{
		get => _rank;
		set
		{
			_rank = value;
			View?.UpdatePicture();
		}
	}
	public CardEnhancement Enhancement
	{
		get => _enhancement;
		set
		{
			_enhancement = value;
			View?.UpdatePaper();
		}
	}
	public Edition Edition
	{
		get => _edition;
		set
		{
			_edition = value;
			View?.UpdateShader();
		}
	}
	public Sequence ActivateAnimation => View.ActivateAnimation();

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

	public Sequence ActivateInPlay()
	{
		Sequence seq = ActivateAnimation;
		// base
		if (Enhancement != CardEnhancement.Stone)
		{
			seq.JoinCallback(() => ScoreCalculator.Instance.AddChip(Chip));
		}

		// enhance
		switch (Enhancement)
		{
			case CardEnhancement.Bonus:
				seq.Append(ActivateAnimation);
				seq.JoinCallback(() => ScoreCalculator.Instance.AddChip(30));
				break;
			case CardEnhancement.Mult:
				seq.Append(ActivateAnimation);
				seq.JoinCallback(() => ScoreCalculator.Instance.AddMult(4));
				break;
			case CardEnhancement.Glass:
				seq.Append(ActivateAnimation);
				seq.JoinCallback(() => ScoreCalculator.Instance.ScaleMult(2));
				break;
			case CardEnhancement.Stone:
				seq.Append(ActivateAnimation);
				seq.JoinCallback(() => ScoreCalculator.Instance.AddChip(50));
				break;
			case CardEnhancement.Lucky:
				seq.Append(ActivateAnimation);
				seq.JoinCallback(() => Chance.Roll(5, () => ScoreCalculator.Instance.AddMult(20)));
				seq.Append(ActivateAnimation);
				seq.JoinCallback(() => Chance.Roll(15, () => Inventory.Instance.PlayerMoney += 20));
				break;
			default:
				break;
		}

		// edition
		switch (Edition)
		{
			case Edition.Foil:
				seq.Append(ActivateAnimation);
				seq.JoinCallback(() => ScoreCalculator.Instance.AddChip(50));
				break;
			case Edition.Holographic:
				seq.Append(ActivateAnimation);
				seq.JoinCallback(() => ScoreCalculator.Instance.AddMult(10));
				break;
			case Edition.Polychrome:
				seq.Append(ActivateAnimation);
				seq.JoinCallback(() => ScoreCalculator.Instance.ScaleMult(1.5d));
				break;
			default:
				break;
		}
		
		IngameEventManager.CallEvent(new CardScoredEventArgs(this, seq));
		return seq;
	}
	
	public Sequence ActivateInHeld()
	{
		Sequence seq = DOTween.Sequence();
		if (Enhancement == CardEnhancement.Steel)
		{
			seq.Append(ActivateAnimation);
			seq.JoinCallback(() => ScoreCalculator.Instance.ScaleMult(1.5d));
		}
		IngameEventManager.CallEvent(new CardHeldEventArgs(this, seq));
		return seq;
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
