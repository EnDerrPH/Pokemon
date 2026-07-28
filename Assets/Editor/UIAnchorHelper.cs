#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class UIAnchorHelper
{
    [MenuItem("uGUI/Anchor To Current Rect %]")]
    static void AnchorToCurrent()
    {
        foreach (Transform transform in Selection.transforms)
        {
            RectTransform t = transform as RectTransform;
            RectTransform parent = t?.parent as RectTransform;

            if (t == null || parent == null)
            {
                Debug.LogWarning("Selection must be a UI element with a RectTransform parent.");
                continue;
            }

            Undo.RecordObject(t, "Anchor To Current Rect");

            Vector2 newAnchorMin = new Vector2(
                t.anchorMin.x + t.offsetMin.x / parent.rect.width,
                t.anchorMin.y + t.offsetMin.y / parent.rect.height);

            Vector2 newAnchorMax = new Vector2(
                t.anchorMax.x + t.offsetMax.x / parent.rect.width,
                t.anchorMax.y + t.offsetMax.y / parent.rect.height);

            t.anchorMin = newAnchorMin;
            t.anchorMax = newAnchorMax;
            t.offsetMin = Vector2.zero;
            t.offsetMax = Vector2.zero;
        }
    }
}
#endif
