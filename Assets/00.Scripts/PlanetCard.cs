using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlanetCard : ConsumableCard
{
	public override bool CheckCondition(int selectedCount)
	{
		return true;
	}
}

public class Mars : PlanetCard
{
	public HandRank SubjectRank = HandRank.FourOfAKind;
	public ScoreComponent UpgradeValue = new ScoreComponent();

	protected override void Effect(List<PlayingCard> selectedCards)
	{
		ScoreCalculator.Instance.UpgradePokerHand(SubjectRank);
	}
}