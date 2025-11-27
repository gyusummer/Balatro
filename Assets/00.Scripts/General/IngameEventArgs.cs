using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IngameEventArgs : System.EventArgs
{
	
}

public class ConsumableConsumedEventArgs : IngameEventArgs
{
	public IConsumable Consumable;

	public ConsumableConsumedEventArgs(IConsumable consumable)
	{
		Consumable = consumable;
	}
}

public class BlindSelected : IngameEventArgs
{
	
}

public class CardDiscardedEventArgs : IngameEventArgs
{
	
}

public class HandPlayedEventArgs : IngameEventArgs
{
	
}

public class CardScoredEventArgs : IngameEventArgs
{
	public PlayingCard Card;

	public CardScoredEventArgs(PlayingCard card)
	{
		Card = card;
	}
}


public class CardHeldEventArgs : IngameEventArgs
{
	
}

public class JokerActivatedEventArgs : IngameEventArgs
{
	public Joker Joker;

	public JokerActivatedEventArgs(Joker joker)
	{
		Joker = joker;
	}
}

public class ScoreCalcFinishedEventArgs : IngameEventArgs
{
	
}

public class BlindFinished : IngameEventArgs
{
	
}