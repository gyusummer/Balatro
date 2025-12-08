using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class PlanetCard : ConsumableCard
{
	protected HandRank HandRank { get; set; }
    public override bool CheckCondition(int selectedCount)
    {
        return true;
    }

    protected override void Effect(List<Card> selectedCards)
    {
        ScoreCalculator.Instance.UpgradePokerHand(HandRank);
    }
}

public class Pluto : PlanetCard
{
	public Pluto()
	{
		HandRank = HandRank.HighCard;
	}
}

public class Mercury : PlanetCard
{
	public Mercury()
	{
		HandRank = HandRank.Pair;
	}
}

public class Uranus : PlanetCard
{
	public Uranus()
	{
		HandRank = HandRank.TwoPair;
	}
}

public class Venus : PlanetCard
{
	public Venus()
	{
		HandRank = HandRank.ThreeOfAKind;
	}
}

public class Saturn : PlanetCard
{
	public Saturn()
	{
		HandRank = HandRank.Straight;
	}
}

public class Jupiter : PlanetCard
{
	public Jupiter()
	{
		HandRank = HandRank.Flush;
	}
}

public class Earth : PlanetCard
{
	public Earth()
	{
		HandRank = HandRank.FullHouse;
	}
}

public class Mars : PlanetCard
{
	public Mars()
	{
		HandRank = HandRank.FourOfAKind;
	}
}

public class Neptune : PlanetCard
{
	public Neptune()
	{
		HandRank = HandRank.StraightFlush;
	}
}

public class PlanetX : PlanetCard
{
	public PlanetX()
	{
		HandRank = HandRank.FiveOfAKind;
	}
}

public class Ceres : PlanetCard
{
	public Ceres()
	{
		HandRank = HandRank.FlushHouse;
	}
}

public class Eris : PlanetCard
{
	public Eris()
	{
		HandRank = HandRank.FlushFive;
	}
}