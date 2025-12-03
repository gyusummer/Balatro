using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindManager : Singleton<BlindManager>
{
	public int BlindGoal = 450;
	public int HandsLeft = 4;
	public int DiscardsLeft = 3;
	public int HandCapacity = 8;
	
	public void InitBlind()
	{
		HandsLeft = RunManager.Variables.Hands;
		DiscardsLeft = RunManager.Variables.Discards;
		HandCapacity = RunManager.Variables.HandCapacity;
	}

	public void StartBlind()
	{
		ScoreCalculator.Instance.TotalScore = 0;
		DeckManager.Instance.InitDrawPile();
		DeckManager.Instance.Hand.Clear();
		FillHand();
	}
	
	public void CheckBlindGoal()
	{
		if (ScoreCalculator.Instance.TotalScore >= BlindGoal)
		{
			WinBlind();
		}
		else if (HandsLeft <= 0)
		{
			// Lose;
			RunManager.Instance.LoseGame();
		}
		// not Win, not Lose => continue;
		FillHand();
	}

	public void WinBlind()
	{
		// broadcast blind end event
		// earn money;
		// go to shop;
		Debug.Log("<color=red>WinBlind</color>");
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
