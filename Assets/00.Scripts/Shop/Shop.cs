using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public interface ITradeable
{
	public bool IsPlayerOwned { get; set; }
	public int Price { get; set; }
	public bool Buy();
	public bool Sell();
}

public interface ITradeableView
{
	public ITradeable Tradeable { get; }
	public void Back();
}

public class Shop : Singleton<Shop>
{
	[SerializeField] private Transform UpperList;
	[SerializeField] private Transform LowerList;
	
	public CustomList<Joker> Jokers = new CustomList<Joker>(3);
	public CustomList<ConsumableCard> Consumables = new CustomList<ConsumableCard>(3);

	public void RefreshGoods()
	{
		Jokers.ClearWith(joker => Destroy(joker.View.gameObject));
		for (int i = 0; i < Jokers.Max; i++)
		{
			Joker joker = JokerFactory.Instance.CreateRandom(true, UpperList);
			Jokers.Add(joker);
		}
		
		Consumables.ClearWith(consume => Destroy(consume.View.gameObject));
		for (int i = 0; i < Consumables.Max; i++)
		{
			ConsumableCard consume = ConsumableSystem.Instance.CreateRandom(true, LowerList);
			Consumables.Add(consume);
		}
	}

	public void NextRound()
	{
		RunManager.Instance.ChangeState(RunManager.RunState.BlindSelect);
	}
}
