using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ConsumableCardView : View<ConsumableCard>, ITradeableView, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public ITradeable Tradeable => Source;
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
		#if UNITY_EDITOR
		if (Application.isPlaying == false)
			return;
		#endif
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
	
	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
	}
	
	public void OnBeginDrag(PointerEventData eventData)
	{
		canvasGroup.blocksRaycasts = false; 
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / rectTransform.localScale.x; 
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		canvasGroup.blocksRaycasts = true;
	}
}
