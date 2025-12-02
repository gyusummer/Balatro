using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITradeable
{
	public int Price { get; set; }
	public bool Buy();
	public bool Sell();
}
public class Shop : Singleton<Shop>
{
	[SerializeField] private Transform UpperList;
	[SerializeField] private Transform LowerList;
	
	[SerializeField] private CustomList<Joker> _jokers = new CustomList<Joker>(3);
	[SerializeField] private CustomList<ConsumableCard> _consumes = new CustomList<ConsumableCard>(3);

	private void Start()
	{
		_jokers.OnRemoved += joker => Destroy(joker.View.gameObject);
		_consumes.OnRemoved += consume => Destroy(consume.View.gameObject);
	}

	public void FillGoods()
	{
		_jokers.Clear();
		for (int i = 0; i < _jokers.Max; i++)
		{
			Joker joker = JokerFactory.Instance.CreateRandom(UpperList);
			_jokers.Add(joker);
		}
		_consumes.Clear();
		for (int i = 0; i < _consumes.Max; i++)
		{
			ConsumableCard consume = ConsumableSystem.Instance.CreateRandom(LowerList);
			_consumes.Add(consume);
		}
	}
}
