using System.Collections;
using System.Collections.Generic;
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
	public Blind CurrentBlind;
	public double BlindGoal = 450;
	public int HandsLeft = 4;
	public int DiscardsLeft = 3;
	public int HandCapacity = 8;
	
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
		
		if (CurrentBlind.Rank == Blind.BlindRank.Boss)
		{
			RunManager.Instance.WinAnte();
		}
		
		// go to shop;
		RunManager.Instance.ChangeState(3);
	}
	
	public void PlayHand(List<Card> selectedCards)
	{
		// Check Poker Hand
		ScoreCalculator.Instance.ScoreHand(selectedCards);
		
		foreach (Card card in selectedCards)
		{
			DeckManager.Instance.Hand.Remove(card);
		}
		selectedCards.Clear();
		HandsLeft--;

		CheckBlindGoal();
	}

	public void DiscardHand(List<Card> selectedCards)
	{
		if (DiscardsLeft <= 0)
		{
			return;
		}
		
		foreach (Card card in selectedCards)
		{
			DeckManager.Instance.Hand.Remove(card);
			IngameEventManager.CallEvent(new CardDiscardedEventArgs(card));
		}
		selectedCards.Clear();
		DiscardsLeft--;

		FillHand();
	}

	public void FillHand()
	{
		DeckManager.Instance.Draw(HandCapacity - DeckManager.Instance.Hand.Count);
	}
}
