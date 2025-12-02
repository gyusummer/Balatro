using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
	[SerializeField] private Transform _jokerHolder;
	[SerializeField] private Transform _consumableHolder;
	
	public CustomList<Joker> Jokers = new CustomList<Joker>(5);
	public CustomList<ConsumableCard> Consumables = new CustomList<ConsumableCard>(2);

	public void Start()
	{
		Jokers.OnAdded += joker => joker.Register();
		Jokers.OnRemoved += joker => joker.Unregister();
	}

	private void AddJoker(Joker joker)
	{
		joker.Register();
		joker.View.transform.SetParent(_jokerHolder);
	}

	private void AddConsumable(ConsumableCard consumable)
	{
		consumable.View.transform.SetParent(_jokerHolder);
	}
}
