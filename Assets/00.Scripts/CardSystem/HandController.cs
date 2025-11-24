using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. Select
// 2. Play
// 3. Discard
public class HandController : Singleton<HandController>
{
	private List<PlayingCard> _selectedCards;
	public int SelectLimit = 5;
	
	public Action<PlayingCard> OnDiscardCard;

	private void Start()
	{
		_selectedCards = new List<PlayingCard>();
		DeckSystem.Instance.Hand.OnCardAdded += OnCardAddedToHand;
	}

	private void OnCardAddedToHand(PlayingCard card)
	{
		card.View.OnClick += OnClickCard;
	}
	
	private void OnClickCard(PlayingCardView cardView)
	{
		// Deselect
		if (_selectedCards.Contains(cardView.Source))
		{
			_selectedCards.Remove(cardView.Source);
			cardView.OnDeselected();
		}
		else if (_selectedCards.Count >= SelectLimit)
		{
			Debug.Log("!Can't Select More!");
		}
		// Select
		else
		{
			_selectedCards.Add(cardView.Source);
			cardView.OnSelected();
		}
	}

	public void PlayHand()
	{
		if (_selectedCards.Count <= 0)
		{
			Debug.Log("!No Cards Selected!");
			return;
		}
		
		// Check Poker Hand
		ScoreCalculator.Instance.ScoreHand(_selectedCards);
		
		foreach (PlayingCard card in _selectedCards)
		{
			DeckSystem.Instance.Hand.RemoveCard(card);
		}
		_selectedCards.Clear();
	}

	public void DiscardHand()
	{
		if (_selectedCards.Count <= 0)
		{
			Debug.Log("!No Cards Selected!");
			return;
		}
		
		foreach (PlayingCard card in _selectedCards)
		{
			OnDiscardCard?.Invoke(card);
			DeckSystem.Instance.Hand.RemoveCard(card);
		}
		_selectedCards.Clear();
	}

	private void OnDestroy()
	{
		// DeckSystem.Instance.Hand.OnCardAdded -= OnCardAddedToHand;
	}
}
