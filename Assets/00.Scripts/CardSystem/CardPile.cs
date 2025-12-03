using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardPile : CustomList<Card>
{
	public CardPile(List<Card> cards, string name) : base(cards, name)
	{
		
	}
}
