using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableCard
{
	public ConsumableCardView View { get; set; }
	public abstract bool CheckCondition(int selectedCount);
	public void Use(List<PlayingCard> selectedCards)
	{
		Effect(selectedCards);
		IngameEventManager.CallEvent(new ConsumeEventArgs(this));
	}
	protected abstract void Effect(List<PlayingCard> selectedCards);
}

public class ConsumableSystem : Singleton<ConsumableSystem>
{
	public List<Type> Tarots = new List<Type>()
	{
		typeof(Fool),
	};
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

	public ConsumableCard CreateConsumable<T>() where T : ConsumableCard, new()
	{
		return new T();
	}
}