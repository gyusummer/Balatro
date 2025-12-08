using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CardPile : CustomList<Card>
{
	public Action<List<Card>> OnOrderChanged;
	
	public CardPile(List<Card> cards, string name) : base(cards, name)
	{
		
	}

	public void SortByRank(Card _)
	{
		_list = _list.OrderByDescending(card => card.Rank).ThenByDescending(card => card.Suit).ToList();
		OnOrderChanged?.Invoke(_list);
		Debug.Log("SortByRank");
	}

	public void SortBySuit(Card _)
	{
		_list = _list.OrderByDescending(card => card.Suit).ThenByDescending(card => card.Rank).ToList();
		OnOrderChanged?.Invoke(_list);
	}
}
