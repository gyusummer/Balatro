using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardPile
{
	public Action<PlayingCard> OnCardAdded;
	public Action<PlayingCard> OnCardRemoved;
	public string Name;
	[SerializeField] private List<PlayingCard> _cards;
	public int Count => _cards.Count;

	public CardPile(List<PlayingCard> cards, string name)
	{
		_cards = cards;
		Name = name;
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

	public List<PlayingCard> CloneCardList()
	{
		List<PlayingCard> cards = new List<PlayingCard>();
		cards.AddRange(_cards);
		return cards;
	}
}
