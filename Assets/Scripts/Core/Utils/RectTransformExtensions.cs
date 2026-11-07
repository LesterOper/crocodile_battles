using UnityEngine;

namespace Core.Utils
{
    public static class RectTransformExtensions
    {
        public static void SetWidth(this RectTransform tr, float width)
        {
            tr.sizeDelta = tr.sizeDelta.WithX(width);
        }

        public static void SetHeight(this RectTransform tr, float height)
        {
            tr.sizeDelta = tr.sizeDelta.WithY(height);
        }

        public static void SetLocalX(this RectTransform tr, float x)
        {
            var p = tr.localPosition;
            tr.localPosition = new Vector3(x, p.y, p.z);
        }
        
        public static void SetLocalY(this RectTransform tr, float y)
        {
            var p = tr.localPosition;
            tr.localPosition = new Vector3(p.x, y, p.z);
        }
        
        public static void SetLocalZ(this RectTransform tr, float z)
        {
            var p = tr.localPosition;
            tr.localPosition = new Vector3(p.x, p.y, z);
        }

        public static void Shift(this RectTransform tr, Vector2 shift)
        {
            tr.localPosition += (Vector3)shift;
        }

        public static Rect GetPixelRect(this RectTransform tr)
            => RectTransformUtility.PixelAdjustRect(tr, tr.GetComponent<Canvas>());

        public static Vector2 GetPixelSize(this RectTransform tr) => tr.GetPixelRect().size;

        public static float GetPixelWidth(this RectTransform tr) => tr.GetPixelSize().x;
        
        public static float GetPixelHeight(this RectTransform tr) => tr.GetPixelSize().y;
    }
}