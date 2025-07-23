using Ui.Common.Classes;
using UnityEngine;

namespace UtilsCommon
{
    public static class RectTransformExtensions
    {
        public static void SetAligned(this RectTransform tr, int width, int height,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment verticalAlignment = VerticalAlignment.Center)
        {
            Vector2 pivot =  tr.pivot;
            Vector2 anchorMin =  tr.anchorMin;
            Vector2 anchorMax =  tr.anchorMax;
            Vector2 offsetMin =  tr.offsetMin;
            Vector2 offsetMax =  tr.offsetMax;

            if (horizontalAlignment == HorizontalAlignment.Left)
            {
                pivot.x = 0;
                anchorMin.x = 0;
                anchorMax.x = 0;
                offsetMin.x = 0;
                offsetMax.x = width;
            }
            else if (horizontalAlignment == HorizontalAlignment.Center)
            {
                pivot.x = 0.5f;
                anchorMin.x = 0.5f;
                anchorMax.x = 0.5f;
                offsetMin.x = -width*0.5f;
                offsetMax.x =  width*0.5f;
            }
            else if (horizontalAlignment == HorizontalAlignment.Right)
            {
                pivot.x = 1;
                anchorMin.x = 1;
                anchorMax.x = 1;
                offsetMin.x = -width;
                offsetMax.x = 0;
            }
            else if (horizontalAlignment == HorizontalAlignment.Stretch)
            {
                pivot.x = 0.5f;
                anchorMin.x = 0;
                anchorMax.x = 1;
                offsetMin.x = 0;
                offsetMax.x = 0;
            }

            if (verticalAlignment == VerticalAlignment.Top)
            {
                pivot.y = 1;
                anchorMin.y = 1;
                anchorMax.y = 1;
                offsetMin.y = -height;
                offsetMax.y = 0;
            }
            else if (verticalAlignment == VerticalAlignment.Center)
            {
                pivot.y = 0.5f;
                anchorMin.y = 0.5f;
                anchorMax.y = 0.5f;
                offsetMin.y = -height*0.5f;
                offsetMax.y =  height*0.5f;
            }
            else if (verticalAlignment == VerticalAlignment.Bottom)
            {
                pivot.y = 0;
                anchorMin.y = 0;
                anchorMax.y = 0;
                offsetMin.y = 0;
                offsetMax.y = height;
            }
            else if (verticalAlignment == VerticalAlignment.Stretch)
            {
                pivot.y = 0.5f;
                anchorMin.y = 0;
                anchorMax.y = 1;
                offsetMin.y = 0;
                offsetMax.y = 0;
            }

            tr.pivot = pivot;
            tr.anchorMin = anchorMin;
            tr.anchorMax = anchorMax;
            tr.offsetMin = offsetMin;
            tr.offsetMax = offsetMax;
        }
    }
}