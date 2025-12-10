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
		
		var tradeable = view.Tradeable;
		
		Debug.Log(tradeable.IsPlayerOwned);
		if (tradeable.IsPlayerOwned)
		{
			Sell(tradeable);
		}
		else
		{
			Buy(tradeable);
		}
	}

	private static int PlayerMoney
	{
		get => Inventory.Instance.PlayerMoney;
		set => Inventory.Instance.PlayerMoney = value;
	}
	
	public void Buy(ITradeable item)
	{
		if (PlayerMoney >= item.Price)
		{
			PlayerMoney -= item.Price;
			item.Buy();
		}
	}

	public void Sell(ITradeable item)
	{
		item.Sell();
		PlayerMoney += item.Price;
	}
}
