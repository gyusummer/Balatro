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
	public int SelectedCount => _selectedCards.Count;
	public int SelectLimit = 5;
	
	private IConsumable _selectedConsumable;
	
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

	private void SelectCard(PlayingCard card)
	{
		_selectedCards.Add(card.View.Source);
		card.View.OnSelected();
	}

	private void DeselectCard(PlayingCard card)
	{
		_selectedCards.Remove(card);
		card.View.OnDeselected();
	}

	public void DeselectAllCards()
	{
		Debug.Log($"Deselect {_selectedCards.Count} Cards");
		for (int i = _selectedCards.Count - 1; i >= 0; i--)
		{
			var c =  _selectedCards[i];
			Debug.Log(c);
			DeselectCard(_selectedCards[i]);
		}
	}
	
	private void OnClickCard(PlayingCardView cardView)
	{
		PlayingCard card = cardView.Source;
		
		if (_selectedCards.Contains(cardView.Source))
		{
			DeselectCard(card);
		}
		else if (_selectedCards.Count >= SelectLimit)
		{
			Debug.Log("!Can't Select More!");
		}
		else
		{
			SelectCard(card);
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

	public void SelectConsumable(IConsumable consumable)
	{
		Debug.Log(_selectedConsumable);
		_selectedConsumable?.View.OnDeselected();
		_selectedConsumable = consumable;
		Debug.Log(_selectedConsumable);
		_selectedConsumable.View.OnSelected();
	}

	public void UseConsumable()
	{
		if (_selectedConsumable.Use(_selectedCards))
		{
			Destroy(_selectedConsumable.View.gameObject);
			_selectedConsumable = null;
		}
	}

	private void OnDestroy()
	{
		// DeckSystem.Instance.Hand.OnCardAdded -= OnCardAddedToHand;
	}
}
