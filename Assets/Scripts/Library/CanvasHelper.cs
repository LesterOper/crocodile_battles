using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Library
{
    public class CanvasHelper : MonoBehaviour
    {
        public static UnityEvent onOrientationChange = new UnityEvent();
        public static UnityEvent onResolutionChange = new UnityEvent();

        private static readonly List<CanvasHelper> helpers = new List<CanvasHelper>();

        private static bool screenChangeVarsInitialized;
        private static ScreenOrientation lastOrientation = ScreenOrientation.Portrait;
        private static Vector2 lastResolution = Vector2.zero;
        private static Vector2 lastSafeArea = Vector2.zero;
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform rectTransform;

        private RectTransform safeAreaTransform;
        public static bool isLandscape { get; private set; }

        private void Awake()
        {
            if (!helpers.Contains(this))
                helpers.Add(this);

            safeAreaTransform = GetComponent<RectTransform>();

            if (!screenChangeVarsInitialized)
            {
                lastOrientation = Screen.orientation;
                lastResolution.x = Screen.width;
                lastResolution.y = Screen.height;
                lastSafeArea = Screen.safeArea.size;

                screenChangeVarsInitialized = true;
            }
        }

        private void Start()
        {
            ApplySafeArea();
        }

        private void Update()
        {
            if (helpers[0] != this)
                return;

            if (Application.isMobilePlatform)
            {
                if (Screen.orientation != lastOrientation)
                    OrientationChanged();

                if (Screen.safeArea.size != lastSafeArea)
                    SafeAreaChanged();
            }
            else
            {
                //resolution of mobile devices should stay the same always, right?
                // so this check should only happen everywhere else
                if (Screen.width != lastResolution.x || Screen.height != lastResolution.y)
                    ResolutionChanged();
            }
        }

        private void ApplySafeArea()
        {
            if (safeAreaTransform == null)
                return;

            var safeArea = Screen.safeArea;

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= canvas.pixelRect.width;
            anchorMin.y /= canvas.pixelRect.height;
            anchorMax.x /= canvas.pixelRect.width;
            anchorMax.y /= canvas.pixelRect.height;

            safeAreaTransform.anchorMin = anchorMin;
            safeAreaTransform.anchorMax = anchorMax;

            
        }

        private void OnDestroy()
        {
            if (helpers != null && helpers.Contains(this))
                helpers.Remove(this);
        }

        private static void OrientationChanged()
        {

            lastOrientation = Screen.orientation;
            lastResolution.x = Screen.width;
            lastResolution.y = Screen.height;

            isLandscape = lastOrientation == ScreenOrientation.LandscapeLeft ||
                          lastOrientation == ScreenOrientation.LandscapeRight ||
                          lastOrientation == ScreenOrientation.LandscapeLeft;
            onOrientationChange.Invoke();
        }

        private static void ResolutionChanged()
        {
            if (lastResolution.x == Screen.width && lastResolution.y == Screen.height)
                return;


            lastResolution.x = Screen.width;
            lastResolution.y = Screen.height;

            isLandscape = Screen.width > Screen.height;
            onResolutionChange.Invoke();
        }

        private static void SafeAreaChanged()
        {
            if (lastSafeArea == Screen.safeArea.size)
                return;


            lastSafeArea = Screen.safeArea.size;

            for (var i = 0; i < helpers.Count; i++) helpers[i].ApplySafeArea();
        }

        public static Vector2 GetCanvasSize()
        {
            return helpers[0].rectTransform.sizeDelta;
        }

        public static Vector2 GetSafeAreaSize()
        {
            for (var i = 0; i < helpers.Count; i++)
                if (helpers[i].safeAreaTransform != null)
                    return helpers[i].safeAreaTransform.sizeDelta;

            return GetCanvasSize();
        }
    }
}