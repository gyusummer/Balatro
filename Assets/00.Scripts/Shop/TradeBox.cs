using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TradeBox : MonoBehaviour, IDropHandler
{
	public void OnDrop(PointerEventData eventData)
	{
		eventData.pointerDrag.TryGetComponent<ITradeableView>(out var view);
		
		if (view == null) return;
		
		var tradeable = view.Tradeable;
		
		if (tradeable.IsPlayerOwned)
		{
			Sell(tradeable);
		}
		else
		{
			if (Buy(tradeable) == false)
			{
				view.Back();
			}
		}
	}

	private static int PlayerMoney
	{
		get => Inventory.Instance.PlayerMoney;
		set => Inventory.Instance.PlayerMoney = value;
	}
	
	public bool Buy(ITradeable item)
	{
		if (PlayerMoney >= item.Price)
		{
			PlayerMoney -= item.Price;
			item.Buy();
			return true;
		}
		else
		{
			return false;
		}
	}

	public void Sell(ITradeable item)
	{
		item.Sell();
		PlayerMoney += item.Price;
	}
}
