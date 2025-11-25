using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsumableCardView : MonoBehaviour, IPointerClickHandler
{
	public ConsumableCard Source = new Fool();
	public ImageContainer ConsumablePapers;
	public Image Paper;

	public Action<ConsumableCardView> OnClick;

	private void OnValidate()
	{
		UpdatePaper();
	}

	public void Init(ConsumableCard consumable)
	{
		Source = consumable;
		consumable.View = this;
		UpdatePaper();
	}

	public void UpdatePaper()
	{
		Paper.sprite = ConsumablePapers.GetImageByKeyOrNull(Source.GetType().ToString());
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
