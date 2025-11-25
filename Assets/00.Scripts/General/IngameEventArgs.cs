using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IngameEventArgs : System.EventArgs
{
	
}

public class ScoreCardEventArgs : IngameEventArgs
{
	public PlayingCard Card;
	public HandInfo HandInfo;
}

public class DiscardEventArgs : IngameEventArgs
{
	
}

public class ConsumeEventArgs : IngameEventArgs
{
	public IConsumable Consumable;

	public ConsumeEventArgs(IConsumable consumable)
	{
		Consumable = consumable;
	}
}