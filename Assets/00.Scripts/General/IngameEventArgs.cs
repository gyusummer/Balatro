using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class IngameEventArgs : System.EventArgs
{
	public Sequence Sequence;
	public IngameEventArgs(Sequence sequence)
	{
		Sequence = sequence;
	}
}

public abstract class CardActivateEventArgs : IngameEventArgs
{
	public Card Card;

	protected CardActivateEventArgs(Card card, Sequence sequence) : base(sequence)
	{
		Card = card;
		Sequence = sequence;
	}
}

public class ConsumableConsumedEventArgs : IngameEventArgs
{
	public ConsumableCard Consumable;

	public ConsumableConsumedEventArgs(ConsumableCard consumable, Sequence sequence) : base(sequence)
	{
		Consumable = consumable;
	}
}

public class BlindSelected : IngameEventArgs
{
	public BlindSelected(Sequence sequence) : base(sequence)
	{
	}
}

public class CardDiscardedEventArgs : IngameEventArgs
{
	public Card Card;
	public CardDiscardedEventArgs(Card card, Sequence sequence) : base(sequence)
	{
		Card = card;
	}
}

public class HandPlayedEventArgs : IngameEventArgs
{
	public HandInfo Info;
	
	public HandPlayedEventArgs(HandInfo handInfo, Sequence sequence) : base(sequence)
	{
		Info = handInfo;
	}
}

public class CardScoredEventArgs : CardActivateEventArgs
{
	public CardScoredEventArgs(Card card, Sequence sequence) : base(card, sequence)
	{
		Card = card;
	}
}

public class CardHeldEventArgs : CardActivateEventArgs
{
	public CardHeldEventArgs(Card card, Sequence sequence) : base(card, sequence)
	{
	}
}

public class JokerActivatedEventArgs : IngameEventArgs
{
	public Joker Joker;

	public JokerActivatedEventArgs(Joker joker, Sequence sequence) : base(sequence)
	{
		Joker = joker;
	}
}

public class ScoreCalcFinishedEventArgs : IngameEventArgs
{
	public ScoreCalcFinishedEventArgs(Sequence sequence) : base(sequence)
	{
	}
}

public class BlindFinished : IngameEventArgs
{
	public BlindFinished(Sequence sequence) : base(sequence)
	{
	}
}