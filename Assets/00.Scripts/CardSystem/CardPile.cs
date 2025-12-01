using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardPile
{
	public Action<Card> OnCardAdded;
	public Action<Card> OnCardRemoved;
	public string Name;
	[SerializeField] private List<Card> _cards;
	public int Count => _cards.Count;

	public CardPile(List<Card> cards, string name)
	{
		_cards = cards;
		Name = name;
	}
	
	public Card GetFirstCard()
	{
		return _cards[0];
	}
	
	public void AddCard(Card card)
	{
		_cards.Add(card);
		OnCardAdded?.Invoke(card);
	}

	public void RemoveCard(Card card)
	{
		if (_cards.Remove(card))
		{
			OnCardRemoved?.Invoke(card);
		}
	}

	public List<Card> CloneCardList()
	{
		List<Card> cards = new List<Card>();
		cards.AddRange(_cards);
		return cards;
	}
}
