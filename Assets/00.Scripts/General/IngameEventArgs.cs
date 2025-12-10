using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IngameEventArgs : System.EventArgs
{
	
}

public class ConsumableConsumedEventArgs : IngameEventArgs
{
	public ConsumableCard Consumable;

	public ConsumableConsumedEventArgs(ConsumableCard consumable)
	{
		Consumable = consumable;
	}
}

public class BlindSelected : IngameEventArgs
{
	
}

public class CardDiscardedEventArgs : IngameEventArgs
{
	public Card Card;
	public CardDiscardedEventArgs(Card card)
	{
		Card = card;
	}
}

public class HandPlayedEventArgs : IngameEventArgs
{
	public HandInfo Info;
	public HandPlayedEventArgs(HandInfo handInfo)
	{
		Info = handInfo;
	}
}

public class CardScoredEventArgs : IngameEventArgs
{
	public Card Card;

	public CardScoredEventArgs(Card card)
	{
		Card = card;
	}
}


public class CardHeldEventArgs : IngameEventArgs
{
	public Card Card;
	
	public CardHeldEventArgs(Card card)
	{
		Card = card;
	}
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