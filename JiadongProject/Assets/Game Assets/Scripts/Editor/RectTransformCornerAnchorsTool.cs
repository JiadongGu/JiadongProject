using UnityEditor;
using UnityEngine;

public class RectTransformCornerAnchorsTool
{
    [MenuItem("Tools/Set Corner Anchors to Zero")]
    private static void SetCornerAnchorsToZero()
    {
        foreach (var rectTransform in Selection.transforms)
        {
            RectTransform selectedRectTransform = rectTransform.GetComponent<RectTransform>();
            if (selectedRectTransform != null)
            {
                SetCornerAnchorsToZero(selectedRectTransform);
            }
        }
    }

    private static void SetCornerAnchorsToZero(RectTransform rectTransform)
    {
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform is null!");
            return;
        }

        RectTransform parentRectTransform = rectTransform.parent as RectTransform;

        if (parentRectTransform == null)
        {
            Debug.LogError("Parent RectTransform is null!");
            return;
        }

        Vector2 parentSize = parentRectTransform.rect.size;
        Vector2 rectPosition = rectTransform.anchoredPosition;
        Vector2 rectSize = rectTransform.rect.size;

        Vector2 anchorMin = new Vector2((rectPosition.x - rectSize.x * 0.5f) / parentSize.x + 0.5f,
                                        (rectPosition.y - rectSize.y * 0.5f) / parentSize.y + 0.5f);

        Vector2 anchorMax = new Vector2((rectPosition.x + rectSize.x * 0.5f) / parentSize.x + 0.5f,
                                        (rectPosition.y + rectSize.y * 0.5f) / parentSize.y + 0.5f);

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Debug.Log("Corner anchors set to zero for " + rectTransform.name + "!");
    }
}
