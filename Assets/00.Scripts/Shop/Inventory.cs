using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
	[SerializeField] private Transform _jokerHolder;
	[SerializeField] private Transform _consumableHolder;
	
	public CustomList<Joker> Jokers = new CustomList<Joker>(5);
	public CustomList<ConsumableCard> Consumables = new CustomList<ConsumableCard>(2);
	
	[field: SerializeField] public int PlayerMoney { get; set; }

	public void Start()
	{
		Jokers.OnAdded += ProcessView;
		Jokers.OnRemoved += joker => joker.Unregister();
		Jokers.OnRemoved += joker => Destroy(joker.View.gameObject);
		Consumables.OnAdded += ProcessView;
		Consumables.OnRemoved += consume => Destroy(consume.View.gameObject);
	}

	private void ProcessView(Joker joker)
	{
		joker.Register();
		if (joker.View == null)
		{
			Debug.Log($"{joker.Name}'s View is null");
			JokerFactory.Instance.Print(joker);
		}
		else
		{
			Debug.Log($"{joker.Name}'s View is not null");
			joker.View.transform.SetParent(_jokerHolder);
		}
	}
	
	private void ProcessView(ConsumableCard consumable)
	{
		if (consumable.View == null)
		{
			Debug.Log($"{consumable.Name}'s View is null");
			ConsumableSystem.Instance.Print(consumable);
		}
		else
		{
			Debug.Log($"{consumable.Name}'s View is not null");
			consumable.View.transform.SetParent(_consumableHolder);
		}
	}
}
