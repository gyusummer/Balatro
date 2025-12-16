using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ITooltipElement : IPointerEnterHandler, IPointerExitHandler
{
    public string Header { get; }
    public string Content { get; }
    public RectTransform Rect { get; }
    public Transform Transform { get; }
}

public class TooltipSystem : Singleton<TooltipSystem>
{
    [SerializeField] private Tooltip _tooltip;
    private static Tooltip Tooltip => Instance._tooltip;
    
    public static void ShowTooltip(ITooltipElement element)
    {
        Tooltip.UpdateImmediately(element);
        Tooltip.gameObject.SetActive(true);
    }

    public static void HideTooltip()
    {
        Tooltip.gameObject.SetActive(false);
        Tooltip.Clear();
    }
}