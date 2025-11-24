using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TarotCard
{
	
}

public class Justice : TarotCard
{
	public int MaxTarget = 1;

	public bool CheckCondition()
	{
		int count = HandController.Instance.SelectedCount;

		if (count != 0 && count <= MaxTarget)
		{
			return true;
		}
		return false;
	}

	public void UseTo(List<PlayingCard> cards)
	{
		foreach (var card in cards)
		{
			card.Enhancement = CardEnhancement.Glass;
		}
	}
}
