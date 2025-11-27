using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardSuit
{
	None = -1,
	Diamond,
	Club,
	Heart,
	Spade,
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
public class PlayingCard
{
	[SerializeField] private CardSuit _suit = CardSuit.Spade;
	public CardSuit Suit
	{
		get => _suit;
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

	[HideInInspector] public PlayingCardView View;

	public PlayingCard(CardSuit suit, CardRank rank)
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

	public PlayingCard(CardSuit suit, CardRank rank, int chip)
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
				// Chance.Roll(15, () => +20$)
				break;
			default:
				break;
		}
		
		IngameEventManager.CallEvent(new ScoreCardEventArgs(this));
	}

	public void ActivateInHand()
	{
		
	}

	public void Destroy()
	{
		Debug.Log($"{ToString()} Destroy");
		DeckSystem.Instance.Deck.RemoveCard(this);
		DeckSystem.Instance.Hand.RemoveCard(this);
	}

	public override string ToString()
	{
		return $"{_rank} of {_suit}";
	}
}
