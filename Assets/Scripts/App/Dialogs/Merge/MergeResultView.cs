using UnityEngine;
using UnityEngine.UI;

namespace App.Dialogs.Merge
{
    public class MergeResultView : MonoBehaviour
    {
        [SerializeField] private MergeTotalView _view;
        [SerializeField] private Image mergeResultSmall;
        [SerializeField] private Image mergeResultNormal;
        [SerializeField] private Image mergeResultBath;

        public void Setup(FilterType type)
        {
            if (type == FilterType.BATH)
            {
                mergeResultNormal.gameObject.SetActive(false);
                mergeResultSmall.gameObject.SetActive(false);
                mergeResultBath.gameObject.SetActive(true);
                _view.SetCurrentResultView(mergeResultBath);
            }
            else
            {
                mergeResultBath.gameObject.SetActive(false);
                if (type == FilterType.ONE_ACTION)
                {
                    mergeResultNormal.gameObject.SetActive(false);
                    mergeResultSmall.gameObject.SetActive(true);
                    _view.SetCurrentResultView(mergeResultSmall);
                }
                else
                {
                    mergeResultNormal.gameObject.SetActive(true);
                    mergeResultSmall.gameObject.SetActive(false);
                    _view.SetCurrentResultView(mergeResultNormal);
                }
            }
        }
    }
}