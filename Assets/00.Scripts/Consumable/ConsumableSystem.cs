using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public abstract class ConsumableCard : ITradeable
{
	public ConsumableCardView View { get; set; }
	public string Name => GetType().ToString();
	public abstract string Description { get; }
	public bool IsPlayerOwned { get; set; }
	public int Price { get; set; }
	public abstract bool CheckCondition(int selectedCount);
	public bool Use(List<Card> selectedCards)
	{
		Debug.Log($"Consumable Use : {this.GetType().Name}");
		if (CheckCondition(selectedCards.Count) == false)
		{
			return false;
		}

		Inventory.Instance.Consumables.Remove(this);
		Effect(selectedCards);
		IngameEventManager.CallEvent(new ConsumableConsumedEventArgs(this));
		UnityEngine.Object.Destroy(View.gameObject);
		return true;
	}
	protected abstract void Effect(List<Card> selectedCards);
	public bool Buy()
	{
		if (Shop.Instance.Consumables.Contains(this) && Inventory.Instance.Consumables.IsFull == false)
		{
			Shop.Instance.Consumables.Remove(this);
			Inventory.Instance.Consumables.Add(this);
			Debug.Log($"{this.Name} Buy");
			return true;
		}
		return false;
	}

	public bool Sell()
	{
		if (Inventory.Instance.Consumables.Contains(this))
		{
			Inventory.Instance.Consumables.Remove(this);
			Debug.Log($"{this.Name} Sell");
			return true;
		}
		return false;
	}
}

public class ConsumableSystem : Singleton<ConsumableSystem>
{
	public ConsumableCard LastConsumableCard { get; private set; }
	
	private void Start()
	{
		IngameEventManager.AddListener<ConsumableConsumedEventArgs>(args =>
		{
			if (args.Consumable is TarotCard or PlanetCard)
			{
				LastConsumableCard = args.Consumable;
			}
		});
	}

	public ConsumableCard CreateRandom(bool print, Transform uiParent = null)
	{
		int n = Random.Range(0, 2);
		if (n == 0)
		{
			return TarotFactory.Instance.CreateRandom(print, uiParent);
		}
		else
		{
			return PlanetFactory.Instance.CreateRandom(print, uiParent);
		}
	}
	
	public void Print(ConsumableCard consumable, Transform uiParent = null)
	{
		switch (consumable)
		{
			case TarotCard:
				TarotFactory.Instance.Print(consumable);
				break;
			case PlanetCard:
				PlanetFactory.Instance.Print(consumable);
				break;
			default:
				break;
		}
	}
}