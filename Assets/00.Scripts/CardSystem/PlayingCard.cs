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

	[HideInInspector] public PlayingCardView View;

	public PlayingCard(CardSuit suit, CardRank rank)
	{
		Suit = suit;
		Rank = rank;
	}

	public PlayingCard(CardSuit suit, CardRank rank, int chip)
	{
		Suit = suit;
		Rank = rank;
		Chip = chip;
	}

	public override string ToString()
	{
		return $"{_rank} of {_suit}";
	}
}
