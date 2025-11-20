using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. Select
// 2. Play
// 3. Discard
public class HandController : Singleton<HandController>
{
	private List<PlayingCardView> _selectedCards;
	public int SelectLimit = 5;

	private void Start()
	{
		_selectedCards = new List<PlayingCardView>();
		DeckSystem.Instance.Hand.OnCardAdded += OnCardAddedToHand;
	}

	private void OnCardAddedToHand(PlayingCard card)
	{
		card.View.OnClick += OnClickCard;
	}
	
	private void OnClickCard(PlayingCardView cardView)
	{
		// Deselect
		if (_selectedCards.Contains(cardView))
		{
			_selectedCards.Remove(cardView);
			cardView.OnDeselected();
		}
		else if (_selectedCards.Count >= SelectLimit)
		{
			Debug.Log("!Can't Select More!");
		}
		// Select
		else
		{
			_selectedCards.Add(cardView);
			cardView.OnSelected();
		}
	}

	public void PlayHand()
	{
		
	}

	public void DiscardHand()
	{
		
	}

	private void OnDestroy()
	{
		DeckSystem.Instance.Hand.OnCardAdded -= OnCardAddedToHand;
	}
}
