using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class CardPile : CustomList<Card>
{
	public Action<IList<Card>> OnOrderChanged;
	
	public CardPile(IList<Card> cards, string name) : base(cards, name)
	{
		
	}

	public void SortByRank()
	{
		_list = _list.OrderByDescending(card => card.Rank).ThenByDescending(card => card.Suit).ToList();
		
		OnOrderChanged?.Invoke(_list);
	}

	public void SortBySuit()
	{
		_list = _list.OrderByDescending(card => card.Suit).ThenByDescending(card => card.Rank).ToList();
		
		OnOrderChanged?.Invoke(_list);
	}
}
