using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableCard
{
	public ConsumableCardView View { get; set; }
	public abstract bool CheckCondition(int selectedCount);
	public bool Use(List<PlayingCard> selectedCards)
	{
		if (CheckCondition(selectedCards.Count) == false)
		{
			return false;
		}

		Effect(selectedCards);
		IngameEventManager.CallEvent(new ConsumeEventArgs(this));
		return true;
	}
	protected abstract void Effect(List<PlayingCard> selectedCards);
}

public class ConsumableSystem : Singleton<ConsumableSystem>
{
	[SerializeField] private ConsumableCardView viewPrefab;
	[SerializeField] private Transform ConsumableHolder;
	
	public ConsumableCard LastConsumableCard { get; private set; }
	
	private void Start()
	{
		IngameEventManager.RegisterEvent<ConsumeEventArgs>(args =>
		{
			if (args.Consumable is TarotCard or PlanetCard)
			{
				LastConsumableCard = args.Consumable;
			}
		});
	}

	public ConsumableCard CreateConsumable(ConsumableCard consumable)
	{
		return PrintCard(consumable).Source;
	}
	
	public ConsumableCardView PrintCard(ConsumableCard consumable, Transform uiParent = null)
	{
		if (uiParent == null)
		{
			uiParent = ConsumableHolder;
		}
		ConsumableCardView consumableView = Instantiate(viewPrefab, uiParent);
		consumableView.Init(consumable);
		return consumableView;
	}
}