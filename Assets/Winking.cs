using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Winking : MonoBehaviour
{
    private Image _image;
    private bool _isActive = false;
    private Tween _tween;
    void Awake() => _image = GetComponent<Image>();
    private void OnEnable() => Wink();
    private void OnDisable() => Wink();
    private void Wink()
    {
        _isActive = !_isActive;

        if (_isActive)
            _tween = _image.DOFade(0.2f, 0.75f).SetLoops(-1, LoopType.Yoyo);
        else
        {
            _tween.Kill();
            _image.DOFade(1f, 0);
        }
            
    }
}
