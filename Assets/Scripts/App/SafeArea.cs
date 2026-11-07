using UnityEngine;

namespace App
{
   public class SafeArea : MonoBehaviour
   {
      private RectTransform myRect;
      private void Awake()
      {
         UpdateSaveArea();
      }

      private void Update()
      {
         if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
         {
            UpdateSaveArea();
         }
         else if(Input.deviceOrientation == DeviceOrientation.LandscapeRight)
            UpdateSaveArea();
      }

      private void UpdateSaveArea()
      {
         var safeArea = Screen.safeArea;
         myRect = GetComponent<RectTransform>();

         var anchorMin = safeArea.position;
         var anchorMax = safeArea.position + safeArea.size;

         anchorMin.x /= Screen.width;
         anchorMin.y /= Screen.height;
         anchorMax.x /= Screen.width;
         anchorMax.y /= Screen.height;

         myRect.anchorMin = anchorMin;
         myRect.anchorMax = anchorMax;
      }
   }
}
