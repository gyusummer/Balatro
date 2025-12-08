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

	public void SortByRank(Card _)
	{
		var sorted = this.OrderByDescending(card => card.Rank).ThenByDescending(card => card.Suit);
		((Collection<Card>)this).Clear(); 
		this.AddRange(sorted);
		
		OnOrderChanged?.Invoke(this);
	}

	public void SortBySuit(Card _)
	{
		var sorted = this.OrderByDescending(card => card.Suit).ThenByDescending(card => card.Rank).ToList();
		((Collection<Card>)this).Clear();
		this.AddRange(sorted);
		
		OnOrderChanged?.Invoke(this);
	}
}
