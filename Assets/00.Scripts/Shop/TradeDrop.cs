using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TradeDrop : MonoBehaviour, IDropHandler
{
	public void OnDrop(PointerEventData eventData)
	{
		eventData.pointerDrag.TryGetComponent<ITradeableView>(out var view);
		
		if (view == null) return;
		
		var com = view.Tradeable;
		
		Debug.Log(com.IsPlayerOwned);
		if (com.IsPlayerOwned)
		{
			com.Sell();
		}
		else
		{
			com.Buy();
		}
	}
}
