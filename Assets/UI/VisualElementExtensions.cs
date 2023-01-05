using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtensions
{
    public static void SetBorderColor(this VisualElement element, Color color)
    {
        element.style.borderTopColor = color;
        element.style.borderRightColor = color;
        element.style.borderBottomColor = color;
        element.style.borderLeftColor = color;
    }
}