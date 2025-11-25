using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsumableCardView : MonoBehaviour, IPointerClickHandler
{
	public IConsumable Source = new Fool();
	public ImageContainer TarotImageSet;
	public ImageContainer PlanetImageSet;
	public Image Paper;

	public Action<ConsumableCardView> OnClick;

	private void OnValidate()
	{
		UpdatePaper();
	}

	public void Init(IConsumable consumable)
	{
		Source = consumable;
		consumable.View = this;
		UpdatePaper();
	}

	public void UpdatePaper()
	{
		switch (Source)
		{
			case TarotCard:
				Paper.sprite = TarotImageSet.GetImageByNameOrFirst(Source.Name);
				break;
			case PlanetCard:
				Paper.sprite = PlanetImageSet.GetImageByNameOrFirst(Source.Name);
				break;
		}
	}
	
	public void OnSelected()
	{
		Paper.color = Color.cyan;
	}

	public void OnDeselected()
	{
		Paper.color = Color.white;
	}
	
	private void OnDestroy()
	{
		Source.View = null;
		Source = null;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		HandController.Instance.SelectConsumable(Source);
		OnClick?.Invoke(this);
	}
}
