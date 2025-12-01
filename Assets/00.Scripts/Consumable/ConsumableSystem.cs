using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableCard
{
	public ConsumableCardView View { get; set; }
	public string Name => GetType().ToString();
	public abstract bool CheckCondition(int selectedCount);
	public bool Use(List<Card> selectedCards)
	{
		Debug.Log($"Consumable Use : {this.GetType().Name}");
		if (CheckCondition(selectedCards.Count) == false)
		{
			return false;
		}

		Effect(selectedCards);
		IngameEventManager.CallEvent(new ConsumableConsumedEventArgs(this));
		return true;
	}
	protected abstract void Effect(List<Card> selectedCards);
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