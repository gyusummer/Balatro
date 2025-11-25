using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlanetCard : ConsumableCard
{
	protected HandRank HandRank { get; set; }
    public override bool CheckCondition(int selectedCount)
    {
        return true;
    }

    protected override void Effect(List<PlayingCard> selectedCards)
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

public static class PlanetFactory
{
	// 딕셔너리: 카드 ID 또는 Enum을 키로, 해당 카드의 타입을 값으로 저장합니다.
	// 하지만 타입을 저장하는 대신, 바로 생성하는 델리게이트를 저장하는 것이 더 유연합니다.
	private static readonly Dictionary<int, Func<PlanetCard>> CardConstructors = new Dictionary<int, Func<PlanetCard>>()
	{
		// 모든 22개 카드의 생성자를 람다 표현식으로 등록
		{ 0, () => new Pluto() },
		{ 1, () => new Mercury() },
		{ 2, () => new Uranus() },
		{ 3, () => new Venus() },
		{ 4, () => new Saturn() },
		{ 5, () => new Jupiter() },
		{ 6, () => new Earth() },
		{ 7, () => new Mars() },
		{ 8, () => new Neptune() },
		{ 9, () => new PlanetX() },
		{ 10, () => new Ceres() },
		{ 11, () => new Eris() },
	};

	private static readonly System.Random rng = new System.Random();
	
	/// <summary>
	/// 랜덤한 행성카드 객체를 생성하여 반환합니다.
	/// </summary>
	public static PlanetCard CreateRandomCard()
	{
		// 1. 딕셔너리의 키(ID) 목록을 가져옵니다.
		List<int> availableKeys = new List<int>(CardConstructors.Keys);
        
		// 2. 랜덤한 키(카드 ID)를 선택합니다.
		int randomIndex = rng.Next(availableKeys.Count);
		int randomCardId = availableKeys[randomIndex];

		// 3. 해당 키에 등록된 생성자(Func<PlanetCard>)를 호출하여 객체를 생성합니다.
		return CardConstructors[randomCardId].Invoke();
	}

	public static PlanetCard CreateCard(int index)
	{
		return CardConstructors[index].Invoke();
	}
}