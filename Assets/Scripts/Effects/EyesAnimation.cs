using App;
using Library;
using UnityEngine;

namespace Effects
{
    public class EyesAnimation : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int Fight = Animator.StringToHash("Fight");

        private void SetAnim(bool anim)
        {
            _animator.SetBool(Fight, anim);
        }
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
    }
}
