using System.Collections;
using System.Collections.Generic;
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
		ScoreCalculator.Instance.TotalScore = 0;
		CurrentBlind = blind;
		BlindGoal = blind.GoalScore;
		ScoreBoard.Instance.UpdateGoal(BlindGoal);
		RunManager.Instance.Round++;
		DeckManager.Instance.InitDrawPile();
		DeckManager.Instance.Hand.Clear();
		FillHand();
	}
	
	public void CheckBlindGoal()
	{
		if (ScoreCalculator.Instance.TotalScore >= BlindGoal)
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
		IngameEventManager.CallEvent(new BlindFinished());
		Economy.GetInterest();
		Inventory.Instance.PlayerMoney += HandsLeft;
		
		if (CurrentBlind.Rank == Blind.BlindRank.Boss)
		{
			RunManager.Instance.WinAnte();
		}
		
		// go to shop;
		RunManager.Instance.ChangeState(3);
	}
	
	public bool PlayHand(List<Card> selectedCards)
	{
		if (HandsLeft <= 0)
		{
			return false;
		}
		// Check Poker Hand
		ScoreCalculator.Instance.ScoreHand(selectedCards);
		
		foreach (Card card in selectedCards)
		{
			DeckManager.Instance.Hand.Remove(card);
		}
		HandsLeft--;

		CheckBlindGoal();
		return true;
	}

	public bool DiscardHand(List<Card> selectedCards)
	{
		if (DiscardsLeft <= 0)
		{
			return false;
		}
		
		foreach (Card card in selectedCards)
		{
			DeckManager.Instance.Hand.Remove(card);
			IngameEventManager.CallEvent(new CardDiscardedEventArgs(card));
		}
		DiscardsLeft--;

		FillHand();
		return true;
	}

	public void FillHand()
	{
		DeckManager.Instance.Draw(HandCapacity - DeckManager.Instance.Hand.Count);
	}
}
