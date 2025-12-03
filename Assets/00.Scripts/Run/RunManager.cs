using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : Singleton<RunManager>
{
	public static class Variables
	{
		// last played consumable
		// tarot consume count
		// skipped count
		public static int Hands = 4;
		public static int Discards = 3;
		public static int HandCapacity = 8;
	}

	public void WinGame()
	{
		Debug.Log("<color=red>WinGame</color>");
	}
	
	public void LoseGame()
	{
		Debug.Log("<color=red>LoseGame</color>");
	}
}
