using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPile
{
	public Action<PlayingCard> OnCardAdded;
	public Action<PlayingCard> OnCardRemoved;
	private List<PlayingCard> _cards;
	public int Count => _cards.Count;

	public CardPile(List<PlayingCard> cards)
	{
		_cards = cards;
	}
	
	public PlayingCard GetFirstCard()
	{
		return _cards[0];
	}
	
	public void AddCard(PlayingCard card)
	{
		_cards.Add(card);
		OnCardAdded?.Invoke(card);
	}

	public void RemoveCard(PlayingCard card)
	{
		if (_cards.Remove(card))
		{
			OnCardRemoved?.Invoke(card);
		}
	}
}
