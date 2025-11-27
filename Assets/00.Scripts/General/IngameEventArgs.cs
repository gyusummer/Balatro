using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IngameEventArgs : System.EventArgs
{
	
}

public class ScoreCardEventArgs : IngameEventArgs
{
	public PlayingCard Card;

	public ScoreCardEventArgs(PlayingCard card)
	{
		Card = card;
	}
}

public class DiscardCardEventArgs : IngameEventArgs
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