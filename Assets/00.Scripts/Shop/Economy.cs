using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economy
{
	public class Change
	{
		public string Detail;
		public int Amount;

		public Change(string detail, int amount)
		{
			Detail = detail;
			Amount = amount;
		}
	}
	
	private static int PlayerMoney
	{
		get => Inventory.Instance.PlayerMoney;
		set => Inventory.Instance.PlayerMoney = value;
	}

	public static void GetInterest()
	{
		int interest = (int)(PlayerMoney / 5);
		switch (interest)
		{
			case > 5:
				interest = 5;
				break;
			case < 0:
				interest = 0;
				break;
		}

		if (interest != 0)
		{
			PlayerMoney += interest;
		}
	}
}
