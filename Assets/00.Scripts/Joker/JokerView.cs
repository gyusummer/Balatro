
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JokerView : View<Joker>, ITradeableView, IBeginDragHandler, IEndDragHandler, IDragHandler, ITooltipSource
{
	private static readonly int ATLAS_UV = Shader.PropertyToID("_AtlasUv");
    
    public string Header => Source.Name;
    public string Content => TooltipSystem.Instance.JokerDescription.GetJokerString(Source.Name).Description;
    public RectTransform Rect => transform as RectTransform;
    public Transform Transform => transform;
    
    public ITradeable Tradeable => Source;
    public void Back()
    {
        transform.position = restorePosition;
    }

    public Joker Source;
    public ImageContainer JokerImageSet;
    public Image Paper;
	[SerializeField] private List<Shader> cardShaders;

    public Action<JokerView> OnClick;

    private void OnValidate()
    {
        UpdatePaper();
        UpdateShader();
    }

    public override void Init(Joker joker)
    {
        Source = joker;
        joker.View = this;
        UpdatePaper();
    }

    public void UpdatePaper()
    {
        if (Source == null) return;
        
        Debug.Log($"{gameObject.name}\n" +
                  $"{Source.Name}");
        Paper.sprite = JokerImageSet.GetImageByNameOrFirst(Source.Name);
        Paper.SetNativeSize();
        UpdateAtlasUv();
    }
    
    private void OnDestroy()
    {
        Source.View = null;
        Source = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{name}: OnPointerClick");
        OnClick?.Invoke(this);
    }
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    
    public Sequence ActivateAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOPunchScale(Vector3.one * AnimationVariable.ActivatePunchScale, AnimationVariable.ActivatePunchTime));
        return seq;
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
	
    private Vector3 restorePosition;
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        restorePosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / rectTransform.localScale.x; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.ShowTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.HideTooltip();
    }
    
    private void UpdateAtlasUv()
    {
        var paperSprite = Paper.sprite;
        Vector4 paperUv = UnityEngine.Sprites.DataUtility.GetOuterUV(paperSprite);
        Paper.material.SetVector(ATLAS_UV, paperUv);
    }

    public void UpdateShader()
    {
        if (Source == null) return;
        
        Material newMat = new Material(Paper.material); 
        var shader = cardShaders[(int)Source.Edition];
        newMat.shader = shader;
        Paper.material = newMat;
    }
}