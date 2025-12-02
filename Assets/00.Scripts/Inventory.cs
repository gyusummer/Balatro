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
		Jokers.OnAdded += RegisterJoker;
		Jokers.OnRemoved += joker => joker.Unregister();
		Jokers.OnRemoved += joker => Destroy(joker.View.gameObject);
	}

	private void RegisterJoker(Joker joker)
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
}
