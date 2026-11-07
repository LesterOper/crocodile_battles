using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace App.Chests
{
    public class ChestAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject _boxCap;
        [SerializeField] private GameObject _light;
        
        public void OpenCap()
        {
            _boxCap.transform.DOMoveY(_boxCap.transform.position.y + 1, 1f);
            Sequence seq = DOTween.Sequence();
            seq.Append(_light.GetComponent<Image>().DOFade(0.5f, 0.8f)).SetLoops(-1, LoopType.Yoyo);
        }
    }
}