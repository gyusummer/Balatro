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
		Debug.Log("ConsumableCard Use");
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
	[SerializeField] private ConsumableCardView viewPrefab;
	[SerializeField] private Transform consumableHolder;
	
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

	public ConsumableCard CreateConsumable(ConsumableCard consumable)
	{
		return Print(consumable).Source;
	}
	
	private ConsumableCardView Print(ConsumableCard consumable, Transform uiParent = null)
	{
		if (uiParent == null)
		{
			uiParent = consumableHolder;
		}
		ConsumableCardView consumableView = Instantiate(viewPrefab, uiParent);
		consumableView.Init(consumable);
		return consumableView;
	}
}