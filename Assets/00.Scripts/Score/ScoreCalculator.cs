using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public enum Edition
{
	None = 0,
	Foil,
	Holographic,
	Polychrome,
	Negative
}

public class ScoreCalculator : Singleton<ScoreCalculator>
{
	public TMP_Text ChipText;
	public TMP_Text MultText;
	public TMP_Text CalcUpperText;
	public TMP_Text CalcUpperExtraText;
	
	public static Dictionary<HandRank, int> s_PokerHandLevel;
	public static Dictionary<HandRank, ScoreComponent> s_BaseScore;
	public static Dictionary<HandRank, ScoreComponent> s_PlanetValue;

	private double _chip;
	public double Chip
	{
		get => _chip;
		set
		{
			_chip = value;
			ChipText.text = value.ToString();
		}
	}
	private double _mult;
	public double Mult
	{
		get => _mult;
		set
		{
			_mult = value;
			MultText.text = value.ToString();
		}
	}
	public double RoundScore;

	protected override void Awake()
	{
		base.Awake();
		s_PokerHandLevel = new Dictionary<HandRank, int>()
		{
			[HandRank.HighCard] = 1,
			[HandRank.Pair] = 1,
			[HandRank.TwoPair] = 1,
			[HandRank.ThreeOfAKind] = 1,
			[HandRank.Straight] = 1,
			[HandRank.Flush] = 1,
			[HandRank.FullHouse] = 1,
			[HandRank.FourOfAKind] = 1,
			[HandRank.StraightFlush] = 1,
			[HandRank.FiveOfAKind] = 1,
			[HandRank.FlushHouse] = 1,
			[HandRank.FlushFive] = 1
		};
		s_BaseScore = new Dictionary<HandRank, ScoreComponent>()
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
		s_PlanetValue = new Dictionary<HandRank, ScoreComponent>()
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

	public IEnumerator EvaluatePlay(List<Card> playedHand)
	{
		HandInfo handInfo = PredictHandRank(playedHand);
		yield return ScoreCards(handInfo.ScoredCards);
		IngameEventManager.CallEvent(new HandPlayedEventArgs(handInfo));
		yield return ActivateHeldCards(playedHand);
		yield return ActivateJokers();
		
		AccumulateScore(Chip * Mult);
	}

	public HandInfo PredictHandRank(List<Card> playedHand)
	{
		EvaluateHand(playedHand, out HandInfo handInfo);
		HandRank handRank = handInfo.Rank;
		CalcUpperText.text = handRank.ToString();
		CalcUpperExtraText.text = $"lvl.{s_PokerHandLevel[handRank]}";
		CalcUpperExtraText.gameObject.SetActive(true);
		
		return handInfo;
	}
	
	private void EvaluateHand(List<Card> playedHand, out HandInfo handInfo)
	{
		handInfo = PokerHand.CheckHandRank(playedHand);
		Debug.Log($"<color=orange>{handInfo.ToString()}</color>");
        
		Chip = s_BaseScore[handInfo.Rank].Chip;
		Mult = s_BaseScore[handInfo.Rank].Mult;
	}

	private IEnumerator ScoreCards(List<Card> scoredCards)
	{
		foreach (var card in scoredCards)
		{
			card.ActivateInPlay();
			yield return new WaitForSeconds(0.5f);
		}
	}

	private IEnumerator ActivateHeldCards(List<Card> playedHand)
	{
		foreach (Card card in DeckManager.Instance.Hand.CloneList())
		{
			if (playedHand.Contains(card))
				continue;
			card.ActivateInHeld();
			yield return new WaitForSeconds(0.5f);
		}
	}

	private IEnumerator ActivateJokers()
	{
		var jokers = Inventory.Instance.Jokers.CloneList();
		foreach (Joker joker in jokers)
		{
			joker.Activate();
			yield return new WaitForSeconds(0.5f);
		}
	}
	
	private void AccumulateScore(double score)
	{
		RoundScore += score;
		BlindManager.Instance.RoundScoreText.text = RoundScore.ToString();
	}
	
	public void UpgradePokerHand(HandRank handRank)
	{
		ScoreComponent handValue = s_BaseScore[handRank];
		ScoreComponent planetValue = s_PlanetValue[handRank];
		
		handValue.Chip += planetValue.Chip;
		handValue.Mult += planetValue.Mult;
		
		s_BaseScore[handRank] = handValue;
		s_PokerHandLevel[handRank] += 1;
	}
}
