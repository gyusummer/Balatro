using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : Singleton<ScoreCalculator>
{
	private static Dictionary<HandRank, ScoreComponent> s_baseScore;
	private static Dictionary<HandRank, ScoreComponent> s_planetValue;

	private double _chip;
	public double Chip
	{
		get => _chip;
		set
		{
			_chip = value;
			ScoreBoard.Instance.UpdateChip(value);
		}
	}
	private double _mult;
	public double Mult
	{
		get => _mult;
		set
		{
			_mult = value;
			ScoreBoard.Instance.UpdateMult(value);
		}
	}
	public double TotalScore;

	protected override void Awake()
	{
		base.Awake();
		s_baseScore = new Dictionary<HandRank, ScoreComponent>()
		{
			[HandRank.HighCard] = new ScoreComponent(5, 1),
			[HandRank.Pair] = new ScoreComponent(10, 2),
			[HandRank.TwoPair] = new ScoreComponent(20, 2),
			[HandRank.ThreeOfAKind] = new ScoreComponent(30, 3),
			[HandRank.Straight] = new ScoreComponent(30, 4),
			[HandRank.Flush] = new ScoreComponent(35, 4),
			[HandRank.FullHouse] = new ScoreComponent(40, 4),
			[HandRank.FourOfAKind] = new ScoreComponent(60, 7),
			[HandRank.StraightFlush] = new ScoreComponent(100, 8),
			[HandRank.FiveOfAKind] = new ScoreComponent(120, 12),
			[HandRank.FlushHouse] = new ScoreComponent(140, 14),
			[HandRank.FlushFive] = new ScoreComponent(160, 16)
		};
		s_planetValue = new Dictionary<HandRank, ScoreComponent>()
		{
			[HandRank.HighCard] = new ScoreComponent(10, 1),
			[HandRank.Pair] = new ScoreComponent(15, 1),
			[HandRank.TwoPair] = new ScoreComponent(20, 1),
			[HandRank.ThreeOfAKind] = new ScoreComponent(20, 2),
			[HandRank.Straight] = new ScoreComponent(30, 3),
			[HandRank.Flush] = new ScoreComponent(15, 2),
			[HandRank.FullHouse] = new ScoreComponent(25, 2),
			[HandRank.FourOfAKind] = new ScoreComponent(30, 3),
			[HandRank.StraightFlush] = new ScoreComponent(40, 4),
			[HandRank.FiveOfAKind] = new ScoreComponent(35, 3),
			[HandRank.FlushHouse] = new ScoreComponent(40, 4),
			[HandRank.FlushFive] = new ScoreComponent(50, 3)
		};
	}

	public void AddChip(double amount)
	{
		Chip += amount;
	}

	public void ScaleChip(double factor)
	{
		Chip *= factor;
	}

	public void AddMult(double amount)
	{
		Mult += amount;
	}

	public void ScaleMult(double factor)
	{
		Mult *= factor;
	}
	
	public void ScoreHand(List<Card> hand)
	{
		HandInfo handInfo = PokerHand.CheckHandRank(hand);
		IngameEventManager.CallEvent(new HandPlayedEventArgs(handInfo));
		Debug.Log($"<color=orange>{handInfo.Rank}</color>");
        
		Chip = s_baseScore[handInfo.Rank].Chip;
		Mult = s_baseScore[handInfo.Rank].Mult;

		foreach (var card in handInfo.ScoredCards)
		{
			card.ActivateInPlay();
		}
		
		AccumulateScore(Chip * Mult);
	}
	
	public void AccumulateScore(double score)
	{
		TotalScore += score;
		ScoreBoard.Instance.UpdateScore(TotalScore);
	}
	
	public void UpgradePokerHand(HandRank handRank)
	{
		ScoreComponent handValue = s_baseScore[handRank];
		ScoreComponent planetValue = s_planetValue[handRank];
		
		handValue.Chip += planetValue.Chip;
		handValue.Mult += planetValue.Mult;
		
		s_baseScore[handRank] = handValue;
	}
}
