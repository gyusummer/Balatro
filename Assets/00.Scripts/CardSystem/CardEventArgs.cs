using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardEventArgs : System.EventArgs
{
	
}

public class EarnScoreEventArgs : CardEventArgs
{
	public PlayingCard Card;
	public HandInfo HandInfo;
}

public class DiscardEventArgs : CardEventArgs
{
	
}