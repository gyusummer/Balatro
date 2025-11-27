
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JokerView : MonoBehaviour
{
    public Joker Source = JokerFactory.CreateJoker(0);
    public ImageContainer JokerImageSet;
    public Image Paper;

    public Action<JokerView> OnClick;

    private void OnValidate()
    {
        UpdatePaper();
    }

    public void Init(Joker joker)
    {
        Source = joker;
        joker.View = this;
        UpdatePaper();
    }

    public void UpdatePaper()
    {
        Debug.Log($"{gameObject.name}\n" +
                  $"{Source.Name}");
        Paper.sprite = JokerImageSet.GetImageByNameOrFirst(Source.Name);
        Paper.SetNativeSize();
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
}