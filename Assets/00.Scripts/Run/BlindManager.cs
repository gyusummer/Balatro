using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Blind
{
	public enum BlindRank
	{
		None,
		Small,
		Big,
		Boss
	}

	public double GoalScore;
	public BlindRank Rank;

	public Blind(double goalScore, BlindRank rank)
	{
		GoalScore = goalScore;
		Rank = rank;
	}
}

public class BlindManager : Singleton<BlindManager>
{
	public TMP_Text HandsText;
	public TMP_Text DiscardsText;
	public TMP_Text GoalText;
	//public TMP_Text RoundScoreText;
	
	public Blind CurrentBlind;
	public double BlindGoal = 450;

	private int _handsLeft = 4;
	public int HandsLeft
	{
		get => _handsLeft;
		set
		{
			_handsLeft = value;
			HandsText.text = value.ToString();
		}
	}
	private int _discardsLeft = 3;
	public int DiscardsLeft
	{
		get => _discardsLeft;
		set
		{
			_discardsLeft = value;
			DiscardsText.text = value.ToString();
		}
	}
	public int HandCapacity = 8;
	public bool CanDiscard => DiscardsLeft > 0;
	
	private void InitBlind()
	{
		HandsLeft = RunManager.Variables.Hands;
		DiscardsLeft = RunManager.Variables.Discards;
		HandCapacity = RunManager.Variables.HandCapacity;
	}

	public void StartBlind(Blind blind)
	{
		InitBlind();
		ScoreCalculator.Instance.RoundScore = 0;
		CurrentBlind = blind;
		BlindGoal = blind.GoalScore;
		GoalText.text = BlindGoal.ToString();
		RunManager.Instance.Round++;
		DeckManager.Instance.InitDrawPile();
		DeckManager.Instance.Hand.Clear();
		FillHand();
	}
	
	public void CheckBlindGoal()
	{
		if (ScoreCalculator.Instance.RoundScore >= BlindGoal)
		{
			WinBlind();
			return;
		}
		else if (HandsLeft <= 0)
		{
			RunManager.Instance.LoseGame();
			return;
		}
		// not Win, not Lose => continue;
		FillHand();
	}

	public void WinBlind()
	{
		Debug.Log("<color=red>WinBlind</color>");
		// broadcast blind end event
		// earn money;
		IngameEventManager.CallEvent(new BlindFinished(DOTween.Sequence()));
		Economy.GetInterest();
		Inventory.Instance.PlayerMoney += HandsLeft;
		
		if (CurrentBlind.Rank == Blind.BlindRank.Boss)
		{
			RunManager.Instance.WinAnte();
		}
		
		ScoreCalculator.Instance.ClearUI();
		
		// go to shop;
		RunManager.Instance.ChangeState(RunManager.RunState.Shop);
	}
	
	public void PlayHand(List<Card> selectedCards)
	{
		if (HandsLeft <= 0)
		{
			return;
		}

		StartCoroutine(PlayHand_Co(selectedCards));
	}

	private IEnumerator PlayHand_Co(List<Card> selectedCards)
	{
		yield return ScoreCalculator.Instance.EvaluatePlay(selectedCards);
		
		foreach (Card card in selectedCards)
		{
			DeckManager.Instance.Hand.Remove(card);
		}
		HandsLeft--;

		CheckBlindGoal();
	}

	public void DiscardHand(List<Card> selectedCards)
	{
		DeckManager.Instance.DiscardHand(selectedCards);
	}

	public void FillHand()
	{
		DeckManager.Instance.FillHand();
	}
}
