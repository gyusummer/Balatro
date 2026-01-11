using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
	public TMP_Text RoundScoreText;
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
			if (_chip == value) return;
			_chip = value;
			ChipText.text = value.ToString();
			ChipText.ForceMeshUpdate();

			for (int i = 0; i < ChipText.text.Length; i++)
			{
				chipTextAnimator.DOPunchCharScale(i, 1.1f, 0.1f);
			}
		}
	}
	private double _mult;
	public double Mult
	{
		get => _mult;
		set
		{
			if (_mult == value) return;
			_mult = value;
			MultText.text = value.ToString();
			MultText.ForceMeshUpdate();
			
			for (int i = 0; i < MultText.text.Length; i++)
			{
				multTextAnimator.DOPunchCharScale(i, 1.1f, AnimationVariable.ClickPunchTime);
			}
		}
	}
	private double _roundScore;

	public double RoundScore
	{
		get => _roundScore;
		set
		{
			if (_roundScore == value) return;
			_roundScore = value;
			RoundScoreText.text = value.ToString();
			RoundScoreText.ForceMeshUpdate();

			for (int i = 0; i < RoundScoreText.text.Length; i++)
			{
				roundScoreTextAnimator.DOPunchCharScale(i, 1.1f, AnimationVariable.ClickPunchTime);
			}
		}
	}

	private DOTweenTMPAnimator chipTextAnimator;
	private DOTweenTMPAnimator multTextAnimator;
	private DOTweenTMPAnimator roundScoreTextAnimator;

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
		
		chipTextAnimator = new DOTweenTMPAnimator(ChipText);
		multTextAnimator = new DOTweenTMPAnimator(MultText);
		roundScoreTextAnimator = new DOTweenTMPAnimator(RoundScoreText);
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
		yield return ScoreCards(handInfo.ScoredCards).WaitForCompletion();
		
		Sequence seq = DOTween.Sequence();
		IngameEventManager.CallEvent(new HandPlayedEventArgs(handInfo, seq));
		yield return seq.WaitForCompletion();
		
		yield return ActivateHeldCards(playedHand).WaitForCompletion();
		yield return ActivateJokers(handInfo).WaitForCompletion();
		
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

	private Sequence ScoreCards(List<Card> scoredCards)
	{
		Sequence seq = DOTween.Sequence();
		foreach (var card in scoredCards)
		{
			seq.Append(card.ActivateInPlay());
		}
		return seq;
	}

	private Sequence ActivateHeldCards(List<Card> playedHand)
	{
		Sequence seq = DOTween.Sequence();
		foreach (Card card in DeckManager.Instance.Hand.CloneList())
		{
			if (playedHand.Contains(card))
				continue;
			seq.Append(card.ActivateInHeld());
		}

		return seq;
	}

	private Sequence ActivateJokers(HandInfo handInfo)
	{
		Sequence seq = DOTween.Sequence();
		var jokers = Inventory.Instance.Jokers.CloneList();
		foreach (Joker joker in jokers)
		{
			seq.Append(joker.Activate(handInfo));
		}

		return seq;
	}
	
	private void AccumulateScore(double score)
	{
		RoundScore += score;
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
