using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. Select
// 2. Play
// 3. Discard
public class HandController : Singleton<HandController>
{
	private List<Card> _selectedCards;
	public int SelectedCount => _selectedCards.Count;
	public int SelectLimit = 5;
	
	private ConsumableCard _selectedConsumable;

	private void Start()
	{
		_selectedCards = new List<Card>();
		DeckManager.Instance.Hand.OnAdded += SubscribeCard;
	}

	private void SubscribeCard(Card card)
	{
		card.View.OnClick += TrySelect;
	}

	private void SelectCard(Card card)
	{
		_selectedCards.Add(card.View.Source);
		card.View.OnSelected();
	}

	private void DeselectCard(Card card)
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
	
	private void TrySelect(CardView cardView)
	{
		Card card = cardView.Source;
		
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

	public void SelectConsumable(ConsumableCard consumable)
	{
		if (consumable.IsPlayerOwned == false)
		{
			return;
		}

		if (_selectedConsumable == consumable)
		{
			UseConsumable();
		}
		
		_selectedConsumable?.View?.OnDeselected();
		_selectedConsumable = consumable;
		_selectedConsumable.View.OnSelected();
	}

	public void UseConsumable()
	{
		if (_selectedConsumable.Use(_selectedCards))
		{
			_selectedConsumable = null;
		}
	}

	public void PlayHand()
	{
		if (_selectedCards.Count > 0)
		{
			BlindManager.Instance.PlayHand(_selectedCards);
		}
	}
	
	public void DiscardHand()
	{
		if (_selectedCards.Count > 0)
		{
			BlindManager.Instance.DiscardHand(_selectedCards);
		}
	}

	private void OnDestroy()
	{
		// DeckSystem.Instance.Hand.OnCardAdded -= OnCardAddedToHand;
	}
}
