using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConsumable
{
	public ConsumableCardView View { get; set; }
	public string Name { get; }
	public bool CheckCondition(int selectedCount);
	public bool Use(List<PlayingCard> selectedCards);
}

public abstract class ConsumableCard : IConsumable
{
	public ConsumableCardView View { get; set; }
	public string Name => GetType().ToString();
	public abstract bool CheckCondition(int selectedCount);
	public bool Use(List<PlayingCard> selectedCards)
	{
		if (CheckCondition(selectedCards.Count) == false)
		{
			return false;
		}

		Effect(selectedCards);
		IngameEventManager.CallEvent(new ConsumableConsumedEventArgs(this));
		return true;
	}
	protected abstract void Effect(List<PlayingCard> selectedCards);
}

public class ConsumableSystem : Singleton<ConsumableSystem>
{
	[SerializeField] private ConsumableCardView viewPrefab;
	[SerializeField] private Transform consumableHolder;
	
	public IConsumable LastConsumableCard { get; private set; }
	
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

	public IConsumable CreateConsumable(IConsumable consumable)
	{
		return PrintCard(consumable).Source;
	}
	
	public ConsumableCardView PrintCard(IConsumable consumable, Transform uiParent = null)
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