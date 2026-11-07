using System.Collections;
using App.PrototypeBattle;
using UnityEngine;

namespace App.Tutorial
{
    public class Pointer : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private AbstractPointerVariant hand;
        [SerializeField] private AbstractPointerVariant arrow;
    
        public void Setup(PointerType pointerType, Transform target, Transform targetCard = null, WeaponType weaponType = WeaponType.NONE, bool isMerge = false, bool isMirror = false, float delay = 0)
        {
            if (pointerType == PointerType.HAND)
            {
                SetupHand(weaponType, target, targetCard, false, isMerge, isMirror);
            }
            else if(pointerType == PointerType.ARROW)
            {
                StartCoroutine(SetupArrow(target,delay, isMirror));
            }
            else if (pointerType == PointerType.ARROW_AND_WHOLE)
            {
                StartCoroutine(SetupArrow(target,delay, isMirror));
            }
            else
            {
                SetupMaskPointer();
            }
        }

        private void SetupMaskPointer()
        {
            hand.gameObject.SetActive(false);
            arrow.gameObject.SetActive(false);
        }

        private void SetupHand(WeaponType weaponType, Transform target, Transform targetCard, bool isClickAnim, bool isMerge, bool isMirror = false)
        {
            hand.gameObject.SetActive(true);
            arrow.gameObject.SetActive(false);
            hand.Setup(weaponType, target, targetCard, isClickAnim, isMerge, isMirror);
        }

        private IEnumerator SetupArrow(Transform target, float delay, bool isMirror = false)
        {
            yield return new WaitForSeconds(delay);
            hand.gameObject.SetActive(false);
            arrow.gameObject.SetActive(true);
            arrow.Setup(target, isMirror);
        }
    }
}