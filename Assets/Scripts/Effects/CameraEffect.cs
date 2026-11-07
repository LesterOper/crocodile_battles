using DG.Tweening;
using EasyButtons;
using UnityEngine;

namespace Effects
{
    [ExecuteInEditMode]
    public class CameraEffect : MonoBehaviour
    {
        [SerializeField] private Material _targetMat;
        [SerializeField] private bool _render;
        private Vector2 startPos;
        private RectTransform _rectTransform;
        private void Awake()
        {
            _rectTransform =  GetComponent<RectTransform>();
            
            startPos = _rectTransform.anchoredPosition;
        }

        [Button]
        public void Animate( float delay = 0)
        {
            _render = true;
            _rectTransform
                .DOAnchorPosX(Screen.width *4, 2).SetDelay(0.2f)
                .OnComplete(() => _rectTransform.DOAnchorPosX(startPos.x, 0));
        }
        
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_render)
            {
                Graphics.Blit(src, dest, _targetMat);
            }
            else
                Graphics.Blit(src, dest);
        }
    }
}