using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ConsumableCardView : View<ConsumableCard>, IPointerClickHandler
{
	public ConsumableCard Source;
	public ImageContainer ImageSet;
	public Image Paper;

	public Action<ConsumableCardView> OnClick;

	private void OnValidate()
	{
		UpdatePaper();
	}

	public override void Init(ConsumableCard consumable)
	{
		Source = consumable;
		consumable.View = this;
		UpdatePaper();
	}

	public void UpdatePaper()
	{
		if (Source == null)
		{
			Debug.Log($"{gameObject.name} Source is null");
			return;
		}
		Paper.sprite = ImageSet.GetImageByNameOrFirst(Source.Name);
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
